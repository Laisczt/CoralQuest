using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenZone : MonoBehaviour
{
    /*
        Telas que limitam o movimento da camera do jogador
    */
    public float cameraSize = 6.5f; // Tamanho da camera para a tela
    
    public static ScreenZone currentArea;   // Variavel estatica que guarda a tela atual da camera

    Rigidbody2D rb;
    Collider2D m_col;
    void Start()
    {
        rb = PlayerControl.Instance.GetComponent<Rigidbody2D>();
        m_col = GetComponent<Collider2D>();
    }


    Vector3 headOffset = new Vector3(0, 0.375f);    // Offset da altura da cabeca da player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(m_col.OverlapPoint(collision.transform.position + headOffset)) // Verifica se a cabeça do jogador está dentro da área
            {
                if(currentArea == null) // essa area vira a area atual se nao ha uma
                {
                    currentArea = this;
                    StopAllCoroutines();
                    StartCoroutine(ChangeArea());
                }
            }
            else    // Se o jogador (cabeça) estiver fora da area e a area atual for essa, limpa a area atual
            {
                if(currentArea == this) currentArea = null;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))  // Se o jogador sair completamente da area
        {
            if(currentArea == this) currentArea = null; // limpa a area
            StartCoroutine(freeCamCheck()); // Verifica se deve ser usada camera livre
        }
    }

    IEnumerator freeCamCheck()  // Se o jogador ficar fora de qualquer area por 10 frames, libera o movimento da camera
    {
        var i = 10;
        while( i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        if(currentArea == null)
        {
            MainCamera.Instance.UseFreeCam();
        }
    }

    
    IEnumerator ChangeArea() // muda a area da camera
    {
        rb.gravityScale = PlayerControl.Instance.DefaultGravityScale;
        
        if(rb.velocity.y > 7f) // se a transicao entre telas for para cima, damos um empulso ao jogador
        {
            rb.velocity = new Vector2(0, PlayerControl.Instance.JumpPower * 1f);
            rb.gravityScale = 0f;
        }

        MainCamera.Instance.ChangeArea(this, cameraSize);   // aplica a mudanca aa camera

        var i = 6;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }      

        rb.gravityScale = PlayerControl.Instance.DefaultGravityScale;
        
          
    }
}
