using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stoner : MonoBehaviour
{
    public static Stoner instance { get; private set; }

    // If you filled this array in the inspector with your UI Stone2D slots,
    // Stoner will use those. If not, Stoner will attempt to find scene Stone2D
    // objects on demand (best-effort).
    [SerializeField] private Stone2D[] stoneInventoryArray = new Stone2D[20];

    private const int InventoryCapacity = 20;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[Stoner] Awake - persistent across scenes");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // We intentionally do not force-bind slots in Start() because the inventory
    // UI may live in another scene; instead we only bind when needed or when scene loads.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to rebind if the inspector array is empty or contains many nulls.
        // This is safe: if you have already assigned slots in inspector, this won't overwrite them.
        TryRebindIfNeeded();
    }

    // Try to populate stoneInventoryArray if it appears unassigned.
    public void TryRebindIfNeeded()
    {
        bool needRebind = false;
        if (stoneInventoryArray == null)
            needRebind = true;
        else
        {
            // if most slots are null, attempt a rebind
            int nullCount = 0;
            for (int i = 0; i < stoneInventoryArray.Length; i++)
            {
                if (stoneInventoryArray[i] == null) nullCount++;
            }
            if (nullCount > stoneInventoryArray.Length / 2)
                needRebind = true;
        }

        if (needRebind)
        {
            Stone2D[] found = FindObjectsOfType<Stone2D>();
            if (found != null && found.Length > 0)
            {
                int len = Mathf.Min(stoneInventoryArray.Length, found.Length);
                for (int i = 0; i < len; i++)
                    stoneInventoryArray[i] = found[i];
                Debug.Log($"[Stoner] Rebound {found.Length} Stone2D slots from scene.");
            }
            else
            {
                Debug.Log("[Stoner] TryRebindIfNeeded: no Stone2D found in scene.");
            }
        }
    }

    // This is the original, working immediate pickup logic — we preserve it.
    public void PickupStone(MagicType type, MagicColor color)
    {
        // If array is null or empty, try to bind right now so we can apply immediately.
        if (stoneInventoryArray == null || stoneInventoryArray.Length == 0)
        {
            stoneInventoryArray = new Stone2D[InventoryCapacity];
            TryRebindIfNeeded();
        }
        else
        {
            // If most slots are null, try to rebind so immediate UI can be used.
            int nullCount = 0;
            for (int i = 0; i < stoneInventoryArray.Length; i++)
                if (stoneInventoryArray[i] == null) nullCount++;
            if (nullCount > stoneInventoryArray.Length / 2)
                TryRebindIfNeeded();
        }

        // Original behavior: find first inactive slot and set it active / set properties.
        for (int i = 0; i < stoneInventoryArray.Length; i++)
        {
            Stone2D slot = stoneInventoryArray[i];
            if (slot != null)
            {
                // If slot exists and is inactive, use it.
                if (!slot.gameObject.activeSelf)
                {
                    slot.SetStoneProperties(type, color);
                    slot.gameObject.SetActive(true);
                    slot.UpdateVisuals();
                    Debug.Log($"[Stoner] Activated stone at index {i} with {color} {type}");
                    return;
                }
            }
        }

        // If none found, try to find any null slot and, if possible, assign to it if scene has a Stone2D to use.
        // This keeps behavior identical to the original script: when no inactive slot available, show warning.
        Debug.LogWarning("[Stoner] No inactive stones available or no UI slots assigned!");
    }

    public Stone2D GetCurrentlyActiveStone()
    {
        if (stoneInventoryArray == null) return null;
        foreach (Stone2D stone in stoneInventoryArray)
        {
            if (stone != null && stone.IsCurrentlyClicked())
            {
                return stone;
            }
        }
        return null;
    }

    public Stone2D[] GetAllStones()
    {
        return stoneInventoryArray;
    }
}