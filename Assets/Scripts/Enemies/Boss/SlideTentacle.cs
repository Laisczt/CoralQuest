using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTentacle : MonoBehaviour
{
    public Boss ParentBoss;
    public int direction = 1;
    public int Damage = 1;
    public float speed = 12;
    public int duration = 80;
    private int _direction;
    private Vector3 homePos;
    private Rigidbody2D m_Rigidbody;
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        _direction = direction;
        homePos = transform.position;
    }

    

    public void Slide()
    {
        StartCoroutine(InnOut());
    }


    private float x;

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, homePos.y + Mathf.Sin(x) / 3, transform.position.z);
        x += Time.fixedDeltaTime;
    }
    public void OutAndDestroy()
    {
        StopAllCoroutines();
        m_Rigidbody.velocity = new Vector2(speed * 2 * -direction, m_Rigidbody.velocity.y);
        StartCoroutine(deathStall());
    }

    private IEnumerator deathStall()
    {
        var i = 30;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
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
        i = 72;
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
        ParentBoss.StopDigging();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerControl>();
            player.Damage(Damage);
            player.Knockback(30, direction * 3.5f);
        }
    }
}
