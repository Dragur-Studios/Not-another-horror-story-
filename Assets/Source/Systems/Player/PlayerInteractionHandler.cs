using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactThreshold = 0.2f;
    [SerializeField] private LayerMask interactableMask; 

    private Camera mainCam;
    private PlayerInputReciever inputs;
    private IInteractable curInteract;
    bool canInteract = true;

    private void Start()
    {
        mainCam = Camera.main;
        inputs = GetComponent<PlayerInputReciever>();
    }

    private void Update()
    {
        HandleDetection();
        HandleInteraction();
    }

    private void HandleDetection()
    {
        // Raycast from camera center forward
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);

        if (Physics.SphereCast(ray, interactThreshold, out var hit,  interactRange, interactableMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                // If we hit a new door, clear the old one
                if (curInteract != interactable)
                {
                    ClearCurrentInteract();
                    curInteract = interactable;
                }

                curInteract.isInteractAvailable = true;
                return;
            }
        }

        // If we didn't hit a door, clear the old one
        ClearCurrentInteract();
    }
    private void HandleInteraction()
    {
        // Poll input from your PlayerInputReciever
        if (inputs.Interact && canInteract) 
        {
            canInteract = false;

            if (curInteract != null)
            {
                curInteract.TryInteract();
            }

            Invoke(nameof(ResetCanInteract), 1.0f);
        }
    }

    void ResetCanInteract()
    {
        canInteract = true;
    }

    private void ClearCurrentInteract()
    {
        if (curInteract != null)
        {
            curInteract.isInteractAvailable = false;
            curInteract = null;
        }
    }
}
