using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
public class ui_Storyline_control : EditorWindow
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

    private ext_StorylineEditor _s_StorylineEditor;
    private ext_StorylineEventSystem _s_StrEvent;

    public static ui_Storyline_control ShowWindow()
    {
        ui_Storyline_control window_control = GetWindow<ui_Storyline_control>();
        window_control.titleContent = new GUIContent("Control panel");
        window_control.minSize = new Vector2(170, 475f);
        window_control.maxSize = new Vector2(170f, 475f);

        return window_control;

    }
    private void OnEnable()
    {
        _s_StrEvent = (ext_StorylineEventSystem)FindObjectOfType(typeof(ext_StorylineEventSystem));
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent.OnStrEdUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        if (ResetValues())
        {
            CreateGUI();
            Repaint();
        }
    }
    private Boolean ResetValues()
    {
        _l_ActionsTotal.text = "Total actions: " + _s_StorylineEditor._IDActionsTotal;
        _l_ActionCurrent.text = "Current action: " + _s_StorylineEditor._IDAction;
        _l_ActionNumber.text = "Action ¹: ";
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

        if (_s_StorylineEditor._CGSprite != null)
        {
            _l_Status2.text = "Done";
        }
        else
        {
            _l_Status2.text = "----";
        }

        if (_s_StorylineEditor._Phrase != "")
        {
            _l_Status3.text = "Done";
        }
        else
        {
            _l_Status3.text = "----";
        }


        if (_s_StorylineEditor._PhraseAuthor != "")
        {
            _l_Status4.text = "Done";
        }
        else
        {
            _l_Status4.text = "----";
        }

        _l_Status5.text = _s_StorylineEditor._PhraseAuthor;

        if (_s_StorylineEditor._IDStepsTotal.Count != 0)
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
            _s_StorylineEditor._ready_for_next_action = true;
        }
        else
        {
            _l_StatusCheck.text = "Not ready ";
            _s_StorylineEditor._ready_for_next_action = false;
        }
        return true;
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
        _s_StrEvent.OnStrEdUpdated -= OnStrEdUpdated;
    }
}