using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public int Health = 100;
    [HideInInspector] public int maxHealth;
   
    public int Damage = 1;
    [SerializeField, HideInInspector] Transform target;

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
            Health -= collision.GetComponent<Attack>().damage;
            Debug.Log("Ouchie - " + Health);
            if(Health <= 0)
            {
                Kill();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControl>().Damage(Damage);
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

}
