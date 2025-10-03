using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public static List<ButtonController> allButtons = new List<ButtonController>();

    [Header("Button Settings")]
    [SerializeField] private string keySymbol;         // A, B, C, ..., X
    [SerializeField] private Renderer keyRenderer;     // Button mesh to change color
    [SerializeField] private bool isActivateButton;    // True if this is the X button
    [SerializeField] private float sequenceTimeout = 9f;
    [SerializeField] private float gateActiveTimeout = 30f;

    [Header("Gate Settings")]
    [SerializeField] private GameObject gateCollider;  // Plane + collider

    private static string currentSequence = "";
    private static string activeCondition = "";
    private static bool readyToActivate = false;

    private bool isPressed = false;
    private Coroutine sequenceCoroutine;
    private Coroutine gateCoroutine;

    private void Awake()
    {
        // Prevent duplicates on reload
        if (!allButtons.Contains(this))
            allButtons.Add(this);

        SetColor(Color.white);

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    public void Touched()
    {
        if (isActivateButton)
        {
            PressActivate();
            return;
        }

        if (isPressed) return; // already pressed

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

            // Set activate button red
            foreach (var btn in allButtons)
            {
                if (btn.isActivateButton)
                    btn.SetColor(Color.red);
            }

            // Start 9-second sequence timeout
            if (sequenceCoroutine != null)
                StopCoroutine(sequenceCoroutine);
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

    private void PressActivate()
    {
        if (!readyToActivate) return;

        if (gateCollider != null)
            gateCollider.SetActive(true);

        Debug.Log("Gate activated for: " + activeCondition);

        // Stop the 9s sequence timer
        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        // Reset keys immediately so they can be pressed again
        ResetKeysOnly();

        // Start 30s gate active timeout
        if (gateCoroutine != null)
            StopCoroutine(gateCoroutine);
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
                btn.SetColor(Color.white);
            }
        }
    }

    private void ResetAll()
    {
        currentSequence = "";
        readyToActivate = false;
        activeCondition = "";

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

    public string GetActiveCondition()
    {
        return activeCondition;
    }

    public void ClearActiveCondition()
    {
        activeCondition = "";
    }
}