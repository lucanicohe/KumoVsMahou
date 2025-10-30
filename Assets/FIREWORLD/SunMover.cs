using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMover : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f; 

    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
