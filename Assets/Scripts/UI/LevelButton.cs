using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    void Start()
    {
        TMP_Text text;
        text = transform.GetChild(3).GetComponent<TMP_Text>();
        string time = text.text;
        string partialTime = "00:00.000";
        var screen = GetComponent<GameStart>().Screen;

        if(screen == "Shallows")
        {
            time = dataSaverManager.instance.game_data.bestTimeLevel1;
            partialTime = dataSaverManager.instance.timer_offsets.elapsedTimeLevel1;
        }
        else if (screen == "Depths")
        {   
            time = dataSaverManager.instance.game_data.bestTimeLevel2;
            partialTime = dataSaverManager.instance.timer_offsets.elapsedTimeLevel2;
        }

        if(time == "00:00.000") text.enabled = false;
        else text.text = time;

        if(partialTime != "00:00.000")
            transform.GetChild(0).gameObject.SetActive(true);
        
    }

    public void ResetProgress()
    {
        var screen = GetComponent<GameStart>().Screen;
        if(screen == "Shallows") dataSaverManager.instance.MenuResetLevel1();
        else if(screen == "Depths") dataSaverManager.instance.MenuResetLevel2();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
