using System;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class WaitForInputNode : VisualDirectorNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }
    }
}
