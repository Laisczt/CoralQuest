using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicStrip : MonoBehaviour
{
    /*
        Controla a cena do quadrinho ao completar o jogo
    */
    [SerializeField] Animator[] pages;  // Lista das paginas
    int currentPage;    // Pagina atual

    int maxPageReached; // Maior pagina vista
    int panels;         // Numero de paineis da pagina atual
    int currentPanel;   // Painel atual
    bool midTransition = true;  // Se uma animacao esta rodando agora

    public GameObject nextPageButton;   // Botao de avancar pagina
    public GameObject previousPageButton;   // Botao de voltar uma pagina

    void Start()
    {
        StartCoroutine(waitToStart());
    }

    IEnumerator waitToStart()       // Espera alguns segundos antes de comecar o quadrinho
    {
        yield return new WaitForSecondsRealtime(3f);

        pages[0].gameObject.SetActive(true);
        panels = pages[0].GetComponent<ComicPage>().panelCount; // Pega o numero de paineis da primeira pagina
        currentPanel = 1;
        
        StartCoroutine(TransitionDelay());
    }

    IEnumerator TransitionDelay(bool longer = false)    // impede acoes por um periodo apos uma animacao ser ativada
    {
        midTransition = true;

        var i = 25;
        if(longer) i = 60; 
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        midTransition = false;
    }
    public void NextPanel()     // Avanca um painel
    {
        if(midTransition) return;
        if(!(currentPanel < panels)) return;
        currentPanel++;
        pages[currentPage].SetTrigger("Advance");

        if(currentPanel == panels && (currentPage + 1 == pages.Length)) StartCoroutine(TransitionDelay(true));
        else StartCoroutine(TransitionDelay());

        if(currentPanel == panels) {    // Ao chegar no ultimo painel...
            nextPageButton.SetActive(true);                     //ativa o botao de proxima pagina
            if(currentPage > maxPageReached) maxPageReached++;  // Atualiza a max pagina alcancada se for o caso
        }
    }

    public void PreviousPage()  // Volta uma pagina
    {
        if(midTransition) return;

        pages[currentPage].SetTrigger("Restart");   // Reinicia a animacao na pagina atual
        currentPanel = 1;
        
        if(currentPage == 0) return;

        pages[currentPage].gameObject.SetActive(false);
        pages[currentPage-1].gameObject.SetActive(true);
        panels = pages[currentPage-1].GetComponent<ComicPage>().panelCount;
        if(panels == 1) nextPageButton.SetActive(true);
        currentPage--;
        StartCoroutine(TransitionDelay());
    }

    public void NextPage()  // Avanca uma pagina
    {
        if(midTransition) return;

        if(currentPage + 1 == pages.Length) {
            SceneManager.LoadScene("Main Menu");
            return;
        }

        pages[currentPage].SetTrigger("Restart");   // Reiniica a animacao na pagina atual
        currentPanel = 1;
        pages[currentPage].gameObject.SetActive(false);
        pages[currentPage+1].gameObject.SetActive(true);
        panels = pages[currentPage+1].GetComponent<ComicPage>().panelCount;

        if(currentPage >= maxPageReached)
        {
            if(panels > 1) nextPageButton.SetActive(false); // Desativa o botao de prox pagina caso esteja indo a uma pagina nova
        }

        previousPageButton.SetActive(true);
        
        currentPage++;

        StartCoroutine(TransitionDelay());

        
    }
}
