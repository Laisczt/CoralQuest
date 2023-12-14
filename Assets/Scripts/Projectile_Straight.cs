using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Straight : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;
    [HideInInspector]
    public Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_RigidBody.gravityScale = 0;

        m_RigidBody.velocity = GetComponent<BasicProjectile>().speed * direction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
