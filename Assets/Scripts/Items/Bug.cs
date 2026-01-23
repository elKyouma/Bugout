using UnityEngine;

public class Bug : Collectable
{
    protected override void Collect()
    {
        Player.Instance.bugs += itemAmount;
        Postprocess.Instance.MultiplyBugEffect();
        PlayerPrefs.SetInt(gameObject.scene.name + transform.parent.gameObject.name, 1);
        ObjectDestroy();
    }
}
