using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStone : MonoBehaviour
{
    public TypeList stoneType;
    public ColorList stoneColor;

    [SerializeField] private GameObject shadowModel;
    [SerializeField] private GameObject stoneModel;
    [SerializeField] private GameObject fireModel;
    [SerializeField] private GameObject windModel;
    [SerializeField] private GameObject iceModel;
    [SerializeField] private GameObject waterModel;
    [SerializeField] private GameObject lightModel;

    [SerializeField] private Material stoneMaterial;

    private MeshRenderer stoneRenderer;

    public enum TypeList
    {
        shadow, stone, fire, wind, ice, water, light
    }

    public enum ColorList
    {
        black, brown, red, yellow, cyan, blue, white
    }

    void Start()
    {
        // Delay sync to ensure Stone2D has initialized
        Invoke("SyncFromActiveSlot", 0.1f);
        UpdateStoneAppearance();
    }

    void SyncFromActiveSlot()
    {
        Stone2D activeSlot = FindObjectOfType<Stone2D>();
        if (activeSlot != null && activeSlot.gameObject.name.Contains("ActiveSlot"))
        {
            stoneType = ConvertTypeToTypeList(activeSlot.stoneType);
            stoneColor = ConvertColorToColorList(activeSlot.stoneColor);
            UpdateStoneAppearance();
            Debug.Log($"MagicStone synced from active slot: {stoneColor} {stoneType}");
        }
    }

    public void OnValidate()
    {
        UpdateStoneAppearance();
    }

    TypeList ConvertTypeToTypeList(MagicType type)
    {
        switch (type)
        {
            case MagicType.Shadow: return TypeList.shadow;
            case MagicType.Stone: return TypeList.stone;
            case MagicType.Fire: return TypeList.fire;
            case MagicType.Wind: return TypeList.wind;
            case MagicType.Ice: return TypeList.ice;
            case MagicType.Water: return TypeList.water;
            case MagicType.Light: return TypeList.light;
            default: return TypeList.shadow;
        }
    }

    ColorList ConvertColorToColorList(MagicColor color)
    {
        switch (color)
        {
            case MagicColor.Black: return ColorList.black;
            case MagicColor.Brown: return ColorList.brown;
            case MagicColor.Red: return ColorList.red;
            case MagicColor.Yellow: return ColorList.yellow;
            case MagicColor.Cyan: return ColorList.cyan;
            case MagicColor.Blue: return ColorList.blue;
            case MagicColor.White: return ColorList.white;
            default: return ColorList.black;
        }
    }

    public void UpdateStoneAppearance()
    {
        ActivateModelForType(stoneType);
        UpdateMaterialColor(stoneColor);
    }

    void ActivateModelForType(TypeList type)
    {
        if (shadowModel) shadowModel.SetActive(false);
        if (stoneModel) stoneModel.SetActive(false);
        if (fireModel) fireModel.SetActive(false);
        if (windModel) windModel.SetActive(false);
        if (iceModel) iceModel.SetActive(false);
        if (waterModel) waterModel.SetActive(false);
        if (lightModel) lightModel.SetActive(false);

        switch (type)
        {
            case TypeList.shadow:
                if (shadowModel) shadowModel.SetActive(true);
                break;
            case TypeList.stone:
                if (stoneModel) stoneModel.SetActive(true);
                break;
            case TypeList.fire:
                if (fireModel) fireModel.SetActive(true);
                break;
            case TypeList.wind:
                if (windModel) windModel.SetActive(true);
                break;
            case TypeList.ice:
                if (iceModel) iceModel.SetActive(true);
                break;
            case TypeList.water:
                if (waterModel) waterModel.SetActive(true);
                break;
            case TypeList.light:
                if (lightModel) lightModel.SetActive(true);
                break;
        }
    }

    void UpdateMaterialColor(ColorList color)
    {
        stoneRenderer = GetComponentInChildren<MeshRenderer>();
        if (stoneRenderer == null || stoneMaterial == null) return;

        Material matInstance = new Material(stoneMaterial);
        stoneRenderer.material = matInstance;

        Color emissionColor = GetBaseColor(color);

        matInstance.EnableKeyword("_EMISSION");
        matInstance.SetColor("_EmissionColor", emissionColor);
    }

    Color GetBaseColor(ColorList color)
    {
        switch (color)
        {
            case ColorList.black: return new Color(0.0f, 0.0f, 0.0f);
            case ColorList.brown: return new Color(0.6f, 0.4f, 0.2f);
            case ColorList.red: return Color.red;
            case ColorList.yellow: return Color.yellow;
            case ColorList.cyan: return Color.cyan;
            case ColorList.blue: return Color.blue;
            case ColorList.white: return Color.white;
            default: return Color.white;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Stoner stoner = FindObjectOfType<Stoner>();
            if (stoner == null)
            {
                Debug.LogError("Stoner not found in scene!");
                return;
            }

            MagicType type = GetMagicType();
            MagicColor color = GetMagicColor();

            stoner.PickupStone(type, color);
            Debug.Log($"Player entered stone trigger! Sending {type} {color} to Stoner");

            // Destroy this stone
            Destroy(gameObject);
        }
    }

    public MagicType GetMagicType()
    {
        switch (stoneType)
        {
            case TypeList.shadow: return MagicType.Shadow;
            case TypeList.stone: return MagicType.Stone;
            case TypeList.fire: return MagicType.Fire;
            case TypeList.wind: return MagicType.Wind;
            case TypeList.ice: return MagicType.Ice;
            case TypeList.water: return MagicType.Water;
            case TypeList.light: return MagicType.Light;
            default: return MagicType.Shadow;
        }
    }

    public MagicColor GetMagicColor()
    {
        switch (stoneColor)
        {
            case ColorList.black: return MagicColor.Black;
            case ColorList.brown: return MagicColor.Brown;
            case ColorList.red: return MagicColor.Red;
            case ColorList.yellow: return MagicColor.Yellow;
            case ColorList.cyan: return MagicColor.Cyan;
            case ColorList.blue: return MagicColor.Blue;
            case ColorList.white: return MagicColor.White;
            default: return MagicColor.Black;
        }




        
    }
}