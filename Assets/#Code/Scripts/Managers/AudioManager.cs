using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    private const float clickSoundVolume = 0.15f;

    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float lobbyMusicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float gameMusicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 0.3f;
    [SerializeField] private AudioSource musicSource; // AudioSource для музыки
    [SerializeField] private AudioSource[] soundSources; // Массив AudioSource для звуков

    public ManagerStatus Status { get; private set; }

    private void Awake()
    {
        // Отключите все AudioSource при запуске
        foreach (var soundSource in soundSources)
        {
            soundSource.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Status == ManagerStatus.Started)
        {
            Messenger.RemoveListener(GameEvents.MAIN_MENU_OPENED, LobbyMusicPlay);
            Messenger.RemoveListener(GameEvents.GAME_STARTED, GameMusicPlay);
            Messenger.RemoveListener(GameEvents.UI_CLICKED, OnUIClick);
        }
    }

    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        Messenger.AddListener(GameEvents.MAIN_MENU_OPENED, LobbyMusicPlay);
        Messenger.AddListener(GameEvents.GAME_STARTED, GameMusicPlay);
        Messenger.AddListener(GameEvents.UI_CLICKED, OnUIClick);

        AudioClipPool soundPool = new();
        soundPool.AddAudioClip("UI Click", Resources.Load<AudioClip>("UI Click"));
        soundPool.AddAudioClip("Open Level", Resources.Load<AudioClip>("Open Level"));
        soundPool.AddAudioClip("Close Level", Resources.Load<AudioClip>("Close Level"));
        soundPool.AddAudioClip("Attack", Resources.Load<AudioClip>("Attack"));
        soundPool.AddAudioClip("Throw", Resources.Load<AudioClip>("Throw"));
        soundPool.AddAudioClip("Hit", Resources.Load<AudioClip>("Hit"));
        soundPool.AddAudioClip("Step", Resources.Load<AudioClip>("Step"));
        soundPool.AddAudioClip("Heal", Resources.Load<AudioClip>("Heal"));
        soundPool.AddAudioClip("Jump", Resources.Load<AudioClip>("Jump"));
        soundPool.AddAudioClip("Landing", Resources.Load<AudioClip>("Landing"));
        soundPool.AddAudioClip("Sliding", Resources.Load<AudioClip>("Sliding"));
        soundPool.AddAudioClip("Enemy Death", Resources.Load<AudioClip>("Enemy Death"));
        soundPool.AddAudioClip("Energy Ball", Resources.Load<AudioClip>("Energy Ball"));
        soundPool.AddAudioClip("Dagger Collect", Resources.Load<AudioClip>("Dagger Collect"));
        soundPool.AddAudioClip("Diamond Collect", Resources.Load<AudioClip>("Diamond Collect"));
        soundPool.AddAudioClip("Game Music", Resources.Load<AudioClip>("Game Music"));
        soundPool.AddAudioClip("Main Menu", Resources.Load<AudioClip>("Main Menu"));

        Status = ManagerStatus.Started;
    }

    // Воспроизвести музыку
    public void PlayMusic(AudioClip music)
    {
        PlayMusic(music, musicVolume);
    }

    // Воспроизвести музыку
    public void PlayMusic(AudioClip music, float volume = 1f)
    {
        musicSource.clip = music;
        musicSource.volume = volume;
        musicSource.Play();
    }

    // Переключить паузу
    public void TogglePauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
        else
            musicSource.Play();
    }

    // Остановить воспроизведение музыки
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Воспроизвести звук с использованием пула AudioSource
    public void PlaySound(AudioClip sound)
    {
        PlaySound(sound, soundVolume);
    }

    // Воспроизвести звук с использованием пула AudioSource
    public void PlaySound(AudioClip sound, float volume = 1f)
    {
        // Ищем доступный AudioSource в пуле, который не воспроизводит звук в данный момент
        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource != null)
        {
            availableSource.clip = sound;
            availableSource.volume = volume;
            availableSource.gameObject.SetActive(true);
            availableSource.Play();
        }
    }

    // Получить доступный AudioSource из пула
    private AudioSource GetAvailableAudioSource()
    {
        foreach (var soundSource in soundSources)
        {
            if (!soundSource.isPlaying)
            {
                return soundSource;
            }
        }
        return null;
    }

    private void LobbyMusicPlay()
    {
        if (musicSource.clip is not null && musicSource.clip.name == "Main Menu") return;

        PlayMusic(AudioClipPool.Instance["Main Menu"], lobbyMusicVolume);
    }

    private void GameMusicPlay()
    {
        PlayMusic(AudioClipPool.Instance["Game Music"], gameMusicVolume);
    }

    private void OnUIClick()
    {
        PlaySound(AudioClipPool.Instance["UI Click"], clickSoundVolume);
    }
}
