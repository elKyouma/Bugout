using System;
using UnityEngine;

namespace VisualDirector.Editor
{
    [Serializable]
    internal class SetDialogueNode : VisualDirectorNode
    {
        public const string IN_PORT_ACTOR_NAME_NAME = "ActorName";
        public const string IN_PORT_ACTOR_SPRITE_NAME = "ActorSprite";
        public const string IN_PORT_LOCATION_NAME = "ActorLocation";
        public const string IN_PORT_DIALOGUE_NAME = "Dialogue";
        public const string IN_PORT_AUDIO_CLIP_NAME = "AudioClip";
        public const string IN_PORT_AUDIO_VOLUME_NAME = "AudioVolume";
        public const string IN_PORT_AUDIO_DELAY_NAME = "AudioDelay";
        public const string IN_PORT_AUDIO_GENERATIVE_NAME = "AudioGenerative";

        public enum Location
        {
            Left = 0,
            Right = 1
        }

        public enum SoundType
        {
            None,
            Clip,
            Generative
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<SoundType>("SoundSupport");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);

            context.AddInputPort<string>(IN_PORT_ACTOR_NAME_NAME)
                .WithDisplayName("Actor Name")
                .Build();
            context.AddInputPort<Sprite>(IN_PORT_ACTOR_SPRITE_NAME)
                .WithDisplayName("Actor Sprite")
                .Build();
            context.AddInputPort<Location>(IN_PORT_LOCATION_NAME)
                .WithDisplayName("Actor Location")
                .Build();
            context.AddInputPort<string>(IN_PORT_DIALOGUE_NAME)
                .Build();
            GetNodeOption(0).TryGetValue(out SoundType type);
            if(type == SoundType.Clip)
            {
                context.AddInputPort<float>(IN_PORT_AUDIO_VOLUME_NAME).WithDefaultValue(1).Build();
                context.AddInputPort<float>(IN_PORT_AUDIO_DELAY_NAME).WithDefaultValue(1).Build();
                context.AddInputPort<AudioClip>(IN_PORT_AUDIO_CLIP_NAME);
            }
            //if(type == SoundType.Generative)
                //context.AddInputPort<Generativ    eSound>(IN_PORT_AUDIO_GENERATIVE_NAME);
        }
    }
}
