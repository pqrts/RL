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

    private Button _selectBodyButton;
    private Button _selectClothesButton;
    private Button _selectHaircutButton;
    private Button _selectMakeupButton;
    private Button _exportToFileButton;

    private static List<string> _characterComponentsToFile = new List<string>();
    private Dictionary<StrFieldType, Label> _associatedWithFieldsStatusLabels = new Dictionary<StrFieldType, Label>();
    private Dictionary<StrPreviewElementType, Sprite> _previewElements = new Dictionary<StrPreviewElementType, Sprite>();
    private Dictionary<StrPreviewElementType, Label> _associatedWithPreviewElementsStatusLabels = new Dictionary<StrPreviewElementType, Label>();
    private Dictionary<StrPreviewElementType, string> _associatedWithPreviewElementsFoldersPaths = new Dictionary<StrPreviewElementType, string>();
    private StrEditorEvents _s_StrEvent;

    public static StrEditorCharacterConstructorWindow ShowWindow()
    {
        StrEditorCharacterConstructorWindow CharacterConstructorWindow = GetWindow<StrEditorCharacterConstructorWindow>();
        CharacterConstructorWindow.titleContent = new GUIContent("Character Constructor");
        CharacterConstructorWindow.minSize = new Vector2(340, 475f);
        CharacterConstructorWindow.maxSize = new Vector2(340f, 475f);
        return CharacterConstructorWindow;
    }
    private void OnEnable()
    {
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StrEvent.StrEditorUpdated += OnStrEditorUpdated;
    }
    private void OnStrEditorUpdated()
    {
        Repaint();
    }
    private void CreateGUI()
    {
        if (InstantiateMainVisualElement())
        {
            InstatiateTextFields();
            RegisterTextFieldsCallback();
            InstatiateButtons();
            SetupRequiredLabelsDictionaries();
            SetupRequiredPreviewElementsDictionary();
            SetupPrevievElementsFoldersPaths();
            SetupPreview();
            SetPreviewStatusLabelsValues();
            AddInstatiatedUIElementsToMainVE();
        }
    }
    private Boolean InstantiateMainVisualElement()
    {
        try
        {
            _characterConstructorVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/CharacterConstructorWindow.uxml");
            _characterConstructorMainVE = _characterConstructorVTAsset.Instantiate();
            rootVisualElement.Add(_characterConstructorMainVE);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
        return true;
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
    private void RegisterTextFieldsCallback()
    {
        _techNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_techNameField.value, StrFieldType.TechNameField));
        _runtimeNameField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_runtimeNameField.value, StrFieldType.RuntimeNameField));
        _characterDescriptionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetUIElementsValues(_characterDescriptionField.value, StrFieldType.CharacterDescriptionField));
    }
    private void InstatiateButtons()
    {
        if (ValidateStoryline())
        {
            _selectBodyButton = new Button(() =>
            {
                OpenElementFolderInFilePanel(StrPreviewElementType.Body);
            });
            _selectBodyButton.text = "Select";

            _selectClothesButton = new Button(() =>
            {
                OpenElementFolderInFilePanel(StrPreviewElementType.Clothes);
            });
            _selectClothesButton.text = "Select";

            _selectHaircutButton = new Button(() =>
            {
                OpenElementFolderInFilePanel(StrPreviewElementType.Haircut);

            });
            _selectHaircutButton.text = "Select";

            _selectMakeupButton = new Button(() =>
            {
                OpenElementFolderInFilePanel(StrPreviewElementType.Makeup);
            });
            _selectMakeupButton.text = "Select";
            _exportToFileButton = new Button(() =>
            {
                if (EditorUtility.DisplayDialog("Notice", " Are you sure about this?", "OK", "Cancel"))
                {
                    CharacterAssembly();
                    _s_StrEvent.EditorUpdated();
                }
            });
            _exportToFileButton.text = "Export to file";
        }
    }
    private void AddInstatiatedUIElementsToMainVE()
    {
        _characterConstructorMainVE.Q<VisualElement>("tech_name_fieldHolder").Add(_techNameField);
        _characterConstructorMainVE.Q<VisualElement>("runtime_name_fieldHolder").Add(_runtimeNameField);
        _characterConstructorMainVE.Q<VisualElement>("description_fieldHolder").Add(_characterDescriptionField);
        _characterConstructorMainVE.Q<VisualElement>("character_body_buttonHolder").Add(_selectBodyButton);
        _characterConstructorMainVE.Q<VisualElement>("character_clothes_buttonHolder").Add(_selectClothesButton);
        _characterConstructorMainVE.Q<VisualElement>("character_haircut_buttonHolder").Add(_selectHaircutButton);
        _characterConstructorMainVE.Q<VisualElement>("character_makeup_buttonHolder").Add(_selectMakeupButton);
        _characterConstructorMainVE.Q<VisualElement>("buttonHolder1").Add(_exportToFileButton);
    }
    private void OpenElementFolderInFilePanel(StrPreviewElementType elementType)
    {
        string targetFolder = _associatedWithPreviewElementsFoldersPaths[elementType];
        string path = EditorUtility.OpenFilePanel("Select sprite", targetFolder, "png");
        SetupPreviewComponent(path, elementType);
        _s_StrEvent.EditorUpdated();
    }
    private void SetupRequiredLabelsDictionaries()
    {
        if (_associatedWithFieldsStatusLabels.Count == 0)
        {
            _associatedWithFieldsStatusLabels.Add(StrFieldType.TechNameField, _statusTechNameLabel);
            _associatedWithFieldsStatusLabels.Add(StrFieldType.RuntimeNameField, _statusRuntimeNameLabel);
            _associatedWithFieldsStatusLabels.Add(StrFieldType.CharacterDescriptionField, _statusCharacterDescriptionLabel);
        }
        if (_associatedWithPreviewElementsStatusLabels.Count == 0)
        {
            _associatedWithPreviewElementsStatusLabels.Add(StrPreviewElementType.Body, _statusBodyLabel);
            _associatedWithPreviewElementsStatusLabels.Add(StrPreviewElementType.Clothes, _statusClothesLabel);
            _associatedWithPreviewElementsStatusLabels.Add(StrPreviewElementType.Haircut, _statusHaircutLabel);
            _associatedWithPreviewElementsStatusLabels.Add(StrPreviewElementType.Makeup, _statusMakeupLabel);
        }
    }
    private void SetupRequiredPreviewElementsDictionary()
    {
        if (_previewElements.Count == 0)
        {
            _previewElements.Add(StrPreviewElementType.Body, _previewBody);
            _previewElements.Add(StrPreviewElementType.Clothes, _previewClothes);
            _previewElements.Add(StrPreviewElementType.Haircut, _previewHaircut);
            _previewElements.Add(StrPreviewElementType.Makeup, _previewHaircut);
        }
    }
    private void SetupPrevievElementsFoldersPaths()
    {
        if (_associatedWithPreviewElementsFoldersPaths.Count == 0)
        {
            _associatedWithPreviewElementsFoldersPaths.Add(StrPreviewElementType.Body, _s_StorylineEditor._s_Folder._body);
            _associatedWithPreviewElementsFoldersPaths.Add(StrPreviewElementType.Clothes, _s_StorylineEditor._s_Folder._clothes);
            _associatedWithPreviewElementsFoldersPaths.Add(StrPreviewElementType.Haircut, _s_StorylineEditor._s_Folder._haircut);
            _associatedWithPreviewElementsFoldersPaths.Add(StrPreviewElementType.Makeup, _s_StorylineEditor._s_Folder._makeup);

        }
    }
    private void SetUIElementsValues(string fieldValue, StrFieldType fieldType)
    {
        if (fieldValue != "")
        {
            string tempUXMLName = StrUXMLElementsNames.StatusLabel + fieldType.GetFieldTypeIndex();
            _associatedWithFieldsStatusLabels[fieldType] = _characterConstructorMainVE.Q<VisualElement>(tempUXMLName) as Label;
            if (_associatedWithFieldsStatusLabels[fieldType] != null)
            {
                _associatedWithFieldsStatusLabels[fieldType].text = fieldValue;
            }
        }
        Repaint();
    }
    private void SetupPreview()
    {

        foreach (KeyValuePair<StrPreviewElementType, Sprite> previewElement in _previewElements)
        {
            if (previewElement.Value != null)
            {
                string tempUXMLName = StrUXMLElementsNames.PreviewHolder + previewElement.Key.GetFieldTypeIndex();
                if (_characterConstructorMainVE.Q<VisualElement>(tempUXMLName) != null)
                {
                    _characterConstructorMainVE.Q<VisualElement>(tempUXMLName).style.backgroundImage = _previewElements[previewElement.Key].texture;
                }
            }
        }
    }
    private void SetPreviewStatusLabelsValues()
    {
        foreach (KeyValuePair<StrPreviewElementType, Sprite> previewElementSprite in _previewElements)
        {
            if (previewElementSprite.Value != null)
            {
                string tempUXMLName = StrUXMLElementsNames.StatusLabel + previewElementSprite.Key.GetFieldTypeIndex();
                _associatedWithPreviewElementsStatusLabels[previewElementSprite.Key] = _characterConstructorMainVE.Q<VisualElement>(tempUXMLName) as Label;
                if (_associatedWithPreviewElementsStatusLabels[previewElementSprite.Key] != null)
                {
                    _associatedWithPreviewElementsStatusLabels[previewElementSprite.Key].text = _previewElements[previewElementSprite.Key].name;
                }
            }
        }
    }

    private void SetupPreviewComponent(string componentPath, StrPreviewElementType componentType)
    {
        if (componentPath.Length != 0)
        {
            string previewComponentName;
            string previewComponentResourcesPath;
            string removedFolder = componentType.GetFieldTypeAssociatedFolder();
            previewComponentName = ÑomposePreviewComponentName(componentPath, removedFolder);
            previewComponentResourcesPath = ÑomposePreviewComponentResourcesPath(componentPath);
            _previewElements[componentType] = CreatePreviewComponent(previewComponentResourcesPath, previewComponentName);
            CreateGUI();
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select sprite", "OK");
        }
    }
    private string ÑomposePreviewComponentName(string componentPath, string removedFolder)
    {
        string temp = componentPath.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
        string temp2 = temp.Replace(".png", "");
        string ToReplace = "Gamedata/Textures/" + removedFolder + "/";
        string componentName = temp2.Replace(ToReplace, "");
        return componentName;
    }
    private string ÑomposePreviewComponentResourcesPath(string componentPath)
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
        foreach (KeyValuePair<StrFieldType, Label> fieldLabel in _associatedWithFieldsStatusLabels)
        {
            if (fieldLabel.Value != null && fieldLabel.Value.text != StrConstantValues.PlaceholderText)
            {
                _characterComponentsToFile.Add(_associatedWithFieldsStatusLabels[fieldLabel.Key].text);
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Fill all fields", "OK");
                break;
            }
        }
        foreach (KeyValuePair<StrPreviewElementType, Label> previewElementLabel in _associatedWithPreviewElementsStatusLabels)
        {
            if (previewElementLabel.Value != null && previewElementLabel.Value.text != StrConstantValues.PlaceholderText)
            {
                _characterComponentsToFile.Add(_associatedWithPreviewElementsStatusLabels[previewElementLabel.Key].text);
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Fill all fields", "OK");
                break;
            }
        }
        foreach (string unit in _characterComponentsToFile)
        {
            Debug.Log(unit);
        }
        if (ExportCharacterToFile())
        {
            _characterComponentsToFile.Clear();
        }
    }

    private Boolean ExportCharacterToFile()
    {
        try
        {
            string fileName = _characterComponentsToFile[0] + ".char";
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

