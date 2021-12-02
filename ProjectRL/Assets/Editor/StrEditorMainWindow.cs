using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEditor;
using System;
using StorylineEditor;
using System.IO;


public class StrEditorMainWindow : EditorWindow
{
    public int id_action = 0;
    public int id_step;
    private VisualElement _mainWindowMainVE;
    private VisualTreeAsset _mainWindowVTAsset;
    private VisualElement _activeCharactersListviewItemVE;
    private VisualTreeAsset _activeCharactersListviewItemVTAsset;
    private StyleSheet _mainWindowSS;
    private TextField _phraseField;
    private Button _controlPanelButton;
    private Button _characterConstructorButton;
    private Button _newStrButton;
    private Button _openStrButton;
    private Button _saveButton;
    private Button _selectCGButton;
    private Button _openCharactersListButton;
    private Button _addCharacterButton;
    private Button _addJumpMarkerButton;
    private Button _addChoiseButton;
    private Button _addEffectButton;
    private Button _newStepButton;
    private Button _newActionButton;
    private Button _exportToStrButton;
    private Button _deactivateCharacterButton;
    private Button _setAuthorButton;
    private Button _deleteSelectedStepButton;
    private ListView _activeCharactersListview;
    private ListView _stepsListview;
    private Sprite _previewBody;
    private Sprite _previewHaircut;
    private Sprite _previewClothes;
    private Sprite _previewMakeup;
    private string _characterName;
    private string _characterDescription;
    private string _characterSprite;
    private string _storylineTitle;
    private string _phraseFieldValue;
    private bool _canRelocateCG;
    private RectTransform _SelectedCharacterRectTransform;
    [SerializeField] private float _CGPositionsSliderValue;
    private StrEditorEvents _StrEvents;
    private StrEditorGodObject _StrEditorRoot;
    private Scroller _CGPositionSlider;
    [MenuItem("Storyline Editor/Open")]
    public static StrEditorMainWindow ShowWindow()
    {
        StrEditorMainWindow main_window = GetWindow<StrEditorMainWindow>();
        main_window.titleContent = new GUIContent("Storyline Editor");
        main_window.minSize = new Vector2(845f, 475f);
        main_window.maxSize = new Vector2(845f, 475f);
        return main_window;
    }
    void OnEnable()
    {
        _canRelocateCG = true;
        _StrEvents = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _StrEditorRoot = (StrEditorGodObject)FindObjectOfType(typeof(StrEditorGodObject));
        _StrEvents.StrEditorUpdated += OnStrEdUpdated;
        _StrEvents.StrCGPositionSliderChanged += OnStrCGpositionSliderChanged;
        if (_StrEditorRoot != null)
        {
            _StrEditorRoot.Init();
        }
    }

    private void OnStrEdUpdated()
    {
        CreateGUI();
    }
    private void OnStrCGpositionSliderChanged(float sliderPosition)
    {
        _canRelocateCG = false;
        _CGPositionsSliderValue = sliderPosition;
        Debug.Log(sliderPosition + "//" + _CGPositionsSliderValue);
        ConvertSliderToCGPosition(_CGPositionsSliderValue);
        _StrEvents.EditorUpdated();
    }
    private void CreateGUI()
    {
        InstantiateMainVisualElement();
        InstatiateActiveCharactersListviewItemVE();
        InstantiateStyleSheets();
        InstatiateTextFields();
        RegisterTextFieldsCallback();
        InstatiateButtons();
        InstatiateActiveCharactersListview();

        _CGPositionSlider = new Scroller(0, 100, (v) => { }, SliderDirection.Horizontal);
        _CGPositionSlider.style.height = 20f;

        // steplist setup
        var items2 = new List<int>();

        for (int i = 0; i < _StrEditorRoot._totalStepsCount.Count; i++)
            items2.Add(i);
        Func<VisualElement> makeItem2 = () => _activeCharactersListviewItemVTAsset.CloneTree();
        Label element_name2 = _activeCharactersListviewItemVE.Q<VisualElement>("name") as Label;
        VisualElement element_icon2 = _activeCharactersListviewItemVE.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem2 = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = "Step: " + i.ToString();
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _StrEditorRoot._tempCharIcon.texture;
        };

        const int itemHeight2 = 30;
        _stepsListview = new ListView(items2, itemHeight2, makeItem2, bindItem2);

        _stepsListview.selectionType = SelectionType.Single;

        _stepsListview.onItemsChosen += obj =>
        {
            Debug.Log(_stepsListview.selectedItem);
        };
        _stepsListview.onSelectionChange += objects =>
        {
            Debug.Log(_stepsListview.selectedItem);
        };
        _stepsListview.style.flexGrow = 1.0f;
        /////////////////////////////////////////////////////////////
        //////
        Label _l_Charlist = _mainWindowMainVE.Q<VisualElement>("charlist") as Label;
        _l_Charlist.text = "Active Characters";
        Label _l_Preview = _mainWindowMainVE.Q<VisualElement>("preview") as Label;
        _l_Preview.text = "Character preview";
        Label _l_CharacterDescription = _mainWindowMainVE.Q<VisualElement>("description") as Label;
        _l_CharacterDescription.text = "Description";
        Label _l_Tools = _mainWindowMainVE.Q<VisualElement>("toolbar1") as Label;
        _l_Tools.text = "Tools";

        Label _l_FileOptions = _mainWindowMainVE.Q<VisualElement>("toolbar2") as Label;
        _l_FileOptions.text = "File options";

        Label _l_EditingOptions = _mainWindowMainVE.Q<VisualElement>("toolbar3") as Label;
        _l_EditingOptions.text = "Editing options";

        Label _l_CharacterName = _mainWindowMainVE.Q<VisualElement>("charname") as Label;
        _l_CharacterName.text = "Runtime Name";
        Label _l_CharaterSprite = _mainWindowMainVE.Q<VisualElement>("sprites") as Label;
        _l_CharaterSprite.text = "Character options";

        Label _l_Status = _mainWindowMainVE.Q<VisualElement>("status") as Label;
        _l_Status.text = "Initialization : " + _StrEditorRoot._initStatus + "      Current file : " + _StrEditorRoot._StorylineName;
        Label _l_Status2 = _mainWindowMainVE.Q<VisualElement>("status2") as Label;
        _l_Status2.text = "Action: " + _StrEditorRoot._actionID + " (Total: " + _StrEditorRoot._actionsTotal.Count + ") / Step: " + _StrEditorRoot._stepID + " (Total: " + _StrEditorRoot._totalStepsCount.Count + ")";

        Label _l_StepsList = _mainWindowMainVE.Q<VisualElement>("steplist") as Label;
        _l_StepsList.text = "Steps list";
        Label _l_CGSlider = _mainWindowMainVE.Q<VisualElement>("CGrlabel") as Label;
        _l_CGSlider.text = "CG position";

        // CG preview
        if (_StrEditorRoot._CGsprite != null)
        {
            _mainWindowMainVE.Q<VisualElement>("CGpreviewArea").style.backgroundImage = _StrEditorRoot._CGsprite.texture;
        }
        var SS4 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/InputFieldCustom.uss");
        // rootVisualElement.styleSheets.Add(SS4);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar").Add(_controlPanelButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar").Add(_characterConstructorButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar2").Add(_newStrButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar2").Add(_openStrButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar2").Add(_saveButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_selectCGButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_openCharactersListButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_addCharacterButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_addJumpMarkerButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_addChoiseButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_addEffectButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_newStepButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_newActionButton);
        _mainWindowMainVE.Q<VisualElement>("leftToolbar3").Add(_exportToStrButton);
        _mainWindowMainVE.Q<VisualElement>("phraseHolder2").Add(_phraseField);
        _mainWindowMainVE.Q<VisualElement>("CharspriteHolder").Add(_deactivateCharacterButton);
        _mainWindowMainVE.Q<VisualElement>("CharspriteHolder").Add(_setAuthorButton);
        _mainWindowMainVE.Q<VisualElement>("charlistBackgroung").Add(_activeCharactersListview);
        _mainWindowMainVE.Q<VisualElement>("steplistArea").Add(_stepsListview);
        _mainWindowMainVE.Q<VisualElement>("CG_sliderHolder").Add(_CGPositionSlider);
        _mainWindowMainVE.Q<VisualElement>("buttonHolder1").Add(_deleteSelectedStepButton);

        // _s_StorylineEditor.SetCGPositionSliderValue(_s_StorylineEditor._CGImage.rectTransform.localPosition.x);
        _CGPositionSlider.value = _CGPositionsSliderValue;
        if (_StrEditorRoot._actionID > _StrEditorRoot._totalActions)
        {
            _CGPositionSlider.valueChanged += (e => ConvertSliderToCGPosition(_CGPositionSlider.value));
        }
    }
    private void InstantiateMainVisualElement()
    {
        try
        {
            _mainWindowVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/MainWindow.uxml");
            _mainWindowMainVE = _mainWindowVTAsset.Instantiate();
            rootVisualElement.Add(_mainWindowMainVE);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    private void InstatiateActiveCharactersListviewItemVE()
    {
        try
        {
            _activeCharactersListviewItemVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/ActiveCharacterTemplate.uxml");
            _activeCharactersListviewItemVE = _activeCharactersListviewItemVTAsset.Instantiate();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    private void InstantiateStyleSheets()
    {
        _mainWindowSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Extensions/StorylineEditor/USS/MainWindow.uss");
        rootVisualElement.styleSheets.Add(_mainWindowSS);
    }
    private void InstatiateTextFields()
    {
        _phraseField = new TextField();
        _phraseField.Q(TextField.textInputUssName).AddToClassList("TextField-Editor");
        _phraseField.multiline = true;
        _phraseField.style.whiteSpace = WhiteSpace.Normal;
        _phraseField.style.height = 180;
        _phraseField.maxLength = 150;
        _phraseField.value = _StrEditorRoot._phrase;
    }
    private void RegisterTextFieldsCallback()
    {
        _phraseField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SelectPhrase(_phraseField.value));
    }
    private void InstatiateButtons()
    {
        _newStrButton = new Button(() => StrEditorStorylineCreatorWindow.ShowWindow());
        _openStrButton = new Button(() => SelectStoryline());
        _newStrButton.text = "New .str";
        _openStrButton.text = "Open .str";
        if (ValidateStoryline())
        {
            _controlPanelButton = new Button(() => OpenControlPanel());
            _characterConstructorButton = new Button(() => StrEditorCharacterConstructorWindow.ShowWindow());
            _saveButton = new Button(() => _StrEvents.EditorUpdated());
            _selectCGButton = new Button(() => SelectCG());
            _openCharactersListButton = new Button(() => OpenCharactersList());
            _addCharacterButton = new Button(() => SelectCharacter());
            _addJumpMarkerButton = new Button(() => StrEditorJumpMarkerWindow.ShowWindow());
            _addChoiseButton = new Button(() => StrEditorChoiseConstructorWindow.ShowWindow());
            _addEffectButton = new Button(() => Debug.Log(" doing nothing"));
            _newStepButton = new Button(() => CreateStep());
            _newActionButton = new Button(() => CreateNewAction());
            _exportToStrButton = new Button(() => ExportToStr());
            _deactivateCharacterButton = new Button(() => DeactivateCharacter());
            _setAuthorButton = new Button(() => SetAuthor());
            _deleteSelectedStepButton = new Button(() => DeleteSelectedStep());
            _controlPanelButton.text = "Control panel";
            _characterConstructorButton.text = "Character Constructor";
            _saveButton.text = "Save";
            _selectCGButton.text = "Select CG";
            _openCharactersListButton.text = "Open Characters list";
            _addCharacterButton.text = "Add character";
            _addJumpMarkerButton.text = "Add jump marker";
            _addChoiseButton.text = "Add choise";
            _addEffectButton.text = "Add effect";
            _newStepButton.text = "New Step";
            _newActionButton.text = "New Action";
            _exportToStrButton.text = "Export to .str";
            _deactivateCharacterButton.text = "Deactivate Character";
            _setAuthorButton.text = "Set as author";
            _deleteSelectedStepButton.text = "Delete selected step";
        }
    }
    private void InstatiateActiveCharactersListview()
    {
        List<GameObject> activeCharactersListviewItems = SetupActiveCharactersListviewItemsList();
        Func<VisualElement> makeItem = () => _activeCharactersListviewItemVTAsset.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < _StrEditorRoot._activeCharacters.Count)
            {
                (e.Q<VisualElement>("name") as Label).text = _StrEditorRoot._activeCharacters[i].name;
            }
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _StrEditorRoot._tempCharIcon.texture;
        };
        _activeCharactersListview = new ListView(activeCharactersListviewItems, StrConstantValues.StandartListviewItemHeight, makeItem, bindItem);
        _activeCharactersListview.selectionType = SelectionType.Single;
        _activeCharactersListview.onItemsChosen += obj => OnActiveCharacterListviewItemSelected();
        _activeCharactersListview.onSelectionChange += objects => OnActiveCharacterListviewItemSelected();
        _activeCharactersListview.style.flexGrow = 1.0f;
    }
    private List<GameObject> SetupActiveCharactersListviewItemsList()
    {
        List<GameObject> itemsList = new List<GameObject>();
        for (int i = 0; i < _StrEditorRoot._activeCharacters.Count; i++)
            if (_StrEditorRoot._activeCharacters[i] != null)
            {
                itemsList.Add(_StrEditorRoot._activeCharacters[i]);
            }
        return itemsList;
    }
    private void OnActiveCharacterListviewItemSelected()
    {
        string tempCharacterName = _activeCharactersListview.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
        SetSelectedCharacterRectTransform(tempCharacterName);
        if (GetPreviewComponents(_activeCharactersListview.selectedIndex))
        {
            if (_previewBody != null && _previewClothes != null && _previewHaircut != null && _previewMakeup != null)
            {
                _mainWindowMainVE.Q<VisualElement>("previewHolder").style.backgroundImage = _previewBody.texture;
                _mainWindowMainVE.Q<VisualElement>("previewHolder2").style.backgroundImage = _previewClothes.texture;
                _mainWindowMainVE.Q<VisualElement>("previewHolder3").style.backgroundImage = _previewHaircut.texture;
                _mainWindowMainVE.Q<VisualElement>("previewHolder4").style.backgroundImage = _previewMakeup.texture;
                Label _characterNameLabel = _mainWindowMainVE.Q<VisualElement>("namecontent") as Label;
                _characterNameLabel.text = _characterName;
                Label _characterDescriptionLabel = _mainWindowMainVE.Q<VisualElement>("descrcontent") as Label;
                _characterDescriptionLabel.text = _characterDescription;
            }
        }
    }
    private void OpenControlPanel()
    {
        StrEditorControlPanelWindow.ShowWindow();
        _StrEvents.EditorUpdated();
    }
    private void CreateStep()
    {
        _StrEditorRoot.CreateNewStep();
        _StrEvents.EditorUpdated();
    }
    private void CreateNewAction()
    {
        if (_StrEditorRoot._readyForNextAction == true)
        {
            _StrEditorRoot.CreateNewAction();
            _StrEvents.EditorUpdated();
            EditorUtility.DisplayDialog("Notice", "New action created", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Unable to create, check required conditions", "OK");
        }
    }
    private void DeactivateCharacter()
    {
        string temp = _activeCharactersListview.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
        if (_StrEditorRoot.DeactivateCharacter(temp))
        {
            EditorUtility.DisplayDialog("Notice", "Character deactivated", "OK");
            _StrEvents.EditorUpdated();
        }
    }
    private void ExportToStr()
    {
        _StrEditorRoot.ExportStorylineToStrFile();
        _StrEvents.EditorUpdated();
        EditorUtility.DisplayDialog("Notice", ".str writed.", "OK");
    }
    private void OpenCharactersList()
    {
        StrEditorCharactersListWindow.ShowWindow();
        _StrEvents.EditorUpdated();
    }
    private void SetAuthor()
    {
        if (_activeCharactersListview.selectedItem != null)
        {
            _StrEditorRoot.SetAuthor(_activeCharactersListview.selectedItem.ToString());
            _StrEvents.EditorUpdated();
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "No character selected", "OK");
        }
    }
    private void DeleteSelectedStep()
    {
        if (_stepsListview.selectedItem != null)
        {
            _StrEditorRoot.DeleteStep(_stepsListview.selectedIndex);
            _StrEvents.EditorUpdated();
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select step first", "OK");
        }
    }
    void SetSelectedCharacterRectTransform(string CharacterName)
    {

        foreach (GameObject unit in _StrEditorRoot._activeCharacters)
        {
            if (unit.name == CharacterName)
            {
                _SelectedCharacterRectTransform = unit.GetComponent<RectTransform>();

            }
        }
    }
    private Boolean ConvertSliderToCGPosition(float CGSliderValue)
    {

        Debug.Log("converted: " + CGSliderValue);
        float PoolX = _StrEditorRoot._�anvasMovingPool;

        float SliderValueOfDivision = PoolX / 100;
        Debug.Log(SliderValueOfDivision);
        float CGPosisitionX = CGSliderValue * SliderValueOfDivision;
        _StrEditorRoot.RelocateCG(CGPosisitionX, CGSliderValue);
        _CGPositionsSliderValue = CGSliderValue;

        return true;
    }
    void SetSelectedCharacterParent()
    {
        _SelectedCharacterRectTransform.transform.SetParent(_StrEditorRoot._CGRectTransform.transform, true);
    }
    void SelectCG()
    {
        Texture2D tex = new Texture2D(1, 1);
        string Path = EditorUtility.OpenFilePanel("Select CG", _StrEditorRoot._folders._CG, "png");
        if (Path.Length != 0)
        {
            string temp = Path.Replace(_StrEditorRoot._folders._root + "/Resources/", "");
            string temp2 = temp.Replace(".png", "");
            string temp3 = temp2.Replace("Gamedata/Textures/CG/", "");
            _StrEditorRoot.AddCG(temp2, temp3);
            _StrEvents.EditorUpdated();
        }
    }
    void SelectStoryline()
    {
        if (EditorUtility.DisplayDialog("Notice", "Usaved progress will be lost. Continue?", "OK", "Cancel"))
        {
            string Path = EditorUtility.OpenFilePanel("Select storyline", _StrEditorRoot._folders._storylines, "str");
            if (Path.Length != 0)
            {
                string temp = Path.Replace(_StrEditorRoot._folders._root + "/Resources/", "");
                string temp2 = temp.Replace(".str", "");
                string temp3 = temp.Replace("Gamedata/Storylines/", "");
                _StrEditorRoot.OpenStoryline(temp3);
                _StrEvents.EditorUpdated();

            }
        }
    }
    private void SelectCharacter()
    {
        string Path = EditorUtility.OpenFilePanel("Select Character", _StrEditorRoot._folders._characters, "char");
        if (Path.Length != 0)
        {
            string temp = Path.Replace(_StrEditorRoot._folders._root + "/Resources/", "");
            string temp2 = temp.Replace(".char", "");
            string temp3 = temp2.Replace("Gamedata/�haracters/", "");
            _StrEditorRoot.AddCharacter(Path, temp3);
            _StrEvents.EditorUpdated();
        }
    }
    private string SelectPhrase(string PhraseText)
    {
        _StrEditorRoot._phrase = PhraseText;
        return (PhraseText);
    }
    private Boolean GetPreviewComponents(int SelectedCharacterID)
    {
        StrCharacter tempStrCharacter = _StrEditorRoot._requiredObjects[SelectedCharacterID].GetComponent<Character>().GetCharacterParameters();
        _previewBody = tempStrCharacter.CharacterBody.sprite;
        _previewClothes = tempStrCharacter.CharacterClothes.sprite;
        _previewHaircut = tempStrCharacter.CharacterHaircut.sprite;
        _previewMakeup = tempStrCharacter.CharacterHaircut.sprite;
        _characterName = tempStrCharacter.CharacterRuntimeName;
        _characterDescription = tempStrCharacter.CharacterDescription;
        _characterSprite = "get sprites list";
        return true;
    }
    private Boolean ValidateStoryline()
    {
        if (_StrEditorRoot.CheckStorylineExistence(_StrEditorRoot._StorylineName))
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
        _StrEvents.StrEditorUpdated -= OnStrEdUpdated;
        _StrEvents.StrCGPositionSliderChanged -= OnStrCGpositionSliderChanged;
    }
}
