using System;
using System.IO;
using UnityEngine;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    public bool CheckToSeeIfFileExist()
    {
        if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }

        return false;
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath , saveFileName));
    }

    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            //  CREATE THE DIRECTORY THE FILE WILL BE WRITTEN TO, IF IT DOES NOT ALREADY EXIST
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE, AT SAVE PATH: " + savePath);

            //  SERIALIZE THE C# GAME DATA OBJECT INTO JSON
            string dataToStore = JsonUtility.ToJson(characterData, true);

            //  WRITE THE FILE TO OUR SYSTEM
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR WHILE TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED!\n" + savePath + "\n" + e);
        }
    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;

        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //  DESERIALIZE DATA FROM JSON TO UNITY
                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR ON LoadSaveFile() FUNCTION\n" + e);
            }
        }

        return characterData;
    }
}
