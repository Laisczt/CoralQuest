using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class BossHealthTentacle : MonoBehaviour, IEnemy
{
    /*
        Tentaculo de vida da boss
        dano causado aa esses tentaculos eh transferido para a boss
    */
    public Boss parentBoss;     // Boss
    [HideInInspector] public float BubbleOriginY;   // Altura na qual sao criadas bolhas (que indicam onde vai surgir um tentaculo)
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private Animator m_Animator;

    public int Lifespan = 360;  // Quantos frames o tentaculo dura antes de desaparecer

    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(basicEnemy == null)  // Inimigos spawnados durante o percorrer do jogo precisam pegar o basic enemy aqui
        {
            basicEnemy = GetComponent<BasicEnemy>();
        }
        m_Animator = GetComponent<Animator>();

         StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Lifespan--;
        if (Lifespan == 0) StartCoroutine(Despawn());
    }
    private IEnumerator Spawn() // Posiciona o tentaculo ao spawnar
    {
        var i = 100;
        while(i > 0){
            i--;
            if(i % 20 == 0) // Spawna bolhas vermelhas no local onde o tentáculo irá surgir
            {
               BubbleManager.Instance.SpawnBubble(new Vector3 (transform.position.x, BubbleOriginY, transform.position.z - 0.25f), 'R');
            }
            yield return new WaitForFixedUpdate();
        }

        i = 90;
        while(i > 0)    // Sobe o tentáculo lentamente
        {
            i--;
            transform.localPosition += new Vector3(0, 0.02f, 0);
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator Despawn()   // Desce lentamente o tentaculo antes de ser destruido
    {
        var i = 90;
        while (i > 0)
        {
            i--;
            transform.localPosition -= new Vector3(0, 0.02f, 0);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    public void Damage(int amount)  // Danifica o tentaculo e a boss
    {
        parentBoss.Damage(amount);
        m_Animator.SetTrigger("Damage");
    }
    public void Kill()
    {
        m_Animator.SetTrigger("Death");
        basicEnemy.Alive = false;

        Destroy(gameObject);
    }
    public void Knockback()
    {

    }
}
