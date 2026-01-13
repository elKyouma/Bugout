using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class StartNode : VisualDirectorNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Start is a special node that has no input, so we don't call DefineCommonPorts
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
