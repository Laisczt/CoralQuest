using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Patrol : MonoBehaviour, IEnemy
{
    /*
        IA de inimigo que bate e volta nas paredes
    */
    public float topSpeed;  // Velocidade maxima
    private float speed;    // Velocidade atual

    private int direction = 1;  // Direcao atual

    [SerializeField] AudioSource bonkSound; // Audio que toca ao bater na parede

    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private Rigidbody2D m_RigidBody;
    private Animator m_Animator;


    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    LayerMask solidMask;
    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        speed = topSpeed;
        solidMask = LayerMask.GetMask("Solid");
    }
    private void FixedUpdate()
    {
        // Movimento do inimigo
        if (!basicEnemy.LockMovement || !basicEnemy.Alive)
        {
            if (speed < topSpeed) speed += 0.25f;
            else if (speed > topSpeed) speed = topSpeed;

            m_RigidBody.velocity = new Vector2(speed * direction, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) // Voyages só colidem com players e objetos solidos (parede, tentáculos), se o objeto n for o player, deve ser sólido 
        {
            // Raycasts para a esquerda e para a direita, usados para definir para que direcao o inimigo deve ir
            // A troca de direcao eh feita dessa forma para evitar que o inimigo fique preso dentro das paredes
            
            var wallL = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity, solidMask); 
            var wallR = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, solidMask);
            
            float distL;
            float distR;

            if(wallL.transform != null) distL = wallL.distance;
            else distL = Mathf.Infinity;

            if(wallR.transform != null) distR = wallR.distance;
            else distR = Mathf.Infinity;


            if(distL < distR)
            {
                if(direction == -1) bonkSound.PlayOneShot(bonkSound.clip);
                direction = 1;
            }
            else 
            {
                if (direction == 1) bonkSound.PlayOneShot(bonkSound.clip);
                direction = -1;
            }
            speed = 0f;
            m_Animator.SetTrigger("Wall Hit");
            m_Animator.SetBool("Mirror", direction < 0);
        }
    }

    public void Damage(int _value)
    {
        m_Animator.SetTrigger("Damage");
    }
    public void Knockback()
    {
        speed = 0f;
    }

    public void Kill()
    {
        basicEnemy.DeathStall(32);
        m_Animator.SetTrigger("Death");
    }
}
