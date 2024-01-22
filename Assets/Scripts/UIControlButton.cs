using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlButton : MonoBehaviour
{
    private ushort pressed = 0;
    public bool GetButtonDown()
    {
        return pressed > 0;
    }

    private void Update()
    {
        if (pressed > 0) pressed--;
    }

    public void Press()
    {
        pressed = 2;
    }


}
