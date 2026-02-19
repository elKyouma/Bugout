using System;
using UnityEngine;
using static VisualDirector.Editor.SetDialogueNode;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class DisableInteractivityNode : VisualDirectorNode
    {
        public const string IN_PORT_TAG_NAME = "IN_PORT_TAG";


        public enum InteractivityType
        {
            Current = 0,
            ByTag = 1
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<InteractivityType>("InteractivityType");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            
            GetNodeOption(0).TryGetValue(out InteractivityType type);
            if (type == InteractivityType.ByTag)
            {
                context.AddInputPort<string>(IN_PORT_TAG_NAME).Build();
            }
        }
    }
}
