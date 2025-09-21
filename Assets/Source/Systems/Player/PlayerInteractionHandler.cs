using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask; // Assign "Interactable" layer in Inspector

    private Camera mainCam;
    private PlayerInputReciever inputs;
    private DoorHandle currentDoor;

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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableMask))
        {
            DoorHandle door = hit.collider.GetComponentInParent<DoorHandle>();

            if (door != null)
            {
                // If we hit a new door, clear the old one
                if (currentDoor != door)
                {
                    ClearCurrentDoor();
                    currentDoor = door;
                }

                currentDoor.isInteractAvailable = true;
                return;
            }
        }

        // If we didn't hit a door, clear the old one
        ClearCurrentDoor();
    }
    bool canInteract = true;
    private void HandleInteraction()
    {
        // Poll input from your PlayerInputReciever
        if (inputs.Interact && canInteract) 
        {
            canInteract = false;

            if (currentDoor != null)
            {
                currentDoor.TryInteract();
            }

            Invoke(nameof(ResetCanInteract), 1.0f);
        }
    }

    void ResetCanInteract()
    {
        canInteract = true;
    }

    private void ClearCurrentDoor()
    {
        if (currentDoor != null)
        {
            currentDoor.isInteractAvailable = false;
            currentDoor = null;
        }
    }
}
