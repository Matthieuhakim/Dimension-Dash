using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{

    public float playerSpeed = 10f;

    [SerializeField]
    private float jumpForce = 12f;
    [SerializeField]
    private float jumpTorque = -1f;

    private Rigidbody2D playerRb;

    [SerializeField]
    private ParticleSystem deathParticles;
    private SpriteRenderer spriteRenderer;

    private bool canJump = true;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

        if (GameManager.Instance.isPlaying)
        {
            MoveRight();
            HandleTouchInput();
            HandleKeyboardInput();

        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        foreach (TouchControl touch in Touchscreen.current.touches)
        {
            if (!touch.isInProgress) continue;

            if (touch.position.ReadValue().x < (Screen.width / 2))
            {
                if (touch.press.wasPressedThisFrame)
                {
                    SwitchDimension();
                }
            }
            else
            {
                Jump();
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (Keyboard.current == null) return;

        //Jump on D, switch dimension on A
        if (Keyboard.current.dKey.isPressed)
        {
            Jump();
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            SwitchDimension();
        }
    }

    private void MoveRight()
    {
        transform.Translate(playerSpeed * Time.deltaTime * Vector2.right, Space.World);
    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;

            playerRb.AddForce(jumpForce * Vector2.up , ForceMode2D.Impulse);
            playerRb.AddTorque(jumpTorque, ForceMode2D.Impulse);
        }
        
    }

    private void SwitchDimension()
    {
        LevelManager.Instance.SwitchDimension();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        switch (collision.gameObject.tag)
        {
            case StringHelper.OBSTACLE_TAG:

                GameManager.Instance.GameOver();
                return;

            case StringHelper.PLATFORM_TAG:
                bool isDeadly = IsDeadlyCollision(collision);

                if (isDeadly)
                {
                    GameManager.Instance.GameOver();
                }
                else
                {

                    LandPlayer();
                }

                return;

            case StringHelper.FINISHLINE_TAG:

                GameManager.Instance.FinishLevel();

                return;
            default:

                LandPlayer();
                return;
        }
    }

    private bool IsDeadlyCollision(Collision2D collision)
    {
        foreach (ContactPoint2D point in collision.contacts)
        {
            // If the collision happened from the side 
            if (point.normal.y < 0.9f)
            {
                return true;
            }
        }

        return false;
    }

    public void Explode()
    {

        ParticleSystem particles = Instantiate(deathParticles, transform.position, transform.rotation).GetComponent<ParticleSystem>();

        var main = particles.main;
        main.startColor = spriteRenderer.color;

        gameObject.SetActive(false);
    }

    private void LandPlayer()
    {
        FreezeRotation();
        canJump = true;
    }

    private void FreezeRotation()
    {
        //Calculate targetRotation
        float currentRotation = playerRb.rotation % 360;
        if (currentRotation < 0) currentRotation += 360;

        float targetRotation = Mathf.Round(currentRotation / 90f) * 90f;

        playerRb.rotation = targetRotation;

        // Reset angular velocity to stop spinning
        playerRb.angularVelocity = 0f;

        // Reset the Rigidbody's vertical velocity
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);

    }

}
