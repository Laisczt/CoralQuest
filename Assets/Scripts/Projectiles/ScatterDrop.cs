using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicProjectile))]
public class ScatterDrop : MonoBehaviour
{
    /*
        Drops com rotacao e direcao aleatoria
    */
    Rigidbody2D m_RigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();

        m_RigidBody.gravityScale = 0.7f;
        var angle = Random.Range(-45, 45);
        m_RigidBody.velocity = Vector2.up.Rotated(Mathf.Deg2Rad * angle) * GetComponent<BasicProjectile>().speed;
        m_RigidBody.angularVelocity = angle * 10f;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.gameObject.CompareTag("Solid"))
        {
            m_RigidBody.gravityScale = 0;
            m_RigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        
    }
}
