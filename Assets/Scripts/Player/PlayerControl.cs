using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField, HideInInspector] Rigidbody2D m_RigidBody;        // Corpo Rigido para fisica
    [SerializeField, HideInInspector] Animator m_Animator;            // Animador
    [SerializeField, HideInInspector] PlayerMovement m_PlayerMovement;// Script de movimento do jogador
    [SerializeField, HideInInspector] PlayerPetrification m_Petrify;  // Script de petrificação
    [SerializeField, HideInInspector] PlayerAttack m_PlayerAttack;    // Script de ataque
    [SerializeField, HideInInspector] PlayerHealth m_PlayerHealth;    // Script de vida

    [HideInInspector] public bool Petrified;                  // Player está petrificado
    [HideInInspector] public bool Alive = true;               // Player vivo
    [HideInInspector] public int SpriteOrientation = 1;       // -1 Quando o Sprite estiver espelhado

    private bool startingAreaSet = false;   // Define a área inicial da câmera

    public static PlayerControl Instance { get; private set; }  // Singleton instance
    public PlayerHealth PlayerHealth { 
        get{
            return m_PlayerHealth;
        }  
    }

    public bool LockMovement {
        get{
            return m_PlayerMovement.LockMovement;
        }
        set{
            m_PlayerMovement.LockMovement = value;
        }
    }

    public float JumpPower {
        get{
            return m_PlayerMovement.JumpPower;
        }
    }

    public bool UsingMobileControls {
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

    [ContextMenu("Petrify")]
    
    public void Petrify()
    // Petrifica o jogador
    {
        if(m_Petrify == null) Debug.LogWarning("Player petrification script not found");
        if(Petrified) return;
        Petrified = true;
        m_RigidBody.velocity = Vector2.zero;
        m_Animator.SetTrigger("Petrify");
        m_Petrify.enabled = true;
    }
    public void Depetrify()
    // Despetrifica
    {
        Petrified = false;
        m_Animator.SetTrigger("Depetrify");
        m_Petrify.enabled = false;
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    public bool Damage(int value)
    {
        return m_PlayerHealth.Damage(value);
    }
    [ContextMenu("Damage1")]
    public void Damage1()
    {
        Damage(1);
    }

    public void Heal(int value)
    {
        m_PlayerHealth.Heal(value);
    }

    [ContextMenu("Heal1")]
    public void Heal1()
    {
        Heal(1);
    }

    public void Knockback(float force, float xDirStrenth)
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
