using UnityEngine;

/// <summary>
/// Автономный компонент, который ставит игру на паузу
/// PROG MISTERIO | 19:05 25.09.2023
/// </summary>
public class Pauser : MonoBehaviour
{
    private const float PauseTimeScale = 0f;
    private const float NormalTimeScale = 1f;

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Изменили клавишу паузы на Esc
        {
            TogglePause(); // Вызывает метод для переключения состояния паузы
        }
    }

    public void TogglePause()
    {
        GameManagers.Audio.TogglePauseMusic();

        isPaused = !isPaused;
        Time.timeScale = isPaused ? PauseTimeScale : NormalTimeScale; // Изменили состояние времени

        // Вызываем событие для уведомления других компонентов о смене состояния паузы
        Messenger<bool>.Broadcast(GameEvents.ON_PAUSE_STATE_CHANGED, isPaused);
    }
}
