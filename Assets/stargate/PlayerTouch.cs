using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                ButtonController button = hit.collider.GetComponent<ButtonController>();
                if (button != null)
                {
                    button.Touched();
                }
            }
        }
    }
}