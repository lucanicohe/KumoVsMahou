using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoner : MonoBehaviour
{
    [SerializeField] private Stone2D[] stoneInventoryArray = new Stone2D[20];

    void Start()
    {
        Stone2D[] allStones = FindObjectsOfType<Stone2D>();
        for (int i = 0; i < allStones.Length && i < 20; i++)
        {
            stoneInventoryArray[i] = allStones[i];
        }
        Debug.Log($"Stoner initialized with {allStones.Length} stones"); 
        
    }

    public void PickupStone(MagicType type, MagicColor color)
    {
        for (int i = 0; i < stoneInventoryArray.Length; i++)
        {
            if (stoneInventoryArray[i] != null && !stoneInventoryArray[i].gameObject.activeSelf)
            {
                stoneInventoryArray[i].gameObject.SetActive(true);
                stoneInventoryArray[i].SetStoneProperties(type, color);
                Debug.Log($"Activated stone at index {i} with {color} {type}");
                return;
            }
        }
        Debug.LogWarning("No inactive stones available!");
    }

    public Stone2D GetCurrentlyActiveStone()
    {
        foreach (Stone2D stone in stoneInventoryArray)
        {
            if (stone != null && stone.IsCurrentlyClicked())
            {
                return stone;
            }
        }
        return null;
    }
}