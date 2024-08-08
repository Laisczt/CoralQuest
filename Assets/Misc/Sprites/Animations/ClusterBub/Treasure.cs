using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    GameObject watch;
    Transform son;
    bool on;
    void Start()
    {
        watch = GameObject.FindWithTag("Save Point");
        son = transform.GetChild(0);
        if(son.name != "Treasure") gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if(!on)
        {
            if(!watch.activeInHierarchy) StartCoroutine(wow());
        }
        else
        {
            son.transform.Rotate(Vector3.down * 3);
        }
    }

    IEnumerator wow()
    {
        on = true;
        son.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        son.GetComponent<AudioSource>().Play();
    }
}
