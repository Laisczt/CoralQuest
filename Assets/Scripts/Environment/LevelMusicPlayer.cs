using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    /*
        Gerencia a musica da fase
    */
    public static LevelMusicPlayer Instance;

    private AudioSource levelMusic;     // Audio
    private AudioLowPassFilter muffle;  // Abafador 
    private bool keepMuffled;           // Trava do abafador

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        levelMusic = GetComponent<AudioSource>();
        muffle = GetComponent<AudioLowPassFilter>();
        muffle.enabled = false;
    }

    public void Play()      // Toca a musica
    {
        levelMusic.Play();
    }
    public void Pause(bool boolvalue)   // Pausa/Despausa
    {
        if(boolvalue)
        {
            Pause();
        }else
        {
            UnPause();
        }
    }
    public void Pause()     // Pausa
    {
        levelMusic.Pause();
    }

    public void UnPause()   // Despausa
    {
        levelMusic.UnPause();
    }

    public void Muffle(bool value, bool keepMuffled = false)
    // Abafa/Desabafa o audio
    // keepMuffled impede que outras chamadas da função ativem ou desativem o abafo do audio
    {
        if(keepMuffled && !value) Debug.LogWarning("Avoid setting keepMuffled to true when unmuffling audio");

        this.keepMuffled = keepMuffled;

        muffle.enabled = this.keepMuffled ? true : value;
    }
    
}
