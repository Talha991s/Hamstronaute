/*  Author: Salick Talhah
 *  Date Created: March 13, 2021
 *  Last Updated: March 29, 2021
 *  Description: 
 */

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveGameScr : MonoBehaviour{

    [Header("References")]
    [SerializeField] private Transform playerCharacterRef;
    [SerializeField] private Transform cameraRef;
    [SerializeField] private QuestGiver questGiverRef;
    [SerializeField] private Gold goldGoalRef;
    [SerializeField] private PlayerHealth playerHealthRef;
    [SerializeField] private PlayerInventory inventoryRef;
    [SerializeField] private RespawnLogic checkpointRef;
    [SerializeField] private Text[] saveSlots = new Text[4];
    [SerializeField] private Transform[] pickupsInLevel;
    [SerializeField] private Transform[] platformsInLevel;
    [SerializeField] private Transform[] mobsInLevel;

    [Header("Settings")]
    [SerializeField] private string savefileName = "Hamstronaut";       //This is the name of the save file. An indexing number will be appended to this name. This is different from the save file header seen in-game.
    [SerializeField] private int levelNumber = 1;                       //TODO: Verify it is the same level
    private string[] saveFileDisplayHeaders;                            //This game will have a maximum 4 save slots hardcoded.
    private string gameVersion = "0.4";

    private void Awake() 
    {
        if (LoadedSaveFile.loadLevelBasedOnSaveFile == true) 
        {
            LoadedSaveFile.loadLevelBasedOnSaveFile = false;
            LoadGameFromSelectedSaveFile();
        }
        else {
            if (playerHealthRef) {
                playerHealthRef.SetHealth(playerHealthRef.maxhealth);
            }
        }
    }

    private void Start() 
    {
        Time.timeScale = 1;
        SetUpSaveSlotHeaders();
    }

    private string GetSaveSlotHeader(int _saveSlotIndex) 
    {
        if (_saveSlotIndex < 1 || _saveSlotIndex > 4) 
        { //This game will have a maximum 4 save slots hardcoded.
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 4.");
            return "[Error] Invalid Save slot index!";
        }

        if (saveFileDisplayHeaders == null) 
        {
            saveFileDisplayHeaders = SaveFileReaderWriter.CheckAvailableSaveFiles(Application.persistentDataPath, savefileName);
        }

        if (saveFileDisplayHeaders != null) 
        {
            if (saveFileDisplayHeaders.Length <= 0) 
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

        return saveFileDisplayHeaders[_saveSlotIndex - 1];
    }

    private void SetUpSaveSlotHeaders() 
    {
        saveSlots[0].text = GetSaveSlotHeader(1);
        saveSlots[1].text = GetSaveSlotHeader(2);
        saveSlots[2].text = GetSaveSlotHeader(3);
        saveSlots[3].text = GetSaveSlotHeader(4);
    }

    private void LoadGameFromSelectedSaveFile() 
    {

        //Check Loaded Save File
        if (LoadedSaveFile.loadedSaveData == null)
        {
            Debug.LogError("[Error] Could not load save file.");
            return;
        }

        //Check if current scene matches save file level
        if (LoadedSaveFile.loadedSaveData.currentLevel == 1) 
        {
            if (SceneManager.GetActiveScene().buildIndex != 1) 
            {
                Debug.LogError("[Error] Save file level data mismatch.");
                return;
            }
        }

        //Set up level if applicable
        StartCoroutine(LoadGameStateRoutine());

    }

    //Saves game data at given save slot index
    public void SaveGame(int _saveSlotIndex) 
    {
        if (_saveSlotIndex <= 0 || _saveSlotIndex > 4) 
        { //This game will have a maximum 4 (1 to 4) save slots hardcoded. 
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 4.");
            return;
        }

        SaveData newSaveData = new SaveData();
        newSaveData.gameVersion = this.gameVersion;

        //Save Player location
        if (playerCharacterRef) 
        {
            newSaveData.playerCoord = new TransformLite(playerCharacterRef.position.x, playerCharacterRef.position.y, playerCharacterRef.position.z, playerCharacterRef.eulerAngles.x, playerCharacterRef.eulerAngles.y, playerCharacterRef.eulerAngles.z);
        }
        else 
        {
            Debug.LogError("[Error] Reference to player character is missing!");
        }

        //Save Player Health
        if (playerHealthRef) 
        {
            newSaveData.healthAmount = playerHealthRef.currentHealth;
        }
        else {
            Debug.LogError("[Error] Reference to player health is missing!");
        }

        //Save Collected Seed amount and inventory
        if (inventoryRef) 
        {
            newSaveData.seedsCollected = inventoryRef.GetPlayerSeedAmount();

            for (int inventorySlot = 0; inventorySlot < newSaveData.playerInventory.Length; inventorySlot++) 
            {

                if (inventoryRef.CheckItemAtInventorySlot(inventorySlot) == null) 
                {
                    newSaveData.playerInventory[inventorySlot] = 0;
                }

                switch (inventoryRef.CheckItemAtInventorySlot(inventorySlot)) 
                {
                    case "Seed":
                        newSaveData.playerInventory[inventorySlot] = 1;
                        break;
                    case "SuperSeed":
                        newSaveData.playerInventory[inventorySlot] = 2;
                        break;
                    case "":
                        newSaveData.playerInventory[inventorySlot] = 0;
                        break;
                    default:
                        newSaveData.playerInventory[inventorySlot] = 0;
                        break;
                }
            }
        }
        else 
        {
            Debug.LogError("[Error] Reference to inventory is missing!");
        }

        //Save Golden seed amount
        //if (questGiverRef) {
        //    newSaveData.goldenSeedsCollected = questGiverRef.quest.GoldenCoinCollected;
        //}
        //else {
        //    Debug.LogWarning("[Warning] Reference to quest system is missing!"); //TODO
        //}

        if (goldGoalRef) {
            newSaveData.goldenSeedsCollected = goldGoalRef.quest.GoldenCoinCollected;
        }
        else {
            Debug.LogWarning("[Warning] Reference to Gold.cs is missing!"); //TODO
        }

        //Save current pickups in level
        if (pickupsInLevel.Length > 0) 
        {
            newSaveData.levelPickUps = new bool[pickupsInLevel.Length];
            for (int i = 0; i < pickupsInLevel.Length; i++) 
            {
                if (pickupsInLevel[i]) 
                {
                    newSaveData.levelPickUps[i] = true;
                }
                else 
                {
                    newSaveData.levelPickUps[i] = false;
                }
            }
        }
        else 
        {
            Debug.LogWarning("[Warning] There are no pickups found in pickupsInLevel references.");
        }

        //Save platform positions
        if (platformsInLevel.Length > 0) 
        {
            newSaveData.platformCoords = new TransformLite[platformsInLevel.Length];
            for (int platformIndex = 0; platformIndex < platformsInLevel.Length; platformIndex++) 
            {
                if (platformsInLevel[platformIndex]) 
                {
                    newSaveData.platformCoords[platformIndex] = new TransformLite(platformsInLevel[platformIndex].position.x, platformsInLevel[platformIndex].position.y, platformsInLevel[platformIndex].position.z, platformsInLevel[platformIndex].eulerAngles.x, platformsInLevel[platformIndex].eulerAngles.y, platformsInLevel[platformIndex].eulerAngles.z);
                }
                else 
                {
                    Debug.LogWarning("[Warning] Platform reference in index is missing.");
                    newSaveData.platformCoords[platformIndex] = new TransformLite(0, 0, 0, 0, 0, 0);
                }
            }
        }
        else 
        {
            Debug.LogWarning("[Warning] There are no platforms found in platformsInLevel references.");
        }

        //Save mob positions
        if (mobsInLevel.Length > 0) 
        {
            newSaveData.mobCoords = new TransformLite[mobsInLevel.Length];
            newSaveData.mobsExist = new bool[mobsInLevel.Length];
            for (int i = 0; i < mobsInLevel.Length; i++) 
            {
                if (mobsInLevel[i]) {
                    newSaveData.mobsExist[i] = true;
                    newSaveData.mobCoords[i] = new TransformLite(mobsInLevel[i].position.x, mobsInLevel[i].position.y, mobsInLevel[i].position.z, mobsInLevel[i].eulerAngles.x, mobsInLevel[i].eulerAngles.y, mobsInLevel[i].eulerAngles.z);
                }
                else {
                    newSaveData.mobsExist[i] = false;
                    newSaveData.mobCoords[i] = new TransformLite(0, 0, 0, 0, 0, 0);
                }
            }
        }
        else 
        {
            Debug.LogWarning("[Warning] There are no mobs found in platformsInLevel references.");
        }

        //Save Checkpoint position
        if (checkpointRef) 
        {
            newSaveData.checkpointCoord = new TransformLite(checkpointRef.currentSpawnPoint.x, checkpointRef.currentSpawnPoint.y, checkpointRef.currentSpawnPoint.z, checkpointRef.currentSpawnPointRotation.x, checkpointRef.currentSpawnPointRotation.y, checkpointRef.currentSpawnPointRotation.z);
        }
        else 
        {
            Debug.LogWarning("[Warning] Reference to RespawnLogic.cs missing!");
        }

        //TEMP settings
        newSaveData.livesAmount = 3;
        newSaveData.ammoAmount = 100;
        newSaveData.aliensKilled = 0;
        newSaveData.currentLevel = 1; //0 means not in a level
        newSaveData.levelsUnlocked = 1;
        newSaveData.savefileHeader = "[Marco] Health: " + newSaveData.healthAmount + "; Golden Seeds: " + newSaveData.goldenSeedsCollected + ";";

        SaveFileReaderWriter.WriteToSaveFile(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".hamsave", newSaveData);

        //Update save slot button header
        saveSlots[_saveSlotIndex-1].text  = newSaveData.savefileHeader;

        Debug.Log("[Notice] Game Saved.");
    }

    IEnumerator LoadGameStateRoutine() 
    {
        Time.timeScale = 0;

        //Set up player character transform
        if (playerCharacterRef) 
        {
            playerCharacterRef.position = new Vector3(LoadedSaveFile.loadedSaveData.playerCoord.positionX, LoadedSaveFile.loadedSaveData.playerCoord.positionY, LoadedSaveFile.loadedSaveData.playerCoord.positionZ);
            playerCharacterRef.eulerAngles = new Vector3(0, LoadedSaveFile.loadedSaveData.playerCoord.orientationY, 0);
        }
        else 
        {
            Debug.LogError("[Error] Reference to player character missing.");
        }

        //Set up camera
        if (cameraRef && playerCharacterRef) {
            cameraRef.position = playerCharacterRef.position;
            cameraRef.rotation = playerCharacterRef.rotation;
        }

        //Set player health
        if (playerHealthRef) 
        {
            playerHealthRef.SetHealth(LoadedSaveFile.loadedSaveData.healthAmount);
        }
        else 
        {
            Debug.LogError("[Error] Reference to player health is missing!");
        }

        //Set golden seeds collected
        if (questGiverRef) {
            questGiverRef.quest.GoldenCoinCollected = LoadedSaveFile.loadedSaveData.goldenSeedsCollected;
        }
        else {
            Debug.LogWarning("[Warning] Reference to quest system is missing!"); //TODO
        }
        if (goldGoalRef) {
            goldGoalRef.SetGold(LoadedSaveFile.loadedSaveData.goldenSeedsCollected);
        }
        else {
            Debug.LogWarning("[Warning] Reference to Gold.cs is missing!"); //TODO
        }

        //Set seed collected amount and inventory
        if (inventoryRef) 
        {
            inventoryRef.SetPlayerSeedAmount(LoadedSaveFile.loadedSaveData.seedsCollected);

            for (int inventorySlot = 0; inventorySlot < 8; inventorySlot++) 
            {
                switch (LoadedSaveFile.loadedSaveData.playerInventory[inventorySlot]) 
                {
                    case 1:
                        inventoryRef.AddItemToList("Seed");
                        break;
                    case 2:
                        inventoryRef.AddItemToList("SuperSeed");
                        break;
                    case 0:
                        //inventoryRef.AddItemToList("");
                        break;
                    default:
                        //inventoryRef.AddItemToList("");
                        break;
                }
            }
        }
        else 
        {
            Debug.LogError("[Error] Reference to inventory is missing!");
        }
        inventoryRef.DisplayInventory(); //Add seed sprites to inventory

        //Remove pickups that are already taken in save file game
        if (LoadedSaveFile.loadedSaveData.levelPickUps.Length > 0) 
        {
            for (int i = 0; i < LoadedSaveFile.loadedSaveData.levelPickUps.Length; i++) 
            {
                if (LoadedSaveFile.loadedSaveData.levelPickUps[i] == false) 
                {
                    if (pickupsInLevel[i]) 
                    {
                        Destroy(pickupsInLevel[i].gameObject);
                    }
                }
            }
        }
        else 
        {
            for (int i = 0; i < pickupsInLevel.Length; i++) 
            {
                if (pickupsInLevel[i]) 
                {
                    Destroy(pickupsInLevel[i].gameObject);
                }
            }
        }

        //Load Platform positions
        if (LoadedSaveFile.loadedSaveData.platformCoords.Length > 0) 
        {
            for (int i = 0; i < platformsInLevel.Length; i++) 
            {
                platformsInLevel[i].position = new Vector3(LoadedSaveFile.loadedSaveData.platformCoords[i].positionX, LoadedSaveFile.loadedSaveData.platformCoords[i].positionY, LoadedSaveFile.loadedSaveData.platformCoords[i].positionZ);
                platformsInLevel[i].eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.platformCoords[i].orientationX, LoadedSaveFile.loadedSaveData.platformCoords[i].orientationY, LoadedSaveFile.loadedSaveData.platformCoords[i].orientationZ);
            }
        }
        else 
        {
            Debug.LogWarning("[Warning] No platform postional data in save file.");
        }

        //Load Enemy positions
        if (LoadedSaveFile.loadedSaveData.mobCoords.Length > 0) 
        {
            for (int i = 0; i < mobsInLevel.Length; i++) 
            {
                if (LoadedSaveFile.loadedSaveData.mobsExist[i] && mobsInLevel[i]) 
                {
                    mobsInLevel[i].position = new Vector3(LoadedSaveFile.loadedSaveData.mobCoords[i].positionX, LoadedSaveFile.loadedSaveData.mobCoords[i].positionY, LoadedSaveFile.loadedSaveData.mobCoords[i].positionZ);
                    mobsInLevel[i].eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.mobCoords[i].orientationX, LoadedSaveFile.loadedSaveData.mobCoords[i].orientationY, LoadedSaveFile.loadedSaveData.mobCoords[i].orientationZ);
                }
                else if (!LoadedSaveFile.loadedSaveData.mobsExist[i] && mobsInLevel[i])
                {
                    Destroy(mobsInLevel[i].gameObject);
                }
                else if (LoadedSaveFile.loadedSaveData.mobsExist[i] && !mobsInLevel[i]) 
                {
                    Debug.LogError("[Error] Mob exists in save file, but not in level.");
                }
            }
        }

        //Load Checkpoint position
        if (checkpointRef) 
        {
            checkpointRef.oldSpawnPoint = checkpointRef.currentSpawnPoint;
            checkpointRef.currentSpawnPoint = new Vector3(LoadedSaveFile.loadedSaveData.checkpointCoord.positionX, LoadedSaveFile.loadedSaveData.checkpointCoord.positionY, LoadedSaveFile.loadedSaveData.checkpointCoord.positionZ);
            checkpointRef.currentSpawnPointRotation = new Vector3(LoadedSaveFile.loadedSaveData.checkpointCoord.orientationX, LoadedSaveFile.loadedSaveData.checkpointCoord.orientationY, LoadedSaveFile.loadedSaveData.checkpointCoord.orientationZ);
        }
        else 
        {
            Debug.LogWarning("[Warning] Reference to RespawnLogic.cs missing!");
        }

        yield return new WaitForSeconds(0.6f);
        Time.timeScale = 1;
    }
}
