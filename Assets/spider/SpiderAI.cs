using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiderBehavior
{
    Normal,
    Stone,
    Wind,
    Ice,
    Boss
}

public class SpiderAI : MonoBehaviour
{
    public Transform player;
    public GameObject faceSpiderObject;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float chaseSpeed = 3f;
    public float gravity = -9.81f;

    public float jumpForce = 8f;
    public ParticleSystem emergeEffect;
    private bool hasEmerged = false;
    private bool isJumping = false;

    private bool faceSpiderActivated = false;
    private CharacterController cc;
    private Vector3 velocity;

    public SpiderStats spiderStats;
    public FaceSpiderController faceSpiderController;
    public SpiderStats faceSpiderStats;

    public SpiderBehavior behavior;
    private Vector3 initialPosition;

    [SerializeField] private AudioSource chaseAudio;
    [SerializeField] private AudioSource _emergeAudio;
    [SerializeField] private AudioSource _teleportudio;

    public PlayerStats playerStats;
    public int iceDamagePerTick = 50;   // UNUSED NOW, ICE BEHAVIOR CHANGED
    private float iceDamageTimer = 0f;

    private bool bossShrunk = false;
    private bool hasTeleportedToB = false;
    public float _movex = 6f;
    public float _movey = 4f;

    public bool IsChasing;

    public Transform model;
    private Vector3 modelOriginalScale;
    private Vector3 modelOriginalLocalPos;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        initialPosition = transform.position;

        if (faceSpiderObject != null)
            faceSpiderObject.SetActive(false);

        if (model != null)
        {
            modelOriginalScale = model.localScale;
            modelOriginalLocalPos = model.localPosition;

            if (behavior == SpiderBehavior.Boss)
            {
                model.localScale = modelOriginalScale * 100f;
                model.localPosition = modelOriginalLocalPos;
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);

        // === BOSS ===
        if (behavior == SpiderBehavior.Boss)
        {
            if (!bossShrunk && distance <= detectionRadius)
            {
                model.localScale = modelOriginalScale;
                model.localPosition = modelOriginalLocalPos;
                bossShrunk = true;
            }

            ApplyGravity();

            if (bossShrunk && distance <= detectionRadius)
                ChaseTowardsPlayer();

            return;
        }

        // === Stone emergence ===
        if (behavior == SpiderBehavior.Stone && !hasEmerged && distance <= detectionRadius)
        {
            StartCoroutine(Emerge());
            return;
        }

        // === PROXIMITY BEHAVIOR (FACE SPIDER) ===
        if (distance <= attackDistance)
        {
            if (behavior == SpiderBehavior.Ice)
            {
                // Ice no longer does damage
                // Now just rotates and moves based on distance
                HandleIceMovement(distance);
                return;
            }

            if (faceSpiderObject != null && !faceSpiderActivated)
            {
                ActivateFaceSpider();
                Destroy(gameObject);
                return;
            }
        }

        // === NORMAL CHASE ===
        if (distance <= detectionRadius && behavior != SpiderBehavior.Wind)
        {
            if (behavior == SpiderBehavior.Ice)
            {
                HandleIceMovement(distance);
                return;
            }

            ChasePlayer();
        }

        // === STONE GRAVITY ===
        if (behavior == SpiderBehavior.Stone)
        {
            ApplyGravity();
        }

        // === WIND TELEPORT ===
        else if (behavior == SpiderBehavior.Wind)
        {
            HandleWindTeleport(distance);
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    void ApplyGravity()
    {
        if (!cc.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else if (isJumping)
        {
            isJumping = false;
            velocity.y = 0f;
        }
        cc.Move(velocity * Time.deltaTime);
    }

    void ChaseTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            cc.Move(dir * chaseSpeed * Time.deltaTime);
        }
    }

    void HandleWindTeleport(float distance)
    {
        if (!hasTeleportedToB && distance <= detectionRadius)
        {
            hasTeleportedToB = true;
            _teleportudio.Play();
            cc.enabled = false;

            Vector3 newPos = transform.position;
            newPos.x += _movex;
            newPos.y -= _movey;
            transform.position = newPos;

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            velocity = Vector3.zero;

            cc.enabled = true;
        }

        if (hasTeleportedToB)
        {
            ChasePlayer();
            ApplyGravity();
        }
    }

    // ========== NEW ICE MOVEMENT ==========
    void HandleIceMovement(float distance)
    {
        if (player == null) return;

        // Distance → speed mapping
        // Far = 0.1f, Close = 10f
        float minSpeed = 0.1f;
        float maxSpeed = 10f;

        float t = 1f - Mathf.Clamp01(distance / detectionRadius);
        float dynamicSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            cc.Move(dir * dynamicSpeed * Time.deltaTime);
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
            faceSpiderController.SetStats(spiderStats);

        faceSpiderActivated = true;
    }

    IEnumerator Emerge()
    {
        hasEmerged = true;
        _emergeAudio.Play();

        if (emergeEffect != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject particleInstance = Instantiate(emergeEffect.gameObject, spawnPos, Quaternion.identity);

            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            float duration = (ps != null) ? ps.main.duration + ps.main.startLifetime.constantMax : 2f;
            Destroy(particleInstance, duration);
        }

        isJumping = true;
        velocity.y = jumpForce;
        yield return new WaitForSeconds(1.5f);

        if (transform.position.y < initialPosition.y)
        {
            Vector3 pos = transform.position;
            pos.y = initialPosition.y;
            transform.position = pos;
        }

        isJumping = false;
        velocity.y = 0f;
    }
}