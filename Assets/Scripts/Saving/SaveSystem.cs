using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public static class SaveSystem
{
    public static int                       MAX_PROFILES    = 6;
    public static string                    DEFAULT_NAME    = "New Profile";
    public static   List<string>            profileNames;
    public static   List<int>               profileLevels;
    public static   List<int>               profileRenown;
    public static   List<int>               profileMoney;

    private const string                    FILE_PREFIX     = "ProfileData";

    static SaveSystem(){
        profileNames            = new List<string>();
        profileLevels           = new List<int>();
        profileRenown           = new List<int>();
        profileMoney            = new List<int>();

        // Check if any save files exist
        for(int i = 1; i <= MAX_PROFILES; i++){
            PlayerData data = LoadPlayerData(i);
            // If there is data, means a profile should occupy that slot
            if(null != data){
                profileNames.Add(data.driverName);
                profileLevels.Add(data.level);
                profileRenown.Add(data.renown);
                profileMoney.Add(data.money);
                continue;
            }
            profileNames.Add(DEFAULT_NAME);
            profileLevels.Add(0);
            profileRenown.Add(0);
            profileMoney.Add(0);
        }
    }

    public static void SaveProfile(Profile profile, int profileSlot){
        BinaryFormatter formatter   = new BinaryFormatter();
        string path                 = Path.Combine(Application.persistentDataPath, FILE_PREFIX + profileSlot.ToString() + ".json");

        Debug.Log("Saving file: " + path);

        FileStream stream           = new FileStream(path, FileMode.Create);

        PlayerData data             = new PlayerData(profile);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayerData(int profileSlot){
        string path                     = Path.Combine(Application.persistentDataPath, FILE_PREFIX + profileSlot.ToString() + ".json");

        if(File.Exists(path)){
            BinaryFormatter formatter   = new BinaryFormatter();
            FileStream      stream      = new FileStream(path, FileMode.Open);
            PlayerData      data        = null;

            try{
                data        = formatter.Deserialize(stream) as PlayerData;
            }
            catch{
                // File is corrupted or is empty
                DeleteProfile(profileSlot);
            }
            finally{
                stream.Close();
            }

            return data;
        }
        else{
            return null;
        }
    }

    // Profile slot can be 1 to MAX_PROFILES
    public static void DeleteProfile(int profileSlot){
        string path             = Path.Combine(Application.persistentDataPath, FILE_PREFIX + profileSlot.ToString() + ".json");

        if(File.Exists(path)){
            try{
                Debug.Log("Deleting profile data found at: " + path);
                File.Delete(path);
                // Sometimes when it loads a corrupt or empty file it has to delete the profile before these are initialized
                try{
                    profileNames[profileSlot-1]     = DEFAULT_NAME;
                    profileMoney[profileSlot-1]     = 0;
                    profileLevels[profileSlot-1]    = 0;
                    profileRenown[profileSlot-1]    = 0;
                }
                catch{}

            }
            catch(Exception e){
                Debug.LogError("Error occured when trying to delete data file: " + path + "\n" + e);
            }
        }
    }
}