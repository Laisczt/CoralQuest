using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerControl))]
public class PlayerHealth : MonoBehaviour
{
    /*
        Vida do player
        inc dano, morte e vida
    */
    [HideInInspector] public PlayerControl m_PlayerControl;  // Controlador
    [HideInInspector] public Animator m_Animator;            // Animador
    [HideInInspector] public PlayerMovement m_PlayerMovement;// Movimento do jogador

    public AudioSource damageSound;
    public int MaxHealth;                // Vida maxima do Player
    public int Health;                      // Vida atual do Player
    public int DamageCooldown = 60;         // Cooldown de dano do jogador (i-frames)
    private int rDamageCooldown;            // Cooldown restante

    public bool DEBUG_INVINCIBLE;           // Torna o player invencível (PARA DEBUG, SOMENTE ACESSIVEL NO EDITOR)


    void OnValidate()
    {
        m_PlayerControl = GetComponent<PlayerControl>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
    }
    void Awake()
    {
        Health = MaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        m_Animator = GetComponent<Animator>();   

    }


    void FixedUpdate()
    {
        // Redefinir variaveis
        if (rDamageCooldown > 0) rDamageCooldown--;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Healing Base"))
        // Tenta curar o jogador enquanto ele estiver ná área de uma árvore
        {   
            if(Health < MaxHealth) Heal(collision.GetComponent<HealingBase>().Use());
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Healing Base") || collision.gameObject.CompareTag("Save Point"))    // Salva o jogo
        {
            if(m_PlayerControl.LastSavePos != collision.transform.position)
            {
                m_PlayerControl.LastSavePos = collision.transform.position;
                Debug.Log("Checkpoint Alcançado");
            }
            dataSaverManager.instance.saveGame(Health < MaxHealth || m_PlayerControl.LastSavePos != collision.transform.position); // Salva o jogo (força a salvar se o player estiver com vida baixa ou este for um savepoint novo)
        }
    }
    
    public bool Damage(int value)                   // Dano, retorna false se o player nao pode levar dano (ainda funciona quando invencivel)
    {
        if (!m_PlayerControl.Alive || value <= 0) return false;
        if (rDamageCooldown > 0) return false;

        if (m_PlayerControl.Petrified) m_PlayerControl.Depetrify(false);

        rDamageCooldown = DamageCooldown;

        if (!DEBUG_INVINCIBLE) Health -= value;
        if (Health <= 0)
        {
            Health = 0;
            m_PlayerControl.Kill();
        }
        else
        {
            m_Animator.SetTrigger("Damage");
            damageSound.PlayOneShot(damageSound.clip);
        }
        HealthBar.Instance.UpdateHB();
        return true;
    }

    

    public void Heal(int value)                     // Cura
    {
        if (!m_PlayerControl.Alive || value <= 0) return;

        Health += value;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        HealthBar.Instance.UpdateHB();
    }

    [ContextMenu("Kill")]
    public void DamageKill()        // Da dano maximo no jogador (Acessivel pelo editor)
    {
        Damage(MaxHealth);
    }
    
}
