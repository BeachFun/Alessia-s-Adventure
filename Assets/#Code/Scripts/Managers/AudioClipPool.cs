using UnityEngine;
using System.Collections.Generic;

public class AudioClipPool
{
    public static AudioClipPool Instance; // Ссылка на экземпляр пула (реализация синглтона)

    // Словарь для хранения аудиоклипов
    private Dictionary<string, AudioClip> audioClipDictionary = new Dictionary<string, AudioClip>();

    public AudioClipPool()
    {
        Instance = this;
    }

    // Метод для добавления аудиоклипа в словарь
    public void AddAudioClip(string key, AudioClip clip)
    {
        if (!audioClipDictionary.ContainsKey(key))
        {
            audioClipDictionary[key] = clip;
        }
        else
        {
            Debug.LogWarning("An AudioClip with the same key already exists.");
        }
    }

    // Индексатор для удобного доступа к аудиоклипам
    public AudioClip this[string key]
    {
        get
        {
            if (audioClipDictionary.ContainsKey(key))
            {
                return audioClipDictionary[key];
            }
            else
            {
                Debug.LogWarning("No AudioClip found with the given key: " + key);
                return null;
            }
        }
    }
}
