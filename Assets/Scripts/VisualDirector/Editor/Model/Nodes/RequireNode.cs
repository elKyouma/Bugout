using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class RequireNode : VisualDirectorNode
    {
        public const string IN_PORT_NUMBER_NAME = "IN_PORT_BUGS_NUMBER";
        public const string IN_PORT_ITEM_TYPE_NAME = "IN_PORT_ITEM_TYPE";
        public const string EXECUTION_PORT_SUCCESS = "EXECUTION_PORT_SUCCESS";
        public const string EXECUTION_PORT_FAIL = "EXECUTION_PORT_FAIL";

        public enum RequireType
        {
            ITEM = 0,
            BUGS = 1
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context) => context.AddOption<RequireType>("InteractivityType");

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputExecutionPort(context);
            context.AddOutputPort(EXECUTION_PORT_SUCCESS)
                .WithDisplayName("Succeed")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            context.AddOutputPort(EXECUTION_PORT_FAIL)
                .WithDisplayName("Fail")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            GetNodeOption(0).TryGetValue(out RequireType type);
            if (type == RequireType.ITEM)
                context.AddInputPort<IGameManager.ItemType>(IN_PORT_ITEM_TYPE_NAME).WithDisplayName("Item Type").Build();
            context.AddInputPort<int>(IN_PORT_NUMBER_NAME).WithDisplayName("Item Type").Build();
        }
    }
}
