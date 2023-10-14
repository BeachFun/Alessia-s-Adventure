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
    [SerializeField] private TMP_Text textDiamonds;
    [SerializeField] private TMP_Text textBestScore;

    [Header("Stars")]
    [SerializeField] private Image imageClearedStar;
    [SerializeField] private Image imageBestTimeStar;
    [SerializeField] private Image imageEnemiesStar;
    [SerializeField] private Image imageDiamondStar;

    [Header("Color Settings")]
    [SerializeField] private Color receivedStar = Color.yellow;
    [SerializeField] private Color nonReceivedStar = Color.gray;


    private int _levelIndex;


    public void UpdateData(int levelIndex)
    {
        LevelData levelData = GameManagers.GameProgress[levelIndex];

        var timeComplete = levelData.TimeComplete is not null ? TimeSpan.Parse(levelData.TimeComplete) : TimeSpan.Zero;
        var timeAccess = TimeSpan.Parse(levelData.TimeAccess);

        textLevelName.text = levelData.LevelName;
        textEnemies.text = $"{levelData.EnemiesKillCounter}/{levelData.EnemiesIntoLevel}";
        textDiamonds.text = $"{levelData.CollectedDiamondCount}/{levelData.MaxDiamonds}";
        textBestScore.text = levelData.BestScore.ToString("D5");
        textBestTime.text = $"{timeComplete.Minutes.ToString("D2")}:" +
            $"{timeComplete.Seconds.ToString("D2")}/" +
            $"{timeAccess.Minutes.ToString("D2")}:" +
            $"{timeAccess.Seconds.ToString("D2")}";

        if (levelData.IsComplete)
        {
            imageClearedStar.color = receivedStar;
            imageEnemiesStar.color = levelData.EnemiesKillCounter >= levelData.EnemiesIntoLevel ? receivedStar : nonReceivedStar;
            imageDiamondStar.color = levelData.CollectedDiamondCount >= levelData.MaxDiamonds ? receivedStar : nonReceivedStar;

            if (!string.IsNullOrEmpty(levelData.TimeComplete) && !string.IsNullOrEmpty(levelData.TimeAccess) &&
            TimeSpan.Parse(levelData.TimeComplete).TotalSeconds <= TimeSpan.Parse(levelData.TimeAccess).TotalSeconds)
            {
                imageBestTimeStar.color = receivedStar;
            }
            else
            {
                imageBestTimeStar.color = nonReceivedStar;
            }
        }
        else
        {
            imageClearedStar.color = nonReceivedStar;
            imageEnemiesStar.color = nonReceivedStar;
            imageDiamondStar.color = nonReceivedStar;
            imageBestTimeStar.color = nonReceivedStar;
        }

        _levelIndex = levelIndex;
    }

    public void Open() => gameObject.SetActive(true);

    public void Close() => gameObject.SetActive(false);

    public void StartLevel()
    {
        LevelManager.LevelIndex = _levelIndex;
        Messenger<int>.Broadcast(GameEvents.LEVEL_IS_SELECTED_FOR_STARTED, _levelIndex);
    }
}
