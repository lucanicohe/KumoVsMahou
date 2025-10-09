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

    [Header("Casting Settings")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private float castRange = 50f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem magicParticles;

    private PlayerStats playerStats;

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;

    private bool isCasting = false;
    private float manaTimer = 0f;
    private float damageTimer = 0f;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on player!");
            enabled = false;
            return;
        }

        if (magicParticles == null)
        {
            Debug.LogError("Particle System not assigned!");
            enabled = false;
            return;
        }

        mainModule = magicParticles.main;
        emissionModule = magicParticles.emission;
        shapeModule = magicParticles.shape;

        magicParticles.Stop();
        UpdateParticleSystem();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCasting();

        if (Input.GetMouseButton(0) && isCasting)
            ContinueCasting();

        if (Input.GetMouseButtonUp(0))
            StopCasting();
    }

    void StartCasting()
    {
        int manaCost = (int)currentType;
        if (!playerStats.HasEnoughMana(manaCost))
        {
            Debug.Log("Not enough mana!");
            return;
        }

        isCasting = true;
        magicParticles.Play();
        manaTimer = 0f;
        damageTimer = 0f;

        CastRaycast(); // instant first hit
    }

    void ContinueCasting()
    {
        int manaPerSecond = (int)currentType;
        manaTimer += Time.deltaTime;
        damageTimer += Time.deltaTime;

        if (manaTimer >= 1f)
        {
            if (playerStats.HasEnoughMana(manaPerSecond))
            {
                playerStats.ConsumeMana(manaPerSecond);
                manaTimer = 0f;
            }
            else
            {
                StopCasting();
                Debug.Log("Out of mana!");
                return;
            }
        }

        // Apply damage once per second
        if (damageTimer >= 1f)
        {
            CastRaycast();
            damageTimer = 0f;
        }
    }

    void StopCasting()
    {
        isCasting = false;
        magicParticles.Stop();
        manaTimer = 0f;
        damageTimer = 0f;
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

    void OnValidate()
    {
        if (Application.isPlaying && magicParticles != null)
            UpdateParticleSystem();
    }

    void UpdateParticleSystem()
    {
        Color particleColor = GetColorFromEnum(currentColor);
        mainModule.startColor = particleColor;
        // (same style adjustments as before)
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
}