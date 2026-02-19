using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class TakeItemRuntimeNode : VisualDirectorRuntimeNode
    {
        public IGameManager.ItemType ItemType;
    }

    public class TakeItemExecutor : IVisualDirectorNodeExecutor<TakeItemRuntimeNode>
    {
        public async Task ExecuteAsync(TakeItemRuntimeNode node, VisualDirector ctx) => ctx.GameManager.TakeItem(node.ItemType);
    }
}
