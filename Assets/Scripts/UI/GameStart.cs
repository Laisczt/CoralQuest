using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour, IDataSaver
{
    /*
        Abrir cena a partir do menu
    */
    public string Screen;
    bool reachedLevel2;
    
    public void ClickPlay()
    {
        if(reachedLevel2) GetComponent<MenuNav>().DelayedChange();
        else LoadScreen();
    }
    public void LoadScreen()
    {
        SceneManager.LoadScene(Screen);
    }

    public void saveData(ref gameData data){}
    public void loadData(gameData data)
    {
        reachedLevel2 = data.ReachedLevel2;
    }

    public void MainMenu()
    {
        if(GameControl.Instance == null) {
            Debug.LogWarning("Game Controller não encontrado");
            return;
        }

        GameControl.Instance.MainMenu();
    }

}
