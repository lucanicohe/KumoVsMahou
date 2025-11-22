using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 3f;
    [SerializeField] private float Gravity = -9.81f;
    [SerializeField] private float JumpHeight = 1.5f;
    [SerializeField] private AudioSource _walking;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public float PlayerSpeed
    {
        get => playerSpeed;
        set => playerSpeed = value; // allows PixelationController to animate speed
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * playerSpeed * Time.deltaTime);

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

        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }
}