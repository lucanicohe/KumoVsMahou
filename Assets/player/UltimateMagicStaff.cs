using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicType
{
    Shadow = 10,
    Stone = 20,
    Fire = 40,
    Wind = 80,
    Ice = 160,
    Water = 320,
    Light = 640
}

public enum MagicColor
{
    Black = 10,
    Brown = 20,
    Red = 30,
    Yellow = 40,
    Cyan = 50,
    Blue = 60,
    White = 70
}

public class UltimateMagicStaff : MonoBehaviour
{
    [Header("Magic Selection")]
    [SerializeField] private MagicType currentType = MagicType.Shadow;
    [SerializeField] private MagicColor currentColor = MagicColor.Black;

    [Header("Magic Prefab")]
    [SerializeField] private GameObject magicPrefab;

    [Header("Casting Settings")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private float castRange = 50f;
    [SerializeField] private float castCooldown = 0.5f;

    private PlayerStats playerStats;
    private float castTimer = 0f;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on player!");
            enabled = false;
            return;
        }

        if (magicPrefab == null)
        {
            Debug.LogError("Magic Prefab not assigned! Staff will not fire.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (castTimer > 0)
            castTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && castTimer <= 0f)
            StartCasting();
    }

    void StartCasting()
    {
        int manaCost = (int)currentType;
        if (!playerStats.HasEnoughMana(manaCost))
        {
            Debug.Log("Not enough mana to cast!");
            return;
        }

        playerStats.ConsumeMana(manaCost);
        castTimer = castCooldown;

        GameObject magicInstance = Instantiate(magicPrefab, castPoint.position, castPoint.rotation);

        // Find all particle systems, including under disabled parents
        ParticleSystem[] allParticles = magicInstance.GetComponentsInChildren<ParticleSystem>(true);
        if (allParticles.Length > 0)
        {
            foreach (ParticleSystem ps in allParticles)
            {
                ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    Material matInstance = new Material(renderer.sharedMaterial);
                    renderer.material = matInstance;

                    Color magicColor = GetColorFromEnum(currentColor);
                    matInstance.SetColor("_ColorA", magicColor);
                    matInstance.SetColor("_ColorB", magicColor);

                    float tilingValue = GetTilingFromType(currentType);
                    Vector4 dissolveTexST = matInstance.GetVector("_DissolveTex_ST");
                    dissolveTexST.x = tilingValue;
                    dissolveTexST.y = tilingValue;
                    matInstance.SetVector("_DissolveTex_ST", dissolveTexST);
                }

                ps.Play();
            }

            float duration = allParticles[0].main.duration + allParticles[0].main.startLifetime.constantMax;
            Destroy(magicInstance, duration);
        }
        else
        {
            Debug.LogError("No ParticleSystem found in spawned prefab (including disabled ones)!");
            Destroy(magicInstance, 0.1f);
        }

        CastRaycast();
    }

    void CastRaycast()
    {
        Ray ray;
        if (Camera.main != null)
            ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        else if (castPoint != null)
            ray = new Ray(castPoint.position, castPoint.forward);
        else
        {
            Debug.LogWarning("No camera or cast point found!");
            return;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, castRange))
        {
            SpiderStats spider = hit.collider.GetComponent<SpiderStats>();
            if (spider != null)
            {
                int damage = (int)currentColor;
                spider.TakeDamage(damage);
                Debug.Log($"Hit {spider.variant} spider for {damage} damage!");
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * castRange, Color.red, 0.2f);
    }

    Color GetColorFromEnum(MagicColor c)
    {
        switch (c)
        {
            case MagicColor.Black: return new Color(0.1f, 0.1f, 0.1f);
            case MagicColor.Brown: return new Color(0.6f, 0.4f, 0.2f);
            case MagicColor.Red: return Color.red;
            case MagicColor.Yellow: return Color.yellow;
            case MagicColor.Cyan: return Color.cyan;
            case MagicColor.Blue: return Color.blue;
            case MagicColor.White: return Color.white;
            default: return Color.white;
        }
    }

    float GetTilingFromType(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return 1f;
            case MagicType.Stone: return 2f;
            case MagicType.Fire: return 3f;
            case MagicType.Wind: return 4f;
            case MagicType.Ice: return 5f;
            case MagicType.Water: return 6f;
            case MagicType.Light: return 7f;
            default: return 1f;
        }
    }
}