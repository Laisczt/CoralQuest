using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;

[Serializable]
public class BasicEnemy : MonoBehaviour
{
    /*
        Funcionalidades de inimigos comuns, como dano corporal, vida e knockback
        Mecanicas do inimigo devem ser especificadas em outro script que implementa a interface IEnemy
        Atente-se a variável LockMovement ao codar o movimento dos inimigos
    */
    public int Health = 100;    // Vida do inimigo
    [HideInInspector] public int maxHealth; // Vida maxima 
    [HideInInspector] public bool Alive = true; // Se esta vivo
    
    public float KnockbackMultiplier = 1f;  // Quao sucetivel a knockback o inimigo eh
    public int BodyDamage = 1;      // Dano ao tocar no corpo do inimigo
    [SerializeField] GameObject HealthDrop; // Drop de vida
    public int HealthDropAdditionalRange = 1;   // Quantidade adicional de vida que o inimigo pode dropar (0 dropa até 1)(-1 impede que drope vida)
    [SerializeField] PlayerControl target;       // Player
    [SerializeField] Rigidbody2D m_RigidBody;
    IEnemy enemy;   // Interface atrelada ao script especifico de  cada inimigo

    public bool LockMovement {get ; private set;}   // Trava o movimento do inimigo no script especifico

    // Start is called before the first frame update

    private void OnValidate()
    {
        enemy = GetComponent<IEnemy>(); // pega o componente que define a IA do inimigo (patrol, stalk, turret)
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

    public void Kill()
    {
        Damage(Health);
    }
    public void Damage(int value, bool knockback = false)   // Danifica o inimigo
    {
        if (!Alive) return;

        enemy.Damage(Mathf.Min(Health, value));

        Health -= value;

        if (Health <= 0)
        {
            Alive = false;
            enemy.Kill();

            if(Random.Range(0f, 1f) > 0.4f && HealthDropAdditionalRange >= 0)   // Drop de vida
            {
                for(int i = 0; i < Random.Range(1, HealthDropAdditionalRange + 2); i++)
                {
                    Instantiate(HealthDrop, transform.position + new Vector3(0, 0.2f), Quaternion.identity);
                }
            }
        }

        // KNOCKBACK
        if (knockback && KnockbackMultiplier != 0)
        {
            enemy.Knockback();
            var direction = (target.transform.position.x - transform.position.x > 0) ? Vector2.right : Vector2.left;    // Direcao do knockback (afasta-se do jogador)

            m_RigidBody.velocity = -1 * direction * value * KnockbackMultiplier;

            StartCoroutine(temp_lockMovement());
        }
        


    }

    IEnumerator temp_lockMovement() // Trava o movimento do inimigo por um curto periodo
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

    public void KillPermanently()
    {
        if(GameControl.Instance.currentLevel != "Shallows") return;
        if(!GameControl.Instance.KilledTentaclesNames.Contains(this.name)) GameControl.Instance.KilledTentaclesNames.Add(this.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Alive && collision.gameObject == target.gameObject) // Danifica o jogador se ele tocar no corpo do inimigo
        {
            if (target.Damage(BodyDamage))
            {
                int knockbackDir = ((target.transform.position - transform.position).x > 0) ? 1 : -1;
                target.Knockback(20, knockbackDir);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.CompareTag("Solid")) // Destrava o movimento do inimigo ao bater numa parede
        {
            StopCoroutine(temp_lockMovement());
            LockMovement = false;
        }        
    }


    public void DeathStall (int stalltime)  // Chamada pelo script de IA
    {
        StartCoroutine(cDeathStall(stalltime));
    }

    IEnumerator cDeathStall(int stalltime)  // destroi o inimigo depois de um delay para rodar animacoes
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
