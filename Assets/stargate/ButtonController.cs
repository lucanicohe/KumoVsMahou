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
    [SerializeField] private ParticleTimeController whooshController; // assign existing whoosh script in inspector

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

        SetColor(Color.white);

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    private void OnDestroy()
    {
        allButtons.Remove(this);
    }

    private void Start()
    {
        if (materialA != null) materialA.color = Color.white;
        if (materialB != null) materialB.color = Color.white;
        if (materialC != null) materialC.color = Color.white;
        if (materialD != null) materialD.color = Color.white;
        if (materialE != null) materialE.color = Color.white;
        if (materialF != null) materialF.color = Color.white;
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
        SetColor(Color.yellow);

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
                    btn.SetColor(Color.red);

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
                case 'A': if (materialA != null) materialA.color = Color.yellow; break;
                case 'B': if (materialB != null) materialB.color = Color.yellow; break;
                case 'C': if (materialC != null) materialC.color = Color.yellow; break;
                case 'D': if (materialD != null) materialD.color = Color.yellow; break;
                case 'E': if (materialE != null) materialE.color = Color.yellow; break;
                case 'F': if (materialF != null) materialF.color = Color.yellow; break;
            }
        }

        yield return new WaitForSeconds(1f);

        if (gateCollider != null)
            gateCollider.SetActive(true);

        // Activate existing whoosh effect
        if (whooshController != null)
        {
            whooshController.StartWhoosh();
        }

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
            if (!btn.isActivateButton)
            {
                btn.isPressed = false;
                btn.SetColor(Color.white);
            }

        if (materialA != null) materialA.color = Color.white;
        if (materialB != null) materialB.color = Color.white;
        if (materialC != null) materialC.color = Color.white;
        if (materialD != null) materialD.color = Color.white;
        if (materialE != null) materialE.color = Color.white;
        if (materialF != null) materialF.color = Color.white;
    }

    private void ResetAll()
    {
        ResetKeysOnly();
        activeCondition = "";
        readyToActivate = false;

        foreach (var btn in allButtons)
        {
            btn.isPressed = false;
            btn.SetColor(Color.white);
        }

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    public void SetColor(Color c)
    {
        if (keyRenderer != null)
            keyRenderer.material.color = c;
    }

    public string GetActiveCondition() => activeCondition;

    public void ClearActiveCondition() => activeCondition = "";
}