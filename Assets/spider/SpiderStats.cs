using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiderVariant
{
    BrownStone,
    RedFire,
    YellowWind,
    CyanIce,
    BlueWater,
    WhiteBoss
}

public class SpiderStats : MonoBehaviour
{
    [Header("Spider Type")]
    public SpiderVariant variant;

    [Header("Stats")]
    public int health = 20;
    public int damagePerSecond = 20;

    [Header("Loot")]
    public GameObject magicStonePrefab; // このスパイダーがドロップするストーン

    [Header("References")]
    public Transform player; // プレイヤーの位置
    [SerializeField] private GameObject rootSpider; 

    void Start()
    {
        // プレイヤーを探す
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        UpdateStats();
    }

    // Inspectorで値を変更したときに自動的にstatsを更新
    private void OnValidate()
    {
        UpdateStats();
    }

    // variantごとにstatsを自動設定
    private void UpdateStats()
    {
        switch (variant)
        {
            case SpiderVariant.BrownStone:
                health = 20; damagePerSecond = 20; break;
            case SpiderVariant.RedFire:
                health = 40; damagePerSecond = 30; break;
            case SpiderVariant.YellowWind:
                health = 80; damagePerSecond = 40; break;
            case SpiderVariant.CyanIce:
                health = 160; damagePerSecond = 50; break;
            case SpiderVariant.BlueWater:
                health = 320; damagePerSecond = 60; break;
            case SpiderVariant.WhiteBoss:
                health = 640; damagePerSecond = 70; break;
        }
    }

    // ダメージを受ける (UltimateMagicStaffから呼ばれる)
    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{variant} spider took {amount} damage. Health now {health}");

        if (health <= 0)
            Die();
    }

    // ダメージ値を取得（AIスクリプトが使用）
    public int GetDamage() => damagePerSecond;

    // 死亡処理
    void Die()
    {
        Debug.Log($"{variant} spider died!");

        // 魔法ストーンをドロップ
        if (magicStonePrefab != null)
            Instantiate(magicStonePrefab, transform.position, Quaternion.identity);

        
        // このスパイダーを破棄
        if (rootSpider != null)
            Destroy(rootSpider);
        else
            Destroy(gameObject);
    }
}