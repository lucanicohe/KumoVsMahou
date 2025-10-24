using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMove : MonoBehaviour
{




    public enum Axis { X, Y, Z }  // whic axis

    [SerializeField] private Axis rotationAxis = Axis.Y;
    [SerializeField] private float angle = 15f;     // how much move
    [SerializeField] private float duration = 1f;   // movement speed 






    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / duration, 1f);
        float currentAngle = Mathf.Lerp(0f, angle, t);

        Vector3 euler = transform.localRotation.eulerAngles;



        // apply rotation to axis
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
