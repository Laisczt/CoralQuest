using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int lifespan = 20;
    public int Damage = 5;
    List<GameObject> enemiesHit = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (--lifespan == 0) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemiesHit.Contains(collision.gameObject))
        {
            enemiesHit.Add(collision.gameObject);
            collision.GetComponent<BasicEnemy>().Damage(Damage, true);
        }
    }
}
