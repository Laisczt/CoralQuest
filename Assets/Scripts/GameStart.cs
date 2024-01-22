using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
   public string Screen;

   public void LoadScreen()
   {
    SceneManager.LoadScene(Screen);
   }
}
