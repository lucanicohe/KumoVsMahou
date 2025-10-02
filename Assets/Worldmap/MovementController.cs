using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private CharacterController playermove;
    [SerializeField] private float PlayerSpeed = 3f;
    [SerializeField] private AudioSource _walking;



    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        playermove.Move(move * PlayerSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            if (!_walking.isPlaying)
            {
                //_walking.Play();

            }
            

        }
        else
        {
            _walking.Stop(); 
        }
        
    }
}
