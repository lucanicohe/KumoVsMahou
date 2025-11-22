

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public static Inventory instance { get; private set; }

    [SerializeField] private Stoner stoner;
    [SerializeField] private MagicStone playerMagicStone;
    [SerializeField] private Canvas inventoryCanvas;


    [SerializeField] private GameObject trueInventoryEmptyObject;

    [SerializeField] private LookController lookController;



    public GameObject _forgeUI;

    private bool inventoryOpen = false;

    public Stone2D activeSlotStone;

    // ---- MOVED FROM MAGICFORGE ----
    public Stone2D forgeEmpty1;
    public Stone2D forgeEmpty2;

    public Button setButton;
    public Button forgeButton1;
    public Button forgeButton2;
    public Button combineButton;

    public Button _deletebutton;

    private bool _forgeOpen = false;
    // --------------------------------

    public void SyncPlayerStoneWithActiveSlot()
    {
        if (activeSlotStone == null || playerMagicStone == null) return;

        playerMagicStone.stoneType = playerMagicStone.ConvertTypeToTypeList(activeSlotStone.stoneType);
        playerMagicStone.stoneColor = playerMagicStone.ConvertColorToColorList(activeSlotStone.stoneColor);
        playerMagicStone.UpdateStoneAppearance();
    }

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

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (stoner == null)
            stoner = Stoner.instance;

        if (playerMagicStone == null)
            playerMagicStone = FindObjectOfType<MagicStone>();

        if (inventoryCanvas != null)
            inventoryCanvas.gameObject.SetActive(false);

        if (_forgeUI != null)
            _forgeUI.SetActive(false);

        // ---------------- forge buttons moved here ----------------

        if (setButton != null) setButton.onClick.AddListener(OnSetButtonPressed);
        if (forgeButton1 != null) forgeButton1.onClick.AddListener(() => OnForgeButtonPressed(forgeEmpty1));
        if (forgeButton2 != null) forgeButton2.onClick.AddListener(() => OnForgeButtonPressed(forgeEmpty2));
        if (combineButton != null) combineButton.onClick.AddListener(OnCombineButtonPressed);

        if (forgeEmpty1 != null) forgeEmpty1.gameObject.SetActive(false);
        if (forgeEmpty2 != null) forgeEmpty2.gameObject.SetActive(false);

        if (_deletebutton != null)
            _deletebutton.onClick.AddListener(OnDeleteButtonPressed);
    

        // -----------------------------------------------------------

        RestoreInventory();
        SyncPlayerStoneWithActiveSlot();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (stoner == null) stoner = Stoner.instance;
        stoner?.TryRebindIfNeeded();     //  bind UI slots first
        RestoreInventory();              //  restore into the correct slots

        SyncPlayerStoneWithActiveSlot(); // update player stone


        //  FIND the player stone after scene load
        if (playerMagicStone == null)
            playerMagicStone = FindObjectOfType<MagicStone>();

        //  NOW sync to the correct restored active slot
        SyncPlayerStoneWithActiveSlot();
    }

    void OnDeleteButtonPressed()
    {
        // get the selected stone from Stoner
        Stone2D selectedStone = stoner.GetCurrentlyActiveStone();
        if (selectedStone == null)
        {
            Debug.Log("No stone selected to delete.");
            return;
        }

        // disable it
        selectedStone.gameObject.SetActive(false);
        selectedStone.Deselect();

        // save updated inventory
        SaveInventory();

        Debug.Log($"Deleted stone: {selectedStone.stoneColor} {selectedStone.stoneType}");
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryCanvas != null)
            inventoryCanvas.gameObject.SetActive(inventoryOpen);

        Cursor.visible = inventoryOpen;
        Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;

        // ---- enable/disable true inventory empty object ----
        if (trueInventoryEmptyObject != null)
            trueInventoryEmptyObject.SetActive(inventoryOpen);

        // ---- ADDED: enable/disable LookController ----
        if (lookController != null)
            lookController.enabled = !inventoryOpen;
        // -----------------------------------------------------

        _forgeUI.SetActive(false);
        _forgeOpen = false;
    }



    // -------- CALLED BY MAGICFORGE NOW ----------
    public void ToggleForge()
    {
        _forgeOpen = !_forgeOpen;

        if (inventoryCanvas != null)
            inventoryCanvas.gameObject.SetActive(_forgeOpen);

        Cursor.visible = _forgeOpen;
        Cursor.lockState = _forgeOpen ? CursorLockMode.None : CursorLockMode.Locked;

        if (_forgeUI != null)
            _forgeUI.SetActive(_forgeOpen);

        if (trueInventoryEmptyObject != null)
            trueInventoryEmptyObject.SetActive(false);

        // ---- ADDED: disable LookController while forge is open ----
        if (lookController != null)
            lookController.enabled = !_forgeOpen;
        // ------------------------------------------------------------
    }


    // --------------------------------------------

    // --------- MOVED MAGICFORGE LOGIC BELOW ---------

    void OnForgeButtonPressed(Stone2D targetEmpty)
    {
        if (targetEmpty == null || stoner == null) return;

        Stone2D selectedStone = stoner.GetCurrentlyActiveStone();
        if (selectedStone == null) return;

        if (!targetEmpty.gameObject.activeSelf)
        {
            targetEmpty.gameObject.SetActive(true);
            targetEmpty.stoneType = selectedStone.stoneType;
            targetEmpty.stoneColor = selectedStone.stoneColor;
            targetEmpty.UpdateVisuals();
            selectedStone.gameObject.SetActive(false);
        }
        else
        {
            MagicType tempType = targetEmpty.stoneType;
            MagicColor tempColor = targetEmpty.stoneColor;

            targetEmpty.stoneType = selectedStone.stoneType;
            targetEmpty.stoneColor = selectedStone.stoneColor;
            targetEmpty.UpdateVisuals();

            selectedStone.stoneType = tempType;
            selectedStone.stoneColor = tempColor;
            selectedStone.UpdateVisuals();
        }

        targetEmpty.Deselect();
        selectedStone.Deselect();
        SaveInventory();
    }

    void OnSetButtonPressed()
    {
        if (activeSlotStone == null || stoner == null) return;

        Stone2D currentStone = stoner.GetCurrentlyActiveStone();
        if (currentStone == null) return;

        MagicType tempType = activeSlotStone.stoneType;
        MagicColor tempColor = activeSlotStone.stoneColor;

        activeSlotStone.stoneType = currentStone.stoneType;
        activeSlotStone.stoneColor = currentStone.stoneColor;
        activeSlotStone.UpdateVisuals();
        activeSlotStone.Deselect();

        currentStone.stoneType = tempType;
        currentStone.stoneColor = tempColor;
        currentStone.UpdateVisuals();
        currentStone.Deselect();

        SyncPlayerStoneWithActiveSlot();
        SaveInventory();
    }

    public void OnCombineButtonPressed()
    {
        if (!forgeEmpty1.gameObject.activeSelf || !forgeEmpty2.gameObject.activeSelf)
            return;

        // Get originals
        MagicType type1 = forgeEmpty1.stoneType;
        MagicColor color1 = forgeEmpty1.stoneColor;

        MagicType type2 = forgeEmpty2.stoneType;
        MagicColor color2 = forgeEmpty2.stoneColor;

        // Build the two allowed cross combinations
        List<(MagicType, MagicColor)> results = new List<(MagicType, MagicColor)>();

        // Combination A: type1 + color2
        results.Add((type1, color2));

        // Combination B: type2 + color1
        results.Add((type2, color1));

        // Randomly pick one of these 2, guaranteed NOT to be identical to the originals
        int pick = Random.Range(0, results.Count);
        MagicType finalType = results[pick].Item1;
        MagicColor finalColor = results[pick].Item2;

        // Add new stone to inventory
        stoner.PickupStone(finalType, finalColor);
        SaveInventory();

        // Clear forge slots
        forgeEmpty1.gameObject.SetActive(false);
        forgeEmpty2.gameObject.SetActive(false);
    }


    // -------- SAVE + RESTORE (unchanged) --------

    void RestoreInventory()
    {
        if (InventorySaveManager.instance == null) return;

        InventorySaveData saveData = InventorySaveManager.instance.GetSaveData();
        if (saveData == null || saveData.stones.Count == 0) return;

        if (stoner == null) stoner = Stoner.instance;
        if (stoner == null) return;

        Stone2D[] allStones = stoner.GetAllStones();
        if (allStones == null) return;

        int count = Mathf.Min(saveData.stones.Count, allStones.Length);
        for (int i = 0; i < count; i++)
        {
            StoneData data = saveData.stones[i];
            if (allStones[i] == null) continue;

            allStones[i].stoneType = data.stoneType;
            allStones[i].stoneColor = data.stoneColor;
            allStones[i].gameObject.SetActive(data.isActive);
            allStones[i].UpdateVisuals();
        }

        // AFTER restoring stones, ALSO restore active slot values:
        if (activeSlotStone != null)
        {
            activeSlotStone.stoneType = saveData.activeSlotType;
            activeSlotStone.stoneColor = saveData.activeSlotColor;
            activeSlotStone.UpdateVisuals();
        }

    }

    public void SaveInventory()
    {
        if (InventorySaveManager.instance == null || stoner == null || activeSlotStone == null) return;

        Stone2D[] allStones = stoner.GetAllStones();
        InventorySaveManager.instance.SaveStoneData(
            allStones,
            activeSlotStone.stoneType,
            activeSlotStone.stoneColor
        );
    }

    public bool IsInventoryOpen() => inventoryOpen;
}