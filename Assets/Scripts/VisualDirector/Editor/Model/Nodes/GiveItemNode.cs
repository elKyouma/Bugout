using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class GiveItemNode : VisualDirectorNode
    {
        public const string IN_PORT_ITEM_TYPE_NAME = "IN_PORT_ITEM_TYPE";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            context.AddInputPort<IGameManager.ItemType>(IN_PORT_ITEM_TYPE_NAME).WithDisplayName("Item type").Build();
        }
    }
}
