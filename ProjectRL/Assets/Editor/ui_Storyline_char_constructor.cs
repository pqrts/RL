using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class ui_Storyline_char_constructor : EditorWindow
{
    private ext_StorylineEd s_target;
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

        rootVisualElement.Add(VTuxml);
        s_target._update_ui_char_contructor = false;
    }
}