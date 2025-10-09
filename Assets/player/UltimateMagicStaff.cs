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

    // These modules are used for general settings and initialized in Start/OnValidate
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

        // Initialize modules from the ParticleSystem instance
        mainModule = magicParticles.main;
        emissionModule = magicParticles.emission;
        shapeModule = magicParticles.shape;

        magicParticles.Stop();
        UpdateParticleSystemColor();
        UpdateParticleSystemType(); // Update particle system based on Type
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

    // This method runs in the editor when variables change
    void OnValidate()
    {
        if (Application.isPlaying && magicParticles != null)
        {
            // Re-initialize modules if necessary within OnValidate
            mainModule = magicParticles.main;
            emissionModule = magicParticles.emission;
            shapeModule = magicParticles.shape;
            
            UpdateParticleSystemColor();
            UpdateParticleSystemType(); // Update particle system based on Type
        }
    }
    
    // Updates the particle system's appearance based on MagicType
    void UpdateParticleSystemType()
    {
        // === DECLARE THE DYNAMIC MODULES HERE ===
        if (magicParticles == null) return;
        var velocityModule = magicParticles.velocityOverLifetime;
        var sizeModule = magicParticles.sizeOverLifetime;
        var colorModule = magicParticles.colorOverLifetime;

        // === BASE SETTINGS FOR ALL BEAMS (RESET STATE) ===
        mainModule.startSpeed = 0f; 
        mainModule.startSize = 0.5f;
        mainModule.gravityModifier = 0f;
        
        // Reset Bursts: Always clear bursts by setting them to an empty array.
        emissionModule.SetBursts(new ParticleSystem.Burst[0]);
        emissionModule.rateOverTime = 20f;

        shapeModule.shapeType = ParticleSystemShapeType.Cone;
        shapeModule.angle = 5f; 
        shapeModule.scale = Vector3.one;

        // Reset dynamic modules
        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.Local;
        
        // FIX: Ensure all velocity components are set to the same MinMaxCurve mode (Two Constants).
        velocityModule.x = new ParticleSystem.MinMaxCurve(0f, 0f); 
        velocityModule.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityModule.z = new ParticleSystem.MinMaxCurve(10f, 15f); // Base forward velocity
        
        sizeModule.enabled = true;
        colorModule.enabled = true;


        switch (currentType)
        {
            case MagicType.Shadow:
                // Dark, swirling, consuming void
                mainModule.startSize = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
                mainModule.startLifetime = 1.5f;
                emissionModule.rateOverTime = 15f;
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(8f, 12f);
                // FIX: Must use the Two Constants mode for MinMaxCurve
                velocityModule.x = new ParticleSystem.MinMaxCurve(-1f, 1f); // Swirl/Wobble
                
                // Color fades from deep color to black and shrinks
                sizeModule.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                    new Keyframe(0f, 1f), new Keyframe(1f, 0.1f))); 
                colorModule.color = new ParticleSystem.MinMaxGradient(
                    new Gradient() { 
                        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) },
                        colorKeys = new GradientColorKey[] { new GradientColorKey(GetColorFromEnum(currentColor), 0f), new GradientColorKey(Color.black, 1f) }
                    }
                );
                break;

            case MagicType.Stone:
                // Heavy, short-range blast of gravel
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.5f);
                mainModule.startLifetime = 0.5f;
                mainModule.gravityModifier = 0.5f;
                emissionModule.rateOverTime = 0f; // Turn off rate emission

                // Define and set the single burst
                ParticleSystem.Burst[] stoneBurst = new ParticleSystem.Burst[1];
                stoneBurst[0] = new ParticleSystem.Burst(0f, 8);
                emissionModule.SetBursts(stoneBurst); // Set the new burst list
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(20f, 30f);
                shapeModule.angle = 20f; // Wider blast angle
                
                // Particles shrink rapidly
                sizeModule.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                    new Keyframe(0f, 1f), new Keyframe(0.3f, 0.8f), new Keyframe(1f, 0f)));
                break;

            case MagicType.Fire:
                // Intense flame jet, spirals slightly, leaves ash
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
                mainModule.startLifetime = 0.75f;
                mainModule.gravityModifier = -0.1f; // Slight upward lift
                emissionModule.rateOverTime = 50f;
                shapeModule.angle = 10f;
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(12f, 18f);
                // FIX: Must use the Two Constants mode for MinMaxCurve
                velocityModule.x = new ParticleSystem.MinMaxCurve(0.5f, 0.5f); // Spiral 
                
                // Color fades from red/orange to black (ash/smoke)
                colorModule.color = new ParticleSystem.MinMaxGradient(
                    new Gradient() { 
                        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.5f), new GradientAlphaKey(0f, 1f) },
                        colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(new Color(0.2f, 0f, 0f), 1f) }
                    }
                );
                break;

            case MagicType.Wind:
                // Razor-sharp, linear, super fast
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f); // Thin
                mainModule.startLifetime = 0.5f;
                emissionModule.rateOverTime = 70f;
                shapeModule.angle = 0f; // Pure beam
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(25f, 40f); // Very fast
                
                // Particles stretch as they move forward (Size over Lifetime)
                sizeModule.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                    new Keyframe(0f, 0.1f), new Keyframe(0.1f, 1f), new Keyframe(1f, 0.1f))); 
                break;
                
            case MagicType.Ice:
                // Crystalline shards, short trails
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1f);
                mainModule.startLifetime = 1.0f;
                emissionModule.rateOverTime = 25f;
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(15f, 25f);
                // FIX: Must use the Two Constants mode for MinMaxCurve
                velocityModule.y = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f); // Gentle wobble
                
                // Particles fade but maintain size longer for a sharp look
                colorModule.color = new ParticleSystem.MinMaxGradient(
                    new Gradient() { 
                        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.8f), new GradientAlphaKey(0f, 1f) },
                        colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(GetColorFromEnum(currentColor), 0.5f) }
                    }
                );
                break;

            case MagicType.Water:
                // Pressurized jet/spray
                mainModule.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
                mainModule.startLifetime = 1.2f;
                mainModule.gravityModifier = 0.2f; // Slight downward arc
                emissionModule.rateOverTime = 40f;
                shapeModule.angle = 8f; // Wider spray
                
                velocityModule.z = new ParticleSystem.MinMaxCurve(10f, 15f);
                
                // Particles fade to a watery translucent state
                colorModule.color = new ParticleSystem.MinMaxGradient(
                    new Gradient() { 
                        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0.3f, 1f) },
                        colorKeys = new GradientColorKey[] { new GradientColorKey(GetColorFromEnum(currentColor), 0f), new GradientColorKey(Color.white, 1f) }
                    }
                );
                break;

            case MagicType.Light:
                // Pure, focused, radiant energy beam
                mainModule.startSpeed = 0f; 
                mainModule.startSize = 3f; // Very large core
                mainModule.startLifetime = 0.2f;
                emissionModule.rateOverTime = 100f; // High density
                shapeModule.shapeType = ParticleSystemShapeType.Box;
                shapeModule.scale = new Vector3(0.1f, 0.1f, 50f); // Long, thin box shape for the main beam

                // Disable Velocity/Size/Color over Lifetime for the core beam effect
                velocityModule.enabled = false;
                sizeModule.enabled = false;
                colorModule.enabled = false;
                break;
        }

        // Ensure gravity is zeroed out for types that don't need it explicitly
        if (currentType != MagicType.Stone && currentType != MagicType.Water)
        {
            mainModule.gravityModifier = 0f;
        }
    }

    // Updates the particle system's start color based on MagicColor
    void UpdateParticleSystemColor()
    {
        Color particleColor = GetColorFromEnum(currentColor);
        mainModule.startColor = particleColor;
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