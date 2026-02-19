using UnityEngine;

namespace VisualDirector
{
    public interface IGameManager
    {
        public void TeleportPlayerToLocation(TeleportTag.Tag teleportTag);

        public enum ItemType { None, Beer, Balloon, Dynamite, Knife, Key };

        bool HasItem(ItemType item, int number);
        bool HasBugs(int number);

        void GiveItem(ItemType item);
        void TakeItem(ItemType item);
    }
}
