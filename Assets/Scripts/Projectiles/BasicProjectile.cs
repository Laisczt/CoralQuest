using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    /*
        Script basico de projeteis que interagem com o player (excluindo o ataque principal)
        Contém funcionalidades de dano, cura, lifespan, se quebra ao bater em parede/player
    */
    public float speed = 1.0f;      // Velocidade do projetil
    public int value = 1;           // Quantidade de dano/cura
    public int lifespan = 300;      // Duracao ate ser destruido
    public bool breaksOnSolidHit;   // Se quebra quando bate numa parede
    public bool breaksOnPlayerHit;  // Se quebra quando atinge o jogador
    public bool IsHealingDrop;      // Falso se da dano, verdadeiro se cura
    public bool hasBreakSprite;     // Se possui animacao de quebrar
    public int breakDuration;      // Duracao da animacao de quebra (antes de destruir o gameobject)

    public AudioSource breakSound;

    private void FixedUpdate()
    {
        lifespan--;
        if (lifespan <= 0) BreakProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (breaksOnSolidHit && collision.gameObject.CompareTag("Solid"))   // Quebra ao bater na parede
        {
            BreakProjectile();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerControl>();
            if(IsHealingDrop)
            {
                player.Heal(value);     // Cura
            }
            else
            {
                if(player.Damage(value))    // Dano e knockback
                {
                    player.Knockback(20, ((player.transform.position - transform.position).x > 0)? 1 : -1);
                }
            }

            if (breaksOnPlayerHit) BreakProjectile();   // Quebra ao bater no jogador
        }
    }

    public void BreakProjectile(){  // Quebra do projetil
        if(breakSound) {
            breakSound.PlayDetached();
        }
        if(hasBreakSprite)
        {
            GetComponent<Animator>().SetTrigger("Break");
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            StartCoroutine(deathStall());
        }
        else Destroy(gameObject);

    }

    private IEnumerator deathStall(){
        var i = breakDuration;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
