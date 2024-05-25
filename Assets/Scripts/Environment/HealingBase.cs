using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBase : MonoBehaviour
{
    public int healAmount = 5;
    public int cooldown = 1800;
    private Animator m_Animator;

    private int rCooldown;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(rCooldown > 0)rCooldown--;
        if(rCooldown == 1) m_Animator.SetTrigger("Regen");
    }

    public int Use()
    {
        if (rCooldown == 0)
        {
            rCooldown = cooldown;
            m_Animator.SetTrigger("Use");
            return healAmount;
        }
        return 0;
    }
}
