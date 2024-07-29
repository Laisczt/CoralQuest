using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNav : MonoBehaviour
{
    /*
        Botoes de UI que ativam outro canvas (e desativam o atual)
    */

    [SerializeField] GameObject DestinationTab;
    public void ChangeTab()
    {
        DestinationTab.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }

    public void DelayedChange()
    {
        StartCoroutine(buttonDelay());
    }

    IEnumerator buttonDelay()
    {
        var i = 8;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        ChangeTab();
    }
}
