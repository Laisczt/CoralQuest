using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public int Health = 100;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public bool Alive = true;


    public float KnockbackMultiplier = 1f;
    public int BodyDamage = 1;
    [SerializeField, HideInInspector] Transform target;
    [SerializeField] Rigidbody2D m_Rigidbody;
    public Animator m_Animator;
    IEnemy enemy;

    public bool lockMovement = false;
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    // Start is called before the first frame update

    private void OnValidate()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        enemy = GetComponent<IEnemy>();
    }
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int value, bool knockback)
    {
        enemy.Damage();

        Health -= value;

        if (Health <= 0)
        {
            enemy.Kill();
            return;
        }

        // KNOCKBACK
        if (knockback)
        {
            enemy.Knockback();
            var direction = (target.transform.position.x - transform.position.x > 0) ? Vector2.right : Vector2.left;

            m_Rigidbody.velocity = -1 * direction * value * KnockbackMultiplier; // Knockback

            StartCoroutine(temp_lockMovement());
        }
        


    }

    IEnumerator temp_lockMovement()
    {
        lockMovement = true;
        var i = 5;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        lockMovement = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().Damage(BodyDamage);
        }
    }

}
