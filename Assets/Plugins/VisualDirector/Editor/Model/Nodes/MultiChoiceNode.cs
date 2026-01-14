using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class MultiChoiceNode : VisualDirectorNode
    {
        public const string IN_PORT_CHOICE1_NAME = "choice1";
        public const string IN_PORT_CHOICE2_NAME = "choice2";
        public const string IN_PORT_CHOICE3_NAME = "choice3";
        public const string IN_PORT_CHOICE4_NAME = "choice4";

        public const string OUT_PORT_CHOICE1_NAME = "execute_choice1";
        public const string OUT_PORT_CHOICE2_NAME = "execute_choice2";
        public const string OUT_PORT_CHOICE3_NAME = "execute_choice3";
        public const string OUT_PORT_CHOICE4_NAME = "execute_choice4";

        public enum Location
        {
            Left = 0,
            Right = 1
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputExecutionPort(context);

            context.AddInputPort<string>(IN_PORT_CHOICE1_NAME)
                .WithDisplayName("Choice A")
                .Build();
            context.AddInputPort<string>(IN_PORT_CHOICE2_NAME)
                .WithDisplayName("Choice B")
                .Build();
            context.AddInputPort<string>(IN_PORT_CHOICE3_NAME)
                .WithDisplayName("Choice C")
                .Build();
            context.AddInputPort<string>(IN_PORT_CHOICE4_NAME)
                .WithDisplayName("Choice D")
                .Build();

            context.AddOutputPort(OUT_PORT_CHOICE1_NAME)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            context.AddOutputPort(OUT_PORT_CHOICE2_NAME)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            context.AddOutputPort(OUT_PORT_CHOICE3_NAME)
                .WithDisplayName("C")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            context.AddOutputPort(OUT_PORT_CHOICE4_NAME)
                .WithDisplayName("D")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

        }
    }
}
