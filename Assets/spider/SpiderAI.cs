using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiderBehavior
{
    Normal,
    Stone,
    Wind
}

public class SpiderAI : MonoBehaviour
{
    // shared settings
    public Transform player;
    public GameObject faceSpiderObject;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float chaseSpeed = 3f;
    public float gravity = -9.81f; // fake gravity because we use CharacterController

    // stone-specific
    public float jumpForce = 8f; // jump power when emerging
    public ParticleSystem emergeEffect; // dirt burst, etc.
    private bool hasEmerged = false;
    private bool isJumping = false;

    // internal
    private bool faceSpiderActivated = false;
    private CharacterController cc;
    private Vector3 velocity;

    // other scripts
    public SpiderStats spiderStats; // chasing spider stats
    public FaceSpiderController faceSpiderController; // controller on face spider
    public SpiderStats faceSpiderStats; // stats on face spider

    public SpiderBehavior behavior;
    private Vector3 initialPosition;

    [SerializeField] private AudioSource chaseAudio;
    [SerializeField] private AudioSource _emergeAudio;
    [SerializeField] private AudioSource _teleportudio;




    //public Transform pointA; // normal gravity

    //public Transform pointB; // sideways gravity


    private Transform targetPoint; // the point the spider is currently moving toward

    //public float teleportSpeed = 5f; // speed at which it moves between points

    private bool useSidewaysGravity = false; // toggled based on point

    private float teleportTimer = 0f;   // counts time
    //public float teleportInterval = 3f; // seconds between teleports

    private bool hasTeleportedToB = false;

    public float _movex = 6f;
    public float _movey = 4f;

    public bool IsChasing; 








    void Start()
    {
        //if (player == null)
        //player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (faceSpiderObject != null)
            faceSpiderObject.SetActive(false);

        cc = GetComponent<CharacterController>();
        //if (cc == null)
        //Debug.LogWarning("[SpiderAI] No CharacterController found on " + gameObject.name);

        // IMPORTANT: remember current starting position (used by stone emerge reset)
        initialPosition = transform.position;

        //if (behavior == SpiderBehavior.Wind && !isTeleporting)
        //StartCoroutine(WindTeleportRoutine());

        if (behavior == SpiderBehavior.Wind)
        {
            //targetPoint = pointB;
            useSidewaysGravity = false;
        }

    }
    /*
    IEnumerator WindTeleportRoutine()
{
    isTeleporting = true;

    while (behavior == SpiderBehavior.Wind)
    {
        // teleport forward
        Vector3 forwardJump = transform.forward * teleportDistance;
        cc.enabled = false;  // temporarily disable CharacterController so Move() doesn't interfere
        transform.position += forwardJump;
        cc.enabled = true;

        yield return new WaitForSeconds(teleportInterval);

        // teleport backward
        Vector3 backwardJump = -transform.forward * teleportDistance;
        cc.enabled = false;
        transform.position += backwardJump;
        cc.enabled = true;

        yield return new WaitForSeconds(teleportInterval);
    }

    isTeleporting = false;
}

*/ 

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Stone-specific emergence
        if (behavior == SpiderBehavior.Stone && !hasEmerged && distance <= detectionRadius)
        {
            StartCoroutine(Emerge());
            return; // wait for emerge before doing other AI that frame
        }

        // proximity face-spider activation
        if (distance <= attackDistance && faceSpiderObject != null && !faceSpiderActivated)
        {
            ActivateFaceSpider();
            Destroy(gameObject);
            return;
        }

        // chasing
        // chasing (but skip automatic chasing for Wind — Wind handles its own teleport+chase)
        if (distance <= detectionRadius && behavior != SpiderBehavior.Wind)
            ChasePlayer();


        // Gravity logic
        if (behavior == SpiderBehavior.Stone)
        {
            if (cc != null)
            {
                if (!cc.isGrounded)
                {
                    velocity.y += gravity * Time.deltaTime;
                }
                else if (isJumping)
                {
                    isJumping = false;
                    velocity.y = 0f;
                }

                cc.Move(velocity * Time.deltaTime);
            }
        }
        else if (behavior == SpiderBehavior.Wind)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (!hasTeleportedToB && distanceToPlayer <= detectionRadius)
            {
                hasTeleportedToB = true;

                _teleportudio.Play(); 

                if (cc != null) cc.enabled = false;

                // Teleport logic using X offset and Z rotation
                Vector3 newPos = transform.position;
                newPos.x += _movex;  
                newPos.y -= _movey;  
                transform.position = newPos;

                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Z rotation 0
                velocity = Vector3.zero;

                if (cc != null) cc.enabled = true;
            }

            if (hasTeleportedToB)
            {
                ChasePlayer();

                // Apply gravity while chasing
                if (cc != null)
                {
                    if (!cc.isGrounded)
                        velocity.y += gravity * Time.deltaTime;
                    else
                        velocity.y = 0f;

                    cc.Move(velocity * Time.deltaTime);
                }
            }
        }










        else
        {
            // Normal spider
            velocity = Vector3.zero;
        }

    }

    void ChasePlayer()
    {
        IsChasing = true; 

        if (player == null || cc == null) return;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);

        cc.Move(dir * chaseSpeed * Time.deltaTime);

        if (chaseAudio != null)
    {
        if (dir.magnitude > 0.1f && !chaseAudio.isPlaying)
            chaseAudio.Play();
        else if (dir.magnitude <= 0.1f && chaseAudio.isPlaying)
            chaseAudio.Stop();
    }
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

    IEnumerator Emerge()
    {
        // mark so we don't run more than once
        hasEmerged = true;

        _emergeAudio.Play(); 

        // play particle effect a bit above current pos
        if (emergeEffect != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f; // tweak as needed
            GameObject particleInstance = Instantiate(emergeEffect.gameObject, spawnPos, Quaternion.identity);

            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            // wait-and-destroy helper
            float duration = 0f;
            if (ps != null)
                duration = ps.main.duration + ps.main.startLifetime.constantMax;
            else
                duration = 2f;

            Destroy(particleInstance, duration);
        }

        // jump up
        isJumping = true;
        velocity.y = jumpForce;

        // Wait a bit so the jump/landing can happen (tweak if needed)
        yield return new WaitForSeconds(1.5f);

        // After landing: if Y is below starting ground, clamp to starting Y
        if (transform.position.y < initialPosition.y)
        {
            Vector3 pos = transform.position;
            pos.y = initialPosition.y;
            transform.position = pos;
        }

        // ensure vertical state cleared
        isJumping = false;
        velocity.y = 0f;
    }
}