using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStone : MonoBehaviour
{
    [Header("Stone Properties")]
    public TypeList stoneType;
    public ColorList stoneColor;

    [Header("Stone Models (assign 7 models)")]
    [SerializeField] private GameObject shadowModel;
    [SerializeField] private GameObject stoneModel;
    [SerializeField] private GameObject fireModel;
    [SerializeField] private GameObject windModel;
    [SerializeField] private GameObject iceModel;
    [SerializeField] private GameObject waterModel;
    [SerializeField] private GameObject lightModel;

    [Header("Stone Material")]
    [SerializeField] private Material stoneMaterial;

    [Header("Stone Light")]
    [SerializeField] private Light stoneLight;

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
        UpdateStoneAppearance();
    }

    public void OnValidate()
    {
        UpdateStoneAppearance();
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

        Color baseColor = GetBaseColor(color);

        matInstance.SetColor("_Color", baseColor);
        if (matInstance.HasProperty("_BaseColor"))
            matInstance.SetColor("_BaseColor", baseColor);

        matInstance.DisableKeyword("_EMISSION");

        if (stoneLight != null)
            stoneLight.color = baseColor;
    }

    Color GetBaseColor(ColorList color)
    {
        switch (color)
        {
            case ColorList.black: return new Color(0.1f, 0.1f, 0.1f);
            case ColorList.brown: return new Color(0.6f, 0.4f, 0.2f);
            case ColorList.red: return Color.red;
            case ColorList.yellow: return Color.yellow;
            case ColorList.cyan: return Color.cyan;
            case ColorList.blue: return Color.blue;
            case ColorList.white: return Color.white;
            default: return Color.white;
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