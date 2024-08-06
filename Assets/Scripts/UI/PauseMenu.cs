using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    /*
        Funcoes de pausa
    */
    public GameObject PausePanel;
    public GameObject PauseButton;

    bool paused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))TogglePause();
    }

    public void PauseGame()
    {
        paused = true;
        PauseButton.SetActive(false);
        PausePanel.SetActive(true);
        LevelMusicPlayer.Instance.Muffle(true);
        Time.timeScale = 0;
        Timer.instance.pause();
    }

    public void ResumeGame()
    {
        paused = false;
        PauseButton.SetActive(true);
        PausePanel.SetActive(false);
        LevelMusicPlayer.Instance.Muffle(false);
        Time.timeScale = 1;
        Timer.instance.unpause();
    }

    public void TogglePause()
    {
        if(paused) ResumeGame();
        else PauseGame();
    }
}
