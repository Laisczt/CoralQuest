using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCamera : MonoBehaviour
{
    /*
        Script da camera principal
        essa camera segue o jogador, respeitando os limites da tela em que se encontra
    */
    private Transform target;      // O objeto que a camera segue (alvo)
    private Collider2D area;         // A regiao em que a camera esta
    public float SpeedFactor = 3f;  // A Velocidade de aproximacao da camera
    public bool smoothMovement;     // Se a camera deve se mover suavemente
    public bool FreeCam;            // Se a camera pode se mover livremente (não limitada a uma tela)
    public bool Freeze;             // Congela a camera

    private Camera m_Camera;        // Acesso ao script Camera
    private float aspectRatioOffsetX;// Compensacao ao tamanho horizontal da camera ao depender do formato da tela
    private float aspectRatioOffsetY;// Compensacao vertical 
    public float aspectRatio;       // Proporcoes da camera
    private float zPos ;            // A posicao padrao da camera no eixo Z

    public static MainCamera Instance;  // Instancia singleton

    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        zPos = transform.position.z;
        m_Camera = GetComponent<Camera>();
        target = PlayerControl.Instance.transform;
        if(target == null){
            Debug.LogError("Player not found - Main Camera");
        }

        // Usamos o tamanho vertical da camera e as dimensoes da tela para calcular o tamanho horizontal
        aspectRatio = ((float)Screen.width) / ((float)Screen.height);
        aspectRatioOffsetX = aspectRatio * m_Camera.orthographicSize;
        aspectRatioOffsetY = m_Camera.orthographicSize;
        BGCamera.Instance.ApplyAspectRatio(aspectRatio);

        
        transform.position = new Vector3(target.position.x, target.position.y, zPos);

        if(area == null) FreeCam = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Freeze) return;
        Vector2 newPos;

        newPos = target.position; // Posi��o desejada (posi��o do jogador/alvo)

       
        if (!FreeCam)
        {
            if (area != null)
            {
                /*
                  Essa parte � o que impede a camera de sair das �reas de cada sala
                  Cada uma dessas condi��es garante que a camera n�o passe das bordas esquerda, direita, superior e inferior

                  Para isso verificamos se a borda da c�mera (newPos.x|y +/- [Tamanho da Camera]) n�o est� al�m da borda oposta da tela (area.bounds.center.x|y +/- area.bounds.extents.x|y)

                  Para as bordas verticais, podemos verificar o tamanho da c�mera usando a propriedade ortographicSize
                  Por�m, para as horizontais, devemos levar em considera��o o formato da tela do usu�rio. Fizemos esse c�lculo em start()
                 */
		if (area.bounds.size.x <= aspectRatioOffsetX * 2)
		{
		    newPos.x = area.transform.position.x + area.offset.x;
		    
		}
                else if (newPos.x - aspectRatioOffsetX < area.bounds.center.x - area.bounds.extents.x)                // ESQUERDA
                {
                    newPos.x = (area.bounds.center.x - area.bounds.extents.x) + aspectRatioOffsetX;
                }
                else if (newPos.x + aspectRatioOffsetX > area.bounds.center.x + area.bounds.extents.x)           // DIREITA
                {
                    newPos.x = (area.bounds.center.x + area.bounds.extents.x) - aspectRatioOffsetX;
                }
                
                if(area.bounds.size.y <= aspectRatioOffsetY * 2)
                { 
                    newPos.y = area.transform.position.y + area.offset.y;
                    
                }
                else if (newPos.y - aspectRatioOffsetY < area.bounds.center.y - area.bounds.extents.y)        // SUPERIOR
                {
                    newPos.y = (area.bounds.center.y - area.bounds.extents.y) + aspectRatioOffsetY;
                }
                else if (newPos.y + aspectRatioOffsetY > area.bounds.center.y + area.bounds.extents.y)   // INFERIOR
                {
                    newPos.y = (area.bounds.center.y + area.bounds.extents.y) - aspectRatioOffsetY;
                }
            }

            Vector2 distance = newPos - (Vector2)transform.position; // Distancia entre a posi��o final e a posi��o atual

            if (!smoothMovement)
            {
                transform.position = newPos; // Caso a c�mera ja esteja perto da personagem, pulamos diretamente � ela
            }
            else
            {
                transform.position += (Vector3)distance * ((float)-1.957 * (Mathf.Pow(0.6f, Time.deltaTime) - 1)) * SpeedFactor;
                /* Caso contrario, aproxima a camera da personagem de forma exponencial
                 Movimento desejado:  transform.position += distance * 0.6
                 Formula obtida atraves da integral definida de 0.6^x entre 0 e deltaTime
                */
            }
        }
        else
        {
            transform.position = newPos;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos); // Mantemos a posi��o Z da C�mera no valor padr�o
    }

    public void ChangeArea(ScreenZone newArea, float newSize)  // Mudan�a de �rea da c�mera
    {
        area = newArea.GetComponent<Collider2D>();
        FreeCam = false;
        StopAllCoroutines();
        StartCoroutine(ChangeCameraSize(newSize)); // Utilizamos o tamanho de c�mera especificado pela �rea
    }

    public void UseFreeCam()
    {
        Debug.LogWarning("DEBUG - USING FREE CAM");
        area = null;
        FreeCam = true;
    }

    IEnumerator ChangeCameraSize(float targetSize)      // Mudamos o tamanho da c�mera Gradualmente
    {
        smoothMovement = true;
        float t = 0;
        var oldSize = m_Camera.orthographicSize;

        float changeRate;
        if(oldSize >= targetSize) changeRate = 4;
        else changeRate = 4 * (oldSize / targetSize);
        

        aspectRatioOffsetX = aspectRatio * targetSize;
        aspectRatioOffsetY = targetSize;

        while (t < 1)
        {
            t += changeRate * Time.deltaTime;
            var et = -(Mathf.Cos(Mathf.PI * t) - 1) / 2; // T com função de easing
            m_Camera.orthographicSize = Mathf.Lerp(oldSize, targetSize, et);
            yield return null;
        }
        m_Camera.orthographicSize = targetSize;
        // Usamos o tamanho vertical da camera e as dimens�es da tela para calcular o tamanho horizontal



        var i = 15;
        while (i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        smoothMovement = false;
    }
}
