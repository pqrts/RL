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
    public void Update()
    {
       
        Repaint();
    }
    public  void CreateGUI()
    {
  
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/control_panel.uxml");
        VisualElement VTuxml = VT.Instantiate();
        TextField action_number = new TextField();

        Label l_actions_total = VTuxml.Q<VisualElement>("total_actions") as Label;
        l_actions_total.text = "Total actions: " + s_target._id_action_total;
        
        Label l_action_current = VTuxml.Q<VisualElement>("current_action") as Label;
        l_action_current.text = "Current action: " + s_target._id_action;

        Label l_action_number = VTuxml.Q<VisualElement>("action_number") as Label;
        l_action_number.text = "Action �: ";

        Label l_info = VTuxml.Q<VisualElement>("info") as Label;
        l_info.text = "Info";

        Label l_moveto = VTuxml.Q<VisualElement>("move_to") as Label;
        l_moveto.text = "Move to action:";

        Label l_act_selection = VTuxml.Q<VisualElement>("act_selection") as Label;
        l_act_selection.text = "Switching";

        Label l_status_main = VTuxml.Q<VisualElement>("status") as Label;
        l_status_main.text = "Action status";

        Label l_status_file = VTuxml.Q<VisualElement>("status_file") as Label;
        l_status_file.text = "Create .str: ";

        Label l_status_cg = VTuxml.Q<VisualElement>("status_CG") as Label;
        l_status_cg.text = "Select CG: ";

        Label l_status_phrase = VTuxml.Q<VisualElement>("status_phrase") as Label;
        l_status_phrase.text = " Add phrase: ";

        Label l_status_author = VTuxml.Q<VisualElement>("status_author") as Label;
        l_status_author.text = "Set author: ";
     


        ////
        Label l_status_1 = VTuxml.Q<VisualElement>("status_1") as Label;
        if (s_target.Check_str_existence(s_target._str_name))
        {
            l_status_1.text = "Done";
        }
        else 
        {
            l_status_1.text = "Null";
        }
        Label l_status_2 = VTuxml.Q<VisualElement>("status_2") as Label;
        if (s_target._CG_sprite != null)
        {
            l_status_2.text = "Done";
        }
        else 
        {
            l_status_2.text = "Null";
        }
        Label l_status_3 = VTuxml.Q<VisualElement>("status_3") as Label;
        if (s_target._phrase != "")
        {
            l_status_3.text = "Done";
        }
        else
        {
            l_status_3.text = "Null";
        }
        Label l_status_4 = VTuxml.Q<VisualElement>("status_4") as Label;
        if (s_target._phrase_author != "")
        {
            l_status_4.text = "Done";
        }
        else
        {
            l_status_4.text = "Null";
        }


        ///

        Button move_to = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                s_target.Select_action(int.Parse(action_number.value));
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        move_to.text = "Move";

        Button next_action = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        next_action.text = "Next Action";

        Button previous_action = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        previous_action.text = "Previous Action";

      


        VTuxml.Q<VisualElement>("moveto_buttonHolder").Add(move_to);
        VTuxml.Q<VisualElement>("next_action_Holder").Add(next_action);
        VTuxml.Q<VisualElement>("previous_action_Holder").Add(previous_action);
        VTuxml.Q<VisualElement>("moveto_fieldHolder").Add(action_number);
        rootVisualElement.Add(VTuxml);
    }

}