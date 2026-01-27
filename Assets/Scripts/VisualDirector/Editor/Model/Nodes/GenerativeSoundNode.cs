using System;
using Unity.GraphToolkit.Editor;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class GenerativeSoundNode : VisualDirectorNode, ISound
    {
        public const string SOUND_PORT_NAME = "SoundPort";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<ISound>(SOUND_PORT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}
