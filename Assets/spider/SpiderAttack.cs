using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderAttack : MonoBehaviour
{
    [SerializeField] private SphereCollider Collider;
    Transform playerTr; // プレイヤーのTransform
    [SerializeField] float speed = 0.2f;
    [SerializeField] float detectionRadius = 5.0f; //プレイヤーを検知する範囲
    [SerializeField] float jumpForce; //ジャンプ力
    [SerializeField] float faceOffset; //プレイヤーの顔高さ
    [SerializeField] int damageOnHit = 10; //　攻撃力

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        Collider = gameObject.AddComponent<SphereCollider>();
        Collider.radius = detectionRadius;
        Collider.isTrigger = true; // トリガーとして動作

        PlayerStats player = GameObject.Find("PlayerObject").GetComponent<PlayerStats>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamage(damageOnHit); // ダメージを与える
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            transform.position = Vector3.MoveTowards(transform.position,playerTr.position, speed * Time.deltaTime);
        }
    }

}

    // Update is called once per frame
   
