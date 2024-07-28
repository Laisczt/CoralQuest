using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHyperlink : MonoBehaviour
{
    /*
        Butoes de UI que abrem um hyperlink
    */
    public string link;

    public void OpenHL()
    {
        Application.OpenURL(link);
    }
}
