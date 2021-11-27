using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using System.IO;

[RequireComponent(typeof(TaglistReader))]
[RequireComponent(typeof(StrEditorGodObject))]
[ExecuteInEditMode]
public class StrEditorReplacer : MonoBehaviour
{
    private TaglistReader _tags;
    private StrEditorGodObject _StrEditorRoot;
    private int _decomposedStepsCount;
    public List<string> _beforeSelectedData = new List<string>();
    public List<string> _afterSelectedData = new List<string>();
    public List<string> _selectedActionData = new List<string>();
    public List<string> _selectedActionSteps = new List<string>();


    public Boolean GetRequieredComponents()
    {
        _StrEditorRoot = GetComponent<StrEditorGodObject>();
        _tags = GetComponent<TaglistReader>();
        return true;
    }
    public List<string> GetSelectedActionData(int selectedActionID)
    {

        int k = 0;
        int f = 0;
        _selectedActionData.Clear();
        _selectedActionSteps.Clear();
        _beforeSelectedData.Clear();
        _afterSelectedData.Clear();

        int nextActionID = selectedActionID + 1;
        string nextActionData = _tags._action + _tags._separator + nextActionID;
        string currentActionData = _tags._action + _tags._separator + selectedActionID;

        for (int i = 0; i < _StrEditorRoot._storylineActions.Count; i++)
        {
            if (_StrEditorRoot._storylineActions[i] != currentActionData)
            {
                _beforeSelectedData.Add(_StrEditorRoot._storylineActions[i]);
            }
            else
            {
                k = i;
                goto Selected;
            }

        }
        Selected:
        for (int r = k; r < _StrEditorRoot._storylineActions.Count; r++)
        {
            if (_StrEditorRoot._storylineActions[r] != nextActionData)
            {
                _selectedActionData.Add(_StrEditorRoot._storylineActions[r]);
            }
            else
            {
                f = r;
            
                goto After;
            }
        }
        After:
        for (int l = f; l < _StrEditorRoot._storylineActions.Count; l++)
        {
            _afterSelectedData.Add(_StrEditorRoot._storylineActions[l]);
        }
        return _selectedActionData;
    }
    private void DecomposeSelectedAction()
    {
        _decomposedStepsCount = 1;
        for (int i = 0; i < _selectedActionData.Count; i++)
        {
            string stepData = _tags._step + _tags._separator + _decomposedStepsCount;
            string nextStepData = _tags._step + _tags._separator + (_decomposedStepsCount + 1);
            string nextActionData = _tags._action + _tags._action + (_StrEditorRoot._actionID + 1);
            if (_selectedActionData[i] == stepData)
            {
                string step_raw = "";
                for (int e = i; e < _selectedActionData.Count; e++)
                {
                    if (_selectedActionData[e] != nextStepData || _selectedActionData[e] != "}")
                    {
                        string temp_tag_skip = "          " + _tags._skip;
                        if (_selectedActionData[e] != temp_tag_skip && _selectedActionData[e] != stepData && _selectedActionData[e] != "/&endstep" && _selectedActionData[e] != "}")
                        {
                            string t = _selectedActionData[e].Replace("          ", "");
                            step_raw = step_raw + t + _tags._separatorVertical;
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
        _StrEditorRoot.SelectedActionSetup();
    }




}