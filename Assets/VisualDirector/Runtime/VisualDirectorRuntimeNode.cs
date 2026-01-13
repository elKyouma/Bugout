using System;
using System.Collections.Generic;

namespace VisualDirector
{
    [Serializable]
    public abstract class VisualDirectorRuntimeNode
    {
        public List<VisualDirectorRuntimeNode> Next = new();
    }
}
