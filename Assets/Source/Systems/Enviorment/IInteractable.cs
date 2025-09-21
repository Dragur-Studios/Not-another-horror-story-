using System;
using UnityEngine;

public class IInteractable : MonoBehaviour
{
    public Action OnInteract;


    protected void HandleInteract()
    {
        OnInteract?.Invoke();
    }

}
