using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace VisualDirector.Editor
{
    [ScriptedImporter(1, VisualDirectorGraph.AssetExtension)]
    internal class VisualNovelDirectorImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<VisualDirectorGraph>(ctx.assetPath);
            if (graph == null)
            {
                Debug.LogError($"Failed to load Visual Director graph asset: {ctx.assetPath}");
                return;
            }
            
            var startNodeModel = graph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNodeModel == null)
                return;

            var runtimeAsset = ScriptableObject.CreateInstance<VisualDirectorRuntimeGraph>();
            var modelToRuntime = new Dictionary<INode, List<VisualDirectorRuntimeNode>>();
            var visited = new HashSet<INode>();
            var queue = new Queue<INode>();

            queue.Enqueue(startNodeModel);

            //Zamieniæ na odwiedzanie wsteczne w razie problemów z perfem
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (visited.Contains(node))
                    continue;

                visited.Add(node);

                var runtimeNodes = TranslateNodeModelToRuntimeNodes(node);
                runtimeAsset.Nodes.AddRange(runtimeNodes);

                modelToRuntime[node] = runtimeNodes;
                var nextNodes = GetNextNodes(node).Where(n => n != null).ToList();

                foreach (var next in nextNodes)
                    queue.Enqueue(next);
            }

            foreach (var kvp in modelToRuntime)
            {
                var modelNode = kvp.Key;
                var runtimeNodes = kvp.Value;

                var lastRuntimeNode = runtimeNodes.LastOrDefault();
                if (lastRuntimeNode == null)
                    continue;

                foreach (var nextModel in GetNextNodes(modelNode))
                {
                    if (modelToRuntime.TryGetValue(nextModel, out var nextRuntime))
                        lastRuntimeNode.Next.Add(nextRuntime.First());
                }
            }

            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        static IEnumerable<INode> GetNextNodes(INode currentNode)
        {

            if (currentNode is MultiChoiceNode)
            {
                var outputs = new List<IPort>
                {
                    currentNode.GetOutputPortByName(MultiChoiceNode.OUT_PORT_CHOICE1_NAME),
                    currentNode.GetOutputPortByName(MultiChoiceNode.OUT_PORT_CHOICE2_NAME),
                    currentNode.GetOutputPortByName(MultiChoiceNode.OUT_PORT_CHOICE3_NAME),
                    currentNode.GetOutputPortByName(MultiChoiceNode.OUT_PORT_CHOICE4_NAME)
                };

                foreach (var port in outputs)
                    yield return port.GetNode();
            }
            else
            {
                var outputPort = currentNode.GetOutputPortByName(VisualDirectorNode.EXECUTION_PORT_DEFAULT_NAME);
                yield return outputPort.GetNode();
            }
        }


        static List<VisualDirectorRuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel)
        {
            var returnedNodes = new List<VisualDirectorRuntimeNode>();
            switch (nodeModel)
            {
                case StartNode _:
                    // Start node does not translate to any runtime node.
                    break;

                case SetDialogueNode setDialogueNodeModel:
                    returnedNodes.Add(new SetDialogueRuntimeNode
                    {
                        ActorName = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_NAME_NAME)),
                        ActorSprite = GetInputPortValue<Sprite>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_SPRITE_NAME)),
                        LocationIndex = (int)GetInputPortValue<SetDialogueNode.Location>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_LOCATION_NAME)),
                        DialogueText = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_DIALOGUE_NAME))
                    });

                    returnedNodes.Add(new WaitForInputRuntimeNode());
                    break;

                case WaitForInputNode _:
                    returnedNodes.Add(new WaitForInputRuntimeNode());
                    break;

                case MultiChoiceNode _:
                    returnedNodes.Add(new MultiChoiceRuntimeNode
                    {
                        choide1 = GetInputPortValue<string>(nodeModel.GetInputPortByName(MultiChoiceNode.IN_PORT_CHOICE1_NAME)),
                        choide2 = GetInputPortValue<string>(nodeModel.GetInputPortByName(MultiChoiceNode.IN_PORT_CHOICE2_NAME)),
                        choide3 = GetInputPortValue<string>(nodeModel.GetInputPortByName(MultiChoiceNode.IN_PORT_CHOICE3_NAME)),
                        choide4 = GetInputPortValue<string>(nodeModel.GetInputPortByName(MultiChoiceNode.IN_PORT_CHOICE4_NAME))
                    });
                    break;

                default:
                    throw new ArgumentException($"Unsupported node model type: {nodeModel.GetType()}");
            }

            return returnedNodes;
        }

        static T GetInputPortValue<T>(IPort port)
        {
            T value = default;

            if (port.isConnected)
            {
                switch (port.firstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.variable.TryGetDefaultValue<T>(out value);
                        return value;
                    case IConstantNode constantNode:
                        constantNode.TryGetValue<T>(out value);
                        return value;
                    default:
                        break;
                }
            }
            else
                port.TryGetValue(out value);

            return value;
        }
    }
}
