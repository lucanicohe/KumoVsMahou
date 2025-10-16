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
    [Header("Currently Attached Stone")]
    [SerializeField] private MagicStone attachedStone; // Shows which stone is equipped

    [Header("Magic Prefab")]
    [SerializeField] private GameObject magicPrefab;

    [Header("Casting Settings")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private float castRange = 50f;
    [SerializeField] private float castCooldown = 0.5f;

    [Header("Magic Type Textures")]
    [SerializeField] private Texture2D shadowTex;
    [SerializeField] private Texture2D stoneTex;
    [SerializeField] private Texture2D fireTex;
    [SerializeField] private Texture2D windTex;
    [SerializeField] private Texture2D iceTex;
    [SerializeField] private Texture2D waterTex;
    [SerializeField] private Texture2D lightTex;

    [Header("Magic Type Audio Clips")]
    [SerializeField] private AudioClip shadowClip;
    [SerializeField] private AudioClip stoneClip;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip windClip;
    [SerializeField] private AudioClip iceClip;
    [SerializeField] private AudioClip waterClip;
    [SerializeField] private AudioClip lightClip;

    private PlayerStats playerStats;
    private AudioSource audioSource;
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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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

        if (Input.GetKeyDown(KeyCode.C) && castTimer <= 0f)
            StartCasting();
    }

    // Public method for stones to attach themselves
    public void AttachStone(MagicStone stone)
    {
        attachedStone = stone;
        Debug.Log($"Stone attached: {stone.stoneColor} {stone.stoneType}");
    }

    // Public method to detach stone (optional - for dropping stones)
    public void DetachStone()
    {
        attachedStone = null;
        Debug.Log("Stone detached from staff");
    }

    void StartCasting()
    {
        // Check if a stone is attached
        if (attachedStone == null)
        {
            Debug.Log("No magic stone attached! Cannot cast.");
            return;
        }

        // Get the type and color from the attached stone
        MagicType currentType = attachedStone.GetMagicType();
        MagicColor currentColor = attachedStone.GetMagicColor();

        int manaCost = (int)currentType;
        if (!playerStats.HasEnoughMana(manaCost))
        {
            Debug.Log("Not enough mana to cast!");
            return;
        }

        playerStats.ConsumeMana(manaCost);
        castTimer = castCooldown;

        PlayMagicSound(currentType);

        GameObject magicInstance = Instantiate(magicPrefab, castPoint.position, castPoint.rotation);

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

                    Color colorA, colorB;
                    GetColorPairFromEnum(currentColor, out colorA, out colorB);
                    matInstance.SetColor("_ColorA", colorA);
                    matInstance.SetColor("_ColorB", colorB);

                    Texture2D mainTex = GetMainTexFromType(currentType);
                    Texture2D dissolveTex = GetDissolveTexFromType(currentType);

                    if (mainTex != null)
                        matInstance.SetTexture("_MainTex", mainTex);
                    if (dissolveTex != null)
                        matInstance.SetTexture("_DissolveTex", dissolveTex);

                    float scrollSpeedY = GetScrollSpeedYFromType(currentType);
                    Vector2 currentScrollSpeed = matInstance.GetVector("_ScrollSpeed");
                    currentScrollSpeed.y = scrollSpeedY;
                    matInstance.SetVector("_ScrollSpeed", currentScrollSpeed);

                    Debug.Log($"Casting: {currentColor} {currentType}, ColorA={colorA}, ColorB={colorB}");
                }

                ps.Play();
            }

            float duration = allParticles[0].main.duration + allParticles[0].main.startLifetime.constantMax;
            Destroy(magicInstance, duration);
        }
        else
        {
            Debug.LogError("No ParticleSystem found in spawned prefab!");
            Destroy(magicInstance, 0.1f);
        }

        CastRaycast(currentColor);
    }

    void PlayMagicSound(MagicType type)
    {
        AudioClip clip = GetAudioClipFromType(type);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    AudioClip GetAudioClipFromType(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return shadowClip;
            case MagicType.Stone: return stoneClip;
            case MagicType.Fire: return fireClip;
            case MagicType.Wind: return windClip;
            case MagicType.Ice: return iceClip;
            case MagicType.Water: return waterClip;
            case MagicType.Light: return lightClip;
            default: return null;
        }
    }

    void CastRaycast(MagicColor currentColor)
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

    void GetColorPairFromEnum(MagicColor c, out Color colorA, out Color colorB)
    {
        switch (c)
        {
            case MagicColor.Black:
                colorA = new Color(0.1f, 0.1f, 0.1f);
                colorB = new Color(0.3f, 0.3f, 0.3f);
                break;
            case MagicColor.Brown:
                colorA = new Color(0.6f, 0.4f, 0.2f);
                colorB = new Color(0.1f, 0.1f, 0.1f);
                break;
            case MagicColor.Red:
                colorA = Color.red;
                colorB = Color.yellow;
                break;
            case MagicColor.Yellow:
                colorA = Color.yellow;
                colorB = new Color(1f, 0.6f, 0f);
                break;
            case MagicColor.Cyan:
                colorA = Color.cyan;
                colorB = Color.white;
                break;
            case MagicColor.Blue:
                colorA = Color.blue;
                colorB = Color.cyan;
                break;
            case MagicColor.White:
                colorA = Color.white;
                colorB = new Color(0.9f, 0.95f, 1f);
                break;
            default:
                colorA = Color.white;
                colorB = Color.white;
                break;
        }
    }

    Texture2D GetMainTexFromType(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return shadowTex;
            case MagicType.Stone: return stoneTex;
            case MagicType.Fire: return fireTex;
            case MagicType.Wind: return windTex;
            case MagicType.Ice: return iceTex;
            case MagicType.Water: return waterTex;
            case MagicType.Light: return lightTex;
            default: return null;
        }
    }

    Texture2D GetDissolveTexFromType(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return shadowTex;
            case MagicType.Stone: return stoneTex;
            case MagicType.Fire: return fireTex;
            case MagicType.Wind: return windTex;
            case MagicType.Ice: return iceTex;
            case MagicType.Water: return waterTex;
            case MagicType.Light: return lightTex;
            default: return null;
        }
    }

    float GetScrollSpeedYFromType(MagicType type)
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