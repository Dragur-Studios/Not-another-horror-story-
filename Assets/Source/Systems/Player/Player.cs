using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action<bool> OnCrouch;
    public Action<bool> OnFocus;
    public Action<bool> OnSprint;

    public void SetCrouch(bool crouching) => OnCrouch?.Invoke(crouching);
    public void SetSprint(bool sprinting) => OnSprint?.Invoke(sprinting);
    public void SetFocus(bool focusing) => OnFocus?.Invoke(focusing);
}
