using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class BossTrigger : MonoBehaviour, IEnemy
{
    /*
        Tentaculo que spawna a boss ao ser morto
    */
    public GameObject boss;
    [SerializeField, HideInInspector] Animator m_Animator;
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;

    private void OnValidate(){
        basicEnemy = GetComponent<BasicEnemy>();
    }

    private void Start(){
        m_Animator = GetComponent<Animator>();
    }
    
    public void Damage(int a){
        m_Animator.SetTrigger("Damage");
    }
    public void Knockback(){}

    public void Kill(){
        m_Animator.SetTrigger("Death");
        basicEnemy.DeathStall(55);
        StartCoroutine(spawnBoss());
    }
    
    private IEnumerator spawnBoss()
    {
        var i = 10;
        while(i > 0){
            i--;
            yield return new WaitForFixedUpdate();
        }
        LevelMusicPlayer.Instance.Play();   // Toca a musica
        boss.SetActive(true);
    }
}
