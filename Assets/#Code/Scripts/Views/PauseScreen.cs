using TimeSpan = System.TimeSpan;
using UnityEngine;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text textLevelName;
    [SerializeField] private TMP_Text textTimeAccess;
    [SerializeField] private TMP_Text textDiamonds;
    [SerializeField] private TMP_Text textEnemies;

    private void UpdateData()
    {
        LevelData data = LevelManagers.Level.CurrentLevelData;

        var timeAccess = TimeSpan.Parse(data.TimeAccess);

        textLevelName.text = data.LevelName;
        textTimeAccess.text = $"Time Access: {timeAccess.Minutes.ToString("D2")} : {timeAccess.Seconds.ToString("D2")}";
        textDiamonds.text = $"Collected Diamonds: {data.CollectedDiamondCount} out of {data.MaxDiamonds}";
        textEnemies.text = $"Killed Enemies: {data.EnemiesKillCounter} out of {data.EnemiesIntoLevel}";
    }

    public void Show()
    {
        UpdateData();

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Messenger.Broadcast(GameEvents.LEVEL_RESTART);
    }

    public void ExitLevel()
    {
        Time.timeScale = 1f;
        Messenger.Broadcast(GameEvents.LEVEL_FAILED);
    }
}
