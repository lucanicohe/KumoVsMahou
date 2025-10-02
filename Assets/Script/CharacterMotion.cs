using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotion : MonoBehaviour
{
    public float moveSpeed = 5f;           // 移動速度
    public float gravity = -9.8f;        // 重力加速度
    public CharacterController controller;  // 移動に使うCharacterController

    // 演算用変数
    private Vector3 velocity;       // 加速度を保持する変数
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ローカル座標をワールド座標に変換して移動方向を計算
        Vector3 moveDirection = transform.TransformDirection(new Vector3(h, 0, v)) * moveSpeed;

        // 重力を加算
        velocity.y += gravity * Time.deltaTime;

        // 移動と重力を一度のcontroller.Moveで処理
        controller.Move((moveDirection + velocity) * Time.deltaTime);
    }
}
