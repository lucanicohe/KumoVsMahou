using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Button startButton;

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
    }

    void OnStartButtonPressed()
    {
        Debug.Log("Start button pressed — resetting save file...");

        InventorySaveManager.ResetSaveToDefaultStatic();
        

        SceneManager.LoadScene("HomeWorld");
    }
}