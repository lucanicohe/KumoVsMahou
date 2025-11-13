using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ESCMENU : MonoBehaviour
{
    [SerializeField] private GameObject escMenuObject;
    [SerializeField] private Button menuButton;

    private bool isMenuActive = false;

    void Start()
    {
        if (escMenuObject != null)
            escMenuObject.SetActive(false);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuButtonPressed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        escMenuObject.SetActive(isMenuActive);

        Time.timeScale = isMenuActive ? 0f : 1f;
        Cursor.visible = isMenuActive;
        Cursor.lockState = isMenuActive ? CursorLockMode.None : CursorLockMode.Locked;

        // mute/unmute all game audio
        AudioListener.volume = isMenuActive ? 0f : 1f;
    }

    void OnMenuButtonPressed()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f; // restore volume before switching scenes
        SceneManager.LoadScene("Menu");
    }
}