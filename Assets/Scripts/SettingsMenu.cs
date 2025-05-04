using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    public TMP_Dropdown     resolutionDropdown;
    public Resolution[]     resolutions;

    public Toggle           resolutionToggle;
    public TMP_InputField   pathToJsonInput;

    private GameManager     gameManager;
    private CanvasScaler    canvasScaler;

    void Start()
    {
        gameManager             = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        canvasScaler            = GameObject.FindWithTag("MainCanvas").GetComponent<CanvasScaler>();

        resolutions             = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options    = new List<string>();

        int resIndex            = -1;

        int savedWidth          = PlayerPrefs.GetInt("ResolutionWidth", 0);
        int savedHeight         = PlayerPrefs.GetInt("ResolutionHeight", 0);
        float savedHz           = PlayerPrefs.GetFloat("ResolutionHz", 0);

        double highestHz        = -1;

        for(int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];

            string option           = resolution.width.ToString() + " x " + resolution.height.ToString() + " " + resolution.refreshRateRatio.value.ToString() + "Hz";
            options.Add(option);

            if(resolution.width == savedWidth && resolution.height == savedHeight && (float)resolution.refreshRateRatio.value == savedHz)
            {
                resIndex            = i;
            }

            // For default resolution (want half of screen)
            if(resolution.refreshRateRatio.value > highestHz)
            {
                highestHz           = resolution.refreshRateRatio.value;
            }
        }

        if(-1 == resIndex)
        {
            // Means no resolution was saved, so fetch a default one
            Resolution targetRes        = resolutions[resolutions.Length/2];

            for(int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution   = resolutions[i];

                if(resolution.width == targetRes.width && resolution.height == targetRes.height && resolution.refreshRateRatio.value == highestHz)
                {
                    // Found it
                    resIndex        = i;
                }
            }
        }

        if(-1 == resIndex)
        {
            // Means something has gone wrong
            Debug.LogError("Could not get a default resolution.");
            resIndex                = resolutions.Length / 2;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value    = resIndex;
        resolutionDropdown.RefreshShownValue();

        int isFullscreen            = PlayerPrefs.GetInt("IsFullscreen", -1);
        if(-1 != isFullscreen){
            resolutionToggle.isOn   = isFullscreen == 1 ? true : false;
            Screen.fullScreen       = resolutionToggle.isOn;
        }
        else{
            // Default
            resolutionToggle.isOn   = false;
            Screen.fullScreen       = resolutionToggle.isOn;
        }

        string jsonPath             = PlayerPrefs.GetString("JsonDir", "NEEDS_TO_BE_SET_IN_SETTINGS");
        if("NEEDS_TO_BE_SET_IN_SETTINGS" != jsonPath){
            pathToJsonInput.text    = jsonPath;
        }
        gameManager.SetPathToJsons(jsonPath);

        // Set canvasScaler's match mode depending on the aspect ratio
        // float screenAspect          = (float)resolutions[resIndex].width / (float)resolutions[resIndex].height;
        float screenAspect          = (float)Screen.width / (float)Screen.height;
        float referenceAspect       = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;

        // If the screen is wider than the reference, prioritize height; otherwise width (so, 16:10 will match width, 32:9 will match height)
        canvasScaler.matchWidthOrHeight = screenAspect >= referenceAspect ? 1 : 0;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        PlayerPrefs.SetFloat("ResolutionHz", (float)resolution.refreshRateRatio.value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen       = isFullscreen;
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
    }

    public void JsonInputEditFinished(string newJsonDir)
    {
        PlayerPrefs.SetString("JsonDir", newJsonDir);
        gameManager.SetPathToJsons(newJsonDir);
    }

    static public void SetDLCState(DLC dlc, bool state)
    {
        PlayerPrefs.SetInt(DLCToName[dlc], state ? 1 : 0);

        Cars.SetDLCState(dlc, state);
        Tracks.SetDLCState(dlc, state);
    }

    static public bool GetDLCState(DLC dlc)
    {
        return 1 == PlayerPrefs.GetInt(DLCToName[dlc], 0);
    }

    // When you add a new DLC, make sure to edit Track's "GetDLCTracks()" and Car's "InitializeCars()"
    // Remember to add SeriesBackground picture, Car picture, Track image and map picture
    [System.Serializable]
    public enum DLC
    {
        // Enum value CANNOT be 0 due to logic reasons in other scripts
        RacinUSAOne             = 1,
        SupercarsOne            = 2
    };

    static public Dictionary<DLC, string> DLCToName    = new Dictionary<DLC, string>()
    {
        {DLC.RacinUSAOne,               "Racin' USA 1"},
        {DLC.SupercarsOne,              "Supercars 1"}
    };

}