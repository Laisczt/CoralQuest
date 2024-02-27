using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] BasicEnemy boss;
    [SerializeField] Transform bossSpawn;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(boss, bossSpawn.position, Quaternion.identity);
            gameObject.SetActive(false);
        }   
    }
}
