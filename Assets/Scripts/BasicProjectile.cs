using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float speed = 1.0f;
    public int value = 1;
    public int lifespan = 300;
    public bool breaksOnSolidHit;
    public bool breaksOnPlayerHit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        lifespan--;
        if (lifespan <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((breaksOnSolidHit && collision.gameObject.CompareTag("Solid")) || 
            (breaksOnPlayerHit && collision.gameObject.name == "Player"))
        {
            Destroy(gameObject);
        }
    }
}
