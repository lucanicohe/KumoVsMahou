using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMove : MonoBehaviour
{
    public enum Axis { X, Y, Z }  // Axis selector

    [SerializeField] private Axis rotationAxis = Axis.Y;
    [SerializeField] private float angle = 15f;     // Maximum rotation angle
    [SerializeField] private float duration = 1f;   // Time to go from 0° to max angle

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / duration, 1f);
        float currentAngle = Mathf.Lerp(0f, angle, t);

        Vector3 euler = transform.localRotation.eulerAngles;

        // Apply rotation to the selected axis
        switch (rotationAxis)
        {
            case Axis.X:
                euler.x = currentAngle;
                break;
            case Axis.Y:
                euler.y = currentAngle;
                break;
            case Axis.Z:
                euler.z = currentAngle;
                break;
        }

        transform.localRotation = Quaternion.Euler(euler);
    }
}
