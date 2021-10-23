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
    private ext_StorylineEditor _s_StorylineEditor;
    private TextField _techNameField;
    private TextField _runtimeNameField;

    private Sprite _previewBody;
    private Sprite _previewHaircut;
    private Sprite _previewClothes;
    private Sprite _previewMakeup;

    private VisualElement _previewBodyHolder;

    private string _runtimeNameFieldValue;
    private string _techNameFieldValue;

    private Label _l_StatusTechName;
    private Label _l_StatusRuntimeName;
    private Label _l_StatusBody;

    private ext_StorylineEventSystem _s_StrEvent;

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
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent = (ext_StorylineEventSystem)FindObjectOfType(typeof(ext_StorylineEventSystem));
        _s_StrEvent.OnStrEdUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
        Repaint();
    }
    public void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/character_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();

        _l_StatusRuntimeName = VTuxml.Q<VisualElement>("status_char_runtime_name") as Label;
        _l_StatusTechName = VTuxml.Q<VisualElement>("status_char_technical_name") as Label;
        _l_StatusBody = VTuxml.Q<VisualElement>("status_character_body") as Label;

        _techNameField = new TextField();
        _runtimeNameField = new TextField();

        _techNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_techNameField.value, StrFieldType.TechName));
        _runtimeNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_runtimeNameField.value, StrFieldType.RuntimeName));

        Button _b_SelectBody = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._body, "png");
                SetPreviewComponent(path, StrPreviewComponentType.Body);
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectBody.text = "Select";

        Button _b_SelectClothes = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._clothes, "png");
                SetPreviewComponent(path, StrPreviewComponentType.Body);
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectClothes.text = "Select";

        Button _b_SelectHaircut = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectHaircut.text = "Select";

        Button _b_SelectMakeup = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectMakeup.text = "Select";

        if (_previewBody != null)
        {
            VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _previewBody.texture;

            _l_StatusBody.text = _previewBody.name;
        }

        VTuxml.Q<VisualElement>("tech_name_fieldHolder").Add(_techNameField);
        VTuxml.Q<VisualElement>("runtime_name_fieldHolder").Add(_runtimeNameField);
        VTuxml.Q<VisualElement>("character_body_buttonHolder").Add(_b_SelectBody);
        VTuxml.Q<VisualElement>("character_clothes_buttonHolder").Add(_b_SelectClothes);
        VTuxml.Q<VisualElement>("character_haircut_buttonHolder").Add(_b_SelectHaircut);
        VTuxml.Q<VisualElement>("character_makeup_buttonHolder").Add(_b_SelectMakeup);
        rootVisualElement.Add(VTuxml);
    }

    private void SetValues(string FieldValue, StrFieldType FieldType)
    {
        if (FieldValue != "")
        {
            if (FieldType == StrFieldType.RuntimeName)
            {
                _runtimeNameFieldValue = FieldValue;
                _l_StatusRuntimeName.text = _runtimeNameFieldValue;
            }
            if (FieldType == StrFieldType.TechName)
            {
                _techNameFieldValue = FieldValue;
                _l_StatusTechName.text = _techNameFieldValue;
            }
        }
        Repaint();
    }

    private void SetPreviewComponent(string ComponentPath, StrPreviewComponentType ComponentType)
    {

        string path = null;
        string PreviewComponentName = null;
        string PreviewComponentResourcesPath = null;
        if (ComponentPath.Length != 0)
        {
            if (ComponentType == StrPreviewComponentType.Body)
            {
                PreviewComponentName = GetComponentName(ComponentPath, "Char_body");
                PreviewComponentResourcesPath = GetComponentResourcesPath(_s_StorylineEditor._s_Folder._body, PreviewComponentName);
                _previewBody = CreatePreviewComponent(PreviewComponentResourcesPath, PreviewComponentName);
                Debug.Log(_previewBody.name + "+" + PreviewComponentName);
                CreateGUI();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select sprite", "OK");
        }

    }
    private string GetComponentName(string ComponentPath, string RemovedPart)
    {
        string temp = ComponentPath.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
        string temp2 = temp.Replace(".png", "");
        string ToReplace = "Gamedata/Textures/" + RemovedPart + "/";
        string ComponentName = temp2.Replace(ToReplace, "");
        return ComponentName;
    }
    private string GetComponentResourcesPath(string ComponentFolder, string ComponentName)
    {
        Debug.Log(ComponentName);
        string ResourcesPath = ComponentFolder.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "") + "/" + ComponentName;
        return ResourcesPath;
    }
    private Sprite CreatePreviewComponent(string ComponentResourcesPath, string ComponentName)
    {
        Debug.Log(ComponentResourcesPath);
        Texture2D tex;
        tex = Resources.Load(ComponentResourcesPath) as Texture2D;
        Sprite PreviewSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        PreviewSprite.name = ComponentName;
        return PreviewSprite;
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

