using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject Player;
    private GameObject PlayerSpawn;
    [SerializeField] GameObject enemySpawnerParent;

    public static bool usingMobileControls = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isMobilePlatform)
        {
            UseMobileControls();
        }

        PlayerSpawn = GameObject.Find("Player Spawn");
        var _player = Instantiate(Player, PlayerSpawn.transform.position, Quaternion.identity);
        _player.name = "Player";

        FindObjectOfType<MainCamera>().FindTarget();

        HealthBar.Instance.FindPlayer();
        MainCamera.Instance.FindTarget();

        var enemySpawners = enemySpawnerParent.GetComponentsInChildren<EnemySpawn>();

        foreach(var element in enemySpawners)
        {
            element.Initialize(_player.transform);
        }
    }

    public static void UseMobileControls()
    {
        if (usingMobileControls)
        {
            Debug.LogError("Mobile controls already enabled");
            return;
        }
        usingMobileControls = true;
        var controls = GameObject.FindGameObjectsWithTag("Mobile UI");
        foreach(GameObject element in controls)
        {
            element.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
