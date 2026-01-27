using UnityEngine;
using System.Collections.Generic;

namespace VisualDirector.Editor
{
    interface ISound { };
    interface ISoundImpl {
        float GetVolume();
        float GetDelay();
        void PlaySound();
    };
}
