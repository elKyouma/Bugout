using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class IfEnding : MonoBehaviour
{
    [SerializeField] private string endingName;
    [SerializeField] private Status endingStatus;

    private void Start()
    {
        var (_, completionStatus, _) = GameManager.Instance.GetEndingData(endingName);
        if (endingStatus != completionStatus)
        {
            gameObject.SetActive(false);
        }
    }
}
