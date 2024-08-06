using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class dataSaverManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] public string fileName;

    [HideInInspector]public gameData game_data;
    private gameData default_gd;
    [HideInInspector]public timerData timer_offsets;
    private timerData default_to;

    public List<IDataSaver> dataSaverObjects;
    public fileDataHandler dataHandler;
    public fileDataHandler timerAuxFile;

    public int SaveCooldown = 60;
    private int rSaveCooldown;
    [HideInInspector] public GameObject SaveBadge;

    public static dataSaverManager instance{ get; private set; }    // Instancia Singleton

    private void Awake(){
        if (instance!= null)
        { 
            Debug.LogError ("Data saver duplicated");
            gameObject.SetActive(false);
        }

        instance = this; 
    }

    private void Start()
    {
        rSaveCooldown = 5;
        default_gd = new gameData();
        default_to = new timerData();
    }
    private void FixedUpdate()
    {
        if(rSaveCooldown > 0) rSaveCooldown--;
    }

    public void newGame()
    {
        Debug.Log("Iniciando novo jogo");
        game_data = new gameData();
        timer_offsets = new timerData();
    }


    public void  loadGame()
    {
        Debug.Log("Carregando Jogo");
        var hasLoaded = dataHandler.TryLoad(out game_data);
        if (!hasLoaded){
            Debug.Log("Save não encontrado, um jogo novo será iniciado");
            newGame();
        }
        loadPartialTimers();
        foreach(IDataSaver dataSaverObj in dataSaverObjects)
        {
            dataSaverObj.loadData(game_data);
        }
    } 

    public void saveGame(bool force = false)
    {
        if(!force && rSaveCooldown > 0) return;
        rSaveCooldown = SaveCooldown;

        Debug.Log("Salvando jogo...");
        foreach(IDataSaver dataSaverObj in dataSaverObjects)
        {
            dataSaverObj.saveData(ref game_data);
        }

        dataHandler.save(game_data);
    }

    public void savePartialTimers()
    {
        timerAuxFile.save(timer_offsets, false);
    }
    public timerData loadPartialTimers()
    {
        if(timerAuxFile == null) return null;
        var hasLoaded = timerAuxFile.TryLoad(out timer_offsets);
        if(!hasLoaded)
        {
            timer_offsets = new timerData();
        }
        return timer_offsets;
    }


    public void PlayAnimation()
    {
        if(SaveBadge == null) return;
        StartCoroutine(_playAnimation());
    }

    IEnumerator _playAnimation()
    {
        SaveBadge.SetActive(true);
        var i = 30;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        SaveBadge.SetActive(false);
    }

    public void SetReachedLevel2()
    {
        game_data.ReachedLevel2 = true;
    }
    public void MenuResetLevel1()
    {
        game_data.KilledTentacles = default_gd.KilledTentacles;
        game_data.Level1SavePos = default_gd.Level1SavePos;
        saveGame();
        timer_offsets.elapsedTimeLevel1 = default_to.elapsedTimeLevel1;
        savePartialTimers();
    }
    public void MenuResetLevel2()
    {
        game_data.Level2SavePos = default_gd.Level2SavePos;
        saveGame();
        timer_offsets.elapsedTimeLevel2 = default_to.elapsedTimeLevel2;
        savePartialTimers();
    }

    
    
    public List<IDataSaver> FindAllDataSaverObjects(){
        IEnumerable<IDataSaver> dataSaverObjects = FindObjectsOfType<MonoBehaviour>()
        .OfType<IDataSaver>();

        return new List<IDataSaver>(dataSaverObjects);
    }
}  
