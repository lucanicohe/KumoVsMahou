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
    public SpiderVariant variant;

    public int health = 20;
    public int damagePerSecond = 20;

    [SerializeField] private Material _spiderMaterial;

    public GameObject _magicStone;

    public Transform player;
    [SerializeField] private GameObject _rootSpider;

    [SerializeField] private GameObject _stoneRoot;

    private Renderer _spiderRenderer;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        _spiderRenderer = GetComponent<Renderer>();
        UpdateStats();

        if (_stoneRoot != null)
            _stoneRoot.SetActive(false);
    }

    private void OnValidate()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (_spiderRenderer == null)
            _spiderRenderer = GetComponent<Renderer>();

        if (_spiderMaterial != null && _spiderRenderer != null)
            _spiderRenderer.sharedMaterial = _spiderMaterial;

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
        if (_spiderMaterial != null)
        {
            _spiderMaterial.SetColor("_Color", color);
            if (_spiderMaterial.HasProperty("_BaseColor"))
                _spiderMaterial.SetColor("_BaseColor", color);
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

        if (_stoneRoot != null)
            _stoneRoot.SetActive(true);

        if (_magicStone != null)
        {
            GameObject stone = Instantiate(_magicStone, transform.position, Quaternion.identity);
            MagicStone ms = stone.GetComponent<MagicStone>();
            if (ms != null)
            {
                switch (variant)
                {
                    case SpiderVariant.BrownStone: ms.stoneType = MagicStone.TypeList.stone; ms.stoneColor = MagicStone.ColorList.brown; break;
                    case SpiderVariant.RedFire:    ms.stoneType = MagicStone.TypeList.fire;  ms.stoneColor = MagicStone.ColorList.red;   break;
                    case SpiderVariant.YellowWind: ms.stoneType = MagicStone.TypeList.wind;  ms.stoneColor = MagicStone.ColorList.yellow; break;
                    case SpiderVariant.CyanIce:    ms.stoneType = MagicStone.TypeList.ice;   ms.stoneColor = MagicStone.ColorList.cyan;   break;
                    case SpiderVariant.BlueWater:  ms.stoneType = MagicStone.TypeList.water; ms.stoneColor = MagicStone.ColorList.blue;  break;
                    case SpiderVariant.WhiteBoss:  ms.stoneType = MagicStone.TypeList.light; ms.stoneColor = MagicStone.ColorList.white; break;
                }

                ms.UpdateStoneAppearance();
            }
        }

        if (_rootSpider != null)
            Destroy(_rootSpider);
        else
            Destroy(gameObject);
    }
}