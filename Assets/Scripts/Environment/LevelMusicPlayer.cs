using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    /*
        Gerencia a musica da fase
    */
    public static LevelMusicPlayer Instance;

    public AudioSource levelMusic;     // Audio
    public AudioSource ambiance;     // Audio
    private AudioLowPassFilter muffle;  // Abafador 
    private bool keepMuffled;           // Trava do abafador

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        muffle = GetComponent<AudioLowPassFilter>();
        muffle.enabled = false;
    }

    public void Play()      // Toca a musica
    {
        levelMusic.Play();
    }
    public void Pause()     // Pausa
    {
        levelMusic.Pause();
    }

    public void UnPause()   // Despausa
    {
        levelMusic.UnPause();
    }

    public void PauseAmbiance() // Pausa o som de fundo
    {
        ambiance.Pause();
    }
    public void UnPauseAmbiance() // Despausa o som de fundo
    {
        ambiance.UnPause();
    }

    public void Muffle(bool value, bool lockmuffle = false)
    // Abafa/Desabafa o audio
    // lockmuffle impede que outras chamadas da função ativem ou desativem o abafo do audio
    {
        if(keepMuffled) return;
        keepMuffled = lockmuffle;
        muffle.enabled = value;
    }
    
}
