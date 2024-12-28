using System.IO;
using Newtonsoft.Json;

public static class IOManager
{
    private static string PathToJsonDir;

    static public void SetJsonDir(string PathToJsonDirParam){
        PathToJsonDir = PathToJsonDirParam;
    } 

    static public RaceResult ReadInNewResult(){

        string[] filedata = Directory.GetFiles(PathToJsonDir); 
      
        // Displaying the file names one by one 
        foreach (string i in filedata) 
        { 
            if(i.Contains(".json")){
              RaceResult result = JsonConvert.DeserializeObject<RaceResult>(File.ReadAllText(i));
              return result;
            }
        } 

        return null;
    }
}