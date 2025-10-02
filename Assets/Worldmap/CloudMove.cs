using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CloudMove : MonoBehaviour
{
    [SerializeField] private Material cloudMaterial;
    [SerializeField] private float scrollSpeed = 0.01f;

    private Vector2 offset;

    void Update()
    {
        offset = cloudMaterial.mainTextureOffset;
        offset.x += scrollSpeed * Time.deltaTime;
        if (offset.x > 1f) offset.x -= 1f;
        cloudMaterial.SetTextureOffset("_MainTex", offset);
    }
}
