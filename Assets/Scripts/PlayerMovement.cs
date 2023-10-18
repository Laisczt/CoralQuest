using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public float maxSpeedX = 1f;
    public float jumpPower = 1f;
    private bool jumping = false;
    private bool canWallJump = false;
    private int wallJumpDirection = 0;
    private int jumps = 1;
    public int maxJumps = 2;
    private bool grounded = false;
    private int wallJumping = 0;
    private float inputX;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }
        inputX = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {

        if(wallJumping > 0)
        {
            myRigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, myRigidBody.velocity.y);
        }
        else
        { 
            myRigidBody.velocity = new Vector2(inputX * maxSpeedX, myRigidBody.velocity.y);
        }
        


        if (jumping)
        {
            if (canWallJump && !grounded)
            {
                wallJumping = 12;

                myRigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, jumpPower*0.8f);
            }
            else if (jumps > 0)
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpPower);
                jumps--;
            }
        }

        if(wallJumping > 0) wallJumping--;
        canWallJump = false;
        grounded = false;
        jumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Vector2.Distance(collision.contacts[0].normal, Vector2.up) < 0.1f)
        {
            jumps = maxJumps;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Vector2.Distance(collision.contacts[0].normal, Vector2.up) < 0.1f)
        {
            grounded = true;
        }
        else if (Vector2.Distance(collision.contacts[0].normal, Vector2.left) < 0.1f)
        {
            wallJumpDirection = -1;
            canWallJump = true;
        }
        else if (Vector2.Distance(collision.contacts[0].normal, Vector2.right) < 0.1f)
        {
            wallJumpDirection = 1;
            canWallJump = true;
        }
            
    }

}
