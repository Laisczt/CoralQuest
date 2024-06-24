using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; private set; }
    private Scene _UIScene;
    private GameObject GameOverMenu;
    private UIControlButton ReviveButton;
    private PlayerControl playerC;

    private bool usingMobileControls;

    [HideInInspector]public bool isGameOver;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerC = PlayerControl.Instance;
        StartCoroutine(LoadUI());
        
        if (Application.isMobilePlatform || usingMobileControls)    // Ativa os controles de celular quando necessário
        {
            UseMobileControls(true);
        }

        // Carrega os dados em cena do savefile WIP
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
        
        Time.timeScale = 1f;
    }

    private void Update()
    {
        /* TODO
            MAKE THIS OPEN PAUSE MENU INSTEAD, ADD A MAIN MENU BUTTON THERE
        */
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }

        if(!isGameOver) return;

        if(ReviveButton.GetButtonDown())
        {
            Restart();
        }
    }

    private bool UILinked = false;  // true quando os controles mobile já foram conectados ao script do jogador

    [ContextMenu("Toggle Mobile Controls")] // Ativa ou desativa os controles de celular
    public void ToggleMobileControls()
    {
        UseMobileControls(!usingMobileControls);
    }
    private void UseMobileControls(bool doUse)  // Conecta os controles de celular no canvas ao script do jogador
    {
        usingMobileControls = doUse;
        playerC.UsingMobileControls = doUse;

        var controls = GameObject.FindGameObjectsWithTag("Mobile UI");
        foreach(GameObject element in controls)
        {
            element.transform.GetChild(0).gameObject.SetActive(doUse);
        }
        if (!UILinked)
        {
            playerC.SetMobileControls(controls);
            UILinked = true;
        }
        
    }
    public void GameOver()
    {
        GameOverMenu.SetActive(true);
        isGameOver = true;
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator LoadUI()
    {

        var isInv = playerC.PlayerHealth.DEBUG_INVINCIBLE;

        playerC.PlayerHealth.DEBUG_INVINCIBLE = true;

        _UIScene = SceneManager.LoadScene("UI", new LoadSceneParameters(LoadSceneMode.Additive));

        while(!_UIScene.isLoaded)
        {
            yield return null;
        }

        GameOverMenu = _UIScene.GetRootGameObjects()[0].transform.GetChild(0).gameObject;

        if(GameOverMenu == null)
        {
            Debug.LogError("Game Over Overlay not found, did you rename the UI Scene / root object?");
        }

        ReviveButton = GameOverMenu.transform.GetChild(0).GetComponent<UIControlButton>();
        if(ReviveButton == null)
        {
            Debug.LogError("Revive Button not found, did you rename it?");
        }


        if (!isInv) playerC.PlayerHealth.DEBUG_INVINCIBLE = false;
    }
}
