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

    [Header("Material")]
    [SerializeField] private Material spiderMaterial;

    [Header("Loot")]
    public GameObject magicStonePrefab;

    [Header("References")]
    public Transform player;
    [SerializeField] private GameObject rootSpider;

    private Renderer spiderRenderer;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        spiderRenderer = GetComponent<Renderer>();
        UpdateStats();
    }

    private void OnValidate()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (spiderRenderer == null)
            spiderRenderer = GetComponent<Renderer>();

        if (spiderMaterial != null && spiderRenderer != null)
        {
            spiderRenderer.sharedMaterial = spiderMaterial;
        }

        switch (variant)
        {
            case SpiderVariant.BrownStone:
                health = 20;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(120, 85, 60, 255));
                transform.localScale = new Vector3(1, 1, 1);
                break;

            case SpiderVariant.RedFire:
                health = 40;
                damagePerSecond = 30;
                SetMaterialColor(new Color32(180, 60, 50, 255));
                transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                break;

            case SpiderVariant.YellowWind:
                health = 80;
                damagePerSecond = 40;
                SetMaterialColor(new Color32(230, 200, 90, 255));
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;

            case SpiderVariant.CyanIce:
                health = 160;
                damagePerSecond = 50;
                SetMaterialColor(new Color32(120, 200, 210, 255));
                transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
                break;

            case SpiderVariant.BlueWater:
                health = 320;
                damagePerSecond = 60;
                SetMaterialColor(new Color32(70, 100, 200, 255));
                transform.localScale = new Vector3(1f, 0.4f, 1f);
                break;

            case SpiderVariant.WhiteBoss:
                health = 640;
                damagePerSecond = 70;
                SetMaterialColor(new Color32(220, 220, 220, 255));
                transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }

    void SetMaterialColor(Color color)
    {
        if (spiderMaterial != null)
        {
            spiderMaterial.SetColor("_Color", color);
            if (spiderMaterial.HasProperty("_BaseColor"))
                spiderMaterial.SetColor("_BaseColor", color);
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