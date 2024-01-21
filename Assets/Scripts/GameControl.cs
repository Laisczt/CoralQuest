using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject Player;
    private PlayerControl s_player;
    private GameObject PlayerSpawn;
    [SerializeField] GameObject enemySpawnerParent;

    private bool usingMobileControls;
    public static GameControl Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (Application.isMobilePlatform || usingMobileControls)
        {
            UseMobileControls(true);
        }

        PlayerSpawn = GameObject.Find("Player Spawn");
        var _player = Instantiate(Player, PlayerSpawn.transform.position, Quaternion.identity);
        _player.name = "Player";
        s_player = _player.GetComponent<PlayerControl>();

        FindObjectOfType<MainCamera>().FindTarget();

        HealthBar.Instance.FindPlayer();
        MainCamera.Instance.FindTarget();

        var enemySpawners = enemySpawnerParent.GetComponentsInChildren<EnemySpawn>();

        foreach(var element in enemySpawners)
        {
            element.Initialize(_player.transform);
        }
    }

    private bool UILinked = false;

    [ContextMenu("Toggle Mobile Controls")]
    public void ToggleMobileControls()
    {
        UseMobileControls(!usingMobileControls);
    }
    private void UseMobileControls(bool doUse)
    {
        usingMobileControls = doUse;

        var controls = GameObject.FindGameObjectsWithTag("Mobile UI");
        foreach(GameObject element in controls)
        {
            element.transform.GetChild(0).gameObject.SetActive(doUse);
        }
        if (!UILinked)
        {
            s_player.SetMobileControls(controls);
            UILinked = true;
        }
        s_player.UsingMobileControls = doUse;
    }
}
