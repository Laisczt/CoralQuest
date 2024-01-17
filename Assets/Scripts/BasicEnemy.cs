using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public int enemyHealth = 100;
    public int damage = 1;
    [SerializeField] Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player Attack"))
        {
            enemyHealth -= collision.GetComponent<Attack>().damage;
            Debug.Log("Ouchie - " + enemyHealth);
            if(enemyHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().Damage(damage);
        }
    }

}
