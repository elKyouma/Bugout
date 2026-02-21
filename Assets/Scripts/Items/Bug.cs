using UnityEngine;

public class Bug : Collectable
{
    protected override void Collect()
    {
        GameManager.Instance.Bugs += (uint)itemAmount;
        GameManager.Instance.postProcess.MultiplyBugEffect();
        PlayerPrefs.SetInt(gameObject.scene.name + transform.parent.gameObject.name, 1);
        ObjectDestroy();
    }
}
