using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Transform activeSlot;
    [SerializeField] private Text stoneDetailsText;

    [Header("参照")]
    [SerializeField] private UltimateMagicStaff staff;

    private List<MagicStone> stonesInInventory = new List<MagicStone>();
    private MagicStone activeStone;
    private bool isUIOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        if (inventoryUIPanel != null)
            inventoryUIPanel.SetActive(false);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isUIOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }
    public void PickUpStone(MagicStone stone)
    {
        stonesInInventory.Add(stone);
        stone.gameObject.SetActive(false);
        DontDestroyOnLoad(stone.gameObject);
        Debug.Log($"石を拾った: {stone.stoneColor} {stone.stoneType}");
    }

    void OpenInventory()
    {
        inventoryUIPanel.SetActive(true);
        isUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshInventoryDisplay();
    }

    void CloseInventory()
    {
        inventoryUIPanel.SetActive(false);
        isUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RefreshInventoryDisplay()
    {
        foreach (Transform child in inventoryGrid)
            Destroy(child.gameObject);

        foreach (MagicStone stone in stonesInInventory)
        {
            GameObject stoneButton = new GameObject("StoneButton");
            stoneButton.transform.SetParent(inventoryGrid);
        }
    }

    public void SetActiveStone(MagicStone stone)
    {
        if (activeStone != null)
        {
            stonesInInventory.Add(activeStone);
            staff.DetachStone();
        }

        activeStone = stone;
        stonesInInventory.Remove(stone);
        staff.AttachStone(stone);

        Debug.Log($"アクティブな石を設定: {stone.stoneColor} {stone.stoneType}");
        RefreshInventoryDisplay();
    }

    public void AddStone(MagicStone stone)
    {
        stonesInInventory.Add(stone);
        DontDestroyOnLoad(stone.gameObject);
    }

    public void RemoveStone(MagicStone stone)
    {
        stonesInInventory.Remove(stone);
    }

    public List<MagicStone> GetAllStones()
    {
        return stonesInInventory;
    }
}
