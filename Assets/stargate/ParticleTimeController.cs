using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleTimeController : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem ps;

    [Header("Whoosh Timing")]
    [SerializeField] private float fullSpeedDuration = 1f; // 1s at full speed
    [SerializeField] private float fadeDuration = 1f;      // 1s fading to 0
    [SerializeField] private float startSpeed = 1f;
    [SerializeField] private float endSpeed = 0f;

    private ParticleSystem.VelocityOverLifetimeModule velModule;
    private float timer = 0f;
    private bool finished = true;

    void Awake()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        velModule = ps.velocityOverLifetime;
        velModule.enabled = true;

        ps.Stop();
        enabled = false; // initially disabled
    }

    void Update()
    {
        if (finished) return;

        timer += Time.deltaTime;

        if (timer <= fullSpeedDuration)
        {
            velModule.speedModifier = new ParticleSystem.MinMaxCurve(startSpeed);
        }
        else if (timer <= fullSpeedDuration + fadeDuration)
        {
            float t = (timer - fullSpeedDuration) / fadeDuration;
            float value = Mathf.Lerp(startSpeed, endSpeed, t);
            velModule.speedModifier = new ParticleSystem.MinMaxCurve(value);
        }
        else
        {
            velModule.speedModifier = new ParticleSystem.MinMaxCurve(endSpeed);
            finished = true;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            enabled = false;
        }
    }

    // Call this from ButtonController
    public void StartWhoosh()
    {
        timer = 0f;
        finished = false;
        velModule.speedModifier = new ParticleSystem.MinMaxCurve(startSpeed);
        ps.Play();
        enabled = true;
    }
}