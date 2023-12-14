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

    public void UpdateHB()
    {
        var a = target.GetComponent<PlayerControl>().health;

        if (a < hp)
        {
            for (int i = hp; i > a; i--)
            {
                healthBar[i - 1].transform.GetChild(0).gameObject.SetActive(false);
                healthBar[i - 1].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else if (a > hp)
        {
            for (int i = hp; i < a; i++)
            {
                healthBar[i].transform.GetChild(0).gameObject.SetActive(true);
                healthBar[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        hp = a;
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

        target.gameObject.GetComponent<PlayerControl>().m_HealthBar = this;
    }
}
