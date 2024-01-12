using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{   
    private GameObject target;

    public static MainCamera Instance
    {
        get
        {
            return FindObjectOfType<MainCamera>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
    }

    public void FindTarget()
    {
        target = GameObject.Find("Player");
    }

    public void FindTarget(GameObject target)
    {
        this.target = target;
    }
}
