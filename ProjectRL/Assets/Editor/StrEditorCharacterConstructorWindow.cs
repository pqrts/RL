using StorylineEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StrEditorCharacterConstructorWindow : EditorWindow
{
    private VisualElement _characterConstructorMainVE;
    private VisualTreeAsset _characterConstructorVTAsset;
    private ext_StorylineEditor _s_StorylineEditor;
    private TextField _techNameField;
    private TextField _runtimeNameField;
    private TextField _characterDescriptionField;

    private static Sprite _previewBody;
    private static Sprite _previewHaircut;
    private static Sprite _previewClothes;
    private static Sprite _previewMakeup;
    private static Sprite[] _previewSprites = new Sprite[] { _previewBody, _previewClothes, _previewHaircut, _previewMakeup };
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
        Debug.Log(_requiredFieldsLabels.Count);
    }
    private void OnStrEditorUpdated()
    {
        CreateGUI();
    }
    private void CreateGUI()
    {
        InstantiateMainVisualElement();
        if (_characterConstructorMainVE != null)
        {
            InstatiateLabels();
            InstatiateTextFields();
            SetupRequiredLabelsDictionary();
            SetupRequiredPreviewElementsDictionary();
        }
        _techNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_techNameField.value, StrFieldType.TechNameField));
        _runtimeNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_runtimeNameField.value, StrFieldType.RuntimeNameField));
        _characterDescriptionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_characterDescriptionField.value, StrFieldType.CharacterDescriptionField));
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

        if (_previewSprites[0] != null)
        {
            _characterConstructorMainVE.Q<VisualElement>("previewHolder").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Body].texture;

            _statusBodyLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Body].name;
        }
        if (_previewSprites[1] != null)
        {
            _characterConstructorMainVE.Q<VisualElement>("previewHolder2").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Clothes].texture;

            _statusClothesLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Clothes].name;
        }
        if (_previewSprites[2] != null)
        {
            _characterConstructorMainVE.Q<VisualElement>("previewHolder3").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Haircut].texture;

            _statusHaircutLabel.text = _requiredPreviewElementsSprites[StrPreviewElementType.Haircut].name;
        }
        if (_previewSprites[3] != null)
        {
            _characterConstructorMainVE.Q<VisualElement>("previewHolder4").style.backgroundImage = _requiredPreviewElementsSprites[StrPreviewElementType.Makeup].texture;

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

        _characterConstructorMainVE.Q<VisualElement>("tech_name_fieldHolder").Add(_techNameField);
        _characterConstructorMainVE.Q<VisualElement>("runtime_name_fieldHolder").Add(_runtimeNameField);
        _characterConstructorMainVE.Q<VisualElement>("description_fieldHolder").Add(_characterDescriptionField);
        _characterConstructorMainVE.Q<VisualElement>("character_body_buttonHolder").Add(selectBodyButton);
        _characterConstructorMainVE.Q<VisualElement>("character_clothes_buttonHolder").Add(selectClothesButton);
        _characterConstructorMainVE.Q<VisualElement>("character_haircut_buttonHolder").Add(selectHaircutButton);
        _characterConstructorMainVE.Q<VisualElement>("character_makeup_buttonHolder").Add(selectMakeupButton);
        _characterConstructorMainVE.Q<VisualElement>("buttonHolder1").Add(exportToFileButton);
        SetupRequiredPreviewElementsDictionary();
        rootVisualElement.Add(_characterConstructorMainVE);
    }

    private void InstantiateMainVisualElement()
    {
        _characterConstructorVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/character_constructor.uxml");
        _characterConstructorMainVE = _characterConstructorVTAsset.Instantiate();
    }
    private void InstatiateLabels()
    {
        _statusRuntimeNameLabel = _characterConstructorMainVE.Q<VisualElement>("status_char_runtime_name") as Label;
        _statusTechNameLabel = _characterConstructorMainVE.Q<VisualElement>("status_char_technical_name") as Label;
        _statusCharacterDescriptionLabel = _characterConstructorMainVE.Q<VisualElement>("status_char_description") as Label;
        _statusBodyLabel = _characterConstructorMainVE.Q<VisualElement>("status_character_body") as Label;
        _statusClothesLabel = _characterConstructorMainVE.Q<VisualElement>("status_character_clothes") as Label;
        _statusHaircutLabel = _characterConstructorMainVE.Q<VisualElement>("status_character_haircut") as Label;
        _statusMakeupLabel = _characterConstructorMainVE.Q<VisualElement>("status_character_makeup") as Label;
    }
    private void InstatiateTextFields()
    {
        _techNameField = new TextField();
        _runtimeNameField = new TextField();
        _characterDescriptionField = new TextField();
        _characterDescriptionField.style.height = 110f;
        _characterDescriptionField.multiline = true;
        _characterDescriptionField.maxLength = 145;
        _characterDescriptionField.style.whiteSpace = WhiteSpace.Normal;
    }
    private void SetupRequiredLabelsDictionary()
    {

        if (_requiredFieldsLabels.Count == 0)
        {
            _requiredFieldsLabels.Add(StrFieldType.TechNameField, _statusTechNameLabel);
            _requiredFieldsLabels.Add(StrFieldType.RuntimeNameField, _statusRuntimeNameLabel);
            _requiredFieldsLabels.Add(StrFieldType.CharacterDescriptionField, _statusCharacterDescriptionLabel);
        }
    }
    private void SetupRequiredPreviewElementsDictionary()
    {
        if (_requiredPreviewElementsSprites.Count == 0)
        {
            if (ValidatePreviewSpritesArray())
            {
                _requiredPreviewElementsSprites.Add(StrPreviewElementType.Body, _previewSprites[0]);
                _requiredPreviewElementsSprites.Add(StrPreviewElementType.Clothes, _previewSprites[1]);
                _requiredPreviewElementsSprites.Add(StrPreviewElementType.Haircut, _previewSprites[2]);
                _requiredPreviewElementsSprites.Add(StrPreviewElementType.Makeup, _previewSprites[3]);
            }
        }
    }
    private Boolean ValidatePreviewSpritesArray()
    {
        for (int i = 0; i < _previewSprites.Length; i++)
        {
            if (_previewSprites[i] == null)
            {
                SetupPreviewPlaceholder(i);
            }
        }
        return true;
    }
    private void SetupPreviewPlaceholder(int previewSpriteIndex)
    {
        Texture2D tempTexture = new Texture2D(1, 1);
        Sprite previewPlaceholder = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(tempTexture.width / 2, tempTexture.height / 2));
        previewPlaceholder.name = _placeholderText;
        _previewSprites.SetValue(previewPlaceholder, previewSpriteIndex);
    }

    private void SetUIElementsValues(string fieldValue, StrFieldType fieldType)
    {
        if (fieldValue != "")
        {
         _requiredFieldsLabels[fieldType].text = fieldValue;
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

