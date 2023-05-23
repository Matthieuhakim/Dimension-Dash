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

    private float jumpForce = 12f;

    //private float jumpTorque = -0.6f;


    private Rigidbody2D playerRb;


    private bool canJump = true;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveRight();
        HandleMouseClick();
        
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

    private void MoveRight()
    {
        transform.Translate(Vector2.right * playerSpeed * Time.deltaTime, Space.World);
    }


    private void Jump()
    {
        if (canJump)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //playerRb.AddTorque(jumpTorque, ForceMode2D.Impulse);
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
        canJump = true;
    }
}
