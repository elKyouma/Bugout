using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class TeleportRuntimeNode : VisualDirectorRuntimeNode
    {
        public TeleportTag.Tag Tag;
    }

    public class TeleportExecutor : IVisualDirectorNodeExecutor<TeleportRuntimeNode>
    {
        public async Task ExecuteAsync(TeleportRuntimeNode node, VisualDirector ctx) => ctx.GameManager.TeleportPlayerToLocation(node.Tag);
    }
}
