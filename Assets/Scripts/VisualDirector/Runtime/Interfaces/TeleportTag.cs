using UnityEngine;

namespace VisualDirector
{
    public class TeleportTag : MonoBehaviour
    {
        public enum Tag
        {
            ApartamentBottom,
            ApartamentMiddle,
            ApartamentTop,
        }

        public Tag tag;
    }
}
