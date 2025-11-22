using UnityEngine;

public class dhdUI : MonoBehaviour
{
    [SerializeField] private GameObject dhdUIObject;
    [SerializeField] private LookController lookController;
    [SerializeField] private UltimateMagicStaff magicStaff;

    private bool uiOpen = false;
    private bool _nearGate = false;

    private void Start()
    {
        dhdUIObject.SetActive(false);
    }

    private void Update()
    {
        // If player is near the gate & presses E → toggle UI
        if (_nearGate && Input.GetKeyDown(KeyCode.E))
        {
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        uiOpen = !uiOpen;
        dhdUIObject.SetActive(uiOpen);

        if (uiOpen)
        {
            // unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // disable player look
            if (lookController != null)
                lookController.enabled = false;

            // disable magic
            if (magicStaff != null)
                magicStaff.enabled = false;

            // hide the "E to use" text
            UIController.instance.EKEY("");
        }
        else
        {
            // lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // enable look
            if (lookController != null)
                lookController.enabled = true;

            // enable magic
            if (magicStaff != null)
                magicStaff.enabled = true;

            // if still near gate, show E again
            if (_nearGate)
                UIController.instance.EKEY("Eキーでつかう");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _nearGate = true;
        UIController.instance.EKEY("Eキーでつかう");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _nearGate = false;
        UIController.instance.EKEY("");

        // If leaving while UI was open, close the UI automatically
        if (uiOpen)
            ToggleUI();
    }
}