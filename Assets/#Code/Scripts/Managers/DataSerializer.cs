using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Модуль отвечающий за сохранение и заргузку данных игры. Реализован через BinaryFormatter
/// </summary>
public class DataSerializer
{
    public DataSerializer()
    {
        _taskQueue = new();
        _active = false;
    }
    public DataSerializer(string dataPath) : this()
    {
        DataPath = dataPath;
    }


    private string _dataPath;
    private bool _active;
    private Queue<(TaskType, object, string)> _taskQueue;


    internal string DataPath
    {
        get => _dataPath;
        set => _dataPath = value;
    }


    /// <summary>
    /// Извлекает значение из очереди и вносит новые значения в очередь
    /// </summary>
    private (TaskType, object, string) TaskQueue
    {
        get => _taskQueue.Peek();
        set
        {
            _taskQueue.Enqueue(value);
            this.Handler();
        }
    }


    private void Handler()
    {
        _active = true;
        while (_taskQueue.Count > 0)
        {
            (TaskType, object, string) data = _taskQueue.Dequeue();

            if (data.Item1 == TaskType.Save) this.Save(data.Item1, data.Item2, data.Item3);
            if (data.Item1 == TaskType.Dump) this.Dump(data.Item1, data.Item2, data.Item3);
        }
        _active = false;
    }


    public void Save<T>(T obj, string key)
    {
        if (obj != null)
            TaskQueue = (TaskType.Save, obj, key);

    }

    public void Dump<T>(T obj, string key)
    {
        if (obj != null)
            TaskQueue = (TaskType.Dump, obj, key);
    }

    /// <summary>
    /// Загрузка данных. Может выполняться слишком долго. Если данных нет возвращает значение default для указанного типа
    /// </summary>
    /// <param name="key">Метка с которой были сохранены данные</param>
    public T Load<T>(string key)
    {
        if (!File.Exists(_dataPath))
        {
            Debug.Log("No saved game");
            return default(T);
        }

        Dictionary<string, object> gamestate = null;
        while (_active)
            new WaitForSeconds(10);

        _active = true;
        using (FileStream stream = new FileStream(_dataPath, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            gamestate = formatter.Deserialize(stream) as Dictionary<string, object>;
        }
        _active = false;

        try
        {
            return (T)gamestate[key];
        }
        catch
        {
            return default(T);
        }
    }


    private void Save<T>(TaskType type, T obj, string key)
    {
        Dictionary<string, object> gamestate = new Dictionary<string, object>();
        gamestate.Add(key, obj);

        using (FileStream stream = new FileStream(_dataPath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, gamestate);
        }

        //Debug.Log($"Успешное сохранение данных типа со сбросон данных: {typeof(T)} | {key}");
    }

    private void Dump<T>(TaskType type, T obj, string key)
    {
        Dictionary<string, object> gamestate = default;
        if (File.Exists(_dataPath))
        {
            //this.Save<object>(type, obj, key);
            //return;
            using (FileStream stream = new FileStream(_dataPath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                gamestate = formatter.Deserialize(stream) as Dictionary<string, object>;
            }
        }
        else
        {
            gamestate = new Dictionary<string, object>();
        }

        if (!gamestate.Keys.Contains(key))
            gamestate.Add(key, obj);
        else
            gamestate[key] = obj;

        using (FileStream stream = new FileStream(_dataPath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, gamestate);
        }
        //Debug.Log($"Уcпешное сохранение данных типа: {typeof(T)} | {key}");
    }

    //private IEnumerator Load<T>()


    /// <summary>
    /// Тип операции модуля
    /// </summary>
    private enum TaskType
    {
        /// <summary>
        /// Загрузка данных
        /// </summary>
        Load,
        /// <summary>
        /// Сохранение данных со сбросом всех данных
        /// </summary>
        Save,
        /// <summary>
        /// Сохранение данных
        /// </summary>
        Dump
    }
}