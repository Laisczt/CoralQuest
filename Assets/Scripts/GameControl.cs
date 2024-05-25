using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; private set; }
    [SerializeField] GameObject Player;
    private PlayerControl playerC;
    [SerializeField] GameObject GameOverMenu;
    private bool usingMobileControls;

    void Start()
    {
        Instance = this;

        playerC = Player.GetComponent<PlayerControl>();

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
    public void EnableGameOverScreen()
    {
        GameOverMenu.SetActive(true);
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
