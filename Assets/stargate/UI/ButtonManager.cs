using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class KeyButton
    {
        public Button button;             
        public Image buttonImage;         
        public Image designImage;         
        public string keySymbol;          
        public bool isActivateButton;     
        public Material chevronMaterial;  
        [HideInInspector] public bool isPressed;
    }

    [Header("All Buttons (A-F + Activate)")]
    [SerializeField] private List<KeyButton> keyButtons = new List<KeyButton>();

    [Header("Colors")]
    [SerializeField] private Color idleColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Color pressedColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color activateReadyColor = new Color(0.5f, 0.6f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource _buttonpress;
    [SerializeField] private AudioSource _error;
    [SerializeField] private AudioSource _dial;
    [SerializeField] private AudioSource _whoosh;

    [Header("Gate")]
    [SerializeField] private GameObject gateCollider;
    [SerializeField] private ParticleTimeController whooshController;

    [Header("Timings")]
    [SerializeField] private float sequenceTimeout = 9f;
    [SerializeField] private float gateActiveTimeout = 30f;

    private string currentSequence = "";
    private string activeCondition = "";
    private bool readyToActivate = false;
    private Coroutine gateCoroutine;
    private Coroutine sequenceCoroutine;

    private void Awake()
    {
        foreach (var btn in keyButtons)
        {
            btn.isPressed = false;

            if (btn.buttonImage != null)
                btn.buttonImage.color = idleColor;

            if (btn.designImage != null)
                btn.designImage.color = Color.black;

            // 🔥 ALWAYS disable emission at start
            if (btn.chevronMaterial != null)
                DisableEmission(btn.chevronMaterial);

            if (btn.isActivateButton && btn.button != null)
                btn.button.interactable = false;

            if (btn.button != null)
                btn.button.onClick.AddListener(() => PressButton(btn));
        }

        if (gateCollider != null)
            gateCollider.SetActive(false);
    }

    private void PressButton(KeyButton btn)
    {
        if (_buttonpress != null)
            _buttonpress.Play();

        if (btn.isActivateButton)
        {
            PressActivateButton(btn);
            return;
        }

        if (btn.isPressed) return;

        btn.isPressed = true;

        if (btn.buttonImage != null)
            btn.buttonImage.color = pressedColor;

        if (btn.designImage != null)
            btn.designImage.color = Color.white;

        currentSequence += btn.keySymbol;

        if (btn.chevronMaterial != null)
        {
            EnableEmission(btn.chevronMaterial, Color.white);
            if (_dial != null) _dial.Play();
        }

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        sequenceCoroutine = StartCoroutine(SequenceTimeout());

        if (currentSequence.Length > 3)
        {
            if (_error != null) _error.Play();
            ResetKeysOnly();
            return;
        }

        if (currentSequence.Length == 3)
            CheckSequence();
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

            foreach (var btn in keyButtons)
            {
                if (btn.isActivateButton)
                {
                    if (btn.buttonImage != null)
                        btn.buttonImage.color = activateReadyColor;

                    if (btn.button != null)
                        btn.button.interactable = true;
                }
            }
        }
        else
        {
            if (_error != null) _error.Play();
            ResetKeysOnly();
        }
    }

    private void PressActivateButton(KeyButton btn)
    {
        if (!readyToActivate) return;

        if (_buttonpress != null)
            _buttonpress.Play();

        StartCoroutine(ActivateGateSequence());
    }

    private IEnumerator ActivateGateSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (_whoosh != null) _whoosh.Play();

        if (gateCollider != null)
            gateCollider.SetActive(true);

        if (whooshController != null)
            whooshController.StartWhoosh();

        if (gateCoroutine != null)
            StopCoroutine(gateCoroutine);

        gateCoroutine = StartCoroutine(GateActiveTimeout());
    }

    private IEnumerator GateActiveTimeout()
    {
        yield return new WaitForSeconds(gateActiveTimeout);
        ResetAll();
    }

    private IEnumerator SequenceTimeout()
    {
        yield return new WaitForSeconds(sequenceTimeout);
        ResetKeysOnly();
    }

    private void ResetKeysOnly()
    {
        currentSequence = "";
        readyToActivate = false;

        foreach (var btn in keyButtons)
        {
            btn.isPressed = false;

            if (btn.buttonImage != null)
                btn.buttonImage.color = idleColor;

            if (btn.designImage != null)
                btn.designImage.color = Color.black;

            // 🔥 TURN EMISSION OFF ALWAYS
            if (btn.chevronMaterial != null)
                DisableEmission(btn.chevronMaterial);

            if (btn.isActivateButton && btn.button != null)
                btn.button.interactable = false;
        }
    }

    private void ResetAll()
    {
        ResetKeysOnly();
        activeCondition = "";
        readyToActivate = false;

        if (gateCollider != null)
            gateCollider.SetActive(false);
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

    public string GetActiveCondition() => activeCondition;
    public void ClearActiveCondition() => activeCondition = "";
}