using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;
    [SerializeField] private VideoPlayer introVideo;
    [SerializeField] private GameObject videoUI; // RawImage or parent

    [SerializeField] private AudioSource _music; 

    void Start()
    {
        _music.Play(); 
        startButton.onClick.AddListener(OnStartButtonPressed);
        endButton.onClick.AddListener(OnEndButtonPressed);

        videoUI.SetActive(false); // hide video at start
    }

    void OnStartButtonPressed()
    {
        InventorySaveManager.ResetSaveToDefaultStatic();
        startButton.interactable = false;
        endButton.interactable = false;

        StartCoroutine(PlayVideoThenLoad());
    }

    private IEnumerator PlayVideoThenLoad()
    {
        _music.Stop(); 
        videoUI.SetActive(true);
        introVideo.Prepare(); // IMPORTANT: Prepare video first

        // Wait until video is ready
        while (!introVideo.isPrepared)
            yield return null;

        introVideo.Play();

        // Wait until video finishes
        while (introVideo.isPlaying)
            yield return null;

        SceneManager.LoadScene("HomeWorld");
    }

    void OnEndButtonPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}