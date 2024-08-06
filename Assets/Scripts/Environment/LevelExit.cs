using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    /*
        Saida do nivel, um tunel que forca a player a continuar andando e entao carrega outra cena
    */
    public string TargetScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            MainCamera.Instance.Freeze = true;
            PlayerControl.Instance.LockMovement = true;
            Timer.instance.pauseAndLock();

            GameControl.Instance.ChangeLevel(TargetScreen);
        }
    }
}
