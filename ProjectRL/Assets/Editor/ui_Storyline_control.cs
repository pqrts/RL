using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
public class ui_Storyline_control : EditorWindow
{
    Sprite preview_body;
    Sprite preview_haircut;
    Sprite preview_clothes;
    Sprite preview_makeup;
    string char_name;
    string char_descr;
    public static ui_Storyline_control ShowWindow()
    {
        ui_Storyline_control window_control = GetWindow<ui_Storyline_control>();
        window_control.titleContent = new GUIContent("Control panel");
        window_control.minSize = new Vector2(170, 500f);
        window_control.maxSize = new Vector2(170f, 500f);
        return window_control;

    }
    private void CreateGUI()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/control_panel.uxml");
        VisualElement VTuxml = VT.Instantiate();

        Label l_actions_total = VTuxml.Q<VisualElement>("total_actions") as Label;
        l_actions_total.text = "Total actions: " + s_target._id_action_total;
        
        Label l_action_current = VTuxml.Q<VisualElement>("current_action") as Label;
        l_action_current.text = "Current action: " + s_target._id_action;

        Label l_action_number = VTuxml.Q<VisualElement>("action_number") as Label;
        l_action_number.text = "Action ¹: ";

        Button move_to = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        move_to.text = "Move to: ";

        TextField action_number = new TextField();


        VTuxml.Q<VisualElement>("moveto_buttonHolder").Add(move_to);
        VTuxml.Q<VisualElement>("moveto_fieldHolder").Add(action_number);
        rootVisualElement.Add(VTuxml);
    }
}