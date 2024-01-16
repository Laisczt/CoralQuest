using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerControl : MonoBehaviour
{
    public GameObject baseAttack;           // Ataque comum
    private Rigidbody2D m_RigidBody;        // Corpo R�gido para f�sica
    private SpriteRenderer m_SpriteRenderer;// Sprite
    private Animator m_Animator;            // Handler de anima��es

    private float inputX;                   // Input esquerda/direita (com suaviza��o)
    private short inputXdiscrete;           // Input esquerda/direita (puro)
    private short inputLeftBuffer = 0;
    private short inputRightBuffer = 0;
    public float maxSpeedX = 1f;            // Velocidade horizontal m�xima
    public float jumpPower = 1f;            // For�a do pulo
    public bool lockMovement = false;
    public bool forceJoystick = false;

    private ushort jumping = 0;             // Buffer de pulo
    public ushort maxJumps = 2;             // Quantia m�xima de pulos que podem ser feitos antes de tocar no ch�o
    private ushort jumps;                   // Quantia de pulos restantes
    private bool canWallJump = false;       // Verdadeiro se o player pode fazer wall jump
    private short wallJumpDirection;        // Dire��o do wall jump (-1 para esquerda e 1 para direita)
    private bool wallJumping = false;         // Dura��o do kick do wall jump restante

    private bool grounded = true;           // Verdadeiro se o player estiver tocando o ch�o
    private ushort groundBuff = 0;
    public bool alive = true;               // Player vivo
    public int score = 0;                   // Placar
    private bool healing = false;           // Player est� tentando curar
    public ushort maxHealth;                // Vida m�xima do Player
    [HideInInspector]
    public int health;                      // Vida atual do Player

    public int attackCooldown = 10;         // Cooldown entre attacks
    private int rAttackCooldown = 0;        // Cooldown restante
    private bool attacking = false;

    private bool startingAreaSet = false;
    private bool usingMobileControls = false;

    private Joystick joystick;
    private UIControlButton jumpButton;
    private UIControlButton attackButton;

    // Awake is called when an enabled script instance is being loaded.
    private void Awake()
    {
        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Atribui��es de componentes (m_HealthBar � feita pela pr�pria barra de vida)
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();

        if (GameControl.usingMobileControls)
        {
            GetMobileControls(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (forceJoystick && !usingMobileControls)
        {
            GameControl.UseMobileControls();
            GetMobileControls(true);
        }

        if (lockMovement)
        {
            inputX = inputXdiscrete;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            if (inputX == 0 && usingMobileControls)
            {
                inputX = joystick.Horizontal;
            }
        }
       
        if(inputX != 0)
        {
            inputXdiscrete = (short)Mathf.Sign(inputX);
        }
        else
        {
            inputXdiscrete = 0;
        }


        short walljumpbuffer = 3;
        var rawInput = inputX;
        if (Mathf.Abs(rawInput) != 1) rawInput = 0;

        if (rawInput > 0)
        {
            inputRightBuffer = walljumpbuffer;
        }
        else if (inputXdiscrete < 0)
        {
            inputLeftBuffer = walljumpbuffer;
        }
        
        
        if (!alive) return;

        if (Input.GetButtonDown("Jump") || (usingMobileControls && jumpButton.GetButtonDown()))
        {
            jumping = 5;
        }

        if (( Input.GetButton("Fire1")   ||   (usingMobileControls && attackButton.GetButtonDown())  )
            && rAttackCooldown == 0 ) // Attack input
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
        m_Animator.SetBool("Running", inputXdiscrete != 0);

        if (!wallJumping) m_RigidBody.velocity = new Vector2(inputX * maxSpeedX, m_RigidBody.velocity.y);


        // Orienta��o do sprite
        if (inputX < 0) m_SpriteRenderer.flipX = true;
        else if (inputX > 0) m_SpriteRenderer.flipX = false;
        



        // PULO
        if (jumping > 0)
        {
            if (canWallJump && !grounded && ((wallJumpDirection == 1)? inputLeftBuffer > 0: inputRightBuffer > 0))
            // Realiza wall jump se o player estiver deslizando numa parede e se movendo na dire��o dela
            {
                StartCoroutine(WallJumpKick(wallJumpDirection));

                Debug.Log((wallJumpDirection == 1) ? inputLeftBuffer : inputRightBuffer);

                if (jumps == maxJumps) jumps--; // Desconta um pulo se esse for o primeiro (wall jumps costumam n�o gastar pulos)
                jumping = 0;

                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower); 
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
        healing = false;
        canWallJump = false;
        if (inputLeftBuffer > 0) inputLeftBuffer--;
        if (inputRightBuffer > 0) inputRightBuffer--;
        if (jumping > 0) jumping--;
        if (rAttackCooldown > 0) rAttackCooldown--;
    }

    IEnumerator WallJumpKick(int direction)
    {
        wallJumping = true;
        int i = 0;
        m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, m_RigidBody.velocity.y);
        
        while (i < 12)
        {
            if (wallJumpDirection != direction) break;
            i++;
            yield return new WaitForFixedUpdate();
        }

        wallJumping = false;

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
        if (groundBuff > 0) groundBuff--;

        var collider = GetComponent<Collider2D>();
        var _playerHeight = collider.bounds.extents.y - collider.offset.y;
        Vector3 characterLeftEdge = transform.position - new Vector3(collider.bounds.extents.x, 0, 0);
        var step = collider.bounds.extents.x;

        var hits = new List<RaycastHit2D>();

        for (int i = 0; i < 3; i++)
        {
            hits.Add(Physics2D.Raycast(characterLeftEdge + new Vector3(step * i,0,0), Vector2.down, _playerHeight + 0.05f));
            //Debug.DrawRay(characterLeftEdge + new Vector3(step * i, 0, 0), Vector2.down * (_playerHeight + 0.05f), Color.red);
        }

        foreach(var element in hits)
        {
            if (element.transform != null && element.transform.CompareTag("Solid")) groundBuff = 5;
        }
        
        grounded = groundBuff > 0;

    }

    public void Damage(int value)
    {
        if (!alive || value <= 0) return;

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

    public void Heal(int value)
    {
        if (!alive || value <= 0) return;

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

    private void GetMobileControls(bool instantiate)
    {
        if (instantiate)
        {
            GameControl.UseMobileControls();
        }
        joystick = FindObjectOfType<Joystick>();
        jumpButton = GameObject.Find("Jump Button").GetComponent<UIControlButton>();
        attackButton = GameObject.Find("Attack Button").GetComponent<UIControlButton>();
        usingMobileControls = true;
    }

}