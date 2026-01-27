using System;
using System.Threading.Tasks;
using UnityEngine;

namespace VisualDirector
{
    [Serializable]
    public class SoundNodeRuntimeNode : VisualDirectorRuntimeNode
    {
        public AudioClip Clip;
        public float Volume;
        public float Delay;
    }

    public class SoundNodeExecutor : IVisualDirectorNodeExecutor<SoundNodeRuntimeNode>
    {
        public async Task ExecuteAsync(SoundNodeRuntimeNode rtNode, VisualDirector ctx)
        {
            if (rtNode.Delay > 0f)
            {
                await Task.Delay(TimeSpan.FromSeconds(rtNode.Delay));
            }
            ctx.audioSource.PlayOneShot(rtNode.Clip, rtNode.Volume);
        }
    }
}
