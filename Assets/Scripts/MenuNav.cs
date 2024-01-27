using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNav : MonoBehaviour
{
    [SerializeField] GameObject DestinationTab;


    public void ChangeTab()
    {
        DestinationTab.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
