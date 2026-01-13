using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal abstract class VisualDirectorNode : Node
    {
        public const string EXECUTION_PORT_DEFAULT_NAME = "ExecutionPort";

        protected void AddInputExecutionPort(IPortDefinitionContext context)
        {
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }

        protected void AddInputOutputExecutionPorts(IPortDefinitionContext context)
        {
            AddInputExecutionPort(context);
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
