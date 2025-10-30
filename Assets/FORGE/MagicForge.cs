using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicForge : MonoBehaviour
{

    // refrences 
    [Header("UI参照")]
    [SerializeField] private GameObject CombineUIPanel;
    [SerializeField] private Transform inventoryGrid;
     [SerializeField] private Transform slotA;
    [SerializeField] private Transform slotB;
    [SerializeField] private Button combineButton;
    [SerializeField] private TextMeshProUGUI detailText;

    [Header("参照")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject magicStonePrefab;

    //which stone
    [SerializeField] private MagicStone targetStone;



    //player 
    [SerializeField] private GameObject player;


    private MagicStone stoneInSlotA;
    private MagicStone stoneInSlotB;
    private bool isUIOpen = false;



    void Start()
    { 
        if (combineButton != null)
            combineButton.onClick.AddListener(CombineStones);
    }



    public void Touched()
    {
        if (isUIOpen)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }




    void OpenUI()
    {
        if (CombineUIPanel != null)
        {
            CombineUIPanel.SetActive(true);
            isUIOpen = true;

            if (player != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }




    void CloseUI()
    {
        if (CombineUIPanel != null)
        {
            CombineUIPanel.SetActive(false);
            isUIOpen = false;

            if (player != null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }


    void RefreshInventoryDisplay()
    {
        foreach (Transform child in inventoryGrid)
            Destroy(child.gameObject);

        foreach (MagicStone stone in inventory.GetAllStones())
        {
            GameObject stoneUI = Instantiate(magicStonePrefab, inventoryGrid);
        }
    }

    public void PlaceInSlotA(MagicStone stone)
    {
        stoneInSlotA = stone;
        inventory.RemoveStone(stone);
    }

    public void PlaceInSlotB(MagicStone stone)
    {
        stoneInSlotB = stone;
        inventory.RemoveStone(stone);
    }


    void CombineStones()
    {
        if (stoneInSlotA == null || stoneInSlotB == null)
        {
            Debug.Log("2つの石が必要です");
            return;
        }

        MagicStone.TypeList newType = Random.value > 0.5f ? stoneInSlotA.stoneType : stoneInSlotB.stoneType;
        MagicStone.ColorList newColor = Random.value > 0.5f ? stoneInSlotA.stoneColor : stoneInSlotB.stoneColor;

        GameObject newStone = Instantiate(magicStonePrefab);
        MagicStone newStoneScript = newStone.GetComponent<MagicStone>();
        newStoneScript.stoneType = newType;
        newStoneScript.stoneColor = newColor;

        inventory.AddStone(newStoneScript);

        Destroy(stoneInSlotA.gameObject);
        Destroy(stoneInSlotB.gameObject);
        stoneInSlotA = null;
        stoneInSlotB = null;

        RefreshInventoryDisplay();
        Debug.Log($"新しい石を作成: {newColor} {newType}");
    }



    void Update()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }
}