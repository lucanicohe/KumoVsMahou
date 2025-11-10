using UnityEngine;

public class SuperStoneRotator : MonoBehaviour
{
    public float speed = 20f; // degrees per second

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(0f, 0f, speed * Time.deltaTime);


        
    }
}