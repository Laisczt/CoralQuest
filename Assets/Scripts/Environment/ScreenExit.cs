using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenExit : MonoBehaviour
{
    public GameObject destination; // Destino da sa�da
    public bool isScrollingTransition = true; // TODO
    public bool isLocked = false; // Se a sa�da pode ser usada ou n�o


    private Collider2D m_Collider; // Collider da sa�da
    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player" && !isLocked)
        {
            if (destination != null)
                StartCoroutine(ChangeArea(destination));
            else
            {
                MainCamera.Instance.FreeCam = true;
                Debug.LogWarning("DEBUG - ENTERING FREE CAM MODE");
            }
        }
    }

    float grav;
    Rigidbody2D rb;
    IEnumerator ChangeArea(GameObject newArea)
    {

        Time.timeScale = 0.4f;

        var horizontal = m_Collider.bounds.extents.x < m_Collider.bounds.extents.y;
        // Caso essa sa�da seja na horizontal e n�o uma queda ou subida (definido pelas propor��es do collider) travamos o movimento do jogador
        if (horizontal)
        {
            PlayerControl.Instance.LockMovement = true;
        }
        else
        {
            rb = PlayerControl.Instance.GetComponent<Rigidbody2D>();
            grav = rb.gravityScale;
            rb.gravityScale = 0;
            if(rb.velocity.y > 0) rb.velocity = new Vector2(0, PlayerControl.Instance.JumpPower * 0.5f);
        }

        MainCamera.Instance.ChangeArea(newArea); // Mudamos a �rea da c�mera


        yield return new WaitForSeconds(0.1f);  // Delay de 0.1
        Time.timeScale = 0.7f;
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 1f;

        if (horizontal)
        {
            PlayerControl.Instance.LockMovement = false; // Destravamos o movimento do jogador
        }
        else
        {
            rb.gravityScale = grav;
        }

        yield return new WaitForSeconds(0.2f);
        newArea.transform.Find("Exits").gameObject.SetActive(true); // Ativamos as sa�das da �rea nova

        transform.parent.gameObject.SetActive(false); // Desativamos as sa�das da �rea em que est�vamos
    }
}
