using UnityEngine;
using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class UpdateDialogueNode : VisualDirectorNode
    {
        public const string IN_PORT_DIALOGUE_NAME = "IN_PORT_DIALOGUE";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            context.AddInputPort<VisualDirectorRuntimeGraph>(IN_PORT_DIALOGUE_NAME).WithDisplayName("Dialogue").Build();
        }
    }
}
