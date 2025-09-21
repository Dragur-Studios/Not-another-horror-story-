using Unity.VisualScripting;
using UnityEngine;

public enum DoorLockType  
{
    None, // no lock on this door, can just be opened or closed.
    
    Key_Lock,

}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

    

}

public class DoorHandle : IInteractable
{
    [SerializeField] GameObject Icon;
    [SerializeField] Animator doorAnim;

    [SerializeField] DoorLockType lockType;

    public float animationTime = 1.0f;

    public bool isInteractAvailable = false;
    
    bool isOpen = false;
    bool isLocked = false;

    private void Update()
    {
        Icon?.SetActive(isInteractAvailable);
    }


    bool canInteract = true;

    private void Start()
    {
        OnInteract += () =>
        {
            canInteract = false; // TURNS off USER INTERFACE ELEMENT [INTERACT]
            doorAnim.SetTrigger("Interact");
            Invoke(nameof(ResetCanInteract), animationTime);
        };
    }

    public bool TryInteract()
    {
        if (!isInteractAvailable)
            return false;

        switch (lockType)
        {
            case DoorLockType.Key_Lock:
                {
                    if (!PlayerHasKey())
                    {

                    }
                }
                break;

        }
        HandleInteract();

        return true;
    }
    
    bool PlayerHasKey()
    {
        //var player = GameManager.Instance.Player;
        return true;
    }
    

    void ResetCanInteract()
    {
        canInteract = true;
    }
   
   

}
