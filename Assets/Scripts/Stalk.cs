using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Stalk : MonoBehaviour
{
    private Vector2 homePos;
    public float radius;
    public float speed;
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;

    private Rigidbody2D m_Rigidbody2D;

    private void OnValidate()
    { 
        basicEnemy = GetComponent<BasicEnemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        homePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (basicEnemy.Revived) basicEnemy.Revived = false;

        Vector2 travelDirection = Vector2.zero;
        if (this.transform.Sees(basicEnemy.Target, radius))
        {
            travelDirection = basicEnemy.Target.transform.position - transform.position;
        }
        else if(Vector2.Distance(transform.position, homePos) > 0.3f)
        {
            travelDirection = homePos - (Vector2) transform.position;
        }
        travelDirection.Normalize();

        m_Rigidbody2D.velocity = travelDirection * speed;

        
    }
}
