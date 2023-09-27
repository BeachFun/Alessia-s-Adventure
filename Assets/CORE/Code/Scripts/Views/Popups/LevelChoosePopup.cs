using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TimeSpan = System.TimeSpan;

public class LevelChoosePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text textLevelName;
    [SerializeField] private TMP_Text textBestTime;
    [SerializeField] private TMP_Text textEnemies;
    [SerializeField] private TMP_Text textBestScore;
    [Header("Stars")]
    [SerializeField] private Image imageClearedStar;
    [SerializeField] private Image imageBestTimeStar;
    [SerializeField] private Image imageEnemiesStar;
    [Header("Color Settings")]
    [SerializeField] private Color receivedStar = Color.yellow;
    [SerializeField] private Color nonReceivedStar = Color.gray;


    private int _levelIndex;


    public void UpdateData(int levelIndex)
    {
        LevelData levelData = GameManagers.GameProgress[levelIndex];

        textLevelName.text = levelData.Name;
        textBestTime.text = $"{levelData.TimeComplete}/{levelData.TimeAccess}";
        textEnemies.text = $"{levelData.EnemiesKillCounter}/{levelData.EnemiesIntoLevel}";
        textBestScore.text = levelData.CollectedDiamondCount.ToString("D5");

        imageClearedStar.color = levelData.IsComplete ? receivedStar : nonReceivedStar;
        imageEnemiesStar.color = levelData.EnemiesKillCounter >= levelData.EnemiesIntoLevel ? receivedStar : nonReceivedStar;

        if (!string.IsNullOrEmpty(levelData.TimeComplete) && !string.IsNullOrEmpty(levelData.TimeAccess) &&
            TimeSpan.Parse(levelData.TimeComplete).TotalSeconds <= TimeSpan.Parse(levelData.TimeAccess).TotalSeconds)
        {
            imageBestTimeStar.color = receivedStar;
        }
        else
        {
            imageBestTimeStar.color = nonReceivedStar;
        }

        _levelIndex = levelIndex;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void StartLevel()
    {
        Messenger<int>.Broadcast(GameEvents.LEVEL_IS_SELECTED_FOR_STARTED, _levelIndex);
    }
}
