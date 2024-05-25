using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Straight : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;
    [HideInInspector]

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetDirection(Vector2 direction){
        if(m_RigidBody == null){
            m_RigidBody = GetComponent<Rigidbody2D>();
        }
        m_RigidBody.velocity = GetComponent<BasicProjectile>().speed * direction;
    }

}
