using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPOV : MonoBehaviour
{
    // パラメータ
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 500f;

    // 演算用変数
    private float rotationX = 0f;       // 縦方向の回転角度（首の回転）

    // ゲーム開始時に呼ばれる
    // Start is called before the first frame update
    void Start()
    {
        // カーソルを非表示＆ロック
        Cursor.lockState = CursorLockMode.Locked;   // カーソルを画面中央に固定
        Cursor.visible = false;                     // カーソルを非表示にする
    }

    // Update is called once per frame
    void Update()
    {
        // マウス入力の取得
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;  // 横のマウス移動量を取得し、感度で調整
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;  // 縦のマウス移動量を取得し、感度で調整

        // Player（体）の回転（左右）
        transform.Rotate(0, mouseX, 0);   // プレイヤー（体）の左右の回転をマウスX方向の入力に合わせて行う

        // Neck（首）の回転（上下）
        rotationX -= mouseY; // マウスY方向の入力によって縦方向の回転を更新
        rotationX = Mathf.Clamp(rotationX, -90f, -90f);   // 回転角度を指定された範囲に制限
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}
