using System;
using UnityEngine;

public class interactableManger : MonoBehaviour 
{
    public float interactionDistance = 3f;
    public LayerMask interactableLayer = 6;
    public Transform playerCamera;

    private IInteractable _currentInteractable;
    private Outline outline;
    
    // Update is called once per frame
    void Update()
    {
        checkForInteractable();
        if (_currentInteractable != null && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton3)))
        {
            _currentInteractable.Interact(null);

        }

    }

    private void checkForInteractable()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    RestOutLine();
                    _currentInteractable = interactable;
                    outline = hitInfo.collider.GetComponent<Outline>();

                    if (outline != null)
                    {
                        outline.enabled = true;
                    }
                }
                return;
            }
        }
        RestOutLine();
    }

    private void RestOutLine()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
        _currentInteractable = null;
        outline = null;
    }
}
