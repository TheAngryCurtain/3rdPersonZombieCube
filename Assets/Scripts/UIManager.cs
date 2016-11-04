using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject startMenuPanel;

    public Image healthFill;
    public Image batteryFill;
    public Text ammoText;
    public Text timer;
    public Text bestTime;
    public Text messageText;

    private RectTransform healthBar;
    private RectTransform batteryBar;
    private float healthBarWidth;
    private float batteryBarWidth;

    void Awake()
    {
        Instance = this;

        healthBar = healthFill.GetComponent<RectTransform>();
        healthBarWidth = healthBar.rect.size.x;

        batteryBar = batteryFill.GetComponent<RectTransform>();
        batteryBarWidth = batteryBar.rect.size.x;
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }

    public void UpdateGameTime(float time)
    {
        timer.text = GetConvertedTime(time);
    }

    public void UpdateBestTime(float time)
    {
        bestTime.text = GetConvertedTime(time);
    }

    private string GetConvertedTime(float time)
    {
        float seconds = time % 60;
        float minutes = seconds / 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdatePlayerHealth(int current, int max)
    {
        healthBar.sizeDelta = new Vector2((current / (float)max) * healthBarWidth, healthBar.rect.size.y);
    }

    public void UpdateFlashLightBattery(int current, int max)
    {
        batteryBar.sizeDelta = new Vector2((current / (float)max) * batteryBarWidth, batteryBar.rect.size.y);
    }

    public void UpdatePlayerAmmo(int current, int max)
    {
        ammoText.text = string.Format("{0}/{1}", current, max);
    }

    public void TogglePause()
    {
        startMenuPanel.SetActive(!startMenuPanel.activeInHierarchy);
        Time.timeScale = (startMenuPanel.activeInHierarchy ? 0f : 1f);
    }

    public void UpdateSensitivity(int sens)
    {
        Text sensText = startMenuPanel.transform.FindChild("Sensitivity Text").GetComponent<Text>();
        sensText.text = string.Format("Sensitivity: {0}", sens);
    }

    public void UpdateInversion(bool inverted)
    {
        Text invText = startMenuPanel.transform.FindChild("Inversion").GetComponent<Text>();
        invText.text = string.Format("Aiming: {0}", (inverted ? "Inverted" : "Not Inverted"));
    }

    public void UpdateKills(int kills)
    {
        Text killsText = startMenuPanel.transform.FindChild("Kills Text").GetComponent<Text>();
        killsText.text = string.Format("Kills: {0}", kills);
    }

    public void UpdateShotsFired(int shots)
    {
        Text shotsText = startMenuPanel.transform.FindChild("Shots Text").GetComponent<Text>();
        shotsText.text = string.Format("Shots Fired: {0}", shots);
    }
}
