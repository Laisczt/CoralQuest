using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField] Transform thumbnailsParent;

    public string Screen;
    
    public void LoadScreen()
    {
        SceneManager.LoadScene(Screen);
    }

    public void ShowThumbnail()
    {
        thumbnailsParent.transform.Find(Screen).gameObject.SetActive(true);
    }

    public void HideThumbnail()
    {
        thumbnailsParent.transform.Find(Screen).gameObject.SetActive(false);
    }
}
