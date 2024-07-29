using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerPetrification))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerControl : MonoBehaviour, IDataSaver
{
    /*
        Controlador do jogador
        interacoes com o jogador a partir de outros scripts sempre passam por aqui
    */
    [SerializeField, HideInInspector] Rigidbody2D m_RigidBody;        // Corpo Rigido para fisica
    [SerializeField, HideInInspector] Animator m_Animator;            // Animador
    [SerializeField, HideInInspector] PlayerMovement m_PlayerMovement;// Script de movimento do jogador
    [SerializeField, HideInInspector] PlayerPetrification m_Petrify;  // Script de petrificação
    [SerializeField, HideInInspector] PlayerAttack m_PlayerAttack;    // Script de ataque
    [SerializeField, HideInInspector] PlayerHealth m_PlayerHealth;    // Script de vida

    [HideInInspector] public bool Petrified;                  // Player está petrificado
    [HideInInspector] public bool Alive = true;               // Player vivo
    [HideInInspector] public int SpriteOrientation = 1;       // -1 Quando o Sprite estiver espelhado

    [HideInInspector] public float DefaultGravityScale;       // Gravidade padrao

    public static PlayerControl Instance { get; private set; }  // Singleton instance
    public PlayerHealth PlayerHealth {  // Acesso ao PlayerHealth script
        get{
            return m_PlayerHealth;
        }  
    }

    public bool LockMovement {  // Trava de movimento
        get{
            return m_PlayerMovement.LockMovement;
        }
        set{
            m_PlayerMovement.LockMovement = value;
        }
    }

    public float JumpPower {    // Forca do pulo
        get{
            return m_PlayerMovement.JumpPower;
        }
    }

    public bool UsingMobileControls {   // Verdadeiro quando os controles de UI para celular estao ativos
        get {
            return UsingMobileControls;
        }
        set {
            m_PlayerMovement.UsingMobileControls = value;
            m_PlayerAttack.UsingMobileControls = value;
            UsingMobileControls = value;
        }
    }

    void OnValidate()
    {
        m_Petrify = GetComponent<PlayerPetrification>();
        m_Petrify.enabled = false;
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_PlayerAttack = GetComponent<PlayerAttack>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_Animator = GetComponent<Animator>();
    }
    
    // Awake is called when an enabled script instance is being loaded.
    void Awake()
    {
	    Instance = this;
    }

    void Start()
    {
        DefaultGravityScale = m_RigidBody.gravityScale;
    }

    [ContextMenu("Petrify")]
    
    public void Petrify()   // Petrifica o jogador
    {
        if(m_Petrify == null) Debug.LogWarning("Player petrification script not found");
        if(Petrified) return;
        Petrified = true;
        m_RigidBody.velocity = Vector2.zero;
        m_Animator.SetTrigger("Petrify");
        m_Petrify.enabled = true;
    }
    public void Depetrify(bool idle = true)   // Despetrifica
    {
        Petrified = false;
        m_Petrify.enabled = false;

        if(idle) m_Animator.SetTrigger("Depetrify");
    }
    
    public void SetMobileControls(GameObject[] controls)    // Ativa os controles mobile
    {
        foreach (var element in controls)
        {
            switch (element.name)
            {
                case "Joystick":
                    m_PlayerMovement.Joystick = element.transform.GetChild(0).GetComponent<Joystick>();
                    break;
                case "Jump":
                    m_PlayerMovement.JumpButton = element.transform.GetChild(0).GetComponent<UIControlButton>();
                    break;
                case "Attack":
                    m_PlayerAttack.AttackButton = element.transform.GetChild(0).GetComponent<UIControlButton>();
                    break;
                default:
                    Debug.LogError("Mobile UI element not recognized, did you rename something?");
                    break;
            }

            m_PlayerMovement.UsingMobileControls = true;
            m_PlayerAttack.UsingMobileControls = true;
        }
    }

    public bool Damage(int value)   // Danifica o jogador
    {
        return m_PlayerHealth.Damage(value);
    }


    [ContextMenu("Damage1")]
    public void Damage1()       // Da 1 dano ao jogador (acessivel pelo editor)
    {
        Damage(1);
    }

    public void Heal(int value)     // Cura o jogador
    {
        m_PlayerHealth.Heal(value);
    }

    [ContextMenu("Heal1")]
    public void Heal1()     // Cura 1 vida ao jogador (acessivel pelo editor)
    {
        Heal(1);
    }

    public void Kill(){     // Mata o jogador
        Alive = false;
        m_Petrify.enabled = false;
        m_PlayerAttack.enabled = false;
        m_PlayerHealth.enabled = false;
        m_PlayerMovement.enabled = false;
        m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);

        m_Animator.SetTrigger("Death");

        StartCoroutine(gameOver());
    }


    IEnumerator gameOver()  // Ativa a tela de gameover apos um delay
    {
        var i = 100;

        Time.timeScale = 0.5f; // Drama

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

    public void Knockback(float force, float xDirStrenth)   // Aplica knockback ao jogador
    {
        m_PlayerMovement.Knockback(force, xDirStrenth);
    }

    /*public void loadData(gameData data){
        this.health = data.hp;
        this.transform.position = data.position;

        HealthBar.Instance.UpdateHB();
    }

    public void saveData(ref gameData data){
        Debug.Log(health);
        data.hp = this.health;
        data.position = this.transform.position;
    }*/

}
