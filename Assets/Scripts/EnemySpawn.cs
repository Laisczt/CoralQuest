using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] BasicEnemy monster;
    Transform player;
    bool hasPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnOnce()
    {
        var spawnedMonster = Instantiate(monster, transform.position, Quaternion.identity);
        spawnedMonster.Target = player;
    }

    public void Initialize(Transform player)
    {
        this.player = player;
        hasPlayer = true;
        SpawnOnce();
    }
}
