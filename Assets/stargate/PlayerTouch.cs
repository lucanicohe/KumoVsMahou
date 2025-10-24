using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{


    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //touch key
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            //Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                //Debug.Log($"[PlayerTouch] Hit object: {hit.collider.name}");

                ButtonController button = hit.collider.GetComponent<ButtonController>();
                if (button != null)
                {
                    //Debug.Log($"[PlayerTouch] pressing button: {button.name}");
                    button.Touched();
                }

                MagicForge forge = hit.collider.GetComponent<MagicForge>();
                if (forge != null)
                {
                    //Debug.Log($"[PlayerTouch] using forge: {forge.name}");
                    forge.Touched();
                }
            }
            else
            {
                Debug.Log("[PlayerTouch] raycast missed.");
            }
        }
    }
}