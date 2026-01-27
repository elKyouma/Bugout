using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class WaitForInputRuntimeNode : VisualDirectorRuntimeNode
    {
    }

    public class WaitForInputExecutor : IVisualDirectorNodeExecutor<WaitForInputRuntimeNode>
    {
        /// <summary>
        /// Asynchronously waits for user input to be detected before proceeding with the execution of the visual novel graph.
        /// </summary>
        public async Task ExecuteAsync(WaitForInputRuntimeNode _, VisualDirector ctx)
        {
            await ctx.InputProvider.InputDetected();
        }
    }
}
