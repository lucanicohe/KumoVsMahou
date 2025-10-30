using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.y += rotationSpeed * Time.deltaTime;
        transform.localEulerAngles = currentRotation;
    }
}
