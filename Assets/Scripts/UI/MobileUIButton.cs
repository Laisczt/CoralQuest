using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    UIControlButton control;
    public int delay = 3;
    private int rdelay;

    private void Start()
    {
        rdelay = delay;
        control = GetComponent<UIControlButton>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(PressContinuously());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        rdelay = delay;
    }

    IEnumerator PressContinuously()
    {
        while(true)
        {
            rdelay = delay;
            control.Press();
            do
            {
                rdelay--;
                yield return null;
            } while(rdelay > 0);
        }
    }
}
