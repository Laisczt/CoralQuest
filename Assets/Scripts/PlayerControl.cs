using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerControl : MonoBehaviour, IDataSaver
{
    public GameObject baseAttack;           // Ataque comum
    private Rigidbody2D m_RigidBody;        // Corpo Rigido para fisica
    private SpriteRenderer m_SpriteRenderer;// Sprite
    private Animator m_Animator;            // Animador

    private float inputX;                   // Input esquerda/direita (com suavizacao)
    private short inputXdiscrete;           // Input esquerda/direita (sem suavizacao)
    private short inputLeftBuffer;          // Buffer para o input de movimento a esquerda (para facilitar wall jump)
    private short inputRightBuffer;         // Buffer para o input de movimento a direita (para facilitar wall jump)
    public float maxSpeedX;                 // Velocidade horizontal maxima
    public float jumpPower;                 // Forca do pulo
    public bool lockMovement = false;       // Trava de movimento horizontal do jogador, preservando o ultimo input usado (para forcar movimento durante transições de tela)
    public bool preventMovement = false;    // Impede mudanças na velocidade horizontal do jogador através desse script

    private ushort jumping;                 // Buffer do input de pulo (para facilitar pulos consecutivos)
    public ushort maxJumps = 2;             // Quantia maxima de pulos que podem ser feitos antes de tocar no ch�o
    private ushort rjumps;                  // Quantia de pulos restantes
    private bool canWallJump;               // Verdadeiro se o player pode fazer wall jump
    private short wallJumpDirection;        // Direcao do wall jump (-1 para esquerda e 1 para direita)
    private bool wallJumping;               // Verdadeiro durante a duração do impulso do wall jump

    private bool grounded = true;           // Verdadeiro se o player estiver tocando o ch�o
    private ushort groundBuff = 0;          // Buffer para a variavel grounded (permite coyote time)
    public bool alive = true;               // Player vivo
    public ushort maxHealth;                // Vida maxima do Player
    public int health;                      // Vida atual do Player
    public int DamageCooldown = 60;         // Cooldown de dano do jogador (i-frames)
    private int rDamageCooldown;            // Cooldown restante

    public int attackCooldown = 10;         // Cooldown entre attacks
    private int rAttackCooldown = 0;        // Cooldown restante
    private bool attacking = false;         // Jogador apertou pra atacar

    private bool startingAreaSet = false;

    public bool DEBUG_INVINCIBLE;

    [HideInInspector] public bool UsingMobileControls;      // Verdadeiro quando controles mobiles estiverem em uso
    [HideInInspector] public Joystick joystick;
    [HideInInspector] public UIControlButton jumpButton;
    [HideInInspector] public UIControlButton attackButton;
    public static PlayerControl Instance { get; private set; }


    LayerMask jumpResetLayerMask;
    // Awake is called when an enabled script instance is being loaded.
    private void Awake()
    {

        health = maxHealth;
        jumpResetLayerMask = LayerMask.GetMask( "Solid", "Platform");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Atribui��es de componentes (m_HealthBar � feita pela pr�pria barra de vida)
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (lockMovement)
        {
            inputX = inputXdiscrete;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            if (inputX == 0 && UsingMobileControls)
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

        if (Input.GetButtonDown("Jump") || (UsingMobileControls && jumpButton.GetButtonDown()))
        {
            jumping = 5;
        }

        if (( Input.GetButton("Fire1")   ||   (UsingMobileControls && attackButton.GetButtonDown())  )
            && rAttackCooldown == 0 ) // Attack input
        {
            attacking = true;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            //healing = true;
        }
    }

    void FixedUpdate()
    {
        if (!alive) return;

        m_Animator.SetFloat("Y speed", m_RigidBody.velocity.y); // Reporta a velocidade vertical ao animator 

        CheckGround(); // Verifica se o player est� no ch�o
        if (grounded) // Reseta os pulos se o player estiver no ch�o e atualiza o animador
        {
            rjumps = maxJumps;

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

        if (/*!wallJumping &&*/ !preventMovement) m_RigidBody.velocity = new Vector2(inputX * maxSpeedX, m_RigidBody.velocity.y);


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

                if (rjumps == maxJumps) rjumps--; // Desconta um pulo se esse for o primeiro (wall jumps costumam n�o gastar pulos)
                jumping = 0;

                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower);
                m_Animator.SetTrigger("Jump");
            }
            else if (rjumps > 0)
            {
                if (grounded)
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower);
                    rjumps--;
                    groundBuff = 0;

                    jumping = 0;
                    m_Animator.SetTrigger("Jump");
                }
                else if (maxJumps > 1)
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, jumpPower * 1.1f);
                    if (rjumps == maxJumps) rjumps--;
                    if (rjumps > 0) rjumps--;

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
        //healing = false;
        canWallJump = false;
        if (rDamageCooldown > 0) rDamageCooldown--;
        if (inputLeftBuffer > 0) inputLeftBuffer--;
        if (inputRightBuffer > 0) inputRightBuffer--;
        if (jumping > 0) jumping--;
        if (rAttackCooldown > 0) rAttackCooldown--;
    }

    IEnumerator WallJumpKick(int direction)
    {
        preventMovement = true;
        int i = 0;
        m_RigidBody.velocity = new Vector2(maxSpeedX * wallJumpDirection, m_RigidBody.velocity.y);
        
        while (i < 12)
        {
            if (wallJumpDirection != direction) break;
            i++;
            yield return new WaitForFixedUpdate();
        }

        preventMovement = false;

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
        if (collision.gameObject.CompareTag("Enemy Projectile"))
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
        Vector3 characterLeftEdge = transform.position - new Vector3(collider.bounds.extents.x - collider.offset.x, 0, 0);
        var step = collider.bounds.extents.x;

        for (int i = 0; i < 3; i++)
        {
            var hit = Physics2D.Raycast(characterLeftEdge + new Vector3(step * i,-_playerHeight, 0),  Vector2.down, 0.05f, jumpResetLayerMask);
            //Debug.DrawRay(characterLeftEdge + new Vector3(step * i, -_playerHeight, 0), Vector2.down * 0.05f, Color.red);
            if (hit.transform != null)
            {
                groundBuff = 5;
            }
        }
        grounded = groundBuff > 0;

    }

    public void Knockback(Vector2 forceVector)
    {
        var knockup = false;

        if (Vector2.Distance(forceVector.normalized, Vector2.up) < 0.3f) knockup = true;
        StartCoroutine(coroutine_Knockback(forceVector, knockup));
    }
    IEnumerator coroutine_Knockback(Vector2 forceVector, bool isKnockup)
    {
        m_RigidBody.velocity = forceVector;

        if(!isKnockup) preventMovement = true;

        var i = 12;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        preventMovement = false;
    }
    public bool Damage(int value)                   // Dano, retorna false se o player não pode levar dano
    {
        if (!alive || value <= 0) return false;
        if (rDamageCooldown > 0) return false;

        rDamageCooldown = DamageCooldown;

        if (!DEBUG_INVINCIBLE) health -= value;
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
        return true;
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

    public void SetMobileControls(GameObject[] controls)
    {
        foreach (var element in controls)
        {
            switch (element.name)
            {
                case "Joystick":
                    joystick = element.transform.GetChild(0).GetComponent<Joystick>();
                    break;
                case "Jump":
                    jumpButton = element.transform.GetChild(0).GetComponent<UIControlButton>();
                    break;
                case "Attack":
                    attackButton = element.transform.GetChild(0).GetComponent<UIControlButton>();
                    break;
                default:
                    Debug.LogError("Mobile UI element not recognized, did you rename something?");
                    break;
            }
        }
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