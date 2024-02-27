using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCamera : MonoBehaviour
{
    private GameObject target;      // O objeto que a câmera segue (alvo)
    public Collider2D area;         // A região em que a câmera está
    public float SpeedFactor = 3f;  // A Velocidade de aproximação da câmera
    public bool smoothMovement;
    public bool FreeCam;
    public bool Freeze;

    private Camera m_Camera;        // Acesso ao script Camera
    private float aspectRatioOffsetX;// Compensação ao tamanho horizontal da camera ao depender do formato da tela
    private float aspectRatioOffsetY;// Compensação 
    public float aspectRatio;       // Proporções da camera
    private const float zPos = -10; // A posição padrão da câmera no eixo Z

    public static MainCamera Instance  // Propriedade estática para facilitar o acesso da câmera por outros scripts (singleton)
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

        // Usamos o tamanho vertical da camera e as dimensões da tela para calcular o tamanho horizontal
        aspectRatio = ((float)Screen.width) / ((float)Screen.height);
        aspectRatioOffsetX = aspectRatio * m_Camera.orthographicSize;
        aspectRatioOffsetY = m_Camera.orthographicSize;
        BGCamera.Instance.ApplyAspectRatio(aspectRatio);

        // Iniciaremos a posição da câmera à mesma posição do jogador
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, zPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Freeze) return;
        Vector2 newPos;

        newPos = target.transform.position; // Posição desejada (posição do jogador/alvo)

        if (!FreeCam)
        {
            if (area != null)
            {
                /*
                  Essa parte é o que impede a camera de sair das áreas de cada sala
                  Cada uma dessas condições garante que a camera não passe das bordas esquerda, direita, superior e inferior

                  Para isso verificamos se a borda da câmera (newPos.x|y +/- [Tamanho da Camera]) não está além da borda oposta da tela (area.bounds.center.x|y +/- area.bounds.extents.x|y)

                  Para as bordas verticais, podemos verificar o tamanho da câmera usando a propriedade ortographicSize
                  Porém, para as horizontais, devemos levar em consideração o formato da tela do usuário. Fizemos esse cálculo em start()
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

            Vector2 distance = newPos - (Vector2)transform.position; // Distancia entre a posição final e a posição atual

            if (!smoothMovement)
            {
                transform.position = newPos; // Caso a câmera ja esteja perto da personagem, pulamos diretamente à ela
            }
            else
            {
                transform.position += (Vector3)distance * ((float)-1.957 * (Mathf.Pow(0.6f, Time.deltaTime) - 1)) * SpeedFactor;
                /* Caso contrário, aproxima a câmera da personagem de forma exponencial
                 Movimento desejado é  transform.position += distance * 0.6
                 Porém ao levar em consideração deltaTime, a fórmula que corretamente aproxima esse movimento é  distance * -1.957 * (0.6^deltaTime - 1)
                 Fórmula obtida através da integral definida de 0.6^x entre 0 e deltaTime
                */
            }
        }
        else
        {
            transform.position = newPos;
        }





        transform.position = new Vector3(transform.position.x, transform.position.y, zPos); // Mantemos a posição Z da Câmera no valor padrão
    }

    public void FindTarget()    // Atribuição de alvo da câmera (será o player, por padrão)
    {
        target = GameObject.Find("Player");
    }

    public void FindTarget(GameObject target) // Atribuição de alvo da câmera (quando especificado algo diferente do jogador)
    {
        this.target = target;
    }

    public void ChangeArea(GameObject newArea)  // Mudança de área da câmera
    {
        area = newArea.GetComponent<Collider2D>();
        FreeCam = false;
        StartCoroutine(ChangeCameraSize(area.GetComponent<ScreenZone>().cameraSize)); // Utilizamos o tamanho de câmera especificado pela área
    }

    IEnumerator ChangeCameraSize(float targetSize)      // Mudamos o tamanho da câmera Gradualmente
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
        // Usamos o tamanho vertical da camera e as dimensões da tela para calcular o tamanho horizontal



        var i = 15;
        while (i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        smoothMovement = false;
    }
}
