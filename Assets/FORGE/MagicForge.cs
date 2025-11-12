using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicForge : MonoBehaviour
{
    public static MagicForge instance { get; private set; }

    private bool _insideForge = false;

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
    }

    void Update()
    {
        if (_insideForge && Input.GetKeyDown(KeyCode.E))
        {
            Inventory.instance.ToggleForge();   // <-- now Inventory handles everything
            UIController.instance.EKEY("");
        }
    }
}