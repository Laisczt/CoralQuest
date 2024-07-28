using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressAnimate : MonoBehaviour
{
    /*
        Animacao de apertar botoes de UI
    */
    public void press()
    {
        GetComponent<Animator>().SetTrigger("Press");
    }
}
