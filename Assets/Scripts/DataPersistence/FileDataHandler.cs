using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPathParam, string dataFileNameParam){
        dataDirPath     = dataDirPathParam;
        dataFileName    = dataFileNameParam;
    }

    public GameData Load(){
        string      fullPath        = Path.Combine(dataDirPath, dataFileName);
        GameData    loadedData      = null;

        if(File.Exists(fullPath)){
            try{
                // Load serialized data from the file
                string dataToLoad   = "";

                using       (FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using   (StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // Deserialize the data from JSON back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e){
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data){
        //data.GetReadyToSave();

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try{
            // Create dir the file will be writen to
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object to JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write the serialized data to the file
            using       (FileStream stream      = new FileStream(fullPath, FileMode.Create)){
                using   (StreamWriter writer    = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e){
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public void Delete(){
        string      fullPath        = Path.Combine(dataDirPath, dataFileName);

        if(File.Exists(fullPath)){
            try{
                Debug.Log("Deleting profile data found at: " + fullPath);
                FileUtil.DeleteFileOrDirectory(fullPath);
            }
            catch(Exception e){
                Debug.LogError("Error occured when trying to delete data file: " + fullPath + "\n" + e);
            }
        }
    }
}
