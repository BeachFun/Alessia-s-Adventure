using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenGameController : MonoBehaviour
{
    [SerializeField] private string sceneName;


    void Awake()
    {
        Messenger.AddListener(StartupNotice.ALL_MANAGERS_STARTED, OpenScene);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(StartupNotice.ALL_MANAGERS_STARTED, OpenScene);
    }

    private void OpenScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
