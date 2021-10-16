using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class ui_Storyline_choise : EditorWindow
{
    public static ui_Storyline_choise ShowWindow()
    {
        ui_Storyline_choise window_choise = GetWindow<ui_Storyline_choise>();
        window_choise.titleContent = new GUIContent("Choise Constructor");
        window_choise.minSize = new Vector2(170, 475f);
        window_choise.maxSize = new Vector2(170f, 475f);

        return window_choise;

    }
    public void CreateGUI()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/choise_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();


        Label l_options = VTuxml.Q<VisualElement>("options") as Label;
        l_options.text = "Options: ";

        Label l_cost = VTuxml.Q<VisualElement>("cost") as Label;
        l_cost.text = "Cost (and type): ";

        Label l_cost_value = VTuxml.Q<VisualElement>("cost_label") as Label;
        l_cost_value.text = "Cost: ";

        Label l_opt_text = VTuxml.Q<VisualElement>("opt_text") as Label;
        l_opt_text.text = "Text: ";

        Label l_jump_to = VTuxml.Q<VisualElement>("jump_to") as Label;
        l_jump_to.text = "Jump to action: ";

        Label l_jump_number = VTuxml.Q<VisualElement>("jump_label") as Label;
        l_jump_number.text = "Action ¹: ";

        Label l_give_item = VTuxml.Q<VisualElement>("give_item") as Label;
        l_give_item.text = "Give item: ";

        Label l_item_id = VTuxml.Q<VisualElement>("give_label") as Label;
        l_item_id.text = "Item ID: ";

        Label l_edit_opt = VTuxml.Q<VisualElement>("edit_opt") as Label;
        l_edit_opt.text = "Editing options: ";




        rootVisualElement.Add(VTuxml);
    }
}
