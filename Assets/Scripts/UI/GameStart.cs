using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    /*
        Abrir cena a partir do menu
    */
    public string Screen;
    
    public void LoadScreen()
    {
        SceneManager.LoadScene(Screen);
    }

}
