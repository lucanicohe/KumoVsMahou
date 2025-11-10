using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private MagicStone attachedStone;

    [SerializeField] private GameObject magicPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float castRange = 50f;
    [SerializeField] private float castCooldown = 0.5f;

    // Textures
    [SerializeField] private Texture2D shadowTex;
    [SerializeField] private Texture2D stoneTex;
    [SerializeField] private Texture2D fireTex;
    [SerializeField] private Texture2D windTex;
    [SerializeField] private Texture2D iceTex;
    [SerializeField] private Texture2D waterTex;
    [SerializeField] private Texture2D lightTex;

    // Sounds
    [SerializeField] private AudioClip shadowClip;
    [SerializeField] private AudioClip stoneClip;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip windClip;
    [SerializeField] private AudioClip iceClip;
    [SerializeField] private AudioClip waterClip;
    [SerializeField] private AudioClip lightClip;

    [SerializeField] private Scrollbar manaBar;

    // 
    [SerializeField] private PlayerStats playerStats;


    //[SerializeField] private GameObject hitEffectPrefab;




    private AudioSource audioSource;
    private Animator animator;
    private float castTimer = 0f;

    void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not assigned in inspector!");
            enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Animator missing on staff!");

        if (magicPrefab == null)
        {
            Debug.LogError("Magic Prefab missing!");
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

        UpdateManaBar();
    }

    void UpdateManaBar()
    {
        if (manaBar == null) return;

        float ratio = (float)playerStats.currentMana / playerStats.maxMana;
        manaBar.size = Mathf.Clamp01(ratio);
    }

    public void AttachStone(MagicStone stone)
    {
        attachedStone = stone;
        Debug.Log($"Stone attached: {stone.stoneColor} {stone.stoneType}");
    }

    public void DetachStone()
    {
        attachedStone = null;
    }

    void StartCasting()
    {
        if (attachedStone == null)
        {
            Debug.Log("No stone attached!");
            return;
        }

        MagicType type = attachedStone.GetMagicType();
        MagicColor color = attachedStone.GetMagicColor();

        int manaCost = (int)type;

        if (!playerStats.HasEnoughMana(manaCost))
        {
            Debug.Log("Not enough mana!");
            return;
        }

        playerStats.ConsumeMana(manaCost);
        castTimer = castCooldown;

        PlayMagicSound(type);

        if (animator != null)
            animator.Play("animationtest", 0, 0f);

        SpawnMagicParticles(type, color);
        CastRaycast(color);
    }

    void SpawnMagicParticles(MagicType type, MagicColor color)
    {
        GameObject magicInstance = Instantiate(magicPrefab, castPoint.position, castPoint.rotation);
        magicInstance.transform.SetParent(castPoint);

        ParticleSystem[] particles = magicInstance.GetComponentsInChildren<ParticleSystem>(true);

        foreach (var ps in particles)
        {
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer == null) continue;

            Material mat = new Material(renderer.sharedMaterial);
            renderer.material = mat;

            Color a, b;
            GetColorPairFromEnum(color, out a, out b);
            mat.SetColor("_ColorA", a);
            mat.SetColor("_ColorB", b);

            Texture2D mainTex = GetMainTexFromType(type);
            if (mainTex != null)
                mat.SetTexture("_MainTex", mainTex);

            mat.SetVector("_ScrollSpeed", new Vector2(0, GetScrollSpeedYFromType(type)));

            ps.Play();
        }

        float duration = particles[0].main.duration + particles[0].main.startLifetime.constantMax;
        Destroy(magicInstance, duration);
    }

    void PlayMagicSound(MagicType type)
    {
        AudioClip clip = GetAudioClipFromType(type);
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    void CastRaycast(MagicColor color)
    {
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, castRange))
        {
            SpiderStats spider = hit.collider.GetComponent<SpiderStats>();
            if (spider != null)
            {
                // Pass both the spider and the hit point to the coroutine
                StartCoroutine(DelayedDamage(spider, (int)color, hit.point, 0.7f));
            }
        }

        //Debug.DrawRay(ray.origin, ray.direction * castRange, Color.red, 0.2f);
    }


    private IEnumerator DelayedDamage(SpiderStats spider, int damage, Vector3 hitPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (spider != null) // check again in case spider was destroyed
        {
            spider.TakeDamage(damage);

            
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
        }
        return null;
    }

    // Color helpers
    void GetColorPairFromEnum(MagicColor c, out Color A, out Color B)
    {
        switch (c)
        {
            case MagicColor.Black: A = new Color(0.1f, 0.1f, 0.1f); B = new Color(0.3f, 0.3f, 0.3f); break;
            case MagicColor.Brown: A = new Color(0.6f, 0.4f, 0.2f); B = new Color(0.1f, 0.1f, 0.1f); break;
            case MagicColor.Red: A = Color.red; B = Color.yellow; break;
            case MagicColor.Yellow: A = Color.yellow; B = new Color(1f, 0.6f, 0f); break;
            case MagicColor.Cyan: A = Color.cyan; B = Color.white; break;
            case MagicColor.Blue: A = Color.blue; B = Color.cyan; break;
            case MagicColor.White: A = Color.white; B = new Color(0.9f, 0.95f, 1f); break;
            default: A = Color.white; B = Color.white; break;
        }
    }

    Texture2D GetMainTexFromType(MagicType t)
    {
        switch (t)
        {
            case MagicType.Shadow: return shadowTex;
            case MagicType.Stone: return stoneTex;
            case MagicType.Fire: return fireTex;
            case MagicType.Wind: return windTex;
            case MagicType.Ice: return iceTex;
            case MagicType.Water: return waterTex;
            case MagicType.Light: return lightTex;
        }
        return null;
    }

    float GetScrollSpeedYFromType(MagicType t)
    {
        switch (t)
        {
            case MagicType.Shadow: return 1f;
            case MagicType.Stone: return 2f;
            case MagicType.Fire: return 3f;
            case MagicType.Wind: return 4f;
            case MagicType.Ice: return 5f;
            case MagicType.Water: return 6f;
            case MagicType.Light: return 7f;
        }
        return 1f;
    }
}