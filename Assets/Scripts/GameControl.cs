using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject Player;
    private GameObject PlayerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawn = GameObject.Find("Player Spawn");
        var _player = Instantiate(Player, PlayerSpawn.transform.position, Quaternion.identity);
        _player.name = "Player";

        /*
        var healthBar = Instantiate(HealthBar, UI.transform);
        healthBar.name = "Health Bar";
        */

        HealthBar.Instance.FindPlayer();
        MainCamera.Instance.FindTarget();

    }

}
