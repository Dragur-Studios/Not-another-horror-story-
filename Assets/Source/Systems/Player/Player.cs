using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action<bool> OnCrouch;
    public Action<bool> OnFocus;
    public Action<bool> OnSprint;


    PlayerInventory inventory;
    PlayerMovementResolver movement;
    PlayerAnimationResolver anim;
    PlayerCameraHandler cam;
    PlayerInteractionHandler interaction;
    PlayerInputReciever inputs;

    public PlayerInventory Inventory { get => inventory; }
    public PlayerMovementResolver Movement { get => movement; } 
    public PlayerAnimationResolver Animator { get => anim; }
    public PlayerCameraHandler Camera { get => cam; }
    public PlayerInteractionHandler Interaction { get => interaction; } 
    public PlayerInputReciever Input { get => inputs; }

    public void SetCrouch(bool crouching) => OnCrouch?.Invoke(crouching);
    public void SetSprint(bool sprinting) => OnSprint?.Invoke(sprinting);
    public void SetFocus(bool focusing) => OnFocus?.Invoke(focusing);

    public void PickUp(IItem item)
    {
        Destroy(item.gameObject);
        inventory.Insert(item);
    }


    internal void Initilize()
    {
        inventory = GetComponent<PlayerInventory>() ?? gameObject.AddComponent<PlayerInventory>();
        movement = GetComponent<PlayerMovementResolver>() ?? gameObject.AddComponent<PlayerMovementResolver>();
        anim = GetComponent<PlayerAnimationResolver>() ?? gameObject.AddComponent<PlayerAnimationResolver>();
        cam = GetComponent<PlayerCameraHandler>() ?? gameObject.AddComponent<PlayerCameraHandler>();
        cam.SetPlayer(this);
        cam.SetAnimator(GameCamera.Singleton.Animator);

        interaction = GetComponent<PlayerInteractionHandler>() ?? gameObject.AddComponent<PlayerInteractionHandler>();
        inputs = GetComponent<PlayerInputReciever>() ?? gameObject.AddComponent<PlayerInputReciever>();
    }
}
