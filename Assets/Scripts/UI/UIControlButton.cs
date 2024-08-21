using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlButton : MonoBehaviour
{
    /*
        Para botoes de UI (em jogo)
    */
    private bool pressed;
    private bool justPressed;
    public bool GetButtonDown()
    {
        justPressed = false;
        return pressed;
    }

    private void Update()
    {
        if (!justPressed) pressed = false;
        justPressed = false;
    }

    public void Press()
    {
        pressed = true;
        justPressed = true;
    }


}
