using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public TMP_Dropdown     resolutionDropdown;
    public Resolution[]     resolutions;

    public Toggle           resolutionToggle;
    public TMP_InputField   pathToJsonInput;

    private GameManager     gameManager;

    void Start(){
        gameManager             = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        resolutions             = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options    = new List<string>();

        int currentResIndex     = 0;
        int overrideResIndex    = -1;

        int savedWidth          = PlayerPrefs.GetInt("ResolutionWidth", 0);
        int savedHeight         = PlayerPrefs.GetInt("ResolutionHeight", 0);
        float savedHz           = PlayerPrefs.GetFloat("ResolutionHz", 0);

        for(int i = 0; i < resolutions.Length; i++){
            Resolution resolution = resolutions[i];

            string option       = resolution.width.ToString() + " x " + resolution.height.ToString() + " " + resolution.refreshRateRatio.value.ToString() + "Hz";
            options.Add(option);

            if(resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height){
                currentResIndex = i;
            }

            if(resolution.width == savedWidth && resolution.height == savedHeight && (float)resolution.refreshRateRatio.value == savedHz){
                overrideResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = overrideResIndex == -1 ? currentResIndex : overrideResIndex;
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
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        PlayerPrefs.SetFloat("ResolutionHz", (float)resolution.refreshRateRatio.value);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen       = isFullscreen;
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
    }

    public void JsonInputEditFinished(string newJsonDir){
        PlayerPrefs.SetString("JsonDir", newJsonDir);
        gameManager.SetPathToJsons(newJsonDir);
    }
}