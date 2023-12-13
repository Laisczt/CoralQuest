using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    public int cooldown = 60;
    public GameObject spawn;

    private int curr_Cooldown = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (curr_Cooldown == 0)
        {
            Instantiate(spawn, transform.position, Quaternion.identity);
            curr_Cooldown = cooldown;
        }
        else if (curr_Cooldown > 0) curr_Cooldown--;
    }
}
