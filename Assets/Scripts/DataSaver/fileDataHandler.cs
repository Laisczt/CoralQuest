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

   public bool TryLoad<v>(out v loadedData)
   {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        loadedData = default;

        if (File.Exists(fullPath)){
            try{
                string dataToLoad = "";

                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using(StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<v>(dataToLoad);
                return true;
            }
            catch(Exception e){
                Debug.LogError("Erro ao carregar dados salvos: " + fullPath + "\n" + e);
                return false;
            }

            
        }
        return false;
   }

    public void save(object data, bool playAnimation = true)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        var success = false;
        try{
            Directory.CreateDirectory(dataDirPath);

            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
                using(StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
            success = true;
        }
        catch(Exception e)
        {
            Debug.LogError("Erro ao tentar salvar dados: " + fullPath + "\n" + e);
        }
        if(success && playAnimation) dataSaverManager.instance.PlayAnimation();
    }
}
