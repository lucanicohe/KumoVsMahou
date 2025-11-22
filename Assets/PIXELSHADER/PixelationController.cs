using UnityEngine;
using System.Collections;

public class PixelationController : MonoBehaviour
{
    
    [SerializeField] private Material pixelMaterial; // URP Pixelation shader
    [SerializeField] private float minPixel = 50f;
    [SerializeField] private float maxPixel = 250f;
    [SerializeField] private float enterDuration = 2f; // smooth pixelation duration
    [SerializeField] private float resetDuration = 2f; // smooth reset duration

    
    [SerializeField] private MovementController movementController; 
    



    [SerializeField] private float maxSpeed = 3f;

    private void Start()
    {
        if (pixelMaterial != null && movementController != null)
        {
            // Instantly restore player speed at scene start
            movementController.PlayerSpeed = maxSpeed;

            // Smooth pixel reset at scene start
            StartCoroutine(ResetPixelation());
        }
    }

    /// <summary>
    /// Trigger pixelation effect when entering a gate.
    /// Stops player instantly and animates pixel size from max -> min.
    /// </summary>
    public void StartPixelation(System.Action onComplete)
    {
        if (pixelMaterial != null && movementController != null)
        {
            // Instantly stop player
            movementController.PlayerSpeed = 0f;

            // Immediately set shader to starting pixel value so effect appears instantly
            pixelMaterial.SetFloat("_PixelSize", maxPixel);

            StartCoroutine(PixelateCoroutine(onComplete));
        }
    }

    private IEnumerator PixelateCoroutine(System.Action onComplete)
    {
        float timer = 0f;

        while (timer < enterDuration)
        {
            timer += Time.deltaTime;
            float t = timer / enterDuration;

            // Animate pixelation smoothly: max -> min
            pixelMaterial.SetFloat("_PixelSize", Mathf.Lerp(maxPixel, minPixel, t));

            yield return null;
        }

        pixelMaterial.SetFloat("_PixelSize", minPixel);

        // Callback to load next scene
        onComplete?.Invoke();
    }

    private IEnumerator ResetPixelation()
    {
        float timer = 0f;

        // Start at min pixel
        pixelMaterial.SetFloat("_PixelSize", minPixel);

        while (timer < resetDuration)
        {
            timer += Time.deltaTime;
            float t = timer / resetDuration;

            // Animate pixelation smoothly: min -> max
            pixelMaterial.SetFloat("_PixelSize", Mathf.Lerp(minPixel, maxPixel, t));

            yield return null;
        }

        pixelMaterial.SetFloat("_PixelSize", maxPixel);
    }
}