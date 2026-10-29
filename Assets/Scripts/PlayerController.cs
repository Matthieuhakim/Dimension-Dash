using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

// Auto-runner: constant forward speed, fixed-arc jump, dimension switching, collision deaths.
// Physics rotation is frozen; only the Visual child spins.
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [FormerlySerializedAs("playerSpeed")]
    [SerializeField]
    private float runSpeed = 10f;
    [FormerlySerializedAs("jumpForce")]
    [SerializeField]
    private float jumpVelocity = 16f;
    [Tooltip("A jump pressed this long before landing still happens.")]
    [SerializeField]
    private float jumpBufferTime = 0.1f;
    [Tooltip("A jump pressed this long after leaving the ground still happens.")]
    [SerializeField]
    private float coyoteTime = 0.05f;

    [Header("Collision")]
    [SerializeField]
    private LayerMask groundLayers;
    [SerializeField]
    private float groundCheckDepth = 0.05f;
    [Tooltip("Ground contacts steeper than this (walls, ceilings) kill the player.")]
    [SerializeField, Range(0f, 90f)]
    private float maxLandingAngle = 45f;

    [Header("Visuals")]
    [SerializeField]
    private Transform visual;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [Tooltip("Rotation of the visual over one full jump.")]
    [SerializeField]
    private float airSpinDegrees = -180f;
    [SerializeField]
    private float landingSnapSpeed = 720f;
    [SerializeField]
    private ParticleSystem deathParticles;

    [Header("Input")]
    [SerializeField]
    private InputAction jumpAction = CreateButton("Jump", "<Keyboard>/d", "<Keyboard>/space", "<Keyboard>/upArrow", "<Gamepad>/buttonSouth");
    [SerializeField]
    private InputAction switchAction = CreateButton("Switch Dimension", "<Keyboard>/a", "<Gamepad>/buttonWest");

    private Rigidbody2D body;
    private BoxCollider2D box;

    // The player spawns on the ground; corrected by the first ground check.
    private bool isGrounded = true;
    private bool jumpHeld;
    private float lastGroundedTime = float.NegativeInfinity;
    private float lastJumpPressTime = float.NegativeInfinity;
    private float airSpinSpeed;

    private static LevelManager Level => LevelManager.Instance;

    private void Reset()
    {
        groundLayers = LayerMask.GetMask(StringHelper.GROUND_LAYER);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        float airTime = 2f * jumpVelocity / (Mathf.Abs(Physics2D.gravity.y) * body.gravityScale);
        airSpinSpeed = airSpinDegrees / airTime;

        // Jump held through a respawn (the key is already down when the action enables) still counts.
        jumpAction.wantsInitialStateCheck = true;
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        switchAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        switchAction.Disable();
    }

    private void Update()
    {
        if (!Level.IsPlaying) return;

        ReadTouches(out bool touchJumpPressed, out bool touchJumpHeld, out bool touchSwitchPressed);

        if (jumpAction.WasPressedThisFrame() || touchJumpPressed)
        {
            lastJumpPressTime = Time.time;
        }
        jumpHeld = jumpAction.IsPressed() || touchJumpHeld;

        if (switchAction.WasPressedThisFrame() || touchSwitchPressed)
        {
            Level.SwitchDimension();
        }

        UpdateVisualRotation();
    }

    private void FixedUpdate()
    {
        if (!Level.IsPlaying) return;

        if (body.position.y < Level.KillHeight)
        {
            Level.KillPlayer();
            return;
        }

        Vector2 velocity = body.linearVelocity;
        velocity.x = runSpeed;

        isGrounded = velocity.y <= 0.01f && CheckGround();
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            velocity.y = 0f;
        }

        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        bool wantsJump = jumpHeld || Time.time - lastJumpPressTime <= jumpBufferTime;
        if (canJump && wantsJump)
        {
            velocity.y = jumpVelocity;
            isGrounded = false;
            SnapVisualRotation();
            lastGroundedTime = float.NegativeInfinity;
            lastJumpPressTime = float.NegativeInfinity;
        }

        body.linearVelocity = velocity;
    }

    // Thin box just under the feet.
    private bool CheckGround()
    {
        Bounds bounds = box.bounds;
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y - groundCheckDepth * 0.5f);
        Vector2 size = new Vector2(bounds.size.x * 0.9f, groundCheckDepth);
        return Physics2D.OverlapBox(center, size, 0f, groundLayers) != null;
    }

    // Left half: switch dimension (on press). Right half: jump (held).
    // A touch keeps the role of the half it started in, even if it slides across.
    private static void ReadTouches(out bool jumpPressed, out bool jumpHeld, out bool switchPressed)
    {
        jumpPressed = jumpHeld = switchPressed = false;

        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        float halfWidth = Screen.width * 0.5f;
        foreach (TouchControl touch in touchscreen.touches)
        {
            bool pressedThisFrame = touch.press.wasPressedThisFrame;
            if (!pressedThisFrame && !touch.press.isPressed) continue;

            bool leftHalf = touch.startPosition.ReadValue().x < halfWidth;
            if (leftHalf)
            {
                switchPressed |= pressedThisFrame;
            }
            else
            {
                jumpPressed |= pressedThisFrame;
                jumpHeld |= touch.press.isPressed;
            }
        }
    }

    private void UpdateVisualRotation()
    {
        float angle = visual.localEulerAngles.z;
        if (isGrounded)
        {
            float target = Mathf.Round(angle / 90f) * 90f;
            angle = Mathf.MoveTowardsAngle(angle, target, landingSnapSpeed * Time.deltaTime);
        }
        else
        {
            angle += airSpinSpeed * Time.deltaTime;
        }
        visual.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Every jump starts from a flat face, so the spin always lands flat even on instant re-jumps.
    private void SnapVisualRotation()
    {
        float angle = Mathf.Round(visual.localEulerAngles.z / 90f) * 90f;
        visual.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision) => CheckDeadlyCollision(collision);

    // Stay matters for large merged colliders (tilemaps): a wall can be part of the collider already being touched.
    private void OnCollisionStay2D(Collision2D collision) => CheckDeadlyCollision(collision);

    private void CheckDeadlyCollision(Collision2D collision)
    {
        if (!Level.IsPlaying) return;

        if (collision.collider.CompareTag(StringHelper.OBSTACLE_TAG))
        {
            Level.KillPlayer();
            return;
        }

        if ((groundLayers.value & (1 << collision.collider.gameObject.layer)) == 0) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            // The normal points from the ground towards the player: up when landing on top.
            Vector2 normal = collision.GetContact(i).normal;

            // A normal pointing forward comes from geometry behind the player (e.g. the corner of a ledge being left).
            if (normal.x > 0.1f) continue;

            if (Vector2.Angle(normal, Vector2.up) > maxLandingAngle)
            {
                Level.KillPlayer();
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Level.IsPlaying && other.CompareTag(StringHelper.FINISHLINE_TAG))
        {
            Level.CompleteLevel();
        }
    }

    public void Explode()
    {
        ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = particles.main;
        main.startColor = spriteRenderer.color;

        gameObject.SetActive(false);
    }

    public void Stop()
    {
        body.linearVelocity = Vector2.zero;
        body.simulated = false;
    }

    private static InputAction CreateButton(string name, params string[] bindings)
    {
        var action = new InputAction(name, InputActionType.Button);
        foreach (string binding in bindings)
        {
            action.AddBinding(binding);
        }
        return action;
    }
}
