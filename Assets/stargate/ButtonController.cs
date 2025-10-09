using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public static List<ButtonController> allButtons = new List<ButtonController>();

    [Header("Button Settings")]
    [SerializeField] private string keySymbol;         
    [SerializeField] private Renderer keyRenderer;     
    [SerializeField] private bool isActivateButton;    
    [SerializeField] private float sequenceTimeout = 9f;
    [SerializeField] private float gateActiveTimeout = 30f;

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
    private Coroutine sequenceCoroutine;
    private Coroutine gateCoroutine;

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
        
        // Disable button emission at start
        DisableButtonEmission();
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

        // Enable emission on this button
        EnableButtonEmission(Color.white);

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

            foreach (var btn in allButtons)
                if (btn.isActivateButton)
                    btn.EnableButtonEmission(Color.red);

            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }

            sequenceCoroutine = StartCoroutine(SequenceTimeout());
        }
        else
        {
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

        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        ResetKeysOnly();

        foreach (char c in sequenceSnapshot)
        {
            yield return new WaitForSeconds(1f);

            switch (c)
            {
                case 'A': EnableEmission(materialA, Color.white); break;
                case 'B': EnableEmission(materialB, Color.white); break;
                case 'C': EnableEmission(materialC, Color.white); break;
                case 'D': EnableEmission(materialD, Color.white); break;
                case 'E': EnableEmission(materialE, Color.white); break;
                case 'F': EnableEmission(materialF, Color.white); break;
            }
        }

        yield return new WaitForSeconds(1f);

        // Activate the gate collider
        if (gateCollider != null)
            gateCollider.SetActive(true);

        // Start the particle effect
        if (whooshController != null)
            // whooshController.StartWhoosh();  //WHOOOOOOOOOOOOOOOOOOOOOOOOOSHSHHHHHHHHH

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
                btn.DisableButtonEmission();
            }
        }

        // Disable emission for all chevron textures
        DisableEmission(materialA);
        DisableEmission(materialB);
        DisableEmission(materialC);
        DisableEmission(materialD);
        DisableEmission(materialE);
        DisableEmission(materialF);
    }

    private void ResetAll()
    {
        ResetKeysOnly();
        activeCondition = "";
        readyToActivate = false;

        foreach (var btn in allButtons)
        {
            btn.isPressed = false;
            btn.DisableButtonEmission();
        }

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    public string GetActiveCondition() => activeCondition;
    public void ClearActiveCondition() => activeCondition = "";

    // --- Button emission (for the button's own renderer material) ---
    private void EnableButtonEmission(Color color)
    {
        if (keyRenderer == null) return;
        
        Material mat = keyRenderer.material;
        mat.EnableKeyword("_EMISSION");
        
        // Red for activate button, gray for normal buttons
        if (color == Color.red)
        {
            mat.SetColor("_EmissionColor", Color.red);
        }
        else
        {
            // Use 170/255 = 0.667 for RGB values (170, 170, 170)
            Color emissionColor = new Color(170f / 255f, 170f / 255f, 170f / 255f);
            mat.SetColor("_EmissionColor", emissionColor);
        }
    }

    private void DisableButtonEmission()
    {
        if (keyRenderer == null) return;
        
        Material mat = keyRenderer.material;
        mat.DisableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
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