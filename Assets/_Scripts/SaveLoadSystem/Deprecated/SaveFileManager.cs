/*  Author: Salick Talhah
 *  Date Created: January 28, 2021
 *  Last Updated: March 13, 2021
 *  Description: Manages and also retains information regarding the loaded save files and all available save files. 
 *  
 */

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private LoadGameFromSaveScr gameLoaderRef;
    [SerializeField] private Transform playerCharacterRef;
    [SerializeField] private Transform[] mobsPresentInScene;
    [SerializeField] private Transform[] platformsPresentInScene;
    [SerializeField] private Transform[] pickupsPresentInScene;

    [Header("Settings")]
    [SerializeField] private string savefileName = "Hamstronaut";       //This is the name of the save file. An indexing number will be appended to this name. This is different from the save file header seen in-game.

    [Header("Temp Debug Read-Only")] 
    [SerializeField] private Vector3 showPlayerLocation = Vector3.zero;
    [SerializeField] private Vector3 showPlayerOrientation = Vector3.zero;
    [SerializeField] private string showOpenSaveHeader = "";

    [SerializeField] private int showHealthAmount;
    [SerializeField] private int showLivesAmount;
    [SerializeField] private int showAmmoAmount;
    [SerializeField] private int showSeedsCollected;
    [SerializeField] private int showAliensKilled;
    [SerializeField] private int showCurrentLevel;                      //0 means not in a level
    [SerializeField] private int showLevelsUnlocked;

    [Header("Temp Settings")]                                           //TODO Remove. Most of these properties are just here for development use and testing; they have no use on final product.
    //[SerializeField] private string savefileHeader = ""; 
    //[SerializeField] private int livesAmount = 3;
    [SerializeField] private int livesAmount = 100;
    [SerializeField] private int ammoAmount = 100;
    [SerializeField] private int seedsCollected = 0;
    [SerializeField] private int aliensKilled = 0;
    [SerializeField] private int currentLevel = 0;                      //0 means not in a level
    [SerializeField] private int levelsUnlocked = 1;
    [SerializeField] private int selectedSaveSlot = 1; 

    [Header("Temp Debug Controls")]
    [SerializeField] private bool saveButton = false;                   //TODO Remove. This is only used during development to test savefile saving.
    [SerializeField] private bool loadButton = false;                   //TODO Remove. This is only used during development to test savefile loading.

    private string[] availableSaveFiles = new string[9];                //Note: This game will have a maximum 8 save slots hardcoded.
    //private SaveData loadedSaveData;                                    //Initial save data being used
    private string gameVersion = "0.3";

    private void Awake() 
    {
        if (LoadedSaveFile.loadLevelBasedOnSaveFile == true) 
        {
            LoadedSaveFile.loadLevelBasedOnSaveFile = false;
            LoadGameFromSelectedSaveFile();
        }
    }

    //TODO Remove. This is only used during development to test
    private void Update() 
    {
        //TODO Remove. This is only used during development to test savefile saving.
        if (saveButton) 
        {
            saveButton = false;
            SaveGame(selectedSaveSlot);
        }
        //TODO Remove. This is only used during development to test savefile loading.
        if (loadButton) 
        {
            loadButton = false;
            LoadGame(selectedSaveSlot); 
        }
    }

    public void QuickSave() 
    {
        SaveData newSaveData = new SaveData();
        newSaveData.savefileHeader = "(Quicksave) Marco    Lives: " + newSaveData.livesAmount + "; Ammo: " + newSaveData.ammoAmount + "; Seeds: " + newSaveData.seedsCollected + "; Levels Unlocked: " + newSaveData.levelsUnlocked;
        newSaveData.gameVersion = this.gameVersion;

        //newSaveData.playerCoord = new TransformLite(playerCharacterRef.position.x, playerCharacterRef.position.y, playerCharacterRef.position.z, playerCharacterRef.eulerAngles.x, playerCharacterRef.eulerAngles.y,playerCharacterRef.eulerAngles.z);

        newSaveData.livesAmount = this.livesAmount;
        newSaveData.ammoAmount = this.ammoAmount;
        newSaveData.seedsCollected = this.seedsCollected;
        newSaveData.aliensKilled = this.aliensKilled;
        newSaveData.currentLevel = this.currentLevel; //0 means not in a level
        newSaveData.levelsUnlocked = this.levelsUnlocked;

        SaveFileReaderWriter.WriteToSaveFile(Application.persistentDataPath + "/" + savefileName + "0.hamsave", newSaveData);
    }

    //Saves game data at given save slot index
    public void SaveGame(int _saveSlotIndex) 
    {
        if (_saveSlotIndex <= 0 || _saveSlotIndex > 8) { //This game will have a maximum 8 (0 to 8) save slots hardcoded. 0 slot should be reserved for quicksave
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 8.");
            return;
        }

        SaveData newSaveData = new SaveData();
        newSaveData.savefileHeader = "Marco    Health: " + newSaveData.healthAmount + "; Ammo: " + newSaveData.ammoAmount + "; Seeds: " + newSaveData.seedsCollected + "; Levels Unlocked: " + newSaveData.levelsUnlocked;
        newSaveData.gameVersion = this.gameVersion;

        //newSaveData.playerCoord = new TransformLite(playerCharacterRef.position.x, playerCharacterRef.position.y, playerCharacterRef.position.z, playerCharacterRef.eulerAngles.x, playerCharacterRef.eulerAngles.y,playerCharacterRef.eulerAngles.z);

        newSaveData.livesAmount = this.livesAmount;
        newSaveData.ammoAmount = this.ammoAmount;
        newSaveData.seedsCollected = this.seedsCollected;
        newSaveData.aliensKilled = this.aliensKilled;
        newSaveData.currentLevel = this.currentLevel; //0 means not in a level
        newSaveData.levelsUnlocked = this.levelsUnlocked;

        SaveFileReaderWriter.WriteToSaveFile(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".hamsave", newSaveData);
    }

    //Saves given game data at given save slot index
    public void SaveGame(int _saveSlotIndex, SaveData _saveData) 
    {
        //This game will have a maximum 8 save slots hardcoded.
        if (_saveSlotIndex <= 0 || _saveSlotIndex > 8) 
        { 
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 8.");
            return;
        }

        _saveData.gameVersion = this.gameVersion;
        _saveData.savefileHeader = "Marco    Lives: " + _saveData.livesAmount + "; Ammo: " + _saveData.ammoAmount + "; Seeds: " + _saveData.seedsCollected + "; Levels Unlocked: " + _saveData.levelsUnlocked;

        SaveFileReaderWriter.WriteToSaveFile(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".hamsave", _saveData);
    }

    //Loads save file data at given save slot index
    public SaveData LoadGame(int _saveSlotIndex) 
    {
        //This game will have a maximum 8 save slots hardcoded.
        if (_saveSlotIndex < 0 || _saveSlotIndex > 8) 
        { 
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 0 to 8.");
            return null;
        }

        if (!File.Exists(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".hamsave")) 
        {
            Debug.LogError("[Error] File does not exist; Cannot load a save file that does not exist.");
            return null;
        }

        SaveData readSaveData = SaveFileReaderWriter.ReadFromSaveFile(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".hamsave");

        if (this.gameVersion != readSaveData.gameVersion) 
        {
            Debug.LogWarning("[Warning] Cannot load save file; incompatible version. ");
            return null;
        }

        LoadedSaveFile.loadedSaveData = readSaveData;

        //TODO Temp: Display in inspector
        this.showOpenSaveHeader = LoadedSaveFile.loadedSaveData.savefileHeader;
        //this.showPlayerLocation = new Vector3(LoadedSaveFile.loadedSaveData.playerCoord.positionX, LoadedSaveFile.loadedSaveData.playerCoord.positionY, LoadedSaveFile.loadedSaveData.playerCoord.positionZ);
        //this.showPlayerOrientation = new Vector3(LoadedSaveFile.loadedSaveData.playerCoord.orientationX, LoadedSaveFile.loadedSaveData.playerCoord.orientationY, LoadedSaveFile.loadedSaveData.playerCoord.orientationZ);
        this.showHealthAmount = LoadedSaveFile.loadedSaveData.healthAmount;
        this.showLivesAmount = LoadedSaveFile.loadedSaveData.livesAmount;
        this.showAmmoAmount = LoadedSaveFile.loadedSaveData.ammoAmount;
        this.showSeedsCollected = LoadedSaveFile.loadedSaveData.seedsCollected;
        this.showAliensKilled = LoadedSaveFile.loadedSaveData.aliensKilled;
        this.showCurrentLevel = LoadedSaveFile.loadedSaveData.currentLevel; //0 means not in a level
        this.showLevelsUnlocked = LoadedSaveFile.loadedSaveData.levelsUnlocked;

        return LoadedSaveFile.loadedSaveData;
    }

    //TODO Untested
    //Returns the loaded SaveData File.
    public SaveData GetSaveData() 
    {
        if (LoadedSaveFile.loadedSaveData == null) 
        {
            Debug.LogError("[Error] No save data loaded yet; returning SaveData with default properties.");
            return new SaveData();
        }

        return LoadedSaveFile.loadedSaveData;
    }

    //TODO Untested
    //Returns the header of a specified save slot. 
    //The header contains information about the save file. If the save slot is empty it will return "Empty Save Slot".
    //Usage: Save Slots should be represented by buttons that the user can click and the button text should be a save slot header to display its information. 
    //Pressing the button should call SaveFileManager.LoadGame(saveSlotIndex) where each button represents different save slots.
    public string GetSaveSlotHeader(int _saveSlotIndex) 
    {
        if (_saveSlotIndex < 0 || _saveSlotIndex > 8) 
        { //This game will have a maximum 8 save slots hardcoded.
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 8.");
            return "[Error] Invalid Save slot index!";
        }

        if (availableSaveFiles == null) 
        {
            availableSaveFiles = SaveFileReaderWriter.CheckAvailableSaveFiles(Application.persistentDataPath, savefileName);
        }

        if (availableSaveFiles != null) 
        {
            if (availableSaveFiles.Length <= 0) 
            {
                Debug.LogError("[Error] availableSaveFiles array not initialized!");
                return "[Error] availableSaveFiles array not initialized!";
            }
        }
        else 
        {
            Debug.LogError("[Error] availableSaveFiles array not initialized!");
            return "[Error] availableSaveFiles array not initialized!";
        }

        return availableSaveFiles[_saveSlotIndex - 1];
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

        StartCoroutine(LoadGameStateRoutine());

    }

    IEnumerator LoadGameStateRoutine() 
    {
        Time.timeScale = 0;

        //Set up level if applicable
        //Load Character Position
        if (playerCharacterRef) {
            //playerCharacterRef.transform.position = new Vector3(LoadedSaveFile.loadedSaveData.playerCoord.positionX, LoadedSaveFile.loadedSaveData.playerCoord.positionY, LoadedSaveFile.loadedSaveData.playerCoord.positionZ);
            //playerCharacterRef.transform.eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.playerCoord.orientationX, LoadedSaveFile.loadedSaveData.playerCoord.orientationY, LoadedSaveFile.loadedSaveData.playerCoord.orientationZ);
        }
        else {
            Debug.LogError("[Error] Reference to playerCharacterRef missing!");
        }

        //Load Mob positions
        //Case: There are more mobs present than in savefile




        //if (LoadedSaveFile.loadedSaveData.mobCoord.Length > 0) {

        //    if ()

        //    //if (mobRefs.Length == LoadedSaveFile.loadedSaveData.mobCoord.Length) {
        //    //    for (int i = 0; i < mobRefs.Length; i++) {
        //    //        mobRefs[i].position = new Vector3(LoadedSaveFile.loadedSaveData.mobCoord[i].positionX, LoadedSaveFile.loadedSaveData.mobCoord[i].positionY,LoadedSaveFile.loadedSaveData.mobCoord[i].positionZ);
        //    //        mobRefs[i].eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.mobCoord[i].orientationX, LoadedSaveFile.loadedSaveData.mobCoord[i].orientationY,LoadedSaveFile.loadedSaveData.mobCoord[i].orientationZ);
        //    //    }
        //    //}
        //    //else {
        //    //    Debug.LogError("[Error] Mobs reference count present in level do not match amount in save file.");
        //    //}
        //}
        //else {
        //    Debug.LogWarning("[Warning] There are no mobs tracked in save file.");
        //}

        yield return new WaitForSeconds(3.0f);
        Time.timeScale = 1;
    }
}
