using UnityEngine;
using System.IO;
using TMPro;

public class CoordinateFinder : MonoBehaviour
{
    public static CoordinateFinder Instance { get; private set; }

    [Header("Assign ONE TMP Text for all coordinates")]
    public TMP_Text coordinateText;

    private string SavePath => Application.persistentDataPath + "/coordinates.json";

    [System.Serializable]
    public class CoordinatesData
    {
        public string ホーム = "ABC";
        public string ストーン = "BDF";
        public string ファイア = "???";
        public string ウィンド = "???";
        public string アイス = "???";
        public string ウォーター = "???";
        public string ボス = "???";
    }

    private CoordinatesData data;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[SETUP CHECK] CoordinateFinder Instance is ready.");

        LoadData();
        UpdateText();
    }

    public void RevealCoordinate(string coordinateName, string code)
    {
        if (data == null) return;

        switch (coordinateName)
        {
            case "ホーム": data.ホーム = code; break;
            case "ストーン": data.ストーン = code; break;
            case "ファイア": data.ファイア = code; break;
            case "ウィンド": data.ウィンド = code; break;
            case "アイス": data.アイス = code; break;
            case "ウォーター": data.ウォーター = code; break;
            case "ボス": data.ボス = code; break;
            default:
                Debug.LogWarning($"Unknown coordinate or key: {coordinateName}. Check your CoordinateTrigger setup.", this);
                return;
        }

        SaveData();
        UpdateText();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("[DATA SAVE] Coordinate data saved.");
    }

    private void LoadData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<CoordinatesData>(json);
            Debug.Log("[DATA LOAD] Coordinate data loaded.");
        }
        else
        {
            data = new CoordinatesData();
            SaveData();
            Debug.Log("[DATA INIT] New coordinate data created.");
        }
    }

    public void UpdateText()
    {
        if (coordinateText == null) return;

        coordinateText.text =
            $"ホーム: {data.ホーム}\n" +
            $"ストーン: {data.ストーン}\n" +
            $"ファイア: {data.ファイア}\n" + // FIX: Corrected typo from data.ファイA
            $"ウィンド: {data.ウィンド}\n" +
            $"アイス: {data.アイス}\n" +
            $"ウォーター: {data.ウォーター}\n" +
            $"ボス: {data.ボス}";
    }

    public CoordinatesData GetData() => data;
}