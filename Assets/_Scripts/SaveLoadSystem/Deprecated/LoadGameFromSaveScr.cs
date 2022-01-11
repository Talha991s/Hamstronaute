/*  Author: Salick Talhah
 *  Date Created: March 12, 2021
 *  Last Updated: March 13, 2021
 *  Description: 
 */

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameFromSaveScr : MonoBehaviour
{



    private void Awake() 
    {
        if (LoadedSaveFile.loadLevelBasedOnSaveFile == true) 
        {
            LoadedSaveFile.loadLevelBasedOnSaveFile = false;
            LoadGameFromSelectedSaveFile();
        }
    }

    public void LoadGameFromSelectedSaveFile() 
    {

        //Check Loaded Save File
        if (LoadedSaveFile.loadedSaveData == null)
        {
            Debug.LogError("[Error] Could not load save file.");
            return;
        }

        //Check if current scene matches save file level
        if (LoadedSaveFile.loadedSaveData.currentLevel == 1) {
            if (SceneManager.GetActiveScene().buildIndex != 1) {
                Debug.LogError("[Error] Save file level data mismatch.");
                return;
            }
        }

        //Set up level if applicable

    }

}
