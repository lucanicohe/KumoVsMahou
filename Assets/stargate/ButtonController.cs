using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public static List<ButtonController> allButtons = new List<ButtonController>();

    [Header("Button Settings")]
    [SerializeField] private string keySymbol;
    [SerializeField] private bool isActivateButton;
    [SerializeField] private float sequenceTimeout = 9f;
    [SerializeField] private float gateActiveTimeout = 30f;

    [Header("Button Visual Components")]
    [SerializeField] private SpriteRenderer buttonSpriteRenderer;
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Light buttonLight;

    [Header("Gate Settings")]
    [SerializeField] private GameObject gateCollider;
    [SerializeField] private ParticleTimeController whooshController;

    [Header("Sequence Textures")]
    [SerializeField] private Material materialA;
    [SerializeField] private Material materialB;
    [SerializeField] private Material materialC;
    [SerializeField] private Material materialD;
    [SerializeField] private Material materialE;
    [SerializeField] private Material materialF;

    private static string currentSequence = "";
    private static string activeCondition = "";
    private static bool readyToActivate = false;

    private bool isPressed = false;
    private Coroutine gateCoroutine;
    private Coroutine chevronCoroutine;

    private void Awake()
    {
        if (allButtons == null)
            allButtons = new List<ButtonController>();

        if (!allButtons.Contains(this))
            allButtons.Add(this);

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    private void OnDestroy()
    {
        allButtons.Remove(this);
    }

    private void Start()
    {
        // Ensure all chevron materials start without emission
        DisableEmission(materialA);
        DisableEmission(materialB);
        DisableEmission(materialC);
        DisableEmission(materialD);
        DisableEmission(materialE);
        DisableEmission(materialF);
        
        // Set button sprite to black and disable light at start
        if (!isActivateButton && buttonSpriteRenderer != null && blackSprite != null)
        {
            buttonSpriteRenderer.sprite = blackSprite;
        }
        
        if (buttonLight != null)
        {
            buttonLight.enabled = false;
        }
    }

    public void Touched()
    {
        if (isActivateButton)
        {
            StartCoroutine(ActivateGateSequence(currentSequence));
            return;
        }

        if (isPressed) return;

        isPressed = true;
        currentSequence += keySymbol;

        // Change sprite to white and enable light
        SetButtonActive(true);

        // Enable corresponding chevron emission with delay
        chevronCoroutine = StartCoroutine(EnableChevronWithDelay(keySymbol, 0.5f));

        if (currentSequence.Length >= 3)
        {
            CheckSequence();
        }
    }

    private void CheckSequence()
    {
        bool valid = true;

        switch (currentSequence)
        {
            case "ABC": activeCondition = "home"; break;
            case "BDF": activeCondition = "stone"; break;
            case "CEA": activeCondition = "fire"; break;
            case "FBD": activeCondition = "wind"; break;
            case "DAE": activeCondition = "ice"; break;
            case "ECF": activeCondition = "water"; break;
            case "FAC": activeCondition = "boss"; break;
            default: valid = false; break;
        }

        if (valid)
        {
            readyToActivate = true;

            // Enable light for activate button
            foreach (var btn in allButtons)
            {
                if (btn.isActivateButton && btn.buttonLight != null)
                {
                    btn.buttonLight.enabled = true;
                }
            }
        }
        else
        {
            // Wrong sequence - reset everything
            ResetKeysOnly();
        }
    }

    private IEnumerator SequenceTimeout()
    {
        yield return new WaitForSeconds(sequenceTimeout);
        ResetKeysOnly();
    }

    private IEnumerator ActivateGateSequence(string sequenceSnapshot)
    {
        if (!readyToActivate || string.IsNullOrEmpty(sequenceSnapshot))
            yield break;

        // Chevrons are already lit from button presses, just activate the gate
        yield return new WaitForSeconds(1f);

        // Activate the gate collider
        if (gateCollider != null)
            gateCollider.SetActive(true);

        // Start the particle effect
        if (whooshController != null)
            // whooshController.StartWhoosh();  //WHOOOOOOOOOOOOOOOOOOOOOOOOOSHSHHHHHHHHH

        // Start the 30 second gate timeout
        if (gateCoroutine != null)
        {
            StopCoroutine(gateCoroutine);
            gateCoroutine = null;
        }

        gateCoroutine = StartCoroutine(GateActiveTimeout());
    }

    private IEnumerator GateActiveTimeout()
    {
        yield return new WaitForSeconds(gateActiveTimeout);
        ResetAll();
    }

    private void ResetKeysOnly()
    {
        currentSequence = "";
        readyToActivate = false;

        foreach (var btn in allButtons)
        {
            if (!btn.isActivateButton)
            {
                btn.isPressed = false;
                btn.SetButtonActive(false);
                
                // Stop any pending chevron coroutines
                if (btn.chevronCoroutine != null)
                {
                    btn.StopCoroutine(btn.chevronCoroutine);
                    btn.chevronCoroutine = null;
                }
            }
            
            // Disable activate button light when resetting
            if (btn.isActivateButton && btn.buttonLight != null)
                btn.buttonLight.enabled = false;
            
            // Disable emission for all chevron textures on this button instance
            btn.DisableEmission(btn.materialA);
            btn.DisableEmission(btn.materialB);
            btn.DisableEmission(btn.materialC);
            btn.DisableEmission(btn.materialD);
            btn.DisableEmission(btn.materialE);
            btn.DisableEmission(btn.materialF);
        }
    }

    private void ResetAll()
    {
        ResetKeysOnly();
        activeCondition = "";
        readyToActivate = false;

        foreach (var btn in allButtons)
        {
            btn.isPressed = false;
            btn.SetButtonActive(false);
        }

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    public string GetActiveCondition() => activeCondition;
    public void ClearActiveCondition() => activeCondition = "";

    // --- Button sprite and light control ---
    private void SetButtonActive(bool active)
    {
        // For activate button, only control light
        if (isActivateButton)
        {
            if (buttonLight != null)
                buttonLight.enabled = active;
            return;
        }

        // For regular buttons, swap sprite and control light
        if (buttonSpriteRenderer != null)
        {
            buttonSpriteRenderer.sprite = active ? whiteSprite : blackSprite;
        }

        if (buttonLight != null)
        {
            buttonLight.enabled = active;
        }
    }

    // --- Enable chevron emission with delay ---
    private IEnumerator EnableChevronWithDelay(string key, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableChevronForKey(key);
    }

    // --- Enable chevron emission when button is pressed ---
    private void EnableChevronForKey(string key)
    {
        switch (key)
        {
            case "A": EnableEmission(materialA, Color.white); break;
            case "B": EnableEmission(materialB, Color.white); break;
            case "C": EnableEmission(materialC, Color.white); break;
            case "D": EnableEmission(materialD, Color.white); break;
            case "E": EnableEmission(materialE, Color.white); break;
            case "F": EnableEmission(materialF, Color.white); break;
        }
    }

    // --- Chevron emission helpers ---
    private void EnableEmission(Material mat, Color color)
    {
        if (mat == null) return;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color);
    }

    private void DisableEmission(Material mat)
    {
        if (mat == null) return;
        mat.DisableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
    }
}