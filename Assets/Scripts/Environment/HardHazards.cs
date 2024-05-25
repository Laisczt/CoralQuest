using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardHazards : MonoBehaviour
{
    public int Damage = 1;
    public float KnockbackForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<PlayerControl>();
            player.Damage(Damage);

            var direction = -collision.GetContact(0).normal.x;
            player.Knockback(KnockbackForce, direction);
        }
    }
}
