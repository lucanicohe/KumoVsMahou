using UnityEngine;

public class IntroductionBox : MonoBehaviour
{
    void Start()
    {
        // Make sure this object starts active
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Check if **any key or mouse button** is pressed
        if (Input.anyKeyDown)
        {
            gameObject.SetActive(false);
        }
    }
}