using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class TentacleTip : MonoBehaviour, IEnemy
{
    [SerializeField] BasicEnemy basicEnemy;
    public List<ScreenExit> blockedExits;

    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int _value)
    {
        basicEnemy.m_Animator.SetTrigger("Damage");
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
        basicEnemy.m_Animator.SetTrigger("Death");

        foreach(Transform i in transform)
        {
            if(i.name == "Barrier") continue;
            i.GetComponent<Animator>().SetTrigger("Death");
        }

        StartCoroutine(deathStall());
    }

    private IEnumerator deathStall()
    {
        var i = 55;

        while (i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }


}
