using UnityEngine;
using PrimeTween;

public class Levitate : MonoBehaviour
{
    [SerializeField] private float height = 1f;
    [SerializeField] private float speed = 1f;

    private float startY;

    void Start()
    {
        startY = transform.localPosition.y;

        // Tween Y position up and down
        Tween.PositionY(
            transform,
            endValue: startY + height,
            duration: 1f / speed,
            ease: Ease.InOutSine,
            cycles: -1,
            cycleMode: CycleMode.Yoyo
        );
    }
}