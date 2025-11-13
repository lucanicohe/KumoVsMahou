using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
        }
        else
        {
            Debug.LogError("Start Button not assigned in IntroManager!");
        }

        if (endButton != null)
        {
            endButton.onClick.AddListener(OnEndButtonPressed);
        }
        else
        {
            Debug.LogError("End Button not assigned in IntroManager!");
        }
    }

    void OnStartButtonPressed()
    {
        Debug.Log("Start button pressed — resetting save file...");
        InventorySaveManager.ResetSaveToDefaultStatic();
        SceneManager.LoadScene("IntroWorld");
    }

    void OnEndButtonPressed()
    {
        Debug.Log("End button pressed — quitting application...");
        Application.Quit();

        // Optional: works in the editor for testing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}