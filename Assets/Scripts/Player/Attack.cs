using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Script para ataque do jogador
    public int lifespan = 20; // Duração do ataque antes que se dissipe
    public int Damage = 5;  // Dano do ataque
    List<GameObject> enemiesHit = new List<GameObject>(); // Lista de inimigos danificados (para não poder bater em um inimigo mais de uma vez por ataque)

    void FixedUpdate()
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
