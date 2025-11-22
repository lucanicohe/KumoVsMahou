using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicForge : MonoBehaviour
{
    public static MagicForge instance { get; private set; }

    [SerializeField] private LookController lookController;
    [SerializeField] private UltimateMagicStaff magicStaff;

    private bool _insideForge = false;
    private bool _forgeOpen = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _insideForge = true;
        UIController.instance.EKEY("Eキーでつかう");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _insideForge = false;
        UIController.instance.EKEY("");

        if (_forgeOpen)
            CloseForge();
    }

    void Update()
    {
        if (_insideForge && Input.GetKeyDown(KeyCode.E))
        {
            ToggleForge();
        }
    }

    private void ToggleForge()
    {
        if (_forgeOpen)
            CloseForge();
        else
            OpenForge();
    }

    private void OpenForge()
    {
        _forgeOpen = true;

        // open UI handled by Inventory
        Inventory.instance.ToggleForge();
        UIController.instance.EKEY("");

        // disable look + magic
        if (lookController != null) lookController.enabled = false;
        if (magicStaff != null) magicStaff.enabled = false;

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseForge()
    {
        _forgeOpen = false;

        // close UI handled by Inventory
        Inventory.instance.ToggleForge();

        // re-enable look + magic
        if (lookController != null) lookController.enabled = true;
        if (magicStaff != null) magicStaff.enabled = true;

        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}