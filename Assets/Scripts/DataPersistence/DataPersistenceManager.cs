using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Unity.VisualScripting;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    public static int                       MAX_PROFILES    = 6;
    public static string                    DEFAULT_NAME    = "New Profile";

    public static   DataPersistenceManager  instance {get; private set;}
    public static   List<string>            profileNames;

    private         GameData                gameData;
    private         List<IDataPersistence>  dataPersistenceObjects;

    private         string                  fileName;
    private         FileDataHandler         dataHandler;

    private const string                    FILE_PREFIX     = "ProfileData";

    private void Start(){
        profileNames            = new List<string>();
        // Check if any save files exist
        for(int i = 1; i <= MAX_PROFILES; i++){
            GameData data = GetLoadedData(FILE_PREFIX + i.ToString() + ".json");
            // If there is data, means a profile should occupy that slot
            if(null != data){
                profileNames.Add(data.driverName);
                continue;
            }
            profileNames.Add(DEFAULT_NAME);
        }
    }

    private void Awake(){
        if(instance != null){
            Debug.LogError("Found more than one Data Persistence Manager in the scene!");
        }
        instance = this;
    }

    private void OnApplicationQuit(){
        SaveGame();
    }

    // 1 to MAX_PROFILES for profile slot
    // Returns whether a new game was created or not (so, if false, means a game was loaded)
    public bool LoadProfile(int profileSlot = -1){
        fileName                = FILE_PREFIX + profileSlot.ToString() + ".json";
        Debug.Log("Loading profile with filename: " + fileName);
        dataHandler             = new FileDataHandler(Application.persistentDataPath, fileName);
        return LoadGame();
    }

    public void NewGame(){
        gameData                = new GameData();
    }

    // Returns whether a new game was created or not (if false, means a game was loaded)
    public bool LoadGame(){
        Debug.Log("Loaded game!");

        bool newGame            = false;

        gameData                = dataHandler.Load();

        if(null == gameData){
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
            newGame             = true;
        }

        // Load data for all objects that require loaded data
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        Debug.Log("Found " + dataPersistenceObjects.Count.ToString() + " data persistence objects.");

        foreach(IDataPersistence obj in dataPersistenceObjects){
            obj.LoadData(gameData);
        }

        return newGame;
    }

    public void SaveGame(){
        Debug.Log("Saved game!");

        // Save data for all objects that save data
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects){
            dataPersistenceObj.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    public void AddDataPersistenceObject(IDataPersistence obj){
        dataPersistenceObjects.Add(obj);
        obj.LoadData(gameData);
    }

    // Profile slot can be 1 to MAX_PROFILES
    public void DeleteGame(int profileSlot){
        fileName                = FILE_PREFIX + profileSlot.ToString() + ".json";
        dataHandler             = new FileDataHandler(Application.persistentDataPath, fileName);
        dataHandler.Delete();
    }

    private GameData GetLoadedData(string fileName){
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        return dataHandler.Load();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
