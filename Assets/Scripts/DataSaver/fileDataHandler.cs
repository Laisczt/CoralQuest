using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class fileDataHandler 
{
   private string dataDirPath = "", dataFileName = "";


    public fileDataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

   public bool TryLoad(out gameData loadedData)
   {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        loadedData = null;

        if (File.Exists(fullPath)){
            try{
                string dataToLoad = "";

                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using(StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<gameData>(dataToLoad);
                return true;
            }
            catch(Exception e){
                Debug.LogError("Error loading save file: " + fullPath + "\n" + e);
                return false;
            }

            
        }
        return false;
   }

    public void save(gameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try{
            Directory.CreateDirectory(dataDirPath);

            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
                using(StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error occurred when trying to save: " + fullPath + "\n" + e);
        }
    }
}
