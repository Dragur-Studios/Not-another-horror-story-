using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent), typeof(Player))]
public class PlayerMovementResolver : MonoBehaviour
{
    Player player;
    PlayerInputReciever inputs;
    NavMeshAgent agent;
    Camera cam;

    MoveMode moveMode = MoveMode.Standing_Idle;
    const MoveMode CrouchBit = (MoveMode)0b00000001;

    Vector2 moveInput;
    Vector3 moveDirection;
    float fullInputTimer;
    float targetAnimVelocity;
    float currentVelocity;

    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float inputDeadzone = 0.1f;
    [SerializeField] float runRampTime = 0.5f;
    [SerializeField] float fullInputThreshold = 0.99f;
    [SerializeField] float walkAnimSpeed = 0.5f;
    [SerializeField] float runAnimSpeed = 1.0f;
    [SerializeField] float sprintAnimSpeed = 1.5f;
    [SerializeField] float holdTimeToRun = 1.0f;
    public float CurrentVelocity => currentVelocity;
    public Vector2 CurrentHV { get; private set; }

    void Awake()
    {
        player = GetComponent<Player>();
        inputs = GetComponent<PlayerInputReciever>();
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;

        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    bool isCrouching = false;
    bool canToggleCrouchMode = true;

    void ResetCanToggleCrouchMode()
    {
        canToggleCrouchMode = true;
    }

    void Update()
    {
        moveInput = inputs.MoveInput;

        // Apply deadzone to raw input
        float deadzone = 0.05f;
        if (Mathf.Abs(moveInput.x) < deadzone) moveInput.x = 0f;
        if (Mathf.Abs(moveInput.y) < deadzone) moveInput.y = 0f;

        bool hasInput = HasMovementInput();

        CalculateMoveDirection();
        RotateCharacter();

        if (hasInput)
        {
            UpdateMoveModeAndVelocity();
        }
        else
        {
            // Explicit idle handling
            SetIdleState();
        }

        // Crouch toggle logic still runs regardless of movement
        if (inputs.Crouch && canToggleCrouchMode)
        {
            canToggleCrouchMode = false;
            isCrouching = !isCrouching;
            Invoke(nameof(ResetCanToggleCrouchMode), 1.0f);
        }

        if (isCrouching)
            moveMode |= CrouchBit;
        else
            moveMode &= ~CrouchBit;

        player.SetCrouch(isCrouching);

        // Calculate local-space HV even when idle
        Vector3 localDir = transform.InverseTransformDirection(moveDirection);
        Vector2 hv = hasInput ? new Vector2(localDir.x, localDir.z) * currentVelocity : Vector2.zero;

        // Sprint scaling
        if ((moveMode & ~CrouchBit) == MoveMode.Sprint && hv.y >= 0.5f)
            hv.y = sprintAnimSpeed;

        // Apply animation deadzone to HV
        float animDeadzone = 0.01f;
        if (hv.magnitude < animDeadzone) hv = Vector2.zero;

        CurrentHV = hv;

        // Sprint event derived from final hv
        bool isSprintingAnim = hv.y >= sprintAnimSpeed - 0.01f;
        player.SetSprint(isSprintingAnim);

        bool isFocus = inputs.Focus;
        player.SetFocus(isFocus);
    }


    bool HasMovementInput() => moveInput.sqrMagnitude >= inputDeadzone * inputDeadzone;

    void SetIdleState()
    {
        moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Idle : MoveMode.Standing_Idle;
        currentVelocity = 0f;
    }

    void CalculateMoveDirection()
    {
        moveDirection = (cam.transform.forward * moveInput.y + cam.transform.right * moveInput.x);
        moveDirection.y = 0f;
        moveDirection.Normalize();
    }

    void RotateCharacter()
    {
        Vector3 faceDir = inputs.Focus ? cam.transform.forward : moveDirection;
        faceDir.y = 0f;
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateMoveModeAndVelocity()
    {
        bool sprinting = inputs.Sprint;
        bool atFullInput = moveInput.magnitude >= fullInputThreshold;

        if (sprinting)
        {
            targetAnimVelocity = sprintAnimSpeed;
            moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Sprint : MoveMode.Sprint;
        }
        else
        {
            // If we were sprinting last frame, drop back to run/walk immediately
            if ((moveMode & ~CrouchBit) == MoveMode.Sprint || (moveMode & ~CrouchBit) == MoveMode.Crouching_Sprint)
            {
                // Force downshift
                targetAnimVelocity = runAnimSpeed;
                moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Run : MoveMode.Run;
            }

            if (!atFullInput || currentVelocity < walkAnimSpeed)
            {
                fullInputTimer = 0f;
                targetAnimVelocity = walkAnimSpeed;
                moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Walk : MoveMode.Walk;
            }

            if (atFullInput)
            {
                fullInputTimer += Time.deltaTime;
                if (fullInputTimer >= holdTimeToRun)
                {
                    targetAnimVelocity = runAnimSpeed;
                    moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Run : MoveMode.Run;
                }
            }
        }

        currentVelocity = Mathf.MoveTowards(currentVelocity, targetAnimVelocity, Time.deltaTime / runRampTime);
    }

}
