using UnityEngine;

public class Health : Collectable
{
    protected override void Collect()
    {
        if (Player.Instance.health < Player.Instance.maxHealth)
        {
            GameManager.Instance.hud.HealthBarHurt();
            Player.Instance.health += itemAmount;
        }
        ObjectDestroy();
    }
}
