using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFlicker : MonoBehaviour
{
    private Light lightComponent;
    
    [SerializeField] private float minIntensity = 500f;
    [SerializeField] private float maxIntensity = 900f;
    [SerializeField] private float flickerSpeed = 5f;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
    }

    private void Update()
    {
        float flicker = Mathf.Sin(Time.time * flickerSpeed) * 0.5f + 0.5f;
        lightComponent.intensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);
    }
}
