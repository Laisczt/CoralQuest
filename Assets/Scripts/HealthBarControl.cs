using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarControl : MonoBehaviour
{
    private GameObject target;
    public GameObject heart;
    private int maxHp;
    private int hp;
    GameObject[] healthBar;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int value)
    {
        if (hp - value <= 0) value = 0; // Caps damage at 0
        else value = hp - value;    // value becomes hp after damage

        for (int i = hp; i > value; i--)
        {
            healthBar[i - 1].SetActive(false); // damage ui heart
        }
        hp = value;
    }

    [ContextMenu("Damage 1")]
    public void Damage1()
    {
        Damage(1);
    }


    public void Heal(int value)
    {
        if (hp + value >= maxHp) value = maxHp; // Caps heal at max health
        else value += hp;   // value becomes hp after heal

        for (int i = hp; i < value; i++)
        {
            healthBar[i].SetActive(true);   // restore ui heart
        }
        hp = value;
    }

    [ContextMenu("Heal 1")]
    public void Heal1()
    {
        Heal(1);
    }

    public void AsignTarget(GameObject target)
    {
        this.target = target;

        maxHp = target.GetComponent<PlayerControl>().maxHealth;
        hp = target.GetComponent<PlayerControl>().health;
        healthBar = new GameObject[maxHp];

        Vector3 pos = transform.position;
        for (int i = 0; i < maxHp; i++)
        {
            healthBar[i] = Instantiate(heart, pos, Quaternion.identity, transform);
            healthBar[i].name = "Heart " + (i + 1).ToString();
            pos += new Vector3(90, 0, 0);
        }

        target.gameObject.GetComponent<PlayerControl>().m_HealthBar = transform.gameObject;
    }
}
