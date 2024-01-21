using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Patrol : MonoBehaviour
{
    public float topSpeed = 3f;

    private float speed;

    private short direction = 1;
    private Rigidbody2D m_RigidBody;

    [SerializeField] BasicEnemy basicEnemy;


    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        speed = topSpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (speed < topSpeed) speed += 0.25f;
        else if (speed > topSpeed) speed = topSpeed;

        if (basicEnemy.Revived)
        {
            speed = 0;
            basicEnemy.Revived = false;
        }

        m_RigidBody.velocity = new Vector2(speed * direction, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Solid"))
        {
            direction *= -1;
            speed = 0f;
        }
    }
}
