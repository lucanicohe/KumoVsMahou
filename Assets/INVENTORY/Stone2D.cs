using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stone2D : MonoBehaviour
{
    public MagicType stoneType;
    public MagicColor stoneColor;

    public MagicStone playerStone;
    
    private Image stoneImage;
    private CanvasGroup canvasGroup;
    private Button stoneButton;
    private bool isCurrentlyClicked = false;

    // Text display
    [SerializeField] private TextMeshProUGUI typeManaCostText;
    [SerializeField] private TextMeshProUGUI colorDamageText;

    // SELECTION INDICATOR - separate object!
    [SerializeField] private Image selectionIndicatorImage;
    [SerializeField] private Color deselectedIndicatorColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color selectedIndicatorColor = Color.green;

    // Stone sprite references (assign in inspector)
    [SerializeField] private Sprite shadowSprite;
    [SerializeField] private Sprite stoneSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite windSprite;
    [SerializeField] private Sprite iceSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite lightSprite;

    void Awake()
    {
        stoneImage = GetComponent<Image>();
        if (stoneImage == null)
        {
            stoneImage = gameObject.AddComponent<Image>();
        }

        stoneButton = GetComponent<Button>();
        if (stoneButton == null)
        {
            stoneButton = gameObject.AddComponent<Button>();
        }

        stoneButton.onClick.AddListener(OnStoneClicked);

        // Set selection indicator to light grey from start
        if (selectionIndicatorImage != null)
        {
            selectionIndicatorImage.color = new Color(0.7f, 0.7f, 0.7f);
        }
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        UpdateVisuals();

        // If this is the active slot, sync with player stone AFTER everything initializes
        if (gameObject.name.Contains("ActiveSlot"))
        {
            Invoke("SyncWithPlayerStone", 0.05f);
        }
    }

    public void SyncWithPlayerStone()
    {
        if (playerStone == null)
        {
            Debug.LogWarning($"[Stone2D] PlayerStone not assigned for {gameObject.name}!");
            return;
        }

        playerStone.stoneType = ConvertTypeToTypeList(stoneType);
        playerStone.stoneColor = ConvertColorToColorList(stoneColor);
        playerStone.UpdateStoneAppearance();
        
    }



    void OnValidate()
    {
        if (Application.isPlaying)
            return;

        stoneImage = GetComponent<Image>();
        UpdateVisuals();

        // Update player stone in inspector in real-time if this is active slot
        if (gameObject.name.Contains("ActiveSlot"))
        {
            UpdatePlayerMagicStone();
        }
    }

    void UpdatePlayerMagicStone()
    {
        MagicStone playerStone = FindObjectOfType<MagicStone>();
        if (playerStone != null)
        {
            playerStone.stoneType = ConvertTypeToTypeList(stoneType);
            playerStone.stoneColor = ConvertColorToColorList(stoneColor);
            playerStone.UpdateStoneAppearance();
        }
    }

    public void SetStoneProperties(MagicType type, MagicColor color)
    {
        stoneType = type;
        stoneColor = color;
        
        if (stoneImage == null)
            stoneImage = GetComponent<Image>();
            
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (stoneImage == null)
            stoneImage = GetComponent<Image>();

        if (stoneImage == null)
        {
            Debug.LogError($"Cannot update visuals - no Image component on {gameObject.name}!");
            return;
        }

        Sprite newSprite = GetSpriteFromType(stoneType);
        if (newSprite != null)
        {
            stoneImage.sprite = newSprite;
        }

        Color newColor = GetColorFromEnum(stoneColor);
        newColor.a = 1f;
        stoneImage.color = newColor;
        stoneImage.enabled = true;
    }

    void OnStoneClicked()
    {
        // Deselect all other stones
        Stone2D[] allStones = FindObjectsOfType<Stone2D>();
        foreach (Stone2D stone in allStones)
        {
            if (stone != this)
                stone.Deselect();
        }

        // Select this stone
        Select();
    }

    public void Select()
    {
        isCurrentlyClicked = true;
        
        if (selectionIndicatorImage != null)
        {
            selectionIndicatorImage.color = Color.green;
        }
        
        CurrentlyActivelyClicked();
        Debug.Log($"Selected stone: {stoneColor} {stoneType}");
    }

    public void Deselect()
    {
        isCurrentlyClicked = false;
        
        if (selectionIndicatorImage != null)
        {
            selectionIndicatorImage.color = new Color(0.7f, 0.7f, 0.7f);
        }
        
        ClearTextDisplay();
        Debug.Log("Stone deselected!");
    }

    void CurrentlyActivelyClicked()
    {
        if (typeManaCostText != null)
        {
            string typeStr = stoneType.ToString();
            int manaCost = (int)stoneType;
            typeManaCostText.text = $"{typeStr} <color=blue>Mana use</color> {manaCost}";
        }

        if (colorDamageText != null)
        {
            string colorStr = stoneColor.ToString();
            int damage = (int)stoneColor;
            colorDamageText.text = $"{colorStr} <color=red>Damage</color> {damage}";
        }
    }

    void ClearTextDisplay()
    {
        if (typeManaCostText != null)
            typeManaCostText.text = "";

        if (colorDamageText != null)
            colorDamageText.text = "";
    }

  

    public bool IsCurrentlyClicked()
    {
        return isCurrentlyClicked;
    }

    Sprite GetSpriteFromType(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return shadowSprite;
            case MagicType.Stone: return stoneSprite;
            case MagicType.Fire: return fireSprite;
            case MagicType.Wind: return windSprite;
            case MagicType.Ice: return iceSprite;
            case MagicType.Water: return waterSprite;
            case MagicType.Light: return lightSprite;
            default: return null;
        }
    }

    Color GetColorFromEnum(MagicColor color)
    {
        switch (color)
        {
            case MagicColor.Black: return new Color(0.1f, 0.1f, 0.1f);
            case MagicColor.Brown: return new Color(0.6f, 0.4f, 0.2f);
            case MagicColor.Red: return Color.red;
            case MagicColor.Yellow: return Color.yellow;
            case MagicColor.Cyan: return Color.cyan;
            case MagicColor.Blue: return Color.blue;
            case MagicColor.White: return Color.white;
            default: return Color.white;
        }
    }

    public int GetManaCost()
    {
        return (int)stoneType;
    }

    public int GetDamage()
    {
        return (int)stoneColor;
    }

    public string GetTypeString()
    {
        return stoneType.ToString();
    }

    public string GetColorString()
    {
        return stoneColor.ToString();
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
}