using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBase : MonoBehaviour
{
    public int healAmount = 5;
    public int cooldown = 3000;

    private int curr_cooldown;
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
        if(curr_cooldown > 0)curr_cooldown--;
    }

    public int Claim()
    {
        if (curr_cooldown == 0)
        {
            curr_cooldown = cooldown;
            return healAmount;
        }
        else return 0;
    }
}
