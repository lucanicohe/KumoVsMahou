using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image1;
    public Image image2;
    public Color hoverColor = Color.red;


    

    void Start()
    {
        if (image1) image1.color = Color.white;
        if (image2) image2.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image1) image1.color = hoverColor;
        if (image2) image2.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image1) image1.color = Color.white;
        if (image2) image2.color = Color.white;
    }
}