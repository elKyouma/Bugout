using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualDirector
{
    [Serializable]
    public abstract class   VisualDirectorRuntimeNode
    {
        [SerializeReference]
        public List<VisualDirectorRuntimeNode> Next = new();
    }
}
