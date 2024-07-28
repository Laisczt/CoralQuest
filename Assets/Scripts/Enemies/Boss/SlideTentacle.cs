using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTentacle : MonoBehaviour
{
    /*
        Ataque 'dig' da boss
    */
    public Boss ParentBoss; // Boss
    public int direction = 1;   // Direcao que o tentaculo se move 
    public int Damage = 1;      // Dano
    public float speed = 12;    // Velocidade
    public int duration = 80;   // Duracao do ataque
    private int _direction;     // Direcao (interna)
    private Vector3 homePos;    // Posicao inicial
    private Rigidbody2D m_Rigidbody;
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        _direction = direction;
        homePos = transform.position;
    }

    public void Slide()     // Inicia o ataque
    {
        StartCoroutine(InnOut());
    }


    private float x;
    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, homePos.y + Mathf.Sin(x) / 3, transform.position.z); // Varia a posicao vertical do tentaculo
        x += Time.fixedDeltaTime * 3;
    }
    private IEnumerator InnOut()
    {
        // Mover para dentro da arena
        m_Rigidbody.velocity = new Vector2(speed * _direction, m_Rigidbody.velocity.y);
        var i = duration;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        // Esperar um pouco antes de voltar
        _direction = 0;
        m_Rigidbody.velocity = new Vector2(speed * _direction, m_Rigidbody.velocity.y);
        i = 30;
        while (i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        // Voltar para fora da arena
        _direction = -direction * 2;
        m_Rigidbody.velocity = new Vector2(speed * _direction, m_Rigidbody.velocity.y);
        i = duration/2 + 1;
        while (i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        _direction = direction;
        m_Rigidbody.velocity = Vector2.zero;
        transform.position = homePos;
        ParentBoss.StopDigging();   // Finaliza o ataque no boss

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))      // Danifica o jogador e aplica um knockback forte
        {
            var player = collision.GetComponent<PlayerControl>();
            player.Damage(Damage);
            player.Knockback(30, direction * 3.5f);
        }
    }

    public void OutAndDestroy() // Move o tentaculo rapidamente para longe da arena e chama deathstall
    {
        StopAllCoroutines();
        m_Rigidbody.velocity = new Vector2(speed * 2 * -direction, m_Rigidbody.velocity.y);
        StartCoroutine(deathStall());
    }

    private IEnumerator deathStall()    // Destroi o tentaculo apos um delay (para dar tempo de sair do fov do jogador)
    {
        var i = 30;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }
}
