using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance { get; private set; }

    [SerializeField] private Stoner stoner;
    [SerializeField] private Stone2D activeSlotStone;
    [SerializeField] private MagicStone playerMagicStone;
    [SerializeField] private Button setButton;
    [SerializeField] private Canvas inventoryCanvas;

    private bool inventoryOpen = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        if (stoner == null)
            stoner = GetComponent<Stoner>();

        if (playerMagicStone == null)
            playerMagicStone = FindObjectOfType<MagicStone>();

        if (setButton != null)
        {
            setButton.onClick.AddListener(OnSetButtonPressed);
        }

        if (inventoryCanvas == null)
        {
            Debug.LogError("Inventory Canvas not assigned!");
        }
        else
        {
            // Only disable the CANVAS, not the whole inventory
            inventoryCanvas.gameObject.SetActive(false);
            Debug.Log("Inventory Canvas hidden");
        }

        // Sync active stone with player stone on start
        if (activeSlotStone != null)
        {
            activeSlotStone.SyncWithPlayerStone();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I key pressed!");
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        Debug.Log($"Toggle called, inventoryOpen = {inventoryOpen}");

        if (inventoryCanvas != null)
        {
            inventoryCanvas.gameObject.SetActive(inventoryOpen);
            Debug.Log($"Canvas set to: {inventoryOpen}");
        }
        else
        {
            Debug.LogError("inventoryCanvas is NULL!");
            return;
        }

        // Toggle cursor
        if (inventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Inventory OPEN - cursor visible and unlocked");
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Inventory CLOSED - cursor hidden and locked");
        }
    }

    void OnSetButtonPressed()
    {
        if (stoner == null)
        {
            Debug.LogError("Stoner not assigned!");
            return;
        }

        Stone2D currentStone = stoner.GetCurrentlyActiveStone();
        
        if (currentStone == null)
        {
            Debug.LogWarning("No stone currently selected!");
            return;
        }

        if (activeSlotStone == null)
        {
            Debug.LogError("Active slot stone not assigned!");
            return;
        }

        // Store temp values
        MagicType tempType = activeSlotStone.stoneType;
        MagicColor tempColor = activeSlotStone.stoneColor;

        // Swap: active slot gets selected stone's properties
        activeSlotStone.stoneType = currentStone.stoneType;
        activeSlotStone.stoneColor = currentStone.stoneColor;
        activeSlotStone.UpdateVisuals();
        activeSlotStone.Deselect();

        // selected stone gets what active slot had
        currentStone.stoneType = tempType;
        currentStone.stoneColor = tempColor;
        currentStone.UpdateVisuals();
        currentStone.Deselect();

        // Sync player stone to active slot's new values
        activeSlotStone.SyncWithPlayerStone();

        Debug.Log($"Swapped! Active slot now: {activeSlotStone.stoneColor} {activeSlotStone.stoneType}");
    }

    MagicStone.TypeList ConvertTypeToTypeList(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return MagicStone.TypeList.shadow;
            case MagicType.Stone: return MagicStone.TypeList.stone;
            case MagicType.Fire: return MagicStone.TypeList.fire;
            case MagicType.Wind: return MagicStone.TypeList.wind;
            case MagicType.Ice: return MagicStone.TypeList.ice;
            case MagicType.Water: return MagicStone.TypeList.water;
            case MagicType.Light: return MagicStone.TypeList.light;
            default: return MagicStone.TypeList.shadow;
        }
    }

    MagicStone.ColorList ConvertColorToColorList(MagicColor color)
    {
        switch (color)
        {
            case MagicColor.Black: return MagicStone.ColorList.black;
            case MagicColor.Brown: return MagicStone.ColorList.brown;
            case MagicColor.Red: return MagicStone.ColorList.red;
            case MagicColor.Yellow: return MagicStone.ColorList.yellow;
            case MagicColor.Cyan: return MagicStone.ColorList.cyan;
            case MagicColor.Blue: return MagicStone.ColorList.blue;
            case MagicColor.White: return MagicStone.ColorList.white;
            default: return MagicStone.ColorList.black;
        }
    }

    public bool IsInventoryOpen()
    {
        return inventoryOpen;
    }
}