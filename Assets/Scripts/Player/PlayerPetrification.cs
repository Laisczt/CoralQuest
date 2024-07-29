using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(PlayerControl))]
public class PlayerPetrification : MonoBehaviour
{
    /*
        Petrifica o jogador
    */
    [SerializeField, HideInInspector] PlayerControl m_PlayerControl;     // Controlador do player
    [SerializeField, HideInInspector] Animator m_Animator;
    public int shakeOffAmount;      // O quanto o jogador deve sacudir para se livrar da petrificacao
    private int rShakeOffAmount;    // Quantidade restante
    public float progressDecayRate = 1;     // Velocidade na qual o progresso de sacudir eh reduzido
    private float progressDecayCounter;     // Contador para a perda de progresso

    private int shaking;        // Buffer se o jogador esta sacudindo ou nao

    private void OnValidate()
    {
        m_PlayerControl = GetComponent<PlayerControl>();
        m_Animator = GetComponent<Animator>();
    }
    private void OnEnable()     // O script eh habilitado sempre que o jogador eh petrificado, variaveis sao redefinidas aqui
    {
        rShakeOffAmount = shakeOffAmount;
        shaking = 0;
        progressDecayCounter = 0;
        prevTouchCount = 0;
    }

    private int prevTouchCount;     // Numero de touchs na tela no frame anterior
    private void Update()
    {
        var touchcount = Input.touchCount;
        var touchProgress = touchcount > prevTouchCount;
        prevTouchCount = touchcount;
        if(Input.anyKeyDown) shaking = 1;   // Clicks no teclado "sacodem" por 1 frame
        if(touchProgress) shaking = 5;      // Toques na tela "sacodem" por 5


        progressDecayCounter += Time.deltaTime;
    }

    private int shook; // Quantidade ja sacudida (a cada 5 o sprite eh atualizado)
    private void FixedUpdate()
    {
        if (rShakeOffAmount <= 0)   // desativa o script quando a player se livra da petrificacao
        {
            m_Animator.ResetTrigger("Shake");
            m_PlayerControl.Depetrify();
            this.enabled = false;
        }

        if (shaking > 0)        // Sacodir
        {
            rShakeOffAmount--;
            shook++;
            if(shook >= 5)
            {
                shook = 0;
                m_Animator.SetTrigger("Shake");
            };
        }

        if (progressDecayCounter >= 1 / progressDecayRate)  // Perda de progresso
        {
            progressDecayCounter -= 1 / progressDecayRate;
            if (rShakeOffAmount < shakeOffAmount) rShakeOffAmount++;
        }

        if(shaking > 0) shaking--;
    }

    public void Shake(){
        shaking = 1;
    }
}
