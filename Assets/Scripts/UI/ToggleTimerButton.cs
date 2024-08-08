using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTimerButton : MonoBehaviour
{
    bool onoff;
    public void Toggle()
    {
        onoff = !onoff;
        if(onoff) On();
        else Off();
    }

    public void On(){
        onoff = true;
        Timer.instance.ShowTimer();
        GetComponent<Animator>().SetBool("Switch", true);
    }

    public void Off()
    {
        onoff = false;
        Timer.instance.HideTimer();
        GetComponent<Animator>().SetBool("Switch", false);
    }

    public bool IsOn()
    {
        return onoff;
    }

    void OnEnable()
    {
        GetComponent<Animator>().SetBool("Switch",onoff);
    }
}
