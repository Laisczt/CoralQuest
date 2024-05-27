using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCamera : MonoBehaviour
{
    private Transform target;      // O objeto que a c�mera segue (alvo)
    public Collider2D area;         // A regi�o em que a c�mera est�
    public float SpeedFactor = 3f;  // A Velocidade de aproxima��o da c�mera
    public bool smoothMovement;
    public bool FreeCam;
    public bool Freeze;

    private Camera m_Camera;        // Acesso ao script Camera
    private float aspectRatioOffsetX;// Compensa��o ao tamanho horizontal da camera ao depender do formato da tela
    private float aspectRatioOffsetY;// Compensa��o 
    public float aspectRatio;       // Propor��es da camera
    private const float zPos = -10; // A posi��o padr�o da c�mera no eixo Z

    public static MainCamera Instance  // Propriedade est�tica para facilitar o acesso da c�mera por outros scripts (singleton)
    {
        get
        {
            return FindObjectOfType<MainCamera>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        target = PlayerControl.Instance.transform;
        if(target == null){
            Debug.LogError("Player not found - Bubble Manager");
        }

        // Usamos o tamanho vertical da camera e as dimens�es da tela para calcular o tamanho horizontal
        aspectRatio = ((float)Screen.width) / ((float)Screen.height);
        aspectRatioOffsetX = aspectRatio * m_Camera.orthographicSize;
        aspectRatioOffsetY = m_Camera.orthographicSize;
        BGCamera.Instance.ApplyAspectRatio(aspectRatio);

        // Iniciaremos a posi��o da c�mera � mesma posi��o do jogador
        transform.position = new Vector3(target.position.x, target.position.y, zPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Freeze) return;
        Vector2 newPos;

        newPos = target.position; // Posi��o desejada (posi��o do jogador/alvo)

       
        if (!FreeCam)
        {
            if(area.bounds.size.x < aspectRatioOffsetX * 2) newPos = new Vector2(area.transform.position.x + area.offset.x, newPos.y);
            if(area.bounds.size.y < aspectRatioOffsetY * 2) newPos = new Vector2(newPos.x, area.transform.position.y + area.offset.y);

            if (area != null)
            {
                /*
                  Essa parte � o que impede a camera de sair das �reas de cada sala
                  Cada uma dessas condi��es garante que a camera n�o passe das bordas esquerda, direita, superior e inferior

                  Para isso verificamos se a borda da c�mera (newPos.x|y +/- [Tamanho da Camera]) n�o est� al�m da borda oposta da tela (area.bounds.center.x|y +/- area.bounds.extents.x|y)

                  Para as bordas verticais, podemos verificar o tamanho da c�mera usando a propriedade ortographicSize
                  Por�m, para as horizontais, devemos levar em considera��o o formato da tela do usu�rio. Fizemos esse c�lculo em start()
                 */

                if (newPos.x - aspectRatioOffsetX < area.bounds.center.x - area.bounds.extents.x)                // ESQUERDA
                {
                    newPos.x = (area.bounds.center.x - area.bounds.extents.x) + aspectRatioOffsetX;
                }
                else if (newPos.x + aspectRatioOffsetX > area.bounds.center.x + area.bounds.extents.x)           // DIREITA
                {
                    newPos.x = (area.bounds.center.x + area.bounds.extents.x) - aspectRatioOffsetX;
                }

                if (newPos.y - aspectRatioOffsetY < area.bounds.center.y - area.bounds.extents.y)        // SUPERIOR
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
                /* Caso contr�rio, aproxima a c�mera da personagem de forma exponencial
                 Movimento desejado �  transform.position += distance * 0.6
                 Por�m ao levar em considera��o deltaTime, a f�rmula que corretamente aproxima esse movimento �  distance * -1.957 * (0.6^deltaTime - 1)
                 F�rmula obtida atrav�s da integral definida de 0.6^x entre 0 e deltaTime
                */
            }
        }
        else
        {
            transform.position = newPos;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos); // Mantemos a posi��o Z da C�mera no valor padr�o
    }

    public void ChangeArea(GameObject newArea)  // Mudan�a de �rea da c�mera
    {
        area = newArea.GetComponent<Collider2D>();
        FreeCam = false;
        StartCoroutine(ChangeCameraSize(area.GetComponent<ScreenZone>().cameraSize)); // Utilizamos o tamanho de c�mera especificado pela �rea
    }

    IEnumerator ChangeCameraSize(float targetSize)      // Mudamos o tamanho da c�mera Gradualmente
    {
        smoothMovement = true;
        float t = 0;
        var oldSize = m_Camera.orthographicSize;

        aspectRatioOffsetX = (((float)Screen.width) / ((float)Screen.height)) * targetSize;
        aspectRatioOffsetY = targetSize;

        while (t < 1)
        {
            m_Camera.orthographicSize = Mathf.Lerp(oldSize, targetSize, t);
            t += 5 * Time.deltaTime;
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
