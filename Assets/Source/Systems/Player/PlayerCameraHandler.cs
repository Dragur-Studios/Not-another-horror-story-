using UnityEngine;
using UnityEngine.InputSystem; // For New Input System polling


public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cinemachineAnchor; // Pivot for the camera
    [SerializeField] private float sensitivityX = 150f;
    [SerializeField] private float sensitivityY = 100f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;   // Horizontal rotation
    private float pitch; // Vertical rotation

    [SerializeField] Animator camAnimator;

    Player player;
    PlayerInputReciever inputs;

    private void Start()
    {
        player = GetComponent<Player>();
        inputs = player.GetComponent<PlayerInputReciever>();
        
        player.OnFocus += (focus) =>
        {
            camAnimator.SetBool("Focus", focus);
        };

        player.OnCrouch += (isCrouching) =>
        {
            camAnimator.SetBool("isCrouching", isCrouching);
        };


    }


    void LateUpdate()
    {
        // --- 1. Read input ---
        Vector2 lookInput = inputs.LookInput;

        // --- 2. Apply sensitivity & accumulate rotation ---
        yaw += lookInput.x * sensitivityX * Time.deltaTime;
        pitch -= lookInput.y * sensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);  

        // --- 4. Apply rotation to anchor ---
        cinemachineAnchor.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
