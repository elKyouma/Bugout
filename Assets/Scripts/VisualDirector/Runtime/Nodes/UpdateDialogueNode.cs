using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class UpdateDialogueRuntimeNode : VisualDirectorRuntimeNode
    {
        public VisualDirectorRuntimeGraph Vs;
    }

    public class UpdateDialogueExecutor : IVisualDirectorNodeExecutor<UpdateDialogueRuntimeNode>
    {
        public async Task ExecuteAsync(UpdateDialogueRuntimeNode node, VisualDirector ctx) => ctx.CurrentInteractable.UpdateDialogue(node.Vs);
    }
}
