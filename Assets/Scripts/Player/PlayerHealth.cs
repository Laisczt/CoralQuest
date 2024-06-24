using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerControl))]
public class PlayerHealth : MonoBehaviour
{

    [SerializeField, HideInInspector] PlayerControl m_PlayerControl;  // Controlador
    [SerializeField, HideInInspector] Animator m_Animator;            // Animador
    [SerializeField, HideInInspector] PlayerMovement m_PlayerMovement;// Movimento do jogador
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
    
    public bool Damage(int value)                   // Dano, retorna false se o player não pôde levar dano (ainda funciona quando invencível)
    {
        if (!m_PlayerControl.Alive || value <= 0) return false;
        if (rDamageCooldown > 0) return false;

        if (m_PlayerControl.Petrified) m_PlayerControl.Depetrify();

        rDamageCooldown = DamageCooldown;

        if (!DEBUG_INVINCIBLE) Health -= value;
        if (Health <= 0)
        {
            Health = 0;
            Kill();
        }
        else
        {
            m_Animator.SetTrigger("Damage");
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

    [ContextMenu("Recover")]
    public void Recover()                           // Revive o jogador sem reiniciar o nível (PARA DEBUG)
    {
        m_PlayerControl.Alive = true;
        Heal(MaxHealth);
        m_Animator.SetTrigger("DEBUG REVIVE");
        m_PlayerMovement.LockMovement = false;
        m_PlayerMovement.PreventMovement = false;
        m_PlayerControl.Petrified = false;
    }

    [ContextMenu("Kill")]
    public void DamageKill()
    {
        Damage(MaxHealth);
    }
    public void Kill()                              // Mata o jogador instantaneamente
    {
        m_PlayerControl.Alive = false;

        m_Animator.SetTrigger("Death");
        m_Animator.SetBool("Running", false);

        m_PlayerMovement.LockMovement = true;

        StartCoroutine(gameOver());
    }
    IEnumerator gameOver()
    {
        var i = 100;

        Time.timeScale = 0.75f; // Drama

        while(i > 70){
            i--;
            yield return new WaitForFixedUpdate();
        }

        Time.timeScale = 1f;

        while(i > 0){
            i--;
            yield return new WaitForFixedUpdate();
        }
        GameControl.Instance.GameOver();
    }
}
