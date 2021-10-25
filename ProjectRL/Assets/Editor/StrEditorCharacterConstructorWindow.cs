using StorylineEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
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

    private Label _statusTechNameLabel;
    private Label _statusRuntimeNameLabel;
    private Label _statusCharacterDescriptionLabel;
    private Label _statusBodyLabel;
    private Label _statusClothesLabel;
    private Label _statusHaircutLabel;
    private Label _statusMakeupLabel;
    private List<string> _characterComponentsToFile = new List<string>();
    private const string _placeholderText = "----";
    private Dictionary<StrFieldType, Label> _requiredFieldsLabels = new Dictionary<StrFieldType, Label>();
    private Dictionary<StrPreviewElementType, Sprite> _requiredPreviewElementsSprites = new Dictionary<StrPreviewElementType, Sprite>();
    private StrEditorEvents _s_StrEvent;

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
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StrEvent.StrEditorUpdated += OnStrEditorUpdated;
       
    }
    private void OnStrEditorUpdated()
    {
        CreateGUI();
        Repaint();
    }
    private void SetupRequiredElementsDictionary()
    {

        if (_requiredFieldsLabels.Count == 0)
        {
            _requiredFieldsLabels.Add(StrFieldType.TechNameField, _statusTechNameLabel);
            _requiredFieldsLabels.Add(StrFieldType.RuntimeNameField, _statusRuntimeNameLabel);
            _requiredFieldsLabels.Add(StrFieldType.CharacterDescriptionField, _statusCharacterDescriptionLabel);
        }

        if (_requiredPreviewElementsSprites.Count == 0)
        {
            if (_previewBody == null)
            {
                _previewBody = _s_StorylineEditor._tempCharIcon;
            }
            if (_previewClothes == null)
            {
                _previewClothes = _s_StorylineEditor._tempCharIcon;
            }
            if (_previewHaircut == null)
            {
                _previewHaircut = _s_StorylineEditor._tempCharIcon;
            }
            if (_previewMakeup == null)
            {
                _previewMakeup = _s_StorylineEditor._tempCharIcon;
            }
            _requiredPreviewElementsSprites.Add(StrPreviewElementType.Body, _previewBody);
            _requiredPreviewElementsSprites.Add(StrPreviewElementType.Clothes, _previewClothes);
            _requiredPreviewElementsSprites.Add(StrPreviewElementType.Haircut, _previewHaircut);
            _requiredPreviewElementsSprites.Add(StrPreviewElementType.Makeup, _previewMakeup);
        }

        foreach (KeyValuePair<StrPreviewElementType, Sprite> unit in _requiredPreviewElementsSprites)
        {
            if (unit.Value != null)
            {
                Debug.Log(unit.Value.name);
            }
        }


    }
    public void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/character_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();

        _statusRuntimeNameLabel = VTuxml.Q<VisualElement>("status_char_runtime_name") as Label;
        _statusTechNameLabel = VTuxml.Q<VisualElement>("status_char_technical_name") as Label;
        _statusCharacterDescriptionLabel = VTuxml.Q<VisualElement>("status_char_description") as Label;
        _statusBodyLabel = VTuxml.Q<VisualElement>("status_character_body") as Label;
        _statusClothesLabel = VTuxml.Q<VisualElement>("status_character_clothes") as Label;
        _statusHaircutLabel = VTuxml.Q<VisualElement>("status_character_haircut") as Label;
        _statusMakeupLabel = VTuxml.Q<VisualElement>("status_character_makeup") as Label;

        _techNameField = new TextField();
        _runtimeNameField = new TextField();
        _characterDescriptionField = new TextField();
        _characterDescriptionField.style.height = 110f;
        _characterDescriptionField.multiline = true;
        _characterDescriptionField.maxLength = 145;
        _characterDescriptionField.style.whiteSpace = WhiteSpace.Normal;

        _techNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_techNameField.value, StrFieldType.TechNameField));
        _runtimeNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_runtimeNameField.value, StrFieldType.RuntimeNameField));
        _characterDescriptionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValues(_characterDescriptionField.value, StrFieldType.CharacterDescriptionField));



        Button selectBodyButton = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._body, "png");
                SetupPreviewComponent(path, StrPreviewElementType.Body);
                _s_StrEvent.EditorUpdated();
            }
        });
        selectBodyButton.text = "Select";

        Button selectClothesButton = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._clothes, "png");
                SetupPreviewComponent(path, StrPreviewElementType.Clothes);
                _s_StrEvent.EditorUpdated();
            }
        });
        selectClothesButton.text = "Select";

        Button selectHaircutButton = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._haircut, "png");
                SetupPreviewComponent(path, StrPreviewElementType.Haircut);
                _s_StrEvent.EditorUpdated();

            }
        });
        selectHaircutButton.text = "Select";

        Button selectMakeupButton = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string path = EditorUtility.OpenFilePanel("Select sprite", _s_StorylineEditor._s_Folder._makeup, "png");
                SetupPreviewComponent(path, StrPreviewElementType.Makeup);
                _s_StrEvent.EditorUpdated();

            }
        });
        selectMakeupButton.text = "Select";

        if (_previewBody != null)
        {
            VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Body].texture;

            _statusBodyLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Body].name;
        }
        if (_previewClothes != null)
        {
            VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Clothes].texture;

            _statusClothesLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Clothes].name;
        }
        if (_previewHaircut != null)
        {
            VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Haircut].texture;

            _statusHaircutLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Haircut].name;
        }
        if (_previewMakeup != null)
        {
            VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Makeup].texture;

            _statusMakeupLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Makeup].name;
        }

        Button exportToFileButton = new Button(() =>
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
        exportToFileButton.text = "Export to file";

        VTuxml.Q<VisualElement>("tech_name_fieldHolder").Add(_techNameField);
        VTuxml.Q<VisualElement>("runtime_name_fieldHolder").Add(_runtimeNameField);
        VTuxml.Q<VisualElement>("description_fieldHolder").Add(_characterDescriptionField);
        VTuxml.Q<VisualElement>("character_body_buttonHolder").Add(selectBodyButton);
        VTuxml.Q<VisualElement>("character_clothes_buttonHolder").Add(selectClothesButton);
        VTuxml.Q<VisualElement>("character_haircut_buttonHolder").Add(selectHaircutButton);
        VTuxml.Q<VisualElement>("character_makeup_buttonHolder").Add(selectMakeupButton);
        VTuxml.Q<VisualElement>("buttonHolder1").Add(exportToFileButton);
        SetupRequiredElementsDictionary();
        rootVisualElement.Add(VTuxml);
    }

    private void SetValues(string fieldValue, StrFieldType fieldType)
    {
        if (fieldValue != "")
        {
            if (_requiredFieldsLabels[fieldType] != null)
            {
                _requiredFieldsLabels[fieldType].text = fieldValue;
            }
            else 
            {
                Debug.Log("null");
            }
        }
        Repaint();
    }

    private void SetupPreviewComponent(string componentPath, StrPreviewElementType componentType)
    {
        if (componentPath.Length != 0)
        {
            string previewComponentName;
            string previewComponentResourcesPath;
            string removedFolder = StrPreviewElementsFolders.GetPreviewElementFolder(componentType);
            previewComponentName = GetPreviewComponentName(componentPath, removedFolder);
            previewComponentResourcesPath = GetPreviewComponentResourcesPath(componentPath);
            _requiredPreviewElementsSprites[componentType] = CreatePreviewComponent(previewComponentResourcesPath, previewComponentName);
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select sprite", "OK");
        }
    }
    private string GetPreviewComponentName(string componentPath, string removedFolder)
    {
        string temp = componentPath.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
        string temp2 = temp.Replace(".png", "");
        string ToReplace = "Gamedata/Textures/" + removedFolder + "/";
        string componentName = temp2.Replace(ToReplace, "");
        return componentName;
    }
    private string GetPreviewComponentResourcesPath(string componentPath)
    {
        string temp = componentPath.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
        string resourcesPath = temp.Replace(".png", "");
        return resourcesPath;
    }
    private Sprite CreatePreviewComponent(string componentResourcesPath, string componentName)
    {
        Debug.Log(componentResourcesPath);
        Texture2D tempTexture;
        tempTexture = Resources.Load(componentResourcesPath) as Texture2D;
        Sprite previewSprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(tempTexture.width / 2, tempTexture.height / 2));
        previewSprite.name = componentName;
        return previewSprite;
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

        Label[] requiredFields = new Label[] { _statusTechNameLabel, _statusRuntimeNameLabel, _statusCharacterDescriptionLabel, _statusBodyLabel, _statusClothesLabel, _statusHaircutLabel, _statusMakeupLabel };
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
    private Boolean ValidateRequiredFieldsValues(Label[] requiredFilds)
    {

        foreach (Label unit in requiredFilds)
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
            string fileName = _statusTechNameLabel.text + ".char";
            StreamWriter SW = new StreamWriter(_s_StorylineEditor._s_Folder._characters + "/" + fileName, true, encoding: System.Text.Encoding.Unicode);
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
        _s_StrEvent.StrEditorUpdated -= OnStrEditorUpdated;
    }
}

