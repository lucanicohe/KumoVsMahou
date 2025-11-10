using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    
    [SerializeField] private TMP_Text _popupText;

    private void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _popupText.text = ""; 
    }

    // Show popup text on screen
    public void EKEY(string msg)
    {
        _popupText.text = msg;

        
        

        
    }

   
}