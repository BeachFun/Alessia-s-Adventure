using System.Linq;
using UnityEngine;

public class HUDCanvasUI : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsForInitialization;
    [Header("References")]
    [SerializeField] private PauseScreen pauseScreen;

    private void Awake()
    {
        objectsForInitialization.ToList().ForEach(e => e.SetActive(true));

        Messenger<bool>.AddListener(GameEvents.ON_PAUSE_STATE_CHANGED, OnPauseChange);
    }

    private void Start()
    {
        objectsForInitialization.ToList().ForEach(e => e.SetActive(false));

        Messenger.Broadcast(StartupNotice.HUD_CANVAS_STARTED);
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GameEvents.ON_PAUSE_STATE_CHANGED, OnPauseChange);
    }


    private void OnPauseChange(bool isPaused)
    {
        if (isPaused) pauseScreen.Show();
        else pauseScreen.Hide();
    }

    public void OnUIClick()
    {
        Messenger.Broadcast(GameEvents.UI_CLICKED);
    }
}
