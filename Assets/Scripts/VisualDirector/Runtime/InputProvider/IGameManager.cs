using UnityEngine;

namespace VisualDirector
{
    public interface IGameManager
    {
        public void TeleportPlayerToLocation(TeleportTag.Tag teleportTag);
    }
}
