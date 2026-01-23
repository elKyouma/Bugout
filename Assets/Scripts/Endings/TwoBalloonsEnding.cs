using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoBalloonsEnding : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.Instance.IsItemInInventory(ItemType.Balloon))
        {
            StartCoroutine("ChangeScene");
        }
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.EndGame("2Balloons");
    }
}
