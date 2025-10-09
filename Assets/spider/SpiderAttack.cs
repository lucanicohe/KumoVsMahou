using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderAttack : MonoBehaviour
{
    Transform playerTr; // プレイヤーのTransform
    [SerializeField] float speed = 0.2f;
    //　攻撃した後のフリーズ時間
    [SerializeField]
    private float attackCooldown = 0.5f;

    public void TakeDamage(int amount)
    {
        // ここに被ダメージ時の具体的な処理
    }
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

}

    // Update is called once per frame
   
