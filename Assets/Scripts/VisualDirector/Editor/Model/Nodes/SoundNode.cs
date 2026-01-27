using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class SoundNode : VisualDirectorNode, ISound
    {
        public const string SOUND_PORT_NAME = "Sound";
        public const string SOUND_ASSET_PORT_NAME = "SoundTrack";
        public const string SOUND_VOLUME_PORT_NAME = "Volume";
        public const string SOUND_DELAY_PORT_NAME = "Delay";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<AudioClip>(SOUND_ASSET_PORT_NAME)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            context.AddInputPort<float>(SOUND_VOLUME_PORT_NAME)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            context.AddInputPort<float>(SOUND_DELAY_PORT_NAME)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            context.AddOutputPort<ISound>(SOUND_PORT_NAME)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}
