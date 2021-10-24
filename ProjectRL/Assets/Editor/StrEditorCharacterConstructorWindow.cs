using StorylineEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class StrEditorCharacterConstructorWindow : EditorWindow
{
    private ext_StorylineEditor _s_StorylineEditor;
    private TextField _techNameField;
    private TextField _runtimeNameField;
    private TextField _characterDescriptionField;

    private Sprite _previewBody;
    private Sprite _previewHaircut;
    private Sprite _previewClothes;
    private Sprite _previewMakeup;

    private string _runtimeNameFieldValue;
    private string _techNameFieldValue;
    private string _characterDescriptionFieldValue;

    private Label _l_StatusTechName;
    private Label _l_StatusRuntimeName;
    private Label _l_StatusCharacterDescription;
    private Label _l_StatusBody;
    private Label _l_StatusClothes;
    private Label _l_StatusHaircut;
    private Label _l_StatusMakeup;
    private List<string> _characterComponentsToFile = new List<string>();
    private const string _placeholderText = "----";

    private ext_StorylineEventSystem _s_StrEvent;

    public static StrEditorCharacterConstructorWindow ShowWindow()
    {
        StrEditorCharacterConstructorWindow window_char_constr = GetWindow<StrEditorCharacterConstructorWindow>();
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
        _l_StatusCharacterDescription = VTuxml.Q<VisualElement>("status_char_description") as Label;
        _l_StatusBody = VTuxml.Q<VisualElement>("status_character_body") as Label;
        _l_StatusClothes = VTuxml.Q<VisualElement>("status_character_clothes") as Label;
        _l_StatusHaircut = VTuxml.Q<VisualElement>("status_character_haircut") as Label;
        _l_StatusMakeup = VTuxml.Q<VisualElement>("status_character_makeup") as Label;

        _techNameField = new TextField();
        _runtimeNameField = new TextField();
        _characterDescriptionField = new TextField();
        _characterDescriptionField.style.height = 110f;
        _characterDescriptionField.multiline = true;
        _characterDescriptionField.maxLength = 145;
        _characterDescriptionField.style.whiteSpace = WhiteSpace.Normal;

        _techNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_techNameField.value, StrFieldType.TechName));
        _runtimeNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_runtimeNameField.value, StrFieldType.RuntimeName));
        _characterDescriptionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_characterDescriptionField.value, StrFieldType.Description));

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
                SetPreviewComponent(path, StrPreviewComponentType.Clothes);
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectClothes.text = "Select";

        Button _b_SelectHaircut = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._haircut, "png");
                SetPreviewComponent(path, StrPreviewComponentType.Haircut);
                _s_StrEvent.EditorUpdated();

            }
        });
        _b_SelectHaircut.text = "Select";

        Button _b_SelectMakeup = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._makeup, "png");
                SetPreviewComponent(path, StrPreviewComponentType.Makeup);
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectMakeup.text = "Select";

        if (_previewBody != null)
        {
            VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _previewBody.texture;

            _l_StatusBody.text = _previewBody.name;
        }
        if (_previewClothes != null)
        {
            VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _previewClothes.texture;

            _l_StatusClothes.text = _previewClothes.name;
        }
        if (_previewHaircut != null)
        {
            VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _previewHaircut.texture;

            _l_StatusHaircut.text = _previewHaircut.name;
        }
        if (_previewMakeup != null)
        {
            VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _previewMakeup.texture;

            _l_StatusMakeup.text = _previewMakeup.name;
        }

        Button _b_ExportToFile = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (EditorUtility.DisplayDialog("Notice", " Are you sure about this?", "OK", "Cancel"))
                {
                    CharacterAssembly();
                    _s_StrEvent.EditorUpdated();
                }
            }
        });
        _b_ExportToFile.text = "Export to file";

        VTuxml.Q<VisualElement>("tech_name_fieldHolder").Add(_techNameField);
        VTuxml.Q<VisualElement>("runtime_name_fieldHolder").Add(_runtimeNameField);
        VTuxml.Q<VisualElement>("description_fieldHolder").Add(_characterDescriptionField);
        VTuxml.Q<VisualElement>("character_body_buttonHolder").Add(_b_SelectBody);
        VTuxml.Q<VisualElement>("character_clothes_buttonHolder").Add(_b_SelectClothes);
        VTuxml.Q<VisualElement>("character_haircut_buttonHolder").Add(_b_SelectHaircut);
        VTuxml.Q<VisualElement>("character_makeup_buttonHolder").Add(_b_SelectMakeup);
        VTuxml.Q<VisualElement>("buttonHolder1").Add(_b_ExportToFile);
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
            if (FieldType == StrFieldType.Description)
            {
                _characterDescriptionFieldValue = FieldValue;
                _l_StatusCharacterDescription.text = _characterDescriptionFieldValue;
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
            }
            if (ComponentType == StrPreviewComponentType.Clothes)
            {
                PreviewComponentName = GetComponentName(ComponentPath, "Char_clothes");
                PreviewComponentResourcesPath = GetComponentResourcesPath(_s_StorylineEditor._s_Folder._clothes, PreviewComponentName);
                _previewClothes = CreatePreviewComponent(PreviewComponentResourcesPath, PreviewComponentName);
                Debug.Log(_previewClothes.name + "+" + PreviewComponentName);
            }
            if (ComponentType == StrPreviewComponentType.Haircut)
            {
                PreviewComponentName = GetComponentName(ComponentPath, "Char_haircut");
                PreviewComponentResourcesPath = GetComponentResourcesPath(_s_StorylineEditor._s_Folder._haircut, PreviewComponentName);
                _previewHaircut = CreatePreviewComponent(PreviewComponentResourcesPath, PreviewComponentName);
                Debug.Log(_previewHaircut.name + "+" + PreviewComponentName);
            }
            if (ComponentType == StrPreviewComponentType.Makeup)
            {
                PreviewComponentName = GetComponentName(ComponentPath, "Char_makeup");
                PreviewComponentResourcesPath = GetComponentResourcesPath(_s_StorylineEditor._s_Folder._makeup, PreviewComponentName);
                _previewMakeup = CreatePreviewComponent(PreviewComponentResourcesPath, PreviewComponentName);
                Debug.Log(_previewMakeup.name + "+" + PreviewComponentName);

            }
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select sprite", "OK");
        }

    }
    private string GetComponentName(string ComponentPath, string RemovedFolder)
    {
        string temp = ComponentPath.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
        string temp2 = temp.Replace(".png", "");
        string ToReplace = "Gamedata/Textures/" + RemovedFolder + "/";
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
    private void CharacterAssembly()
    {
       
        Label[] requiredFields = new Label[] { _l_StatusTechName, _l_StatusRuntimeName, _l_StatusCharacterDescription, _l_StatusBody, _l_StatusClothes, _l_StatusHaircut, _l_StatusMakeup };
        if (ValidateRequiredFieldsValues(requiredFields))
        {
            foreach (Label unit in requiredFields)
            {
                _characterComponentsToFile.Add(unit.text);
            }
            if (ExportCharacterToFile())
            {
                _characterComponentsToFile.Clear();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Fill all fields", "OK");
        }
    }
    private Boolean ValidateRequiredFieldsValues(Label[] RequiredFilds)
    {

        foreach (Label unit in RequiredFilds)
        {
            if (unit.text == _placeholderText)
            {
                return false;
            }
        }
        return true;
    }
    private Boolean ExportCharacterToFile()
    {
        try
        {
            string FileName = _l_StatusTechName.text +".char";
            StreamWriter SW = new StreamWriter(_s_StorylineEditor._s_Folder._characters + "/" + FileName, true, encoding: System.Text.Encoding.Unicode);
            foreach (string unit in _characterComponentsToFile)
            { 
               SW.WriteLine(unit);
            }
            SW.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return true;
    }
    private void OnDisable()
    {
        _s_StrEvent.OnStrEdUpdated -= OnStrEdUpdated;
    }
}

