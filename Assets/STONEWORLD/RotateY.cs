using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : MonoBehaviour
{
    [SerializeField] float speed = 50f;

    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
