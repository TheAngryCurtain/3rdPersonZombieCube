using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject spawnerObject;
    public GameObject playerPrefab;
    public Transform playerStart;

    private SaveLoadController saveController;
    public GameData Data;
    private GameObject player;
    private Spawner spawner;
    private bool spawnerDead = false;
    private bool playerDead = false;
    private float gameTime = 0;

    public static int currentSensitivity = 1;

    private bool isInverted = true;
    private int minSensitivity = 1;
    private int maxSensitivity = 5;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        saveController = new SaveLoadController();
        saveController.CheckForSave();
        Data = new GameData();

        player = Instantiate(playerPrefab, playerStart.position, playerStart.rotation) as GameObject;
        spawner = spawnerObject.transform.GetChild(0).GetComponent<Spawner>();
        UpdateInvertPreference(isInverted);

        UIManager.Instance.SetMessage("Destroy the Spawner");
        UIManager.Instance.UpdateBestTime(saveController.GetBestTime());

        Invoke("ClearMessage", 3f);
    }

    private void ClearMessage()
    {
        UIManager.Instance.SetMessage("");
    }

    void Update()
    {
        if (!spawnerDead && !playerDead)
        {
            gameTime += Time.deltaTime;
            UIManager.Instance.UpdateGameTime(gameTime);
        }

        if (!spawnerDead)
        {
            spawner.UpdateZombiesGoal(player.transform.position);
        }
    }

    public void NotifySpawnerDead()
    {
        spawnerDead = true;
        UIManager.Instance.SetMessage("Spawner Destroyed");

        float previousBestTime = saveController.GetBestTime();
        if (gameTime < previousBestTime || previousBestTime == 0f)
        {
            Debug.LogFormat("New best time! was: {0}, now: {1}", previousBestTime, gameTime);
            Data.bestTime = gameTime;
            saveController.UpdateData(Data);
        }
        Time.timeScale = 0.25f;

        Invoke("Restart", 2f);
    }

    public void NotifyPlayerDead()
    {
        playerDead = true;
        UIManager.Instance.SetMessage("You Died");
        Time.timeScale = 0.25f;

        Invoke("Restart", 2f);
    }

    // restart the game over again
    public void Restart()
    {
        Time.timeScale = 1f;
        Application.LoadLevel(0);
    }

    public void TogglePause()
    {
        UIManager.Instance.TogglePause();
        UIManager.Instance.UpdateInversion(isInverted);
        UIManager.Instance.UpdateSensitivity(currentSensitivity);
        UIManager.Instance.UpdateKills(Data.zombiesKilled);
        UIManager.Instance.UpdateShotsFired(Data.shotsFired);
    }

    public void UpdateInvertPreference(bool inverted)
    {
        isInverted = inverted;
        UIManager.Instance.UpdateInversion(inverted);
    }

    public void AdjustSensitivity(int direction)
    {
        currentSensitivity += direction;
        if (currentSensitivity < minSensitivity)
        {
            currentSensitivity = minSensitivity;
        }
        
        if (currentSensitivity > maxSensitivity)
        {
            currentSensitivity = maxSensitivity;
        }

        UIManager.Instance.UpdateSensitivity(currentSensitivity);
    }
}
