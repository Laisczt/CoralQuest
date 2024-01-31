using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Stalk : MonoBehaviour
{
    private Vector2 homePos;
    public float TerritoryRadius;
    public float speed;
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;

    private Rigidbody2D m_Rigidbody2D;

    private void OnValidate()
    { 
        basicEnemy = GetComponent<BasicEnemy>();
    }

    LayerMask playerMask;
    // Start is called before the first frame update
    void Start()
    {
        playerMask = LayerMask.GetMask("Player", "Solid");
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        homePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 travelDirection = Vector2.zero;
        if (Vector3.Distance(homePos, basicEnemy.Target.position) <= TerritoryRadius && this.transform.Sees(basicEnemy.Target, 8, playerMask))
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
