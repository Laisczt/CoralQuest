using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string TargetScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MainCamera.Instance.Freeze = true;
        PlayerControl.Instance.lockMovement = true;
        StartCoroutine(StallTransition());
    }

    IEnumerator StallTransition()
    {
        var i = 90;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        
        SceneManager.LoadScene(TargetScreen);

    }
}
