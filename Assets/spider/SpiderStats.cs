using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum SpiderVariant
{
    BrownStone,
    RedFire,
    YellowWind,
    Cyannlce,
    BlueWater,
    WhiteBoss,
}
public class SpiderStats : MonoBehaviour
{
    [Header("Spider Type")]
    public SpiderVariant variant;

    [Header("Stats")]
    public int health = 20;
    public int damagePerSecond = 20;

    [Header("Loot")]
    public GameObject magicStonePrefab;

    [Header("Reference")]
    public Transform player;

    [Header("Colors")]
    public Material spider;
    public Color[] albedo;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //

    }

    private void OnValidate() // variantごとにstatsを変更
    {
        UpdateStats();
        UpdateColors();
    }
    // Update is called once per frame


    private void UpdateColors()
    {
        switch (variant)
        {

        }
    }
    private void UpdateStats() //　variantごとのstats
    {
        switch (variant)
        {
            case SpiderVariant.BrownStone:
                health = 20;
                damagePerSecond = 20;
                break;
            case SpiderVariant.RedFire:
                health = 40;
                damagePerSecond = 30;
                break;
            case SpiderVariant.YellowWind:
                health = 80;
                damagePerSecond = 40;
                break;
            case SpiderVariant.Cyannlce:
                health = 160;
                damagePerSecond = 50;
                break;
            case SpiderVariant.BlueWater:
                health = 320;
                damagePerSecond = 60;
                break;
            case SpiderVariant.WhiteBoss:
                health = 640;
                damagePerSecond = 70;
                break;
        }
    }

    public void TakeDamage(int amount)  // ダメージ処理
    {
        health -= amount;
        Debug.Log(variant + "spider took" + amount + "damage.Health" + health);

        if (health <= 0)
        {
            Die();
        }
    }
    void Die()  // 死亡処理
    {
        Debug.Log(variant + "spider died!");

        if (magicStonePrefab != null)
        {
            Instantiate(magicStonePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public int GetDamage()  // ダメージ値の取得
    {
        return damagePerSecond;
    }
}
