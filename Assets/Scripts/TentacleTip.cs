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

    public void Damage()
    {

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
        Destroy(gameObject);
    }

}
