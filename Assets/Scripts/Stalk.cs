using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalk : MonoBehaviour
{
    private Vector2 homePos;
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
        Vector2 travelDirection = Vector2.zero;
        if(this.gameObject.Sees(target, radius))
        {
            travelDirection = target.transform.position - transform.position;
        }
        else if(Vector2.Distance(transform.position, homePos) > 0.3f)
        {
            travelDirection = homePos - (Vector2) transform.position;
        }
        travelDirection.Normalize();

        m_Rigidbody2D.velocity = travelDirection * speed;
    }
}
