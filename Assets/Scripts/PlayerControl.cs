using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public GameObject baseAttack;
    private SpriteRenderer m_SpriteRenderer;
    public GameObject m_HealthBar;

    public int maxHealth;                   // Player max hp
    [HideInInspector]
    public int health;                      // Player current hp
    public float maxSpeedX = 1f;            // Max horizontal movement speed
    public float jumpPower = 1f;            // Jump power
    private bool jumping = false;           // Player is attempting to jump
    private bool canWallJump = false;       // Character is touching a wall
    private short wallJumpDirection = 0;    // Direction a wall jump would take place in
    private int jumps = 1;                  // How many air jumps the player can still do before touching the ground
    public int maxJumps = 2;                // Maximum amount of air jumps
    private bool grounded = false;          // Player is on the ground
    private int wallJumping = 0;            // Remaining duration of wall jump kick
    private float inputX;                   // X axis input
    private float inputXdiscrete;           // X axis input without smoothing
    public int attackCooldown = 10;         // Cooldown between attacks
    private int rAttackCooldown = 0;        // remaining cooldown between attacs

    // Start is called before the first frame update

    private void Awake()
    {
        health = maxHealth;
    }
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump")) // Left/Right input
        {
            jumping = true;
        }
        inputX = Input.GetAxis("Horizontal");
        inputXdiscrete = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Fire1") && rAttackCooldown == 0) // Attack input
        {
            rAttackCooldown = attackCooldown;
            Instantiate(baseAttack, transform.position, Quaternion.Euler(new Vector3(0,(m_SpriteRenderer.flipX)? 180 : 0,0)));
        }
    }

    void FixedUpdate()
    {

        if(wallJumping > 0) // Wall jump kick
        {
            myRigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, myRigidBody.velocity.y);
        }
        else // Walk
        { 
            myRigidBody.velocity = new Vector2(inputX * maxSpeedX, myRigidBody.velocity.y);
        }
        if (inputX < 0) m_SpriteRenderer.flipX = true;
        else if (inputX > 0) m_SpriteRenderer.flipX = false;


        if (jumping) 
        {
            if (canWallJump && !grounded && -inputXdiscrete == wallJumpDirection) // Wall jump
            {
                wallJumping = 12; // Kick duration

                if (jumps == maxJumps) jumps--; // If first jump is a wall jump, remove a jump

                myRigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, jumpPower*0.7f);
            }
            else if (jumps > 0) // Jumps and multijumps
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpPower);
                jumps--;
            }
        }

        if(wallJumping > 0) wallJumping--;
        canWallJump = false;
        grounded = false;
        jumping = false;
        if (rAttackCooldown > 0) rAttackCooldown--;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Solid") && Vector2.Distance(collision.contacts[0].normal, Vector2.up) < 0.1f) // Reset jumps when touching ground
        {
            jumps = maxJumps;
        }
    }
    private void OnCollisionStay2D(Collision2D collision) // Allows wall jumps when on a wall
    {
        if (collision.gameObject.CompareTag("Solid"))
        {
            if (Vector2.Distance(collision.contacts[0].normal, Vector2.up) < 0.1f) // If player is on the ground
            {
                grounded = true;
            }
            if (Vector2.Distance(collision.contacts[0].normal, Vector2.left) < 0.1f) // If player is on the left side of a wall 
            {
                wallJumpDirection = -1;
                canWallJump = true;
            }
            else if (Vector2.Distance(collision.contacts[0].normal, Vector2.right) < 0.1f) // If player is on the right side of a wall
            {
                wallJumpDirection = 1;
                canWallJump = true;
            }
        }       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var damage = collision.gameObject.GetComponent<BasicEnemy>().damage;
            health -= damage;
            if (health < 0) health = 0;
            m_HealthBar.GetComponent<HealthBarControl>().Damage(damage);
        }/*else if (collision.gameObject.CompareTag("Heal"))
        {
        
             * todo

        }*/
    }
}
