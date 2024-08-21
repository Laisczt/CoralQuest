using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class PlayerMovement : MonoBehaviour
{
    /*
        Movimentacao do player
        inc. corrida, pulo e knockback
    */
    [SerializeField, HideInInspector] PlayerControl m_PlayerControl;  // Controlador do jogador
    [SerializeField, HideInInspector] Rigidbody2D m_RigidBody;        // Corpo Rigido para fisica
    [SerializeField, HideInInspector] SpriteRenderer m_SpriteRenderer;// Sprite
    [SerializeField, HideInInspector] Animator m_Animator;            // Animador
    [SerializeField, HideInInspector] PlayerPetrification m_Petrify;  // Script de petrificação
    
    private float inputX;                   // Input esquerda/direita (com suavizacao)
    private short inputXdiscrete;           // Input esquerda/direita (sem suavizacao)
    private short inputLeftBuffer;          // Buffer para o input de movimento a esquerda (para facilitar wall jump)
    private short inputRightBuffer;         // Buffer para o input de movimento a direita (para facilitar wall jump)
    public float MaxSpeedX;                 // Velocidade horizontal maxima
    public float JumpPower;                 // Forca do pulo
    [HideInInspector] public bool LockMovement;   // Trava de movimento horizontal do jogador, preservando o ultimo input usado (para forcar movimento durante transições de tela)
    public bool PreventMovement;            // Impede mudanças na velocidade horizontal do jogador através desse script

    private ushort jumping;                 // Buffer do input de pulo (para facilitar pulos consecutivos)
    public ushort MaxJumps = 2;             // Quantia maxima de pulos que podem ser feitos antes de tocar no ch�o
    private ushort rjumps;                  // Quantia de pulos restantes
    private bool canWallJump;               // Verdadeiro se o player pode fazer wall jump
    private short wallJumpDirection;        // Direcao do wall jump (-1 para esquerda e 1 para direita)
    [SerializeField] AudioSource JumpSound;     // Som de pulo

    private bool grounded = true;           // Verdadeiro se o player estiver tocando o ch�o
    private ushort groundBuff;              // Buffer para a variavel grounded (permite coyote time)

    private LayerMask jumpResetLayerMask;   // Mask para colisão com superficies que resetam o pulo 

    [HideInInspector] public bool UsingMobileControls;      // Verdadeiro quando controles mobiles estiverem em uso
    [SerializeField, HideInInspector] public Joystick Joystick;             // Joystick de controles mobile
    [SerializeField, HideInInspector] public UIControlButton JumpButton;    // Botão de pulo mobile
    


    private float lastJoystickInput;        // Última direção de input do joystick

    void OnValidate()
    {
        m_PlayerControl = GetComponent<PlayerControl>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_Petrify = GetComponent<PlayerPetrification>();
    }
    void Start()
    {
        jumpResetLayerMask = LayerMask.GetMask( "Solid", "Platform");
    }

    // Update is called once per frame
    void Update()
    {
        // Input de movimento
        if (LockMovement)
        {
            inputX = inputXdiscrete;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            if (inputX == 0 && UsingMobileControls)
            {
                inputX = Joystick.Horizontal;
                
                lastJoystickInput = inputX;
            }

        }
        if(inputX != 0)
        {
            var prev = inputXdiscrete;
            inputXdiscrete = (short)Mathf.Sign(inputX);
            if(inputXdiscrete != prev) m_Petrify.Shake();
        }
        else
        {
            inputXdiscrete = 0;
        }



        // Buffering de direção de Walljump
        short walljumpbuffer = 5;
        var rawInput = inputX;
        if (Mathf.Abs(rawInput) != 1) rawInput = 0;

        if (inputXdiscrete > 0)
        {
            inputRightBuffer = walljumpbuffer;
        }
        else if (inputXdiscrete < 0)
        {
            inputLeftBuffer = walljumpbuffer;
        }

        // Pulo
        if (Input.GetButtonDown("Jump") || (UsingMobileControls && JumpButton.GetButtonDown()))
        {
            jumping = 5;
        }
    }

    void FixedUpdate()
    {
        if(!m_PlayerControl.Alive)
        {
            m_RigidBody.velocity = new Vector2(0 , m_RigidBody.velocity.y);
            return;
        }
        if(m_PlayerControl.Petrified) return;

        m_Animator.SetFloat("Y speed", m_RigidBody.velocity.y); // Reporta a velocidade vertical ao animator 

        CheckGround(); // Verifica se o player est� no ch�o
        if (grounded) // Reseta os pulos se o player estiver no ch�o e atualiza o animador
        {
            rjumps = MaxJumps;

            m_Animator.SetBool("Grounded", true);
        }
        else
        {
            m_Animator.SetBool("Grounded", false);
        }

        if (!grounded && m_RigidBody.velocity.y < 0f) // Altera a resistência do ar ao cair
        {
            m_RigidBody.drag = 5f;
        }
        else
        {
            m_RigidBody.drag = 0f;
        }

        // MOVIMENTO HORIZONTAL
        m_Animator.SetBool("Running", inputXdiscrete != 0);

        if (!PreventMovement) m_RigidBody.velocity = new Vector2(inputX * MaxSpeedX, m_RigidBody.velocity.y);


        // ORIENTACAO DO SPRITE
        if (inputX < 0)
        {
            m_PlayerControl.SpriteOrientation = -1;
            m_SpriteRenderer.flipX = true;
        }
        else if (inputX > 0)
        {
            m_PlayerControl.SpriteOrientation = 1;
            m_SpriteRenderer.flipX = false;
        }

        // PULO
        if (jumping > 0)
        {
            if (canWallJump && !grounded && ((wallJumpDirection == 1 && inputLeftBuffer > 0) || ( wallJumpDirection == -1 && inputRightBuffer > 0)))
            // Realiza wall jump se o player estiver deslizando numa parede e se movendo na dire��o dela
            {
                _wallkickcoroutine = StartCoroutine(WallJumpKick(wallJumpDirection));

                if (rjumps == MaxJumps) rjumps--; // Desconta um pulo se esse for o primeiro (wall jumps costumam nao gastar pulos)
                jumping = 0;

                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpPower * 0.97f);
                m_Animator.SetTrigger("Jump");
                JumpSound.PlayOneShot(JumpSound.clip);
            }
            else if (rjumps > 0)
            {
                if (grounded)
                // Pulo no chão
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpPower);
                    rjumps--;
                    groundBuff = 0;

                    jumping = 0;
                    m_Animator.SetTrigger("Jump");
                    JumpSound.PlayOneShot(JumpSound.clip);
                }
                else if (MaxJumps > 1)
                // Pulo no ar
                {
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpPower * 1.1f);
                    if (rjumps == MaxJumps) rjumps--;   // Desconta um pulo a mais se esse for o primeiro
                    if (rjumps > 0) rjumps--;

                    jumping = 0;
                    m_Animator.SetTrigger("Jump");
                    JumpSound.PlayOneShot(JumpSound.clip);

                }

            }
        }

        // Redefinir Variaveis
        canWallJump = false;
        if (inputLeftBuffer > 0) inputLeftBuffer--;
        if (inputRightBuffer > 0) inputRightBuffer--;
        if (jumping > 0) jumping--;
    }

    Coroutine _wallkickcoroutine;
    IEnumerator WallJumpKick(int direction)
    // Mantem velocidade horizontal do jogador por uma duração logo após um wall jump
    {
        PreventMovement = true;
        int i = 0;
        m_RigidBody.velocity = new Vector2(MaxSpeedX * wallJumpDirection, m_RigidBody.velocity.y);
        
        while (i < 12)
        {
            i++;
            yield return new WaitForFixedUpdate();
        }

        PreventMovement = false;

    }
    private void CheckGround()
    // Verifica se o jogador está no chão
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Solid"))
        {
            var olddir = wallJumpDirection;
            // Define direção para wall jump
            if (Vector2.Distance(collision.contacts[0].normal, Vector2.left) < 0.1f) // Se o player estiver tocando uma parede à direita
            {
                wallJumpDirection = -1;
                canWallJump = true;
            }
            else if (Vector2.Distance(collision.contacts[0].normal, Vector2.right) < 0.1f) //  Se o player estiver tocando uma parede à esquerda
            {
                wallJumpDirection = 1;
                canWallJump = true;
            }
            if(olddir != wallJumpDirection && _wallkickcoroutine != null){
                StopCoroutine(_wallkickcoroutine);
                PreventMovement = false;
            } 
        }
    }

    public void Knockback(float force, float xDirStrenth)
    // Método público para aplicar knockback ao jogador
    {
        if(!m_PlayerControl.Alive) return;
        StartCoroutine(coroutine_Knockback(force, xDirStrenth));
    }
    IEnumerator coroutine_Knockback(float force, float xDirStrenth)
    {
        m_RigidBody.velocity = new Vector2(xDirStrenth * 7, force - Mathf.Min( Mathf.Abs(xDirStrenth) * 7, 10));

        if(xDirStrenth != 0) PreventMovement = true;

        var i = 12;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        PreventMovement = false;
    }
}
