using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    /*
        Timer de nível
    */
    DateTime startTime; // Quando o timer foi iniciado
    TimeSpan timeElapsed;   // Tempo passado desde o início (menos tempo pausado)
    TimeSpan timePaused;    // Tempo passado com o jogo pausado
    TimeSpan offset;    // Tempo adicional  (usado ao recarregar a cena ao reviver)

    public TMP_Text TimerText;  // O texto no HUD

    public static Timer instance;   // Instancia singleton


    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;
    }


    bool running = true;    // Se o timer está rodando ou pausado
    // Update is called once per frame
    void Update()
    {
        timeElapsed = DateTime.Now - startTime - timePaused + offset;
        if(running) // Calcula o tempo e atualiza o texto do HUD
        {
            TimerText.text = timeElapsed.ToString("mm':'ss'.'fff"); // Formato 00:00.000
        }
        else    // Pausa
        {
            timePaused += TimeSpan.FromSeconds(Time.unscaledDeltaTime);
        }
    }

    private bool lockp = false;
    public void pause() 
    {
        if(lockp) return;
        running = false;
    }
    public void pauseAndLock()
    {
        lockp = true;
        pause();
    }
    public void unpause() 
    {
        running = true;
    }

    public void SetOffset(TimeSpan offset)
    {
        this.offset = offset;
    }

    public TimeSpan getTimeElapsed()
    {
        return timeElapsed;
    }

    public void ShowTimer()   // Mostra o timer
    {
        TimerText.enabled = true;
    }
    public void HideTimer()   // Esconde o timer (não impede que rode)
    {
        TimerText.enabled = false;
    }
}
