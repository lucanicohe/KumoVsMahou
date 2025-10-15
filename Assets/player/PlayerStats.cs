using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Limits")]
    public int maxHealth = 320;
    public int maxMana = 320;

    [Header("Current Stats")]
    public int currentHealth;
    public int currentMana;

    //　攻撃した後のフリーズ時間
    [SerializeField]
    private float attackCooldown = 0.5f;
    private bool isCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        // 初期化：最大値で開始
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    // マナが足りるか確認
    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    // マナを消費
    public void ConsumeMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;

        Debug.Log("Mana consumed: " + amount + " | Remaining: " + currentMana);
    }

    // ダメージを受ける
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Took damage: " + amount + " | Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // プレイヤー死亡処理
    void Die()
    {
        Debug.Log("Player died!");
        // ゲームオーバー処理をここに追加
        // 例: SceneManager.LoadScene("GameOver");
    }
}
