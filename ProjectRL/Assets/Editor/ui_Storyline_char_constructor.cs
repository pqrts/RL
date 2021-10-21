using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using StorylineEditor;

public class ui_Storyline_char_constructor : EditorWindow
{
    private ext_StorylineEd s_target;
    private TextField _field_tech_name;
    private TextField _field_runtime_name;

    private static string _f_type_runtime_name = "runtime_name";
    private static string _f_type_tech_name = "tech_name";

    private string _value_runtime_name;
    private string _value_tech_name;

    private Label l_status_tech_name;
    private Label l_status_runtime_name;



    public static ui_Storyline_char_constructor ShowWindow()
    {
        ui_Storyline_char_constructor window_char_constr = GetWindow<ui_Storyline_char_constructor>();
        window_char_constr.titleContent = new GUIContent("Character Constructor");
        window_char_constr.minSize = new Vector2(340, 475f);
        window_char_constr.maxSize = new Vector2(340f, 475f);
        return window_char_constr;

    }
    private void OnEnable()
    {
        s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
    }
    private void Update()
    {
        if (s_target._update_ui_char_contructor == true)
        {
            CreateGUI();
        }
    }
    public void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/character_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();


        l_status_runtime_name = VTuxml.Q<VisualElement>("status_char_runtime_name") as Label;
        l_status_tech_name = VTuxml.Q<VisualElement>("status_char_technical_name") as Label;
        
        _field_tech_name = new TextField();
        _field_runtime_name = new TextField();

        _field_tech_name.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_values(_field_tech_name.value, StrFieldType.TechName));
        _field_runtime_name.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_values(_field_runtime_name.value, StrFieldType.RuntimeName));

        Button select_body = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
         
                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        select_body.text = "Select";

        Button select_clothes = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        select_clothes.text = "Select";

        Button select_haircut = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
               
                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        select_haircut.text = "Select";

        Button select_makeup = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        select_makeup.text = "Select";


        VTuxml.Q<VisualElement>("tech_name_fieldHolder").Add(_field_tech_name);
        VTuxml.Q<VisualElement>("runtime_name_fieldHolder").Add(_field_runtime_name);
        VTuxml.Q<VisualElement>("character_body_buttonHolder").Add(select_body);
        VTuxml.Q<VisualElement>("character_clothes_buttonHolder").Add(select_clothes);
        VTuxml.Q<VisualElement>("character_haircut_buttonHolder").Add(select_haircut);
        VTuxml.Q<VisualElement>("character_makeup_buttonHolder").Add(select_makeup);

        rootVisualElement.Add(VTuxml);
        s_target._update_ui_char_contructor = false;
    }


    private void Set_values(string field_value, StrFieldType field_type)
    {
        if (field_value != "")
        {
            if (field_type == StrFieldType.RuntimeName)
            {

                _value_runtime_name = field_value;

                l_status_runtime_name.text = _value_runtime_name;
            }
            if (field_type == StrFieldType.TechName)
            {
                _value_tech_name = field_value;
                l_status_tech_name.text = _value_tech_name;
            }
        }
        Repaint();
    }

    private void Set_preview_component(string component_name, StrPreviewComponentType component_type)
    { 
    
    }
}
