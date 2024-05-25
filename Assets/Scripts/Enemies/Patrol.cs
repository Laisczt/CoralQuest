using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Patrol : MonoBehaviour, IEnemy
{
    public float topSpeed = 3f;

    private float speed;

    private short direction = 1;

    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private Rigidbody2D m_RigidBody;
    private Animator m_Animator;


    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        speed = topSpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!basicEnemy.LockMovement || !basicEnemy.Alive)
        {
            if (speed < topSpeed) speed += 0.25f;
            else if (speed > topSpeed) speed = topSpeed;

            m_RigidBody.velocity = new Vector2(speed * direction, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Solid"))
        {
            direction *= -1;
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
