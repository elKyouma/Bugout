using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player") return;

        if (GameManager.Instance.DoesInventoryHaveTheSameItems(ItemType.Balloon))
            GameManager.Instance.EndGame("Trap");
        else
            GameManager.Instance.EndGame("Tutorial");
    }
}
