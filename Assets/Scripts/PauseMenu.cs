using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject PauseButton;

    public void PauseGame()
    {
        PauseButton.SetActive(false);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        PauseButton.SetActive(true);
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
