using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using System;

using System.IO;
[ExecuteInEditMode]
public class extStrEditorReplacer : MonoBehaviour
{
    global_taglist _s_Tag;
    ext_StorylineEditor _s_StorylineEditor;
    private int _decomposedStepsCount;

    public List<string> _beforeSelectedData = new List<string>();
    public List<string> _afterSelectedData = new List<string>();
    public List<string> _selectedActionData = new List<string>();
    public List<string> _selectedActionSteps = new List<string>();


    public Boolean GetScripts()
    {
        _s_StorylineEditor = GetComponent<ext_StorylineEditor>();
        _s_Tag = GetComponent<global_taglist>();
        return true;
    }
    public Boolean GetSelectedActionData()
    {

        int k = 0;
        int f = 0;
        _selectedActionData.Clear();
        _selectedActionSteps.Clear();
        _beforeSelectedData.Clear();
        _afterSelectedData.Clear();

        int nextActionID = _s_StorylineEditor._actionID + 1;
        string nextActionData = _s_Tag._action + _s_Tag._separator + nextActionID;
        string currentActionData = _s_Tag._action + _s_Tag._separator + _s_StorylineEditor._actionID;

        for (int i = 0; i < _s_StorylineEditor._actionsToStr.Count; i++)
        {
            if (_s_StorylineEditor._actionsToStr[i] != currentActionData)
            {
                _beforeSelectedData.Add(_s_StorylineEditor._actionsToStr[i]);
            }
            else
            {
                k = i;
                goto Selected;
            }

        }
        Selected:
        for (int r = k; r < _s_StorylineEditor._actionsToStr.Count; r++)
        {
            if (_s_StorylineEditor._actionsToStr[r] != nextActionData)
            {
                _selectedActionData.Add(_s_StorylineEditor._actionsToStr[r]);
            }
            else
            {
                f = r;
                DecomposeSelectedAction();
                goto After;
            }
        }
        After:
        for (int l = f; l < _s_StorylineEditor._actionsToStr.Count; l++)
        {
            _afterSelectedData.Add(_s_StorylineEditor._actionsToStr[l]);
        }
        return true;
    }
    private void DecomposeSelectedAction()
    {
        _decomposedStepsCount = 1;
        for (int i = 0; i < _selectedActionData.Count; i++)
        {
            string stepData = _s_Tag._step + _s_Tag._separator + _decomposedStepsCount;
            string nextStepData = _s_Tag._step + _s_Tag._separator + (_decomposedStepsCount + 1);
            string nextActionData = _s_Tag._action + _s_Tag._action + (_s_StorylineEditor._actionID + 1);
            if (_selectedActionData[i] == stepData)
            {
                string step_raw = "";
                for (int e = i; e < _selectedActionData.Count; e++)
                {
                    if (_selectedActionData[e] != nextStepData || _selectedActionData[e] != "}")
                    {
                        string temp_tag_skip = "          " + _s_Tag._skip;
                        if (_selectedActionData[e] != temp_tag_skip && _selectedActionData[e] != stepData && _selectedActionData[e] != "/&endstep" && _selectedActionData[e] != "}")
                        {
                            string t = _selectedActionData[e].Replace("          ", "");
                            step_raw = step_raw + t + _s_Tag._separator_vert;
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

                _selectedActionSteps.Add(step_raw);


                step_raw = "";
                _decomposedStepsCount += 1;

            }
        }
        _s_StorylineEditor.SelectedActionSetup();
    }

    


}