using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float playerSpeed = 10f;
    public float Speed
    {
        get { return playerSpeed; }
    }

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
            HandleMouseClick();
            HandleKeyboardInput();

            if (!canJump)
            {
                StartCoroutine(AdjustRotation());
            }

        }

    }


    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Jump on right side click, switch dimension on left side click
            if (Input.mousePosition.x >= (Screen.width / 2))
            {

                Jump();
            }
            else
            {
                SwitchDimension();
            }


        }
    }

    private void HandleKeyboardInput()
    {
        //Jump on D, switch dimension on A
        if (Input.GetKeyDown(KeyCode.D))
        {

            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchDimension();
        }


        
    }

    private void MoveRight()
    {
        transform.Translate(Vector2.right * playerSpeed * Time.deltaTime, Space.World);
    }


    private void Jump()
    {
        if (canJump)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            playerRb.AddTorque(jumpTorque, ForceMode2D.Impulse);
            canJump = false;
        }
        
    }

    private void SwitchDimension()
    {
        LevelManager.Instance.SwitchDimension();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: Check which objects refresh jump or kill player
        //Make landing perfectly on the edge of the square
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
                    canJump = true;
                }
                
                return;

            case StringHelper.FINISHLINE_TAG:


                GameManager.Instance.FinishLevel();

                return;
            default:
                canJump = true;
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

    private IEnumerator AdjustRotation()
    {
        float rotationThreshold = 0.8f; // Adjust this value to control the threshold for snapping rotation

        while (playerRb.velocity.y < 0f)
        {

            RaycastHit2D hit = Physics2D.Raycast(playerRb.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                Vector2 groundPosition = hit.point;

                // Calculate the distance to the ground
                float distanceToGround = Mathf.Abs(playerRb.position.y - groundPosition.y);
                // Check if the player is close to the ground
                if (distanceToGround <= rotationThreshold)
                {
                    // Snap rotation to the nearest 90-degree angle
                    float currentRotation = playerRb.rotation;
                    float targetRotation = Mathf.Round(currentRotation / 90f) * 90f;

                    playerRb.rotation = targetRotation;

                    // Reset angular velocity to stop spinning
                    playerRb.angularVelocity = 0f;

                }
            }

            yield return null;
        }
    }

    public void Explode()
    {

        ParticleSystem particles = Instantiate(deathParticles, transform.position, transform.rotation).GetComponent<ParticleSystem>();

        var main = particles.main;
        main.startColor = spriteRenderer.color;

        gameObject.SetActive(false);
    }

}
