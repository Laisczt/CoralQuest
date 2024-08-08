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
    Rigidbody2D m_RigidBody;        // Corpo Rigido para fisica
    Animator m_Animator;            // Animador
    PlayerMovement m_PlayerMovement;// Script de movimento do jogador
    PlayerPetrification m_Petrify;  // Script de petrificação
    PlayerAttack m_PlayerAttack;    // Script de ataque
    PlayerHealth m_PlayerHealth;    // Script de vida

    public Vector3 LastSavePos;

    [HideInInspector] public bool Petrified;                  // Player está petrificado
    [HideInInspector] public bool Alive = true;               // Player vivo
    [HideInInspector] public int SpriteOrientation = 1;       // -1 Quando o Sprite estiver espelhado

    [HideInInspector] public float DefaultGravityScale;       // Gravidade padrao

    public AudioSource deathSound;

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
        deathSound.Play();

        StartCoroutine(gameOver());
    }


    IEnumerator gameOver()  // Ativa a tela de gameover apos um delay
    {
        var i = 100;

        Time.timeScale = 0.5f; // Drama

        while(i > 75){
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

    public void loadData(gameData data){
        if(GameControl.Instance.currentLevel == "Shallows")
        {
            LastSavePos = data.Level1SavePos;
        }
        else if(GameControl.Instance.currentLevel == "Depths")
        {
            LastSavePos = data.Level2SavePos;
        }

        if(LastSavePos != new Vector3(0,0,-100)) transform.position =  new Vector3(LastSavePos.x, LastSavePos.y, transform.position.z);
    }

    public void saveData(ref gameData data){
        if(GameControl.Instance.currentLevel == "Shallows")
        {
            data.Level1SavePos = LastSavePos;
            if(LastSavePos == new Vector3(0,0,-100))
                data.Level2SavePos = LastSavePos;
        }
        else if (GameControl.Instance.currentLevel == "Depths")
        {
            data.Level2SavePos = LastSavePos;
        }
    }

}
