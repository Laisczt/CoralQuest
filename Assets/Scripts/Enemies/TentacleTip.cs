using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class TentacleTip : MonoBehaviour, IEnemy
{
    /*
        Cabeca do tentaculo que bloqueia travessia pelo mapa, deve ser morto para progredir
    */
    [HideInInspector] public BasicEnemy basicEnemy;
    [HideInInspector] public Animator m_Animator;

    public AudioSource damageSound;
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
        damageSound.PlayOneShot(damageSound.clip);
    }
    public void Knockback()
    {

    }
    public void Kill()
    {
        m_Animator.SetTrigger("Death");

        damageSound.PlayDetached();

        Destroy(transform.GetChild(0).gameObject);

        foreach(Transform i in transform)   // "Mata" todos os segmentos de tentaculo associados
        {
            if(i.name == "Barrier") continue;
            i.GetComponent<Animator>().SetTrigger("Death");
        }

        basicEnemy.KillPermanently();

        basicEnemy.DeathStall(55);
    }
}
