using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
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
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x < (Screen.width / 2))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        SwitchDimension();
                    }
                }
                else
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        Jump();
                    }
                }
                
            }
        }
    }


    private void HandleKeyboardInput()
    {
        //Jump on D, switch dimension on A
        if (Input.GetKey(KeyCode.D))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.A))
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
        playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);

    }

}
