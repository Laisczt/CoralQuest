using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class dataSaverManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] public string fileName;

    public gameData game_data;

    public List<IDataSaver> dataSaverObjects;
    public fileDataHandler dataHandler;

    public static dataSaverManager instance{ get; private set; }

    private void Awake(){
        if (instance!= null) Debug.LogError ("Data saver duplicated :(");

        instance = this; //instance used == the one the onject is using 
        //                aka this one
    }

   public void newGame()
   {
     game_data = new gameData();
   }


    public void  loadGame()
    {
        game_data = dataHandler.load();
        if (game_data == null){
            Debug.Log("Game not found. Creatung Gae    m");
            newGame();
        }

        foreach(IDataSaver dataSaverObj in dataSaverObjects)
        {
            dataSaverObj.loadData(game_data);
            Debug.Log(dataSaverObj);
        }
    }

    public void saveGame()
    {
        foreach(IDataSaver dataSaverObj in dataSaverObjects)
        {
            dataSaverObj.saveData(ref game_data);
        }

        dataHandler.save(game_data);
    }
    

    
    void OnApplicationQuit()
    {
        saveGame();
    }

    public List<IDataSaver> FindAllDataSaverObjects(){
        IEnumerable<IDataSaver> dataSaverObjects = FindObjectsOfType<MonoBehaviour>()
        .OfType<IDataSaver>();

        return new List<IDataSaver>(dataSaverObjects);
    }
   
}  
