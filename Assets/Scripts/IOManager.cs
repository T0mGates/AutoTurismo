using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class IOManager
{
    private static string PathToJsonDir;

    static public void SetJsonDir(string PathToJsonDirParam){
        PathToJsonDir = PathToJsonDirParam;
    }

    static public string GetJsonDir(){
        return PathToJsonDir;
    }

    static public void ClearJsonDir(){
        string[] fileData = new string[0];

        try{
            fileData = Directory.GetFiles(PathToJsonDir);
        }
        catch{
            return;
        }

        // Go through the files one by one
        foreach (string filePath in fileData)
        {
            if(filePath.Contains(".json")){
              // Delete the json file
              Debug.Log("Deleting file found at: " + filePath);
              File.Delete(filePath);
            }
        }
    }

    static public RaceResult ReadInNewResult(){
        string[] fileData = new string[0];

        try{
            fileData = Directory.GetFiles(PathToJsonDir);
        }
        catch{
            return null;
        }

        // Displaying the file paths one by one
        foreach (string filePath in fileData)
        {
            if(filePath.Contains(".json")){
              RaceResult result = JsonConvert.DeserializeObject<RaceResult>(File.ReadAllText(filePath));
              // Delete the json file since this function is 'while trued' until it does not find a json file
              Debug.Log("Deleting file found at: " + filePath);
              File.Delete(filePath);
              return result;
            }
        }

        return null;
    }
}