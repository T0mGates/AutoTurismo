using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class IOManager
{
    private static string PathToJsonDir;

    static public void SetJsonDir(string PathToJsonDirParam){
        PathToJsonDir = PathToJsonDirParam;
    }

    static public RaceResult ReadInNewResult(){

        string[] fileData = Directory.GetFiles(PathToJsonDir);

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