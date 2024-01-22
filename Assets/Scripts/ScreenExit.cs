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
        if(collision.gameObject.name == "Player" && destination != null && !isLocked)
        {
            StartCoroutine(ChangeArea(destination));
        }
    }

    IEnumerator ChangeArea(GameObject newArea)
    {
        
        Time.timeScale = 0.4f;

        // Caso essa sa�da seja na horizontal e n�o uma queda ou subida (definido pelas propor��es do collider) travamos o movimento do jogador
        if (m_Collider.bounds.extents.x < m_Collider.bounds.extents.y){
            GameObject.Find("Player").GetComponent<PlayerControl>().lockMovement = true;
        }

        MainCamera.Instance.ChangeArea(newArea); // Mudamos a �rea da c�mera
        MainCamera.Instance.smoothMovement = true;

        yield return new WaitForSeconds(0.2f);  // Delay de 0.2 segundos

        Time.timeScale = 1f;

        GameObject.Find("Player").GetComponent<PlayerControl>().lockMovement = false; // Destravamos o movimento do jogador

        newArea.transform.Find("Exits").gameObject.SetActive(true); // Ativamos as sa�das da �rea nova

        MainCamera.Instance.smoothMovement = false;

        transform.parent.gameObject.SetActive(false); // Desativamos as sa�das da �rea em que est�vamos
    }
}
