using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private PlayerControl target;
    private Rigidbody2D m_RigidBody;
    IEnemy enemy;

    public bool LockMovement {get ; private set;}

    // Start is called before the first frame update

    private void OnValidate()
    {
        enemy = GetComponent<IEnemy>();
    }
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        target = PlayerControl.Instance;
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
        LockMovement = true;
        var i = 5;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        LockMovement = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Alive && collision.gameObject == target.gameObject)
        {
            if (target.Damage(BodyDamage))
            {
                int knockbackDir = ((target.transform.position - transform.position).x > 0) ? 1 : -1;
                target.Knockback(20, knockbackDir);
            }
        }
    }


    public void DeathStall (int stalltime)
    {
        StartCoroutine(cDeathStall(stalltime));
    }

    IEnumerator cDeathStall(int stalltime)
    {
        var i = stalltime;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

}
