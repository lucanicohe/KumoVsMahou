using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicForge : MonoBehaviour
{

    // refrences 
    [SerializeField] private GameObject forgeUIPanel;
    [SerializeField] private Dropdown typeDropdown;
    [SerializeField] private Dropdown colorDropdown;



    //which stone
    [SerializeField] private MagicStone targetStone;



    //player 
    [SerializeField] private GameObject player;



    private bool isUIOpen = false;



    void Start()
    {
        if (forgeUIPanel != null)
            forgeUIPanel.SetActive(false);

        SetupDropdowns();
    }




    void SetupDropdowns()
    {
        if (typeDropdown != null)
        {
            typeDropdown.ClearOptions();
            List<string> typeOptions = new List<string> { "shadow", "stone", "fire", "wind", "ice", "water", "light" };
            typeDropdown.AddOptions(typeOptions);
            typeDropdown.onValueChanged.AddListener(OnTypeChanged);
        }

        if (colorDropdown != null)
        {
            colorDropdown.ClearOptions();
            List<string> colorOptions = new List<string> { "black", "brown", "red", "yellow", "cyan", "blue", "white" };
            colorDropdown.AddOptions(colorOptions);
            colorDropdown.onValueChanged.AddListener(OnColorChanged);
        }
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
        if (forgeUIPanel != null)
        {
            forgeUIPanel.SetActive(true);
            isUIOpen = true;

            if (player != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (targetStone != null)
            {
                typeDropdown.value = (int)targetStone.stoneType;
                colorDropdown.value = (int)targetStone.stoneColor;
            }
        }
    }




    void CloseUI()
    {
        if (forgeUIPanel != null)
        {
            forgeUIPanel.SetActive(false);
            isUIOpen = false;

            if (player != null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }




    void OnTypeChanged(int index)
    {
        if (targetStone != null)
        {
            targetStone.stoneType = (MagicStone.TypeList)index;
            targetStone.OnValidate();
            Debug.Log($"Forge: Type changed to {targetStone.stoneType}");
        }
    }




    void OnColorChanged(int index)
    {
        if (targetStone != null)
        {
            targetStone.stoneColor = (MagicStone.ColorList)index;
            targetStone.OnValidate();
            Debug.Log($"Forge: Color changed to {targetStone.stoneColor}");
        }
    }




    void Update()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }
}