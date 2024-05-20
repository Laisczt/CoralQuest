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
    [SerializeField] GameObject HealthDrop;
    public int HealthDropAdditionalRange = 1;
    [SerializeField, HideInInspector] Transform target;
    [HideInInspector] public Rigidbody2D m_RigidBody;
    [HideInInspector] public Animator m_Animator;
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
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        enemy = GetComponent<IEnemy>();
    }
    void Start()
    {
        if(enemy == null)
        {
            enemy = GetComponent<IEnemy>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int value, bool knockback)
    {
        if (!Alive) return;

        enemy.Damage(Mathf.Min(Health, value));

        Health -= value;

        if (Health <= 0)
        {
            Alive = false;
            enemy.Kill();

            if(Random.Range(0f, 1f) > 0.4f && HealthDropAdditionalRange >= 0)
            {
                for(int i = 0; i < Random.Range(1, HealthDropAdditionalRange + 2); i++)
                {
                    Instantiate(HealthDrop, transform.position + new Vector3(0, 0.2f), Quaternion.identity);
                }
            }
            

            return;
        }

        // KNOCKBACK
        if (knockback && KnockbackMultiplier != 0)
        {
            enemy.Knockback();
            var direction = (target.transform.position.x - transform.position.x > 0) ? Vector2.right : Vector2.left;

            m_RigidBody.velocity = -1 * direction * value * KnockbackMultiplier; // Knockback

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
        if (Alive && collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerControl>();
            if (player.Damage(BodyDamage))
            {
                int knockbackDir = ((player.transform.position - transform.position).x > 0) ? 1 : -1;
                player.Knockback(20, knockbackDir);
            }
        }
    }

}
