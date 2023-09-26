using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUISceneControllers : MonoBehaviour
{
    [SerializeField] private string levelsMapSceneName;

    public void OpenLevelsMap()
    {
        SceneManager.LoadScene(levelsMapSceneName);
    }
}
