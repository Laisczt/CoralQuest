using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerControl : MonoBehaviour, IDataSaver
{
    public GameObject baseAttack;           // Ataque comum
    private Rigidbody2D m_RigidBody;        // Corpo R�gido para f�sica
    private SpriteRenderer m_SpriteRenderer;// Sprite
    private Animator m_Animator;            // Handler de anima��es
    [HideInInspector]

    private float inputX;                   // Input esquerda/direita (com suaviza��o)
    private short inputXdiscrete;           // Input esquerda/direita (puro)
    public float maxSpeedX = 1f;            // Velocidade horizontal m�xima
    public float jumpPower = 1f;            // For�a do pulo
    public bool lockMovement = false;

    private ushort jumping = 0;             // Buffer de pulo
    public ushort maxJumps = 2;             // Quantia m�xima de pulos que podem ser feitos antes de tocar no ch�o
    private ushort jumps;                   // Quantia de pulos restantes
    private bool canWallJump = false;       // Verdadeiro se o player pode fazer wall jump
    private short wallJumpDirection;        // Dire��o do wall jump (-1 para esquerda e 1 para direita)
    private ushort wallJumping = 0;         // Dura��o do kick do wall jump restante

    private bool grounded = true;           // Verdadeiro se o player estiver tocando o ch�o
    private ushort groundBuff = 0;
    public bool alive = true;               // Player vivo
    public int score = 0;                   // Placar
    private bool healing = false;           // Player est� tentando curar
    public ushort maxHealth;                // Vida m�xima do Player
    public int health = 5;                      // Vida atual do Player

    public int attackCooldown = 10;         // Cooldown entre attacks
    private int rAttackCooldown = 0;        // Cooldown restante
    private bool attacking = false;

    private bool startingAreaSet = false;

    // Awake is called when an enabled script instance is being loaded.

    // Start is called before the first frame update
    void Start()
    {
        // Atribui��es de componentes (m_HealthBar � feita pela pr�pria barra de vida)
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        if (lockMovement)
        {
            inputXdiscrete = (short)Mathf.Sign(inputX);
            inputX = inputXdiscrete;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputXdiscrete = (short)Input.GetAxisRaw("Horizontal");
        }

        
        if (!alive) return;

        if (Input.GetButtonDown("Jump"))
        {
            jumping = 5;
        }

        if (Input.GetButton("Fire1") && rAttackCooldown == 0) // Attack input
        {
            attacking = true;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            healing = true;
        }
    }

    void FixedUpdate()
    {
        if (!alive) return;

        m_Animator.SetFloat("Y speed", m_RigidBody.velocity.y); // Reporta a velocidade vertical ao animator 

        CheckGround(); // Verifica se o player est� no ch�o
        if (grounded) // Reseta os pulos se o player estiver no ch�o e atualiza o animador
        {
            jumps = maxJumps;

            m_Animator.SetBool("Grounded", true);
        }
        else
        {
            m_Animator.SetBool("Grounded", false);
        }

        if (!grounded && m_RigidBody.velocity.y < 0f)
        {
            m_RigidBody.drag = 5f;
        }
        else
        {
            m_RigidBody.drag = 0f;
        }


        // MOVIMENTO HORIZONTAL
        if (wallJumping > 0) // Caso o player tenha feito um wall jump, sua velocidade � travada por alguns frames
        {
            m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, m_RigidBody.velocity.y);
        }
        else // Movimenta��o normal pelo input horizontal
        {
            m_Animator.SetBool("Running", inputXdiscrete != 0);
            m_RigidBody.velocity = new Vector2(inputX * maxSpeedX, m_RigidBody.velocity.y);
        }

        // Orienta��o do sprite
        if (inputX < 0) m_SpriteRenderer.flipX = true;
        else if (inputX > 0) m_SpriteRenderer.flipX = false;
        



        // PULO
        if (jumping > 0)
        {
            if (canWallJump && !grounded && -inputXdiscrete == wallJumpDirection)
            // Realiza wall jump se o player estiver deslizando numa parede e se movendo na dire��o dela
            {
                wallJumping = 12; // Dura��o do kick

                if (jumps == maxJumps) jumps--; // Desconta um pulo se esse for o primeiro (wall jumps costumam n�o gastar pulos)
                jumping = 0;

                m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, jumpPower); // Wall jumps s�o menos verticais que pulos normais
            }
            else if (jumps > 0)
            {
                if (grounded)
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower);
                    jumps--;
                    groundBuff = 0;

                    jumping = 0;
                    m_Animator.SetTrigger("Jump");
                }
                else if (maxJumps > 1)
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower * 1.1f);
                    if (jumps == maxJumps) jumps--;
                    if (jumps > 0) jumps--;

                    jumping = 0;
                    m_Animator.SetTrigger("Jump");

                }

            }
        }

        // ATAQUE
        if (attacking)
        {
            rAttackCooldown = attackCooldown;
            Instantiate(baseAttack, transform.position, Quaternion.Euler(new Vector3(0, (m_SpriteRenderer.flipX) ? 180 : 0, 0)));
        }


        // Redefinir variaveis
        attacking = false;
        canWallJump = false;
        healing = false;
        if (jumping > 0) jumping--;
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
        else if (collision.gameObject.CompareTag("Enemy Projectile"))
        {
            var damage = collision.gameObject.GetComponent<BasicProjectile>().value;
            Damage(damage);
        }
        /*else if (collision.gameObject.CompareTag("Heal"))
        {
            var heal = collision.gameObject.GetComponent<BasicProjectile>().value;
            Heal(heal);
        }*/

        if (!startingAreaSet)
        {
            // A �rea da c�mera quando o jogo come�a ser� a mesma �rea em que o jogador est�
            if (collision.gameObject.CompareTag("Camera Areas"))
            {
                startingAreaSet = true;
                MainCamera.Instance.ChangeArea(collision.gameObject);       // Muda a �rea da c�mera
                collision.transform.Find("Exits").gameObject.SetActive(true); // Ativa as sa�das da �rea
            }
        }
    }

    private void CheckGround()
    {
        var _playerHeight = GetComponent<Collider2D>().bounds.extents.y - GetComponent<Collider2D>().offset.y;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _playerHeight + 0.05f);
        //Debug.DrawRay(transform.position, (_playerHeight + 0.05f) * Vector2.down, Color.red);
        if (hit.transform != null)
        {
            groundBuff = (ushort)((hit.transform.CompareTag("Solid")) ? 5 : 0);
        }
        else
        {
            if (groundBuff > 0) groundBuff--;
        }

        grounded = groundBuff > 0;

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
        HealthBar.Instance.UpdateHB();
    }

    private void Heal(int value)
    {
        if (!alive) return;

        health += value;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        HealthBar.Instance.UpdateHB();
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


    public void loadData(gameData data){
        this.health = data.hp;
        this.transform.position = data.position;

        HealthBar.Instance.UpdateHB();
    }

    public void saveData(ref gameData data){
        Debug.Log(health);
        data.hp = this.health;
        data.position = this.transform.position;
    }

}