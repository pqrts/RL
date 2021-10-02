using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class global_str_reader : MonoBehaviour
{
        //scripts
    [SerializeField] private string _storyline_name;
    global_taglist _s_tag;
    global_folders _s_folder;
    //for reading
    public string _str_version;
    public bool _read_complete;
    //ids
    private int _id_action;
    //data
    public List<string> _list_init_data = new List<string>();
    public List<string> _list_action_data = new List<string>();
    public List<string> _list_required_objects = new List<string>();
    public List<string> _list_required_CG = new List<string>();

    void Start()
    {
        _s_folder = GetComponent<global_folders>();
        _s_tag = GetComponent<global_taglist>();
        _read_complete = false;
        if (Read_meta())
        {
            if (Decompose_init())
            {
                if (Read_action(1))
                {
                    _read_complete = true;
                }
            }
        }
    }
    private Boolean Read_meta()
    {
        string path = _s_folder._storylines + "/"+ _storyline_name;
        StreamReader SR = new StreamReader(path);
        string line = SR.ReadLine();
        _list_init_data.Add(line);
        while (line != _s_tag._start)
        {
            line = SR.ReadLine();
            if (line.StartsWith(_s_tag._skip))
            {
                continue;
            }
            else
            {
                _list_init_data.Add(line);

            }
        }
        return true;
    }
    public Boolean Read_action(int p_action_id)
    {
        _list_action_data.Clear();
        int id_action_next = p_action_id + 1;
        string action_next = _s_tag._action + _s_tag._separator + id_action_next;
        string action_current = _s_tag._action + _s_tag._separator + p_action_id;
        string path = _s_folder._storylines + "/storyline_3_part_1.str";

        StreamReader SR = new StreamReader(path);
        string line = SR.ReadLine();
        while (line != null)
        {
            line = SR.ReadLine();

            if (line == action_current)
            {
                goto Fill;

            }
        }
        Fill:
        line = SR.ReadLine();
        while (line != action_next)
        {
            line = SR.ReadLine();
            _list_action_data.Add(line);
        }
        return true;
    }
    private Boolean Decompose_init()
    {
        for (int i = 0; i < _list_init_data.Count; i++)
        {
            string t = _list_init_data[i];
            if (t == _s_tag._skip)
            {
                continue;
            }
            else
            {
                if (t == _s_tag._version)
                {
                    _str_version = _list_init_data[i + 1];
                }
                if (t == _s_tag._required_objects)
                {
                    string line = _list_init_data[i + 1];
                    string[] units = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit in units)
                    {
                        _list_required_objects.Add(unit);
                    }
                }
                if (t == _s_tag._required_cg)
                {
                    string line = _list_init_data[i + 1];
                    string[] units = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit in units)
                    {
                        _list_required_CG.Add(unit);
                    }
                }
            }

        }
        return true;
    }

}
