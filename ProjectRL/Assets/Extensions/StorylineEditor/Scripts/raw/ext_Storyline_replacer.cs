using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using System;

using System.IO;
[ExecuteInEditMode]
public class ext_Storyline_replacer : MonoBehaviour
{
    global_taglist _s_tag;
    ext_StorylineEd _s_str_ed;
    private int _id_decomposed_steps;

    public List<string> _list_before_selected_data = new List<string>();
    public List<string> _list_after_selected_data = new List<string>();
    public List<string> _list_selected_action_data = new List<string>();
    public List<string> _list_selected_action_steps = new List<string>();


    public Boolean Get_scripts()
    {
        _s_str_ed = GetComponent<ext_StorylineEd>();
        _s_tag = GetComponent<global_taglist>();
        return true;
    }
    public Boolean Get_selected_action_data()
    {

        int k = 0;
        int f = 0;
        _list_selected_action_data.Clear();
        _list_selected_action_steps.Clear();
        _list_before_selected_data.Clear();
        _list_after_selected_data.Clear();

        int id_action_next = _s_str_ed._id_action + 1;
        string action_next = _s_tag._action + _s_tag._separator + id_action_next;
        string action_current = _s_tag._action + _s_tag._separator + _s_str_ed._id_action;

        for (int i = 0; i < _s_str_ed._actions_to_str.Count; i++)
        {
            if (_s_str_ed._actions_to_str[i] != action_current)
            {
                _list_before_selected_data.Add(_s_str_ed._actions_to_str[i]);
            }
            else
            {
                k = i;
                goto Selected;
            }

        }
        Selected:
        for (int r = k; r < _s_str_ed._actions_to_str.Count; r++)
        {
            if (_s_str_ed._actions_to_str[r] != action_next)
            {
                _list_selected_action_data.Add(_s_str_ed._actions_to_str[r]);
            }
            else
            {
                f = r;
                Decompose_selected_action();
                goto After;
            }
        }
        After:
        for (int l = f; l < _s_str_ed._actions_to_str.Count; l++)
        {
            _list_after_selected_data.Add(_s_str_ed._actions_to_str[l]);
        }
        return true;
    }
    private void Decompose_selected_action()
    {
        _id_decomposed_steps = 1;
        for (int i = 0; i < _list_selected_action_data.Count; i++)
        {
            string step_unit = _s_tag._step + _s_tag._separator + _id_decomposed_steps;
            string step_unit_next = _s_tag._step + _s_tag._separator + (_id_decomposed_steps + 1);
            string action_unit_next = _s_tag._action + _s_tag._action + (_s_str_ed._id_action + 1);
            if (_list_selected_action_data[i] == step_unit)
            {
                string step_raw = "";
                for (int e = i; e < _list_selected_action_data.Count; e++)
                {
                    if (_list_selected_action_data[e] != step_unit_next || _list_selected_action_data[e] != "}")
                    {
                        string temp_tag_skip = "          " + _s_tag._skip;
                        if (_list_selected_action_data[e] != temp_tag_skip && _list_selected_action_data[e] != step_unit && _list_selected_action_data[e] != "/&endstep" && _list_selected_action_data[e] != "}")
                        {
                            string t = _list_selected_action_data[e].Replace("          ", "");
                            step_raw = step_raw + t + _s_tag._separator_vert;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                _list_selected_action_steps.Add(step_raw);


                step_raw = "";
                _id_decomposed_steps += 1;

            }
        }
        _s_str_ed.Selected_action_setup();
    }

    


}