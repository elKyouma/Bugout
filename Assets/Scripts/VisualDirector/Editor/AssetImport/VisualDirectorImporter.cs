using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;
using static VisualDirector.Editor.DisableInteractivityNode;
using static VisualDirector.Editor.SetDialogueNode;

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

                // If this is a dialogue node and its next nodes are not choice nodes,
                // insert a WaitForInput runtime node after the dialogue so the runtime
                // will wait for input before continuing.
                var nextModels = GetNextNodes(node).Where(n => n != null).ToList();
                if (node is SetDialogueNode && !nextModels.Any(n => n is MultiChoiceNode)) // Any seems wastefull however it would be difficult to do better stream of execution
                {
                    var waitNode = new WaitForInputRuntimeNode();
                    runtimeNodes.Last().Next.Add(waitNode);
                    runtimeNodes.Add(waitNode);
                }

                runtimeAsset.Nodes.AddRange(runtimeNodes);
                modelToRuntime[node] = runtimeNodes;

                foreach (var next in nextModels)
                    queue.Enqueue(next);
            }

            foreach (var kvp in modelToRuntime)
            {
                var modelNode = kvp.Key;
                var runtimeNodes = kvp.Value;

                var lastRuntimeNode = runtimeNodes.LastOrDefault();
                if (lastRuntimeNode == null)
                    continue;

                foreach (var nextModel in GetNextNodes(modelNode).Where(n => n != null).ToList())
                {
                    if (modelToRuntime.TryGetValue(nextModel, out var nextRuntime))
                        lastRuntimeNode.Next.Add(nextRuntime.First());
                }
            }

            //runtimeAsset.Nodes.AddRange(modelToRuntime.Values.SelectMany(v => v));
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
                {
                    if (port == null || port.firstConnectedPort == null)
                        continue;

                    yield return port.firstConnectedPort.GetNode();
                }
            }
            else if (currentNode is RequireNode)
            {
                yield return currentNode.GetOutputPortByName(RequireNode.EXECUTION_PORT_SUCCESS).firstConnectedPort.GetNode();
                yield return currentNode.GetOutputPortByName(RequireNode.EXECUTION_PORT_FAIL).firstConnectedPort.GetNode();
            }
            else
            {
                var outputPort = currentNode.GetOutputPortByName(VisualDirectorNode.EXECUTION_PORT_DEFAULT_NAME);
                yield return outputPort.firstConnectedPort.GetNode();
            }
        }


        List<VisualDirectorRuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel)
        {
            var returnedNodes = new List<VisualDirectorRuntimeNode>();
            switch (nodeModel)
            {
                case StartNode _:
                    // Start node does not translate to any runtime node.
                    break;

                case SetDialogueNode setDialogueNodeModel:

                    setDialogueNodeModel.GetNodeOption(0).TryGetValue(out SoundType audioType);// do it base on this, not null value
                    AudioClip audio = null;
                    float volume = 0f;
                    float delay = 0f;
                    if (audioType == SoundType.Clip)
                    {
                        var port = setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_AUDIO_CLIP_NAME);
                        audio = GetInputPortValue<AudioClip>(port);
                        volume = GetInputPortValue<float>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_AUDIO_VOLUME_NAME));
                        delay = GetInputPortValue<float>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_AUDIO_DELAY_NAME));
                    }
                    
                    
                    returnedNodes.Add(new SetDialogueRuntimeNode
                    {
                        ActorName = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_NAME_NAME)),
                        ActorSprite = GetInputPortValue<Sprite>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_SPRITE_NAME)),
                        LocationIndex = (int)GetInputPortValue<SetDialogueNode.Location>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_LOCATION_NAME)),
                        DialogueText = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_DIALOGUE_NAME)),
                        AudioClip = audio,
                        AudioVolume = volume,
                        AudioDelay = delay,
                        //AudioGenerative = GetInputPortValue<GenerativeSound>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_AUDIO_GENERATIVE_NAME))
                    });
                    
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

                case DisableInteractivityNode node:
                    {
                        node.GetNodeOption(0).TryGetValue(out DisableInteractivityNode.InteractivityType type);// do it base on this, not null value
                        string tag = "";
                        if (type == InteractivityType.ByTag)
                            tag = GetInputPortValue<string>(nodeModel.GetInputPortByName(DisableInteractivityNode.IN_PORT_TAG_NAME));

                        returnedNodes.Add(new DisableInteractivityRuntimeNode
                        {
                            Tag = tag
                        });
                    }
                    break;
                case TeleportNode node:
                    returnedNodes.Add(new TeleportRuntimeNode
                    {
                        Tag = GetInputPortValue<TeleportTag.Tag>(nodeModel.GetInputPortByName(TeleportNode.IN_PORT_TELEPORT_TAG))
                    });
                    break;
                case UpdateDialogueNode node:
                    returnedNodes.Add(new UpdateDialogueRuntimeNode
                    {
                        Vs = GetInputPortValue<VisualDirectorRuntimeGraph>(nodeModel.GetInputPortByName(UpdateDialogueNode.IN_PORT_DIALOGUE_NAME))
                    });
                    break;
                case GiveItemNode node:
                    returnedNodes.Add(new GiveItemRuntimeNode
                    {
                        ItemType = GetInputPortValue<IGameManager.ItemType>(nodeModel.GetInputPortByName(GiveItemNode.IN_PORT_ITEM_TYPE_NAME))
                    });
                    break;
                case TakeItemNode node:
                    returnedNodes.Add(new TakeItemRuntimeNode
                    {
                        ItemType = GetInputPortValue<IGameManager.ItemType>(nodeModel.GetInputPortByName(TakeItemNode.IN_PORT_ITEM_TYPE_NAME))
                    });
                    break;
                case RequireNode node:
                    {
                        node.GetNodeOption(0).TryGetValue(out RequireNode.RequireType type);// do it base on this, not null value
                        IGameManager.ItemType itemType = IGameManager.ItemType.None;
                        if (type == RequireNode.RequireType.ITEM)
                            itemType = GetInputPortValue<IGameManager.ItemType>(nodeModel.GetInputPortByName(RequireNode.IN_PORT_ITEM_TYPE_NAME));
                        returnedNodes.Add(new RequireRuntimeNode
                        {
                            ItemType = itemType,
                            Number = GetInputPortValue<int>(nodeModel.GetInputPortByName(RequireNode.IN_PORT_NUMBER_NAME))
                        });
                    }
                    break;
                default:
                    throw new ArgumentException($"Unsupported node model type: {nodeModel.GetType()}");
            }

            return returnedNodes;
        }

        static T GetInputPortValue<T>(IPort port)
        {
            T value = default(T);
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
                }
            }

            port.TryGetValue(out value);
            return value; 
        }
    }
}
