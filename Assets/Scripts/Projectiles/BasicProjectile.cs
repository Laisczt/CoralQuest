using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float speed = 1.0f;
    public int value = 1;
    public int lifespan = 300;
    public bool breaksOnSolidHit;
    public bool breaksOnPlayerHit;
    public bool IsHealingDrop;
    public bool hasBreakSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        lifespan--;
        if (lifespan <= 0) BreakProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (breaksOnSolidHit && collision.gameObject.CompareTag("Solid"))
        {
            BreakProjectile();
        }
        if (breaksOnPlayerHit && collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerControl>();
            if(IsHealingDrop)
            {
                player.Heal(value);
            }
            else
            {
                if(player.Damage(value))
                {
                    player.Knockback(20, ((player.transform.position - transform.position).x > 0)? 1 : -1);
                }
            }

            BreakProjectile();
        }
    }

    public void BreakProjectile(){
        if(hasBreakSprite){
                GetComponent<Animator>().SetTrigger("Break");
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                StartCoroutine(deathStall());
            }
            else Destroy(gameObject);
    }

    private IEnumerator deathStall(){
        var i = 15;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
