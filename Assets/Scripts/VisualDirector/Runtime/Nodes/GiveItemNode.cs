using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class GiveItemRuntimeNode : VisualDirectorRuntimeNode
    {
        public IGameManager.ItemType ItemType;
    }

    public class GiveItemExecutor : IVisualDirectorNodeExecutor<GiveItemRuntimeNode>
    {
        public async Task ExecuteAsync(GiveItemRuntimeNode node, VisualDirector ctx) => ctx.GameManager.GiveItem(node.ItemType);
    }
}
