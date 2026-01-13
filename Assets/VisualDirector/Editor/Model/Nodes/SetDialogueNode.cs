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

        public enum Location
        {
            Left = 0,
            Right = 1
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
   
        }
    }
}
