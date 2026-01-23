using UnityEngine;

public class Ammo : Collectable
{
    protected override void Collect()
    {
        if (Player.Instance.ammo < Player.Instance.maxAmmo)
        {
            GameManager.Instance.hud.HealthBarHurt();
            Player.Instance.ammo += itemAmount;
        }
        ObjectDestroy();
    }
}
