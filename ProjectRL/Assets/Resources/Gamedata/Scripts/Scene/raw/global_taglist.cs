
using UnityEngine;
using System.IO;
using System;
[ExecuteAlways]
public class global_taglist : MonoBehaviour
{
    private global_folders _s_folder;
    public string _version;
    public string _start;
    public string _action;
    public string _separator;
    public string _phrase;
    public string _CG;
    public string _init;
    public string _required_objects;
    public string _required_cg;
    public string _skip;
    public string _step;
    public string _character_relocated;
    public string _activate;
    public string _inactivate;
    public string _author;
    public string _rescale;
    public string _null;
    public string _separator_vert;
    public string _cg_position;

    void Start()
    {
        _s_folder = GetComponent<global_folders>();
        Setup_tags();
    }
    public Boolean Setup_tags()
    {
        string tag_list_path = _s_folder._configs + "/tag_list.cfg";
        StreamReader SR = new StreamReader(tag_list_path);
        string line = SR.ReadLine();
        _version = line;
        int count = 1;
        while (line != null)
        {
            line = SR.ReadLine();
            switch (count)
            {
                case 0:
                    count += 1;
                    break;
                case 1:
                    _start = line;
                    count += 1;
                    break;
                case 2:
                    _action = line;
                    count += 1;
                    break;
                case 3:
                    _separator = line;
                    count += 1;
                    break;
                case 4:
                    _phrase = line;
                    count += 1;
                    break;
                case 5:
                    _CG = line;
                    count += 1;
                    break;
                case 6:
                    _init = line;
                    count += 1;
                    break;
                case 7:
                    _required_objects = line;
                    count += 1;
                    break;
                case 8:
                    _required_cg = line;
                    count += 1;
                    break;
                case 9:
                    _skip = line;
                    count += 1;
                    break;
                case 10:
                    _step = line;
                    count += 1;
                    break;
                case 11:
                    _character_relocated = line;
                    count += 1;
                    break;
                case 12:
                    _activate = line;
                    count += 1;
                    break;
                case 13:
                    _inactivate = line;
                    count += 1;
                    break;
                case 14:
                    _author = line;
                    count += 1;
                    break;
                case 15:
                    _rescale = line;
                    count += 1;
                    break;
                case 16:
                    _null = line;
                    count += 1;
                    break;
                case 17:
                    _separator_vert = line;
                    count += 1;
                    break;
                case 18:
                    _cg_position = line;
                    count = 0;
                    break;
            }
        }
        SR.Close();
        return true;
    }
}
