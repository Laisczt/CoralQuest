using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class TentacleTip : MonoBehaviour, IEnemy
{
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private Animator m_Animator;
    public List<ScreenExit> blockedExits;

    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int _value)
    {
        m_Animator.SetTrigger("Damage");
    }
    public void Knockback()
    {

    }
    public void Kill()
    {
        foreach(var exit in blockedExits)
        {
            exit.isLocked = false;
        }
        m_Animator.SetTrigger("Death");

        foreach(Transform i in transform)
        {
            if(i.name == "Barrier") continue;
            i.GetComponent<Animator>().SetTrigger("Death");
        }

        basicEnemy.DeathStall(55);
    }
}
