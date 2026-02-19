using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class TeleportNode : VisualDirectorNode
    {
        public const string IN_PORT_TELEPORT_TAG = "IN_PORT_TELEPORT_TAG";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            context.AddInputPort<string>(IN_PORT_TELEPORT_TAG).Build();
        }
    }
}
