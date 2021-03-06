using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
public class StrEditorControlPanelWindow : EditorWindow
{
    private Label _l_ActionsTotal;
    private Label _l_ActionCurrent;
    private Label _l_ActionNumber;
    private Label _l_Info;
    private Label _l_MoveTo;
    private Label _l_ActionSelection;
    private Label _l_StatusMain;
    private Label _l_StatusFile;
    private Label _l_StatusCG;
    private Label _l_StatusPhrase;
    private Label _l_StatusAuthor;
    private Label _l_StatusAuthorName;
    private Label _l_StatusSteps;
    private Label _l_StatusCheck;

    private Label _l_Status1;
    private Label _l_Status2;
    private Label _l_Status3;
    private Label _l_Status4;
    private Label _l_Status5;
    private Label _l_Status6;

    private StrEditorGodObject _s_StorylineEditor;
    private StrEditorEvents _s_StrEvent;

    public static StrEditorControlPanelWindow ShowWindow()
    {
        StrEditorControlPanelWindow window_control = GetWindow<StrEditorControlPanelWindow>();
        window_control.titleContent = new GUIContent("Control panel");
        window_control.minSize = new Vector2(170, 475f);
        window_control.maxSize = new Vector2(170f, 475f);
        return window_control;

    }
    private void OnEnable()
    {
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StorylineEditor = (StrEditorGodObject)FindObjectOfType(typeof(StrEditorGodObject));
        _s_StrEvent.StrEditorUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
     
        _l_ActionsTotal.text = "Total actions: " + _s_StorylineEditor._totalActions.ToString();
        _l_ActionCurrent.text = "Current action: " + _s_StorylineEditor._actionID.ToString();
        _l_ActionNumber.text = "Action ?: ";
        _l_Info.text = "Info";
        _l_MoveTo.text = "Move to action:";
        _l_ActionSelection.text = "Navigation";
        _l_StatusMain.text = "Required: ";
        _l_StatusFile.text = "Create .str: ";
        _l_StatusCG.text = "Select CG: ";
        _l_StatusPhrase.text = " Add phrase: ";
        _l_StatusAuthor.text = "Set author: ";
        _l_StatusAuthorName.text = "Author: ";
        _l_StatusSteps.text = "Create step: ";
        if (_s_StorylineEditor.CheckStorylineExistence(_s_StorylineEditor._StorylineName))
        {
            _l_Status1.text = "Done";
        }
        else
        {
            _l_Status1.text = "----";
        }

        if (_s_StorylineEditor._CGsprite != null)
        {
            _l_Status2.text = "Done";
        }
        else
        {
            _l_Status2.text = "----";
        }

        if (_s_StorylineEditor._phrase != "")
        {
            _l_Status3.text = "Done";
        }
        else
        {
            _l_Status3.text = "----";
        }


        if (_s_StorylineEditor._phraseAuthor != "")
        {
            _l_Status4.text = "Done";
        }
        else
        {
            _l_Status4.text = "----";
        }

        _l_Status5.text = _s_StorylineEditor._phraseAuthor;

        if (_s_StorylineEditor._totalStepsCount.Count != 0)
        {
            _l_Status6.text = "Done";
        }
        else
        {
            _l_Status6.text = "----";
        }

        if (_l_Status1.text == "Done" && _l_Status2.text == "Done" && _l_Status3.text == "Done" && _l_Status4.text == "Done" && _l_Status6.text == "Done")
        {
            _l_StatusCheck.text = "Ready for next action";
            _s_StorylineEditor._readyForNextAction = true;
        }
        else
        {
            _l_StatusCheck.text = "Not ready ";
            _s_StorylineEditor._readyForNextAction = false;
        }
        Repaint();
    }

    private void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/control_panel.uxml");
        VisualElement VTuxml = VT.Instantiate();
        TextField _field_ActionNumber = new TextField();



        _l_ActionsTotal = VTuxml.Q<VisualElement>("total_actions") as Label;
        _l_ActionCurrent = VTuxml.Q<VisualElement>("current_action") as Label;
        _l_ActionNumber = VTuxml.Q<VisualElement>("action_number") as Label;
        _l_Info = VTuxml.Q<VisualElement>("info") as Label;
        _l_MoveTo = VTuxml.Q<VisualElement>("move_to") as Label;
        _l_ActionSelection = VTuxml.Q<VisualElement>("act_selection") as Label;
        _l_StatusMain = VTuxml.Q<VisualElement>("status") as Label;
        _l_StatusFile = VTuxml.Q<VisualElement>("status_file") as Label;
        _l_StatusCG = VTuxml.Q<VisualElement>("status_CG") as Label;
        _l_StatusPhrase = VTuxml.Q<VisualElement>("status_phrase") as Label;
        _l_StatusAuthor = VTuxml.Q<VisualElement>("status_author") as Label;
        _l_StatusAuthorName = VTuxml.Q<VisualElement>("status_name") as Label;
        _l_StatusSteps = VTuxml.Q<VisualElement>("status_steps") as Label;
        _l_StatusCheck = VTuxml.Q<VisualElement>("status_check") as Label;
        ////

        _l_Status1 = VTuxml.Q<VisualElement>("status_1") as Label;
        _l_Status2 = VTuxml.Q<VisualElement>("status_2") as Label;
        _l_Status3 = VTuxml.Q<VisualElement>("status_3") as Label;
        _l_Status4 = VTuxml.Q<VisualElement>("status_4") as Label;
        _l_Status5 = VTuxml.Q<VisualElement>("status_5") as Label;
        _l_Status6 = VTuxml.Q<VisualElement>("status_6") as Label;
        ///

        

        Button b_MoveTo = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StorylineEditor.SelectAction(int.Parse(_field_ActionNumber.value));
                _s_StrEvent.EditorUpdated();
            }
        });
        b_MoveTo.text = "Move";

        Button _b_NextAction = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_NextAction.text = "Next Action";

        Button _b_PreviousAction = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_PreviousAction.text = "Previous Action";

        VTuxml.Q<VisualElement>("moveto_buttonHolder").Add(b_MoveTo);
        VTuxml.Q<VisualElement>("next_action_Holder").Add(_b_NextAction);
        VTuxml.Q<VisualElement>("previous_action_Holder").Add(_b_PreviousAction);
        VTuxml.Q<VisualElement>("moveto_fieldHolder").Add(_field_ActionNumber);
        rootVisualElement.Add(VTuxml);
    }
    private Boolean ValidateStoryline()
    {
        if (_s_StorylineEditor.CheckStorylineExistence(_s_StorylineEditor._StorylineName))
        {
            return true;
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            return false;
        }
    }
    private void OnDisable()
    {
        _s_StrEvent.StrEditorUpdated -= OnStrEdUpdated;
    }
}