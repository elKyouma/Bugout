using System.Threading.Tasks;

namespace VisualDirector
{
    public interface IVisualDirectorNodeExecutor<in TNode> where TNode : VisualDirectorRuntimeNode
    {
        Task ExecuteAsync(TNode node, VisualDirector ctx);
    }
}
