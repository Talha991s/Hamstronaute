/*  Author: Salick Talhah
 *  Date Created: January 28, 2021
 *  Last Updated: March 13, 2021
 *  Description: This static class contains functions that allow SaveData objects to be written as save files and reads save files.
 */

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveFileReaderWriter
{
    //Writes the given SaveData object as a save file at given path.
    public static void WriteToSaveFile(string _filepath, SaveData _newSaveFile) 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_filepath, FileMode.Create); //Note: the filepath includes the filename to be created/overwritten
        formatter.Serialize(stream, _newSaveFile);
        stream.Close();
    }

    //Reads save file from a given file path and returns its properties as a SaveData file.
    public static SaveData ReadFromSaveFile(string _filepath) 
    {
        if(File.Exists(_filepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_filepath, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else 
        {
            //Debug.LogError("[Error] Save file not found in " + _filepath);
            return null;
        }
    }

    //Returns an array of available save files that can be loaded
    //TODO Remove; move functionality to a more dynamic class
    public static string[] CheckAvailableSaveFiles(string _saveFileDirectory, string _saveFileName) 
    {
        string[] saveFileNames = new string[4]; //This game will have a maximum 4 save slots hardcoded.
        BinaryFormatter formatter = new BinaryFormatter();

        for (int index = 0; index < 4; index++) 
        {
            if (File.Exists(_saveFileDirectory + "/" + _saveFileName + (index + 1).ToString() + ".hamsave")) 
            {
                FileStream stream = new FileStream(_saveFileDirectory + "/" + _saveFileName + (index + 1).ToString() + ".hamsave", FileMode.Open);
                SaveData data = formatter.Deserialize(stream) as SaveData;
                saveFileNames[index] = data.savefileHeader;
                stream.Close();
            }
            else 
            {
                //Debug.Log("Save file does not exist at index " + index);
                saveFileNames[index] = "Empty Save Slot";
            }
        }

        return saveFileNames;
    }
}