using System;
using System.Threading.Tasks;

namespace VisualDirector
{
    [Serializable]
    public class GenerativeAudioRuntimeNode : VisualDirectorRuntimeNode
    {
    }

    public class GenerativeAudioExecutor : IVisualDirectorNodeExecutor<GenerativeAudioRuntimeNode>
    {
        public async Task ExecuteAsync(GenerativeAudioRuntimeNode _, VisualDirector ctx)
        {
            await ctx.InputProvider.InputDetected();
        }
    }
}
