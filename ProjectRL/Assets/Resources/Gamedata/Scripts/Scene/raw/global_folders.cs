using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
[ExecuteAlways]
public class global_folders : MonoBehaviour
{
    public string _root;
    public string _storylines;
    public string _configs;
    public string _CG;
    public string _characters;
    public string _body;
    public string _haircut;
    public string _clothes;
    public string _makeup;
    void Start()
    {
        Setup_folders();
    }
    public Boolean Setup_folders()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        _root = Path.Combine(Application.persistentDataPath, save_file_name);
#else
        _root = Path.Combine(Application.dataPath);
#endif

        _storylines = _root + "/Resources/Gamedata/Storylines";
        _configs = _root + "/Extensions/StorylineEditor/Configs";
        _CG = _root + "/Resources/Gamedata/Textures/CG";
        _characters = _root + "/Resources/Gamedata/Ñharacters";
        _body = _root + "/Resources/Gamedata/Textures/Char_body";
        _haircut = _root + "/Resources/Gamedata/Textures/Char_haircut";
        _clothes = _root + "/Resources/Gamedata/Textures/Char_clothes";
        _makeup = _root + "/Resources/Gamedata/Textures/Char_makeup";
        return true;
    }
}
