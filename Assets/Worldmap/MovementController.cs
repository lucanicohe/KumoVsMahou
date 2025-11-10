using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{



    [SerializeField] private float PlayerSpeed = 3f;
    [SerializeField] private float Gravity = -9.81f;
    [SerializeField] private float JumpHeight = 1.5f;

    [SerializeField] private AudioSource _walking; 




    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;




    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 
        // if player is on ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small downward force to stick to slopes



        // input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * PlayerSpeed * Time.deltaTime);

        

        if (_walking != null)
    {
        if (isGrounded && move.magnitude > 0.1f)
        {
            if (!_walking.isPlaying)
                _walking.Play();
        }
        else
        {
            _walking.Stop();
        }
    }



        // gravity
        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        

        // jumping !!
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }
}