using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class EndingPlaceholder : MonoBehaviour
{
    [SerializeField] private EndingSO ending;

    private void Awake()
    {
        GetComponent<Image>().sprite = ending.icon;

        if (PlayerPrefs.GetString("CurrendEnding") == ending.name)
            Tween.Scale(transform, Vector3.one * 0.5f, 0.5f, Ease.InOutSine)
                .OnComplete(() => Tween.Scale(transform, Vector3.one * 1.5f, 1f, Ease.InOutSine, -1, CycleMode.Yoyo));
        else
            Tween.Scale(transform, Vector3.one, 0.5f, Ease.InOutExpo);
    }
}
