using UnityEngine;

namespace VisualDirector
{
    public interface IDialogueController : IDisabable
    {
        void UpdateDialogue(VisualDirectorRuntimeGraph vs);
    }
}
