using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    public GameObject baseAttack;           // Ataque comum
    private Rigidbody2D m_RigidBody;        // Corpo Rígido para física
    private SpriteRenderer m_SpriteRenderer;// Sprite
    private Animator m_Animator;            // Handler de animações
    [HideInInspector]           
    public HealthBarControl m_HealthBar;    // Barra de Vida

    private float inputX;                   // Input esquerda/direita (com suavização)
    private short inputXdiscrete;           // Input esquerda/direita (puro)
    public float maxSpeedX = 1f;            // Velocidade horizontal máxima
    public float jumpPower = 1f;            // Força do pulo

    private bool jumping = false;           // Verdadeiro se o player estiver tentando pular
    public ushort maxJumps = 2;             // Quantia máxima de pulos que podem ser feitos antes de tocar no chão
    private ushort jumps;                   // Quantia de pulos restantes
    private bool canWallJump = false;       // Verdadeiro se o player pode fazer wall jump
    private short wallJumpDirection;        // Direção do wall jump (-1 para esquerda e 1 para direita)
    private ushort wallJumping = 0;         // Duração do kick do wall jump restante

    private bool grounded = true;           // Verdadeiro se o player estiver tocando o chão
    public bool alive = true;               // Player vivo
    public int score = 0;                   // Placar
    private bool healing = false;           // Player está tentando curar
    public ushort maxHealth;                // Vida máxima do Player
    [HideInInspector]
    public int health;                      // Vida atual do Player

    public int attackCooldown = 10;         // Cooldown entre attacks
    private int rAttackCooldown = 0;        // Cooldown restante


    // Awake is called when an enabled script instance is being loaded.
    private void Awake()
    {
        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Atribuições de componentes (m_HealthBar é feita pela própria barra de vida)
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputXdiscrete = (short) Input.GetAxisRaw("Horizontal");

        if (!alive) return;
        
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }

        if (Input.GetButtonDown("Fire1") && rAttackCooldown == 0) // Attack input
        {
            rAttackCooldown = attackCooldown;
            Instantiate(baseAttack, transform.position, Quaternion.Euler(new Vector3(0,(m_SpriteRenderer.flipX)? 180 : 0,0)));
        }
        if (Input.GetButtonDown("Fire2"))
        {
            healing = true;
        }
    }

    void FixedUpdate()
    {
        if (!alive) return;

        m_Animator.SetFloat("Y speed", m_RigidBody.velocity.y); // Reporta a velocidade vertical ao animador 

        CheckGround(); // Verifica se o player está no chão
        if (grounded) // Reseta os pulos se o player estiver no chão e atualiza o animador
        {
            jumps = maxJumps; 

            m_Animator.SetBool("Grounded", true);
        }
        else
        {
            m_Animator.SetBool("Grounded", false);
        }


        // MOVIMENTO HORIZONTAL
        if(wallJumping > 0) // Caso o player tenha feito um wall jump, sua velocidade é travada por alguns frames
        {
            m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, m_RigidBody.velocity.y);
        }
        else // Movimentação normal pelo input horizontal
        {
            m_Animator.SetBool("Running", inputXdiscrete != 0);
            m_RigidBody.velocity = new Vector2(inputX * maxSpeedX, m_RigidBody.velocity.y);
        }

        // Orientação do sprite
        if (inputX < 0) m_SpriteRenderer.flipX = true;
        else if (inputX > 0) m_SpriteRenderer.flipX = false;


        // PULO
        if (jumping)
        {
            if (canWallJump && !grounded && -inputXdiscrete == wallJumpDirection)
            // Realiza wall jump se o player estiver deslizando numa parede e se movendo na direção dela
            {
                wallJumping = 12; // Duração do kick

                if (jumps == maxJumps) jumps--; // Desconta um pulo se esse for o primeiro (wall jumps costumam não gastar pulos)

                m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, jumpPower*0.7f); // Wall jumps são menos verticais que pulos normais
            }
            else if (jumps > 0) // Pulos Comuns
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower);
                jumps--;

                m_Animator.SetTrigger("Jump");
            }
        }

        // Reset de variáveis
        canWallJump = false;
        jumping = false;
        healing = false;
        if (wallJumping > 0) wallJumping--;
        if (rAttackCooldown > 0) rAttackCooldown--;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
    
    }
    private void OnCollisionStay2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Solid"))
        {
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

    private void OnCollisionExit2D(Collision2D collision)
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        { 
            var damage = collision.gameObject.GetComponent<BasicEnemy>().damage;
            Damage(damage);
        }
        else if(collision.gameObject.CompareTag("Enemy Projectile"))
        {
            var damage = collision.gameObject.GetComponent<BasicProjectile>().value;
            Damage(damage);
        }
        else if (collision.gameObject.CompareTag("Heal"))
        {
            var heal = collision.gameObject.GetComponent<BasicProjectile>().value;
            Heal(heal);
        }
    }

    private void CheckGround()
    {
        var _playerHeight = GetComponent<Collider2D>().bounds.extents.y - GetComponent<Collider2D>().offset.y;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _playerHeight + 0.05f);
        //Debug.DrawRay(transform.position, (_playerHeight + 0.05f) * Vector2.down, Color.red);
        if (hit.transform != null)
        {
            grounded = hit.transform.CompareTag("Solid");
        }
        else
        {
            grounded = false;
        }
        
    }

    private void Damage(int value)
    {
        if (!alive) return;

        health -= value;
        if (health <= 0)
        {
            health = 0;
            Kill();
        }
        else
        {
            m_Animator.SetTrigger("Damage");
        }
        m_HealthBar.UpdateHB();
    }

    private void Heal(int value)
    {
        if (!alive) return;

        health += value;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        m_HealthBar.UpdateHB();
        Debug.Log(health);
    }

    [ContextMenu("Heal 1")]
    public void Heal1()
    {
        Heal(1);
    }

    [ContextMenu("Damage 1")]
    public void Damage1()
    {
        Damage(1);
    }

    [ContextMenu("Recover")]
    public void Recover()
    {
        alive = true;
        Heal(maxHealth);
        m_Animator.SetTrigger("DEBUG REVIVE");
    }

    [ContextMenu("Kill")]
    public void Kill()
    {
        alive = false;

        m_Animator.SetTrigger("Death");
        m_Animator.SetBool("Running", false);
        m_RigidBody.velocity = Vector2.zero;
    }
}
