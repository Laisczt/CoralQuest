using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class BossHealthTentacle : MonoBehaviour, IEnemy
{
    public Boss parentBoss;
    [HideInInspector] public float BubbleOriginY;
    private BasicEnemy basicEnemy;

    public int Lifespan = 360;

    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());

        if(basicEnemy == null)
        {
            basicEnemy = GetComponent<BasicEnemy>();
        }

        var hit = Physics2D.Raycast(transform.position + Vector3.up * 5, Vector2.down, 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Lifespan--;
        if (Lifespan == 0) StartCoroutine(Despawn());
    }

    [SerializeField] List<GameObject> Bubbles = new List<GameObject>();
    private IEnumerator Spawn()
    {
        var i = 100;
        while(i > 0){
            i--;
            if(i % 20 == 0)
            {
                GameObject newbub = Instantiate(Bubbles[Random.Range(0, Bubbles.Count)], new Vector3 (transform.position.x, BubbleOriginY, transform.position.z - 0.25f), Quaternion.identity);
                if(Random.Range(0f,1f) <= 0.5f)
                {
                    newbub.GetComponent<SpriteRenderer>().flipX = true;
                }
                StartCoroutine(bubbleDeathStall(newbub));
            }
            yield return new WaitForFixedUpdate();
        }

        i = 90;
        while(i > 0)
        {
            i--;
            transform.localPosition += new Vector3(0, 0.02f, 0);
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator Despawn()
    {
        var i = 90;
        while (i > 0)
        {
            i--;
            transform.localPosition -= new Vector3(0, 0.02f, 0);
            yield return new WaitForFixedUpdate();
        }
        Kill();
    }

    private IEnumerator bubbleDeathStall(GameObject bub)
    {
        var i = 74;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Destroy(bub);
    }

    public void Damage(int amount)
    {
        parentBoss.Damage(amount);
        basicEnemy.m_Animator.SetTrigger("Damage");
    }
    public void Kill()
    {
        basicEnemy.m_Animator.SetTrigger("Death");
        StartCoroutine(deathStall());
        basicEnemy.Alive = false;

        StopCoroutine(Spawn());
        StopCoroutine(Despawn());
    }
    public void Knockback()
    {

    }

    private IEnumerator deathStall()
    {
        var i = 30;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        foreach(var j in GameObject.FindGameObjectsWithTag("Boss Bubble"))
        {
            Destroy(j);
        }

        Destroy(gameObject);
    }
}
