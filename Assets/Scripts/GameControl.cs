using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject Player;
    private PlayerControl s_player;
    [SerializeField] GameObject PlayerSpawn;
    [SerializeField] GameObject enemyParent;
    [SerializeField] Camera bgCamera;

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
        bgCamera.enabled = true;

        HealthBar.Instance.FindTarget();
        MainCamera.Instance.FindTarget();


        var enemies = enemyParent.GetComponentsInChildren<BasicEnemy>();
        foreach (var element in enemies)
        {
            element.Target = _player.transform;
        }

        if(dataSaverManager.instance != null)
        {
            dataSaverManager.instance.dataHandler = new fileDataHandler(Application.persistentDataPath, dataSaverManager.instance.fileName);
            dataSaverManager.instance.dataSaverObjects = dataSaverManager.instance.FindAllDataSaverObjects();
            dataSaverManager.instance.loadGame();
        }
        else
        {
            Debug.LogError("Data Saver Manager not found");
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
