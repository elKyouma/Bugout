using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace VisualDirector.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    internal class VisualDirectorGraph : Graph
    {
        const string k_graphName = "Visual Director Graph";
        internal const string AssetExtension = "vdg";
        [MenuItem("Assets/Create/Visual Director Graph")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<VisualDirectorGraph>(k_graphName);
        }
        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);
            CheckGraphErrors(infos);
        }

        void CheckGraphErrors(GraphLogger infos)
        {
            List<StartNode> startNodes = GetNodes().OfType<StartNode>().ToList();

            switch (startNodes.Count)
            {
                case 0:
                    infos.LogError("Add a StartNode in your Visual Novel graph.", this);
                    break;
                case >= 1:
                    {
                        foreach (var startNode in startNodes.Skip(1))
                        {
                            infos.LogWarning($"VisualDirector only supports one StartNode per graph. Only the first created one will be used.", startNode);
                        }
                        break;
                    }
            }
        }

       
    }
}
