using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    /*
        Barra de vida do jogador
    */
    private PlayerHealth target;    // O player
    public Animator heart;          // O prefab do coracao
    private int maxHp;      // Vida maxima
    private int hp;         // Vida atual
    Animator[] healthBar;   // Todos os coracoes
    private float offset = 100.125f;    // Offset do tamanho do sprite do coracao na UI

    public static HealthBar Instance{ get; private set; }

    public void Start()
    {
        Instance = this;
        target = PlayerControl.Instance.GetComponent<PlayerHealth>();
        initialize();
    }
    private void initialize()   // Initialize cria todos os coracoes e os adiciona a healthBar[]
    {
        maxHp = target.MaxHealth;
        hp = target.Health;
        healthBar = new Animator[maxHp];

        Vector3 pos = transform.position + new Vector3(offset * (maxHp-1) + 60f, 0 );
        for (int i = maxHp - 1; i >= 0; i--)
        {
            healthBar[i] = Instantiate(heart, pos, Quaternion.identity, transform);
            healthBar[i].name = "Heart " + (i + 1).ToString();
            healthBar[i].SetBool("Filled", true);
            pos -= new Vector3(offset, 0, 0);
        }
    }
    public void UpdateHB()      // Atualiza a barra de vida 
    {
        var a = target.Health;      // Pega a vida do jogador
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
