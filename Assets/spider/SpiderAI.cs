using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public Transform player;
    public GameObject faceSpiderObject;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float chaseSpeed = 3f;
    public float gravity = -9.81f;

    private bool faceSpiderActivated = false;
    private CharacterController cc;
    private Vector3 velocity;

    public SpiderStats spiderStats; // chasing spider stats
    public FaceSpiderController faceSpiderController; // controller on face spider
    public SpiderStats faceSpiderStats; // stats on face spider

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (faceSpiderObject != null)
            faceSpiderObject.SetActive(false);

        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

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
        else
            velocity.y = 0;

        cc.Move(velocity * Time.deltaTime);
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