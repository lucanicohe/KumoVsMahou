using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StoneSpiderAI : MonoBehaviour
{
    public Transform player;
    public GameObject faceSpiderObject;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float chaseSpeed = 3f;
    public float gravity = -9.81f;
    public float jumpForce = 8f; // jump power when emerging
    public ParticleSystem emergeEffect; // dirt burst, etc.

    private bool faceSpiderActivated = false;
    private bool hasEmerged = false;
    private bool isJumping = false;
    private CharacterController cc;
    private Vector3 velocity;

    // Other scripts
    public SpiderStats spiderStats;
    public FaceSpiderController faceSpiderController;
    public SpiderStats faceSpiderStats;

    private Vector3 initialPosition;
    private Vector3 hiddenPositionOffset = new Vector3(0, -1.5f, 0); // how deep it hides


    public SpiderBehavior behavior;




    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (faceSpiderObject != null)
            faceSpiderObject.SetActive(false);

        cc = GetComponent<CharacterController>();

        // hide spider underground
        //initialPosition = transform.position;
        //transform.position += hiddenPositionOffset;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!hasEmerged && distance <= detectionRadius)
        {
            StartCoroutine(Emerge());
        }

        if (!hasEmerged)
            return; // do nothing until it has jumped out

        if (distance <= attackDistance && faceSpiderObject != null && !faceSpiderActivated)
        {
            ActivateFaceSpider();
            Destroy(gameObject);
            return;
        }

        if (distance <= detectionRadius)
            ChasePlayer();

        if (!cc.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else if (isJumping)
        {
            // Stop jump when landed
            isJumping = false;
            velocity.y = 0;
        }

        cc.Move(velocity * Time.deltaTime);
    }

    IEnumerator Emerge()
    {
        hasEmerged = true;

        // Instantiate particle effect at spider position, slightly above
        if (emergeEffect != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f; // adjust 0.5f height as needed
            GameObject particleInstance = Instantiate(emergeEffect.gameObject, spawnPos, Quaternion.identity);

            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            // Get duration and destroy after particles finish
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(particleInstance, duration);
        }

        // Jump up from underground
        isJumping = true;
        velocity.y = jumpForce;

        // Wait until spider lands
        yield return new WaitForSeconds(1.5f);

        // Reset position just in case
        if (transform.position.y < initialPosition.y)
            transform.position = initialPosition;
    }

    void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);

        cc.Move(dir * chaseSpeed * Time.deltaTime);
    }

    void ActivateFaceSpider()
    {
        if (faceSpiderObject == null) return;

        faceSpiderObject.SetActive(true);

        if (spiderStats != null && faceSpiderStats != null)
        {
            faceSpiderStats.variant = spiderStats.variant;
            faceSpiderStats.player = spiderStats.player;
        }

        if (spiderStats != null && faceSpiderController != null)
        {
            faceSpiderController.SetStats(spiderStats);
        }

        faceSpiderActivated = true;
    }
}