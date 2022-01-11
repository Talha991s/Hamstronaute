/*  Author: Salick Talhah
 *  Date Created: February 14, 2021
 *  Last Updated: February 14, 2021
 *  Description: This script is used for the Options control. - Volume - graphic and resolution.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixer sfxMixer;
    Resolution[] resolutions;

    public  TMPro.TMP_Dropdown resolutionDropdown;


    void Start()
    {
       // QualitySettings.SetQualityLevel(1);
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();  //from textmeshpro

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("volime", Mathf.Log10(volume)*20);
    }
    public void SetSfx(float volume)
    {
        Debug.Log(volume);
        sfxMixer.SetFloat("sfxvol", Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolute = resolutions[resolutionIndex];

        Screen.SetResolution(resolute.width, resolute.height, Screen.fullScreen);
    }
}
