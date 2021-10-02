using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;


public class global_stats : MonoBehaviour
{
    [SerializeField] public int _items_diamond;
    [SerializeField] public int _items_cups;
    [SerializeField] public float _time_interval;
    [SerializeField] public float _time_remaining;
    [SerializeField] private string _temp_save_path;
    [SerializeField] private string _temp_save_file_name = "stats.json";

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        save_path = Path.Combine(Application.persistentDataPath, save_file_name);
#else
        _temp_save_path = Path.Combine(Application.dataPath, _temp_save_file_name);
#endif
        LoadFromFile();
    }
    public void SaveToFile()
    {

        stats_parameters _parameters = new stats_parameters
        {
            _items_diamonds_parameter = this._items_diamond,
            _items_cups_parameter = this._items_cups,
            _time_interval_parameter = this._time_interval,
            _time_remaining_parameter = this._time_remaining,
        };

        string json_content = JsonUtility.ToJson(_parameters, prettyPrint: true);
        File.WriteAllText(_temp_save_path, contents: json_content);

    }
    public void LoadFromFile()
    {
        if (!File.Exists(_temp_save_path))
        {
            Debug.Log("File missed");
            return;
        }
        string json_content2 = File.ReadAllText(_temp_save_path);
        stats_parameters _parameters_from_json = JsonUtility.FromJson<stats_parameters>(json_content2);
        this._items_diamond = _parameters_from_json._items_diamonds_parameter;
        this._items_cups = _parameters_from_json._items_cups_parameter;
        this._time_interval = _parameters_from_json._time_interval_parameter;
        this._time_remaining = _parameters_from_json._time_remaining_parameter;

    }
    private void OnApplicationPause(bool pause)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SaveToFile();
        }
    }


}
