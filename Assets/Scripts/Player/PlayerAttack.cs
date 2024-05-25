using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerControl))]
public class PlayerAttack : MonoBehaviour
{

    [SerializeField, HideInInspector] PlayerControl m_PlayerControl;  // Controlador
    [SerializeField, HideInInspector] PlayerPetrification m_Petrify;  // Script de petrificaçao
    [SerializeField, HideInInspector] Animator m_Animator;            // Animador
    public int AttackCooldown = 12;         // Cooldown entre attacks
    private int rAttackCooldown;            // Cooldown restante
    private bool attacking;                 // Jogador apertou pra atacar
    private float attackOffsetX = 2;        // Offset da posição do ataque principal em x
    private float attackOffsetY = 0.15f;    // '' em y

    public GameObject BaseAttack;           // Ataque comum
    [SerializeField] AudioSource attackSound;   // Som de ataque

    [HideInInspector] public bool UsingMobileControls;      // Verdadeiro quando controles mobiles estiverem em uso
    [SerializeField, HideInInspector] public UIControlButton AttackButton;  // Botão de ataque mobile

    void OnValidate()
    {
        m_PlayerControl = GetComponent<PlayerControl>();
        m_Petrify = GetComponent<PlayerPetrification>();
        m_Animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Ataque
        if (( Input.GetButton("Fire1")   ||   (UsingMobileControls && AttackButton.GetButtonDown())  )
            && rAttackCooldown == 0 )
        {
            if(m_PlayerControl.Petrified) m_Petrify.Shake();
            else attacking = true;
        }
    }

    void FixedUpdate()
    {
         // ATAQUE
        if (attacking)
        {
            rAttackCooldown = AttackCooldown;
            Instantiate(BaseAttack, transform.position + new Vector3(attackOffsetX * m_PlayerControl.SpriteOrientation, attackOffsetY), Quaternion.Euler(new Vector3(0, -90 + m_PlayerControl.SpriteOrientation * 90, 0)));
            m_Animator.SetTrigger("Attack");
            attackSound.Play();
        }

        // Redefinir Variaveis
        attacking = false;
        if (rAttackCooldown > 0) rAttackCooldown--;
    }
}
