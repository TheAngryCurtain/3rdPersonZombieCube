using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadController
{
    private GameData currentData;
    private string path = "/3rdPersonZombieData.zzz";

    public void CheckForSave()
    {
        LoadData();
        if (currentData == null)
        {
            currentData = new GameData();
            SaveData();
        }
    }

    public void UpdateData(GameData data)
    {
        currentData = data;
        //Debug.LogFormat("Updating -> shots: {0}, kills: {1}, time: {2}", currentData.shotsFired, currentData.zombiesKilled, currentData.bestTime);
        SaveData();
    }

    public float GetBestTime()
    {
        return currentData.bestTime;
    }

    private void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + path);
        bf.Serialize(file, currentData);
        file.Close();
    }

    private void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + path, FileMode.Open);
            currentData = (GameData)bf.Deserialize(file);
            //Debug.LogFormat("Found data -> shots: {0}, kills: {1}, time: {2}", currentData.shotsFired, currentData.zombiesKilled, currentData.bestTime);
            file.Close();
        }
    }
}
