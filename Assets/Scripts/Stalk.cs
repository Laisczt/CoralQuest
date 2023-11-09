using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalk : MonoBehaviour
{
    private Vector3 homePos;
    public GameObject target;
    public float radius;
    public float speed;

    private Rigidbody2D m_Rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player");
        homePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 travelDirection;
        if (Vector3.Distance(target.transform.position, transform.position) < radius)
        {
            travelDirection = target.transform.position - transform.position;
        }
        else
        {
            if (Vector3.Distance(transform.position, homePos) > 0.3f) travelDirection = homePos - transform.position;
            else travelDirection = Vector2.zero;
        }
        travelDirection.Normalize();

        m_Rigidbody2D.velocity = travelDirection * speed;
    }
}
