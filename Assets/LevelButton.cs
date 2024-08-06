using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    private TMP_Text text;
    void Start()
    {
        text = transform.GetChild(3).GetComponent<TMP_Text>();
        string time = text.text;
        Vector3 savepos = new Vector3();
        var screen = GetComponent<GameStart>().Screen;

        if(screen == "Shallows")
        {
            time = dataSaverManager.instance.game_data.bestTimeLevel1;
            savepos = dataSaverManager.instance.game_data.Level1SavePos;
        }
        else if (screen == "Depths")
        {   
            time = dataSaverManager.instance.game_data.bestTimeLevel2;
            savepos = dataSaverManager.instance.game_data.Level2SavePos;
        }

        if(time == "00:00.000") text.enabled = false;
        else
        {
            text.text = time;

            if(savepos != new Vector3(0,0,-100)) transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void ResetProgress()
    {
        var screen = GetComponent<GameStart>().Screen;
        if(screen == "Shallows") dataSaverManager.instance.MenuResetLevel1();
        else if(screen == "Depths") dataSaverManager.instance.MenuResetLevel2();
         transform.GetChild(0).gameObject.SetActive(false);
    }
}
