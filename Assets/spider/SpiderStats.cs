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
    public GameObject magicStonePrefab;

    [Header("References")]
    public Transform player;
    [SerializeField] private GameObject rootSpider; 

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        UpdateStats();
    }

    private void OnValidate()
    {
        UpdateStats();
    }

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

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{variant} spider took {amount} damage. Health now {health}");

        if (health <= 0)
            Die();
    }

    public int GetDamage() => damagePerSecond;

    void Die()
    {
        Debug.Log($"{variant} spider died!");

        if (magicStonePrefab != null)
            Instantiate(magicStonePrefab, transform.position, Quaternion.identity);

        
        if (rootSpider != null)
            Destroy(rootSpider);
        else
            Destroy(gameObject);
    }
}