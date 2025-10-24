using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateColliderController : MonoBehaviour
{
    [SerializeField] private string defaultWorldName; // maybe not needed?

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;




        //  the active condition from ButtonController
        string condition = ButtonController.allButtons.Count > 0 ? ButtonController.allButtons[0].GetActiveCondition() : "";

        if (string.IsNullOrEmpty(condition))
        {
            Debug.Log("Gate entered but no valid sequence active!");
            return;
        }




        // scene based on condition
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
                {
                    SceneManager.LoadScene(defaultWorldName);
                }
                else
                {
                    Debug.Log("Unknown condition: " + condition);
                }
                break;
        }




        //  condition so gate can't be reused until new sequence
        foreach (var btn in ButtonController.allButtons)
        {
            btn.ClearActiveCondition();
        }
    }
}