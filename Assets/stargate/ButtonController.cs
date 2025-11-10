using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public static List<ButtonController> allButtons = new List<ButtonController>();

    // buttons 
    [SerializeField] private string keySymbol;
    [SerializeField] private bool isActivateButton;
    [SerializeField] private float sequenceTimeout = 9f;
    [SerializeField] private float gateActiveTimeout = 30f;

    // visual components 
    [SerializeField] private SpriteRenderer buttonSpriteRenderer;
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Light buttonLight;

    // unified light color
    private Color lightBlue = new Color(0.5f, 0.6f, 1f);

    //audio SE
    [SerializeField] private AudioSource _buttonpress; 
    [SerializeField] private AudioSource _error;
    [SerializeField] private AudioSource _dial;
    [SerializeField] private AudioSource _whoosh;

    // gate settings 
    [SerializeField] private GameObject gateCollider;
    [SerializeField] private ParticleTimeController whooshController;

    //textures 
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
        //make sure all chevron materials start without emission
        DisableEmission(materialA);
        DisableEmission(materialB);
        DisableEmission(materialC);
        DisableEmission(materialD);
        DisableEmission(materialE);
        DisableEmission(materialF);

        // set button sprite to black at start
        if (!isActivateButton && buttonSpriteRenderer != null && blackSprite != null)
        {
            buttonSpriteRenderer.sprite = blackSprite;
        }

        // unified button light color + base intensity
        if (buttonLight != null)
        {
            buttonLight.enabled = true;
            buttonLight.color = lightBlue;
            buttonLight.intensity = 0.5f;  // idle
        }
    }

    public void Touched()
    {
        // play the button press sound
        if (_buttonpress != null)
        {
            _buttonpress.Play();
        }

        if (isActivateButton)
        {
            StartCoroutine(ActivateGateSequence(currentSequence));
            return;
        }

        if (isPressed) return;

        isPressed = true;
        currentSequence += keySymbol;

        // change intensity only
        SetButtonActive(true);

        // enable corresponding chevron emission with delay
        chevronCoroutine = StartCoroutine(EnableChevronWithDelay(keySymbol, 0.5f));

        // if sequence length exceeds 3, treat as wrong sequence
        if (currentSequence.Length > 3)
        {
            if (_error != null)
                _error.Play();

            ResetKeysOnly();
            return;
        }

        if (currentSequence.Length == 3)
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

            // activate button uses intensity = 1
            foreach (var btn in allButtons)
            {
                if (btn.isActivateButton && btn.buttonLight != null)
                {
                    btn.buttonLight.color = lightBlue;
                    btn.buttonLight.intensity = 1f;
                }
            }
        }
        else
        {
            if (_error != null)
                _error.Play();

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

        yield return new WaitForSeconds(1f);

        if (_whoosh != null)
            _whoosh.Play();

        if (gateCollider != null)
            gateCollider.SetActive(true);

        if (whooshController != null)
            whooshController.StartWhoosh();

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

                if (btn.chevronCoroutine != null)
                {
                    btn.StopCoroutine(btn.chevronCoroutine);
                    btn.chevronCoroutine = null;
                }
            }

            // reset activate button intensity
            if (btn.isActivateButton && btn.buttonLight != null)
            {
                btn.buttonLight.color = lightBlue;
                btn.buttonLight.intensity = 0.5f;
            }

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

    private void SetButtonActive(bool active)
    {
        // all buttons use same blue + only intensity changes
        if (buttonLight != null)
        {
            buttonLight.color = lightBlue;
            buttonLight.intensity = active ? 1f : 0.5f;
        }

        // sprite logic unchanged
        if (!isActivateButton && buttonSpriteRenderer != null)
        {
            buttonSpriteRenderer.sprite = active ? whiteSprite : blackSprite;
        }
    }

    private IEnumerator EnableChevronWithDelay(string key, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableChevronForKey(key);
    }

    private void EnableChevronForKey(string key)
    {
        if (_dial != null)
            _dial.Play();

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