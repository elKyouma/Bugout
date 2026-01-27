using System.Collections.Generic;
using UnityEngine;

namespace VisualDirector
{
    public class VisualDirectorRuntimeGraph : ScriptableObject
    {
        [SerializeReference]
        public List<VisualDirectorRuntimeNode> Nodes = new();
    }
}
