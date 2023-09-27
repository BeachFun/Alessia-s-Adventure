using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelsMapUISceneController : MonoBehaviour
{
    [SerializeField] private string mainSceneName;
    [SerializeField] private string[] levelsNames;
    [Header("Location")]
    [SerializeField] private Sprite[] locationSprites;
    [Header("UI Elements")]
    [SerializeField] private Image locationImage;
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [SerializeField] private GameObject[] groupListsLevels;

    private int _currentLocation;


    private void Start()
    {
        LocationChangeHandler();

        Messenger<int>.AddListener(GameEvents.LEVEL_IS_SELECTED_FOR_STARTED, OpenLevel);
    }

    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvents.LEVEL_IS_SELECTED_FOR_STARTED, OpenLevel);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void OpenLevel(int levelNumber)
    {
        if (levelNumber > levelsNames.Length || levelNumber < 0) return;

        SceneManager.LoadScene(levelsNames[levelNumber]);
    }

    public void LeftButtonOnClick()
    {
        if (_currentLocation > 0)
        {
            _currentLocation--;

            LocationChangeHandler();
        }
    }

    public void RightButtonOnClick()
    {
        if (_currentLocation < locationSprites.Length - 1)
        {
            _currentLocation++;

            LocationChangeHandler();
        }
    }

    private void LocationChangeHandler()
    {
        locationImage.sprite = locationSprites[_currentLocation];

        buttonLeft.gameObject.SetActive(_currentLocation > 0);
        buttonRight.gameObject.SetActive(_currentLocation < locationSprites.Length - 1);

        groupListsLevels.ToList().ForEach(e => e.SetActive(false));
        groupListsLevels[_currentLocation].SetActive(true);
    }
}
