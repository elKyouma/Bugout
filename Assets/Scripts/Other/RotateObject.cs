using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 rotationAxis = Vector3.back;

    void Update() => transform.Rotate(rotationAxis * speed * Time.deltaTime);
}