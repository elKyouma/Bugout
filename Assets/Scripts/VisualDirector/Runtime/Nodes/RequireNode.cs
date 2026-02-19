using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class RequireRuntimeNode : VisualDirectorRuntimeNode
    {
        public IGameManager.ItemType ItemType;
        public int Number;
    }

    public class RequireExecutor : IVisualDirectorNodeExecutor<RequireRuntimeNode>
    {
        public async Task ExecuteAsync(RequireRuntimeNode node, VisualDirector ctx) => ctx.choiceId = ctx.GameManager.HasItem(node.ItemType, node.Number) ? 0 : 1;
    }
}
