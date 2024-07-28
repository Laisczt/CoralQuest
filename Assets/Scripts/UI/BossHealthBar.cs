using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    /*
        Barra de vida da boss
    */
    private float filled;   // Porcentagem preenchida da barra de vida
    private RectTransform filledBar;    // transform com o sprite de 'barra preenchida'
    private float fullSize;     // tamanho em x total da barra
    private Animator c_Animator;    // Animador da 'barra preenchida'
    private int damageFrames;   // duracao da animacao de dano 

    public static BossHealthBar Instance { get; private set; }      // singleton instance


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        filledBar = transform.GetChild(1).GetComponent<RectTransform>();
        c_Animator = filledBar.GetComponent<Animator>();
        fullSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        StartCoroutine(increaseHealthStart());
    }

    private void FixedUpdate()
    {
        c_Animator.SetInteger("DamageFrames", damageFrames);    // Reportar se a barra deve ser danificada ao animator

        if (damageFrames > 0) damageFrames--;
    }

    IEnumerator increaseHealthStart()   // Animacao de preencher a barra de vida ao iniciar
    {
        filled = 0;
        float t = 0f;

        var i = 150;    // duração
        while(i > 0)
        {
            i--;
            t += 1f / 150f;
            filled = t;
            ChangeHealth(filled);
            yield return new WaitForFixedUpdate();
        }
        filled = 1;
        ChangeHealth(filled);
    }

    public void ChangeHealth(float newPercent)  // Alterar porcentagem preenchida
    {
        if (newPercent < filled) damageFrames = 5;
        filled = newPercent;

        filledBar.sizeDelta = new Vector2(newPercent * fullSize, filledBar.sizeDelta.y);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
