using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour, IDataSaver
{
    /*

        Controle de varias funcoes do jogo

    */
    public static GameControl Instance { get; private set; }    // Instancia singleton
    private Scene _UIScene;     // Cena contendo o HUD
    private GameObject GameOverMenu;    // Menu de gameover
    private UIControlButton ReviveButton;   // Botao de reviver
    private PlayerControl playerC;      // Player

    timerData timeroffsets;
    TimeSpan time;
    bool newBestTime;
    bool timerOn;
    bool levelFinished;

    private bool usingMobileControls;   // verdadeiro caso a HUD Mobile estiver ativa

    [HideInInspector]public bool isGameOver;    // verdadeiro caso o player tenha morrido
    [HideInInspector]public string currentLevel;    // O nome do nivel(cena) atual

    [HideInInspector]public List<string> KilledTentaclesNames;

    void Awake()
    {
        Instance = this;
        currentLevel = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        playerC = PlayerControl.Instance;

        StartCoroutine(LoadUI());   // Carrega o HUD
        
        if (Application.isMobilePlatform || usingMobileControls)    // Ativa os controles de celular quando necessário
        {
            UseMobileControls(true);
        }

        // Carrega os dados em cena do savefile
        if(dataSaverManager.instance != null)
        {
            dataSaverManager.instance.dataHandler = new fileDataHandler(Application.persistentDataPath, dataSaverManager.instance.fileName);
            dataSaverManager.instance.timerAuxFile = new fileDataHandler(Application.persistentDataPath, "TimerAux");
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
        if(isGameOver && ReviveButton.GetButtonDown())
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
        Timer.instance.pause();
        GameOverMenu.SetActive(true);
        isGameOver = true;
    }

    public void Restart(){
        Timer.instance.pauseAndLock();
        savePartialTimer();
        SceneManager.LoadScene(currentLevel);
    }

    public void MainMenu(){
        SceneManager.LoadScene("Main Menu");
    }

    private void savePartialTimer()    // Salva o timer parcial 
    {
        if(levelFinished) time = new TimeSpan();
        else time = Timer.instance.getTimeElapsed();

        if(currentLevel == "Shallows") dataSaverManager.instance.timer_offsets.elapsedTimeLevel1 = time.ToString("mm':'ss'.'fff");
        else if(currentLevel == "Depths") dataSaverManager.instance.timer_offsets.elapsedTimeLevel2 = time.ToString("mm':'ss'.'fff");
        dataSaverManager.instance.savePartialTimers();
    }

    public void ChangeLevel(string sceneName)   // muda de cena (ao completar o nível)
    {
        if(levelFinished) return;
        levelFinished = true;

        time = Timer.instance.getTimeElapsed();

        PlayerControl.Instance.LastSavePos = new Vector3(0,0, -100);

        TimeSpan previoustime;
        if(currentLevel == "Shallows")
        {
            dataSaverManager.instance.SetReachedLevel2();
            KilledTentaclesNames = new List<string>();

            previoustime = TimeSpan.ParseExact(dataSaverManager.instance.game_data.bestTimeLevel1, "mm\\:ss\\.fff", CultureInfo.InvariantCulture);
            Debug.Log("Time - " + time);
            Debug.Log("Previous time - " + previoustime);
            if(time < previoustime || previoustime == TimeSpan.Zero)
            {
                newBestTime = true;
            }
            Debug.Log("Improvement? " + newBestTime);
            
        }
        else if(currentLevel == "Depths")
        {
            previoustime = TimeSpan.ParseExact(dataSaverManager.instance.game_data.bestTimeLevel2, "mm\\:ss\\.fff", CultureInfo.InvariantCulture);
            if(time < previoustime || previoustime == TimeSpan.Zero)
            {
                newBestTime = true;
            }
        }
        dataSaverManager.instance.saveGame(true);
        StartCoroutine(stallChangeScene(sceneName));
    }

    IEnumerator stallChangeScene(string sceneName)
    {
        var i = 1.5f;

        while(i > 0)
        {
            Debug.Log("Mudando de cena em " + i + "...");
            yield return new WaitForSecondsRealtime(0.25f);
            i-= 0.25f;
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadUI()    // Carrega a a HUD em jogo
    {

        var isInv = playerC.PlayerHealth.DEBUG_INVINCIBLE;

        playerC.PlayerHealth.DEBUG_INVINCIBLE = true;   // Mantém o player invencível enquanto a UI carrega

        _UIScene = SceneManager.LoadScene("UI", new LoadSceneParameters(LoadSceneMode.Additive));   // Carrega a UI por cima da cena inicial

        while(!_UIScene.isLoaded)   // Espera até a UI estar carregada
        {
            yield return null;
        }

        GameOverMenu = _UIScene.GetRootGameObjects()[0].transform.Find("Overlays").GetChild(1).gameObject;

        if(GameOverMenu == null)
        {
            Debug.LogError("Game Over Overlay not found, did you rename or reorder it?");
        }

        ReviveButton = GameOverMenu.transform.GetChild(0).GetComponent<UIControlButton>();
        if(ReviveButton == null)
        {
            Debug.LogError("Revive Button not found, did you rename it?");
        }

        var savebadge = _UIScene.GetRootGameObjects()[0].transform.Find("Overlays").transform.GetChild(2).gameObject;
        if(savebadge == null)
        {
            Debug.Log("Save Badge not found, saving animation will not be played in this scene");
        }
        else 
        {
            dataSaverManager.instance.SaveBadge = savebadge;
        }

        timerbutton = _UIScene.GetRootGameObjects()[0].transform.Find("Overlays").GetChild(0).GetChild(0).GetChild(0).GetComponent<ToggleTimerButton>();
        if(timerOn) timerbutton.On();


        if(currentLevel == "Shallows") 
            Timer.instance.SetOffset(TimeSpan.ParseExact(timeroffsets.elapsedTimeLevel1, "mm\\:ss\\.fff", CultureInfo.InvariantCulture));
        else if(currentLevel == "Depths") 
            Timer.instance.SetOffset(TimeSpan.ParseExact(timeroffsets.elapsedTimeLevel2, "mm\\:ss\\.fff", CultureInfo.InvariantCulture));

        if (!isInv) playerC.PlayerHealth.DEBUG_INVINCIBLE = false;

        
    }
    ToggleTimerButton timerbutton;
    public void loadData(gameData data)
    {
        if(currentLevel == "Shallows")
        {
            KilledTentaclesNames = data.KilledTentacles;
            StartCoroutine(killTents(KilledTentaclesNames));
            
        }
        timeroffsets = dataSaverManager.instance.timer_offsets;
        timerOn = data.ShowTimer;
    }
    IEnumerator killTents(List<string> tents)
    {
        yield return new WaitForSecondsRealtime(0.5f);  // Um pequeno delay é necessario para que os tentáculos sejam instanciados corretamente
        foreach(var t in tents)
        {
            Destroy(GameObject.Find(t));
        }
    }

    public void saveData(ref gameData data)
    { 
        if(currentLevel == "Shallows")
        {
            data.KilledTentacles = KilledTentaclesNames;
            if(newBestTime) data.bestTimeLevel1 = time.ToString("mm':'ss'.'fff");
        }
        else if(currentLevel == "Depths")
        {
            if(newBestTime) data.bestTimeLevel2 = time.ToString("mm':'ss'.'fff");
        }
        data.ShowTimer = timerbutton.IsOn();

        savePartialTimer();
    }
}
