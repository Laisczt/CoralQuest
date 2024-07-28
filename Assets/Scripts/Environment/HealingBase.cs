using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBase : MonoBehaviour
{
    /*
        Arvores que curam o jogador ao passar por eles
    */
    public int healAmount = 5;      // Vida curada  
    public int cooldown = 1800;     // Cooldown entre usos
    private Animator m_Animator;

    private int rCooldown;      // Cooldown atual
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(rCooldown > 0)rCooldown--;
        if(rCooldown == 1) m_Animator.SetTrigger("Regen");
    }

    public int Use()    // O script de vida do jogador lida com a logica da cura, essa funcao retorna a quantidade de vida que pode ser curada
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
