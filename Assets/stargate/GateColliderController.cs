using UnityEngine;
using UnityEngine.SceneManagement;

public class GateColliderController : MonoBehaviour
{
    [SerializeField] private string defaultWorldName;
    [SerializeField] private PixelationController pixelController; 

    

    private UIButtonManager buttonManager;

    private void Start()
    {
        buttonManager = FindObjectOfType<UIButtonManager>();
        if (buttonManager == null)
            Debug.LogError("UIButtonManager not found in scene!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (buttonManager == null) return;

        string condition = buttonManager.GetActiveCondition();
        if (string.IsNullOrEmpty(condition))
        {
            Debug.Log("Gate entered but no valid sequence active!");
            return;
        }

        // Trigger pixelation + movement animation, then load scene
        if (pixelController != null)
        {
            pixelController.StartPixelation(() => LoadSceneByCondition(condition));
        }
        else
        {
            LoadSceneByCondition(condition);
        }

        buttonManager.ClearActiveCondition();
    }

    private void LoadSceneByCondition(string condition)
    {
        switch (condition)
        {
            case "home": SceneManager.LoadScene("HomeWorld"); break;
            case "stone": SceneManager.LoadScene("StoneWorld"); break;
            case "fire": SceneManager.LoadScene("FireWorld"); break;
            case "wind": SceneManager.LoadScene("WindWorld"); break;
            case "ice": SceneManager.LoadScene("IceWorld"); break;
            case "water": SceneManager.LoadScene("WaterWorld"); break;
            case "boss": SceneManager.LoadScene("BossWorld"); break;
            default:
                if (!string.IsNullOrEmpty(defaultWorldName))
                    SceneManager.LoadScene(defaultWorldName);
                else
                    Debug.Log("Unknown condition: " + condition);
                break;
        }
    }
}