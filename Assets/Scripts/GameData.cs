using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameData
{
    public float bestTime;
    public int zombiesKilled;
    public int shotsFired;

    public GameData()
    {
        bestTime = 0f;
        zombiesKilled = 0;
        shotsFired = 0;
    }
}
