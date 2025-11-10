using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StoneData
{
    public MagicType stoneType;
    public MagicColor stoneColor;
    public bool isActive;
}

[System.Serializable]
public class InventorySaveData
{
    public List<StoneData> stones = new List<StoneData>();
    public MagicType activeSlotType;
    public MagicColor activeSlotColor;
}

public class InventorySaveManager : MonoBehaviour
{
    public static InventorySaveManager instance { get; private set; }

    private string savePath;
    private InventorySaveData saveData;
    private bool gameJustStarted = false;

    void Awake()
    {
        gameJustStarted = false; 

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/inventory_save.json";

        if (!gameJustStarted)
        {
            
            //ResetSaveToDefaultStatic();
            gameJustStarted = true; 
        }








        LoadGame();


    }
    
    

    public void ResetSaveToDefault()
    {
        saveData = new InventorySaveData();

        // Optional: Add one black shadow stone in active slot by default
        StoneData defaultStone = new StoneData
        {
            stoneType = MagicType.Shadow,
            stoneColor = MagicColor.Black,
            isActive = true
        };
        saveData.stones.Add(defaultStone);
        saveData.activeSlotType = MagicType.Shadow;
        saveData.activeSlotColor = MagicColor.Black;

        SaveGame(); // writes JSON to disk immediately
    }

    public static void ResetSaveToDefaultStatic()
    {
        if (instance != null)
            instance.ResetSaveToDefault();
        else
        {
            // If instance doesn't exist yet, create a temporary one to reset
            GameObject temp = new GameObject("InventorySaveManager_Temp");
            var manager = temp.AddComponent<InventorySaveManager>();
            manager.ResetSaveToDefault();
            Destroy(temp); // Clean up
        }
    }
    




    void OverwriteSaveWithDefault()
    {
        // Create fresh default inventory
        saveData = new InventorySaveData
        {
            stones = new List<StoneData>(), // empty inventory, all stones inactive
            activeSlotType = MagicType.Shadow,
            activeSlotColor = MagicColor.Black
        };

        // Optional: Pre-fill stone slots with inactive placeholders if you want a fixed number
        for (int i = 0; i < 20; i++)
        {
            saveData.stones.Add(new StoneData
            {
                stoneType = MagicType.Shadow,
                stoneColor = MagicColor.Black,
                isActive = false
            });
        }

        // Write immediately to disk
        SaveGame();
        Debug.Log("Inventory fully reset for fresh game start");
    }





    void Start()
    {
        // Reset inventory JSON on fresh game start
        //ResetInventoryOnGameStart();
    }



    public void SaveGame()
    {
        if (saveData == null) saveData = new InventorySaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    



    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<InventorySaveData>(json);
            Debug.Log($"Game loaded from {savePath}");
        }
        else
        {
            saveData = new InventorySaveData();
            Debug.Log("No save file found, creating new game data");
        }

        if (saveData.stones == null)
            saveData.stones = new List<StoneData>();
    }

    public void SaveStoneData(Stone2D[] allStones, MagicType activeType, MagicColor activeColor)
    {
        if (saveData == null) saveData = new InventorySaveData();
        saveData.stones.Clear();

        if (allStones != null)
        {
            foreach (Stone2D stone in allStones)
            {
                if (stone != null)
                {
                    saveData.stones.Add(new StoneData
                    {
                        stoneType = stone.stoneType,
                        stoneColor = stone.stoneColor,
                        isActive = stone.gameObject.activeSelf
                    });
                }
            }
        }

        saveData.activeSlotType = activeType;
        saveData.activeSlotColor = activeColor;

        SaveGame();
    }

    public InventorySaveData GetSaveData()
    {
        if (saveData == null)
            LoadGame();
        return saveData;
    }
}