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

    private Sprite _preview_body;
    private Sprite _preview_haircut;
    private Sprite _preview_clothes;
    private Sprite _preview_makeup;

    private VisualElement _preview_body_holder;

    private string _value_runtime_name;
    private string _value_tech_name;

    private Label l_status_tech_name;
    private Label l_status_runtime_name;
    private Label l_status_body;



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
        l_status_body = VTuxml.Q<VisualElement>("status_character_body") as Label;


        _field_tech_name = new TextField();
        _field_runtime_name = new TextField();

        _field_tech_name.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_values(_field_tech_name.value, StrFieldType.TechName));
        _field_runtime_name.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_values(_field_runtime_name.value, StrFieldType.RuntimeName));

        Button select_body = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", s_target._s_folder._body, "png");

                Set_preview_component(path, StrPreviewComponentType.Body);
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

      

        if (_preview_body != null)
        {
            VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _preview_body.texture;
       
            l_status_body.text = _preview_body.name;
        }

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

    private void Set_preview_component(string component_path, StrPreviewComponentType component_type)
    {

        string path = null;
        string preview_component_name = null;
        string preview_component_resources_path = null;
        if (component_type == StrPreviewComponentType.Body)
        {
            if (component_path.Length != 0)
            {
                preview_component_name = Get_component_name(component_path, "Char_body");
                preview_component_resources_path = Get_component_resources_path(s_target._s_folder._body, preview_component_name);
                _preview_body = Create_preview_component(preview_component_resources_path, preview_component_name);
                Debug.Log(_preview_body.name + "+" + preview_component_name);
               
                CreateGUI();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Select sprite", "OK");
            }
        }

    }
    private string Get_component_name(string source_string, string removed_part)
    {
        string t = source_string.Replace(s_target._s_folder._root + "/Resources/", "");
        string t2 = t.Replace(".png", "");
        string m = "Gamedata/Textures/" + removed_part + "/";
        string component_name = t2.Replace(m, "");
        return component_name;
    }
    private string Get_component_resources_path(string component_folder, string component_name)
    {
        Debug.Log(component_name);
        string res_path = component_folder.Replace(s_target._s_folder._root + "/Resources/", "") + "/" + component_name;
        return res_path;
    }
    private Sprite Create_preview_component(string component_resources_path, string component_name)
    {
        Debug.Log(component_resources_path);
        Texture2D tex;
        tex = Resources.Load(component_resources_path) as Texture2D;
        Sprite preview_sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        preview_sprite.name = component_name;
        return preview_sprite;
    }
}

