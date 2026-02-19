using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class GenerativeSound : ScriptableObject
{
    //map or defined values? -> we could map certain words to sound different so this seems kinda powerful

    //wha do i want? Square/chainsaw... and custom, so... I need enums and each would have different values and custom editor for it?
    //Version with audioclips is much easier  so we can start from here. there should be option to set either few sounds to chooce from or sound + modulation.
    //This also seems like complicated system

    //so maybe list of <Audioclip, freqModulationFactor, volume, modulation volume>. In future audioclip should be interchangable with type of
    //wave + freq

    //we will introduce conctepts from above in future with custom editor. This sound making system sounds so complicated that specialized
    //editor would be very usefull, with ability to test it on some dialogue

    //Plan:
    //1. AudioClip
    //2. Modulations
    //2.5 make context button for tring read example text
    //3. Different types 
    //4. Custom editor
    //5. Custom pauses
    public AudioClip basicSound;
}
