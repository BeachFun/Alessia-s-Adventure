using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUISceneControllers : MonoBehaviour
{
    [SerializeField] private string levelsMapSceneName;

    private void Start()
    {
        Messenger.Broadcast(GameEvents.MAIN_MENU_OPENED);
    }

    public void OpenLevelsMap()
    {
        SceneManager.LoadScene(levelsMapSceneName);
    }

    public void OnUIClick()
    {
        Messenger.Broadcast(GameEvents.UI_CLICKED);
    }
}
