using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(BasicEnemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class Stalk : MonoBehaviour, IEnemy
{
    /*
        IA de inimigo que segue o jogador diretamente
        Esse inimigo possui uma posicao inicial, a qual retorna caso o jogador se afaste, um raio de deteccao do jogador,
        e um territorio, alem do qual nao persegue o jogador, mesmo que esteja dentro do raio de deteccao
    */
    private Vector2 homePos;    // Posicao inicial
    public float TerritoryRadius;   // Raio do territorio
    public float TopSpeed = 4.5f;   // Velocidade maxima
    private float speed = 0;        // Velocidade atual
    private float baseSpeed = 1f;   // Velocidade base
    [HideInInspector] public BasicEnemy basicEnemy;
    private PlayerControl target;
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody;

    public AudioSource damageSound;    // Som de dano
    public AudioSource deathSound;     // Som de morte
    public AudioSource chaseSound;    // Som que toca enquanto esta perseguindo o jogador
    private bool chasing;   // Se esta perseguindo o jogador
    private bool moving;    // Se esta se movendo



    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    LayerMask playerMask;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerControl.Instance;
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        playerMask = LayerMask.GetMask("Player", "Solid");
        homePos = transform.position;
    }


    private void FixedUpdate()
    {
        if (basicEnemy.LockMovement) return;
        if (!basicEnemy.Alive)  // Ao morrer, para de se mover rapidamente
        {
            m_RigidBody.velocity /= 1.3f;
            return;
        }

        Vector2 travelDirection = Vector2.zero;
        if (Vector3.Distance(homePos, target.transform.position) <= TerritoryRadius && transform.Sees(target.transform, 8, playerMask)) // Persegue o jogador
        {
            travelDirection = target.transform.position - transform.position;
            speed += 0.1f;

            moving = true;
        }
        else if (Vector2.Distance(transform.position, homePos) > 0.3f)  // Retorna à posicao inicial
        {
            travelDirection = homePos - (Vector2)transform.position;
            speed -= 0.0125f;
            moving = true;
        }
        else    // Parado
        {
            speed = baseSpeed;
            moving = false;
        }

        travelDirection.Normalize();

        if (speed > TopSpeed) speed = TopSpeed;
        if (speed < baseSpeed) speed = baseSpeed;

        m_RigidBody.velocity = travelDirection * speed;

        // Audio
        if (moving == true && chasing == false)
        {
            chaseSound.Play();
            chasing = true;
        }
        if (moving == false && chasing == true)
        {
            chaseSound.Stop();
            chasing = false;
        }

        // Sprite
        m_Animator.SetBool("Moving", moving);
    }
    public void Knockback()
    {
        if(speed > baseSpeed + 1f) speed = baseSpeed + 1f; // Limita a velocidade ao receber knockback
    }

    public void Damage(int _value)
    {
        m_Animator.SetTrigger("Damage");
        damageSound.PlayOneShot(damageSound.clip);
    }

    public void Kill()
    {
        chaseSound.Stop();
        deathSound.PlayDetached();
        m_Animator.SetTrigger("Death");
        basicEnemy.DeathStall(55);
    }

    
}
