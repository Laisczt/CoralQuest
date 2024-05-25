using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth target;
    public Animator heart;
    private int maxHp;
    private int hp;
    Animator[] healthBar;

    public static HealthBar Instance{ get; private set; }

    public void Start()
    {
        Instance = this;
        target = PlayerControl.Instance.GetComponent<PlayerHealth>();
        initialize();
    }
    private float offset = 195.65f;
    private void initialize()
    {
        maxHp = target.MaxHealth;
        hp = target.Health;
        healthBar = new Animator[maxHp];

        Vector3 pos = transform.position + new Vector3(offset * (maxHp-1) + 128.75f, 0 );
        for (int i = maxHp - 1; i >= 0; i--)
        {
            healthBar[i] = Instantiate(heart, pos, Quaternion.identity, transform);
            healthBar[i].name = "Heart " + (i + 1).ToString();
            healthBar[i].SetBool("Filled", true);
            pos -= new Vector3(offset, 0, 0);
        }
    }
    public void UpdateHB()
    {
        var a = target.Health;
        if (a < hp)
        {
            for (int i = hp; i > a; i--)
            {
                healthBar[i - 1].SetBool("Filled", false);
            }
        }
        else if (a > hp)
        {
            for (int i = hp; i < a; i++)
            {
                healthBar[i].SetBool("Filled", true);
            }
        }

        hp = a;
    }
    

 
}
