using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Blinking : MonoBehaviour
{
    [SerializeField] private Image _tensionImage; // assign your intense image here
    [SerializeField] private float blinkInterval = 0.2f; // seconds for on/off








    

    private Coroutine blinkCoroutine;

    void Start()
    {
        if (_tensionImage != null)
        {
            // start invisible
            Color c = _tensionImage.color;
            c.a = 0f;
            _tensionImage.color = c;

            // start blinking
            blinkCoroutine = StartCoroutine(BlinkImage());
        }
    }

    IEnumerator BlinkImage()
    {
        if (_tensionImage == null) yield break;

        while (true)
        {
            Color c = _tensionImage.color;
            c.a = c.a == 0f ? 1f : 0f; // hard toggle
            _tensionImage.color = c;

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;

            // make sure image is invisible
            if (_tensionImage != null)
            {
                Color c = _tensionImage.color;
                c.a = 0f;
                _tensionImage.color = c;
            }
        }
    }
}