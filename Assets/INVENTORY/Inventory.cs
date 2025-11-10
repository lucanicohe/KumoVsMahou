using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(gameObject);
        Debug.Log("Inventory persisting across scenes");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (stoner == null)
            stoner = GetComponent<Stoner>() ?? Stoner.instance;

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
            inventoryCanvas.gameObject.SetActive(false);
            Debug.Log("Inventory Canvas hidden");
        }

        // Restore inventory from save (best-effort). This will also rely on Stoner.GetAllStones
        RestoreInventory();

        // Sync active stone with player stone
        if (activeSlotStone != null)
        {
            activeSlotStone.SyncWithPlayerStone();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When a new scene loads we attempt to rebind inventory UI and restore saved data.
        if (stoner == null) stoner = Stoner.instance;
        if (stoner != null)
        {
            // Try to rebind UI slots (Stoner will attempt best-effort)
            // and then restore from save
            RestoreInventory();
        }

        if (stoner == null) stoner = Stoner.instance;

    if (stoner != null)
        stoner.TryRebindIfNeeded(); // force rebinding slots

    RestoreInventory();           // then restore from save

        // Sync the active slot to MagicStone **after restore**
        if (activeSlotStone != null)
            activeSlotStone.SyncWithPlayerStone();
        
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

        if (inventoryCanvas != null)
        {
            inventoryCanvas.gameObject.SetActive(inventoryOpen);
        }
        else
        {
            Debug.LogError("inventoryCanvas is NULL!");
            return;
        }

        if (inventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void RestoreInventory()
    {
        if (InventorySaveManager.instance == null) return;

        InventorySaveData saveData = InventorySaveManager.instance.GetSaveData();
        if (saveData == null || saveData.stones.Count == 0) return;

        if (stoner == null) stoner = Stoner.instance;
        if (stoner == null)
        {
            Debug.LogWarning("RestoreInventory: Stoner missing, skipping restore for now.");
            return;
        }

        Stone2D[] allStones = stoner.GetAllStones();
        if (allStones == null)
        {
            Debug.LogWarning("RestoreInventory: No Stone2D slots available in Stoner.");
            return;
        }

        int count = Mathf.Min(saveData.stones.Count, allStones.Length);
        for (int i = 0; i < count; i++)
        {
            StoneData data = saveData.stones[i];
            if (allStones[i] == null)
                continue;

            allStones[i].stoneType = data.stoneType;
            allStones[i].stoneColor = data.stoneColor;
            allStones[i].gameObject.SetActive(data.isActive);
            allStones[i].UpdateVisuals();
        }

        if (activeSlotStone != null)
        {
            activeSlotStone.stoneType = saveData.activeSlotType;
            activeSlotStone.stoneColor = saveData.activeSlotColor;
            activeSlotStone.UpdateVisuals();
        }

        Debug.Log($"Restored inventory with {count} stones");
    }

    void OnSetButtonPressed()
    {
        if (stoner == null)
        {
            stoner = Stoner.instance;
            if (stoner == null)
            {
                Debug.LogError("Stoner not assigned!");
                return;
            }
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

        // Save to file
        SaveInventory();
        activeSlotStone.SyncWithPlayerStone();

        Debug.Log($"Swapped! Active slot now: {activeSlotStone.stoneColor} {activeSlotStone.stoneType}");
    }

    public void SaveInventory()
    {
        if (InventorySaveManager.instance == null) return;
        if (stoner == null || activeSlotStone == null) return;

        Stone2D[] allStones = stoner.GetAllStones();
        InventorySaveManager.instance.SaveStoneData(allStones, activeSlotStone.stoneType, activeSlotStone.stoneColor);
    }

    public bool IsInventoryOpen()
    {
        return inventoryOpen;
    }
}