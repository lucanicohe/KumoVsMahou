using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{


    [SerializeField] private Material cloudMaterial;
    [SerializeField] private float scrollSpeed = 0.01f;




    private Vector2 offset;

    void Start()
    {
        offset = Vector2.zero;
        // URP lit shader uses _BaseMap instead of _MainTex
        cloudMaterial.SetTextureOffset("_BaseMap", offset);
    }

    void Update()
    {


        // current offset
        offset = cloudMaterial.GetTextureOffset("_BaseMap");


        // the texture
        offset.x += scrollSpeed * Time.deltaTime;



        // around when offset exceeds 1

        if (offset.x > 1f)
            offset.x -= 1f;
        
        

        // the new offset to URP's base map
        cloudMaterial.SetTextureOffset("_BaseMap", offset);
    }
}