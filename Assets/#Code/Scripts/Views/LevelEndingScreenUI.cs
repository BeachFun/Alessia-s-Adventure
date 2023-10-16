using System.Collections;
using System.Collections.Generic;
using TimeSpan = System.TimeSpan;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndingScreenUI : MonoBehaviour
{
    [Header("Text labels")]
    [SerializeField] private TMP_Text textLevelName;
    [SerializeField] private TMP_Text textBoolResult;
    [SerializeField] private TMP_Text textPassageTime;
    [SerializeField] private TMP_Text textPassageTimeScore;
    [SerializeField] private TMP_Text textEnemies;
    [SerializeField] private TMP_Text textEnemiesScore;
    [SerializeField] private TMP_Text textDiamonds;
    [SerializeField] private TMP_Text textDiamondsScore;
    [SerializeField] private TMP_Text textTotalScore;
    [SerializeField] private TMP_Text textBestScoreLabel;

    [Header("Stars")]
    [SerializeField] private Image imageClearedStar;
    [SerializeField] private Image imageBestTimeStar;
    [SerializeField] private Image imageEnemiesStar;
    [SerializeField] private Image imageDiamondStar;

    [Header("Stars Show Settings")]
    [SerializeField] private float star1WaitForSeconds = 0.9f;
    [SerializeField] private float star2WaitForSeconds = 0.6f;
    [SerializeField] private float star3WaitForSeconds = 0.3f;
    [SerializeField] private float star4WaitForSeconds = 0.3f;
    [SerializeField] private Color receivedStar = Color.yellow;


    private bool _isCompleted;
    private LevelData _data;
    private (int, int, int) _scores;

    private void Awake()
    {
        Messenger<bool, LevelData, (int, int, int)>.AddListener(GameEvents.LEVEL_PASS_DATA_COLLECTED, UpdateData);

        Hide();
    }

    private void OnDestroy()
    {
        Messenger<bool, LevelData, (int, int, int)>.RemoveListener(GameEvents.LEVEL_PASS_DATA_COLLECTED, UpdateData);
    }

    internal void UpdateData(bool isCompleted,  LevelData levelData, (int, int, int) scores)
    {
        Show();

        textLevelName.text = levelData.LevelName;
        textBoolResult.text = isCompleted ? "completed" : "failed";

        var timeComplete = TimeSpan.Parse(levelData.TimeComplete);
        var timeAccess = TimeSpan.Parse(levelData.TimeAccess);

        textEnemies.text = $"{levelData.EnemiesKillCounter}/{levelData.EnemiesIntoLevel}";
        textDiamonds.text = $"{levelData.CollectedDiamondCount}/{levelData.MaxDiamonds}";
        textPassageTime.text = $"{timeComplete.Minutes.ToString("D2")}:" +
            $"{timeComplete.Seconds.ToString("D2")}/" +
            $"{timeAccess.Minutes.ToString("D2")}:" +
            $"{timeAccess.Seconds.ToString("D2")}";

        textPassageTimeScore.text = scores.Item1.ToString();
        textEnemiesScore.text = scores.Item2.ToString();
        textDiamondsScore.text = scores.Item3.ToString();
        textTotalScore.text = levelData.BestScore.ToString();

        if (levelData.BestScore > GameManagers.GameProgress[levelData.LevelIndex].BestScore)
        {
            textBestScoreLabel.gameObject.SetActive(true);
        }

        _isCompleted = isCompleted;
        _data = levelData;
        _scores = scores;

        StartCoroutine(ShowStars());
    }

    internal void OpenWithAnimation()
    {

    }

    public void Show()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Close Level"]);
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    public void ExitLevel()
    {
        StopAllCoroutines();

        Messenger.Broadcast(GameEvents.LEVEL_EXIT);
    }


    private IEnumerator ShowStars()
    {
        yield return new WaitForSeconds(star1WaitForSeconds);

        imageClearedStar.gameObject.SetActive(true);

        if (_isCompleted)
        {
            imageClearedStar.color = receivedStar;

            yield return new WaitForSeconds(star2WaitForSeconds);

            imageBestTimeStar.gameObject.SetActive(true);
            if (TimeSpan.Parse(_data.TimeComplete) <= TimeSpan.Parse(_data.TimeAccess))
            {
                imageBestTimeStar.color = receivedStar;
            }

            yield return new WaitForSeconds(star3WaitForSeconds);

            imageEnemiesStar.gameObject.SetActive(true);
            if (_data.EnemiesKillCounter >= _data.EnemiesIntoLevel)
            {
                imageEnemiesStar.color = receivedStar;
            }

            yield return new WaitForSeconds(star4WaitForSeconds);

            imageDiamondStar.gameObject.SetActive(true);
            if (_data.CollectedDiamondCount >= _data.MaxDiamonds)
            {
                imageDiamondStar.color = receivedStar;
            }
        }
        else
        {
            yield return new WaitForSeconds(star2WaitForSeconds);
            imageBestTimeStar.gameObject.SetActive(true);
            yield return new WaitForSeconds(star3WaitForSeconds);
            imageEnemiesStar.gameObject.SetActive(true);
            yield return new WaitForSeconds(star4WaitForSeconds);
            imageDiamondStar.gameObject.SetActive(true);
        }
    }
}
