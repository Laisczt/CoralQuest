using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        SpawnOnce();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnOnce()
    {
        Instantiate(monster, transform.position, Quaternion.identity);
    }
}
