using StorylineEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[ExecuteInEditMode]
[Serializable]
[RequireComponent(typeof(StrEditorEvents))]
[RequireComponent(typeof(StrEditorStorylineComposer))]
[RequireComponent(typeof(StrEditorEncryptor))]
[RequireComponent(typeof(StrEditorDecomposer))]
public class StrEditorGodObject : MonoBehaviour, IStrEditorRoot
{
    private bool _isStrEditorRootObjectInitialized;
    public Sprite _tempCharIcon;
    public GameObject _canvas;
    [SerializeField] private int _refereceResolutionWidht;
    [HideInInspector] public string _initStatus;
    public int _actionID;
    public int _totalActions;
    public int _stepID;
    public TaglistReader _tags;
    public global_folders _folders;
    private StrEditorReplacer _replacer;
    private StrEditorDecomposer _decomposer;
    ext_Storyline_exeptions _exeptions;
    private StrEditorStorylineComposer _composer;
    private StrEditorEncryptor _encryptor;
    [SerializeField] public string _editorUser;
    [SerializeField] private float _version;
    //for Initialization part
    public List<GameObject> _requiredObjects = new List<GameObject>();
    [HideInInspector] public List<Sprite> _requiredCG = new List<Sprite>();
    //steps parameters
    private List<RectTransform> _activeRectTransforms = new List<RectTransform>();
    public List<GameObject> _activeCharacters = new List<GameObject>();
    public List<string> _choiseOptions = new List<string>();
    [SerializeField] private List<string> _composedStoryline = new List<string>();
    public string _StorylineName;
    //for str form
    public List<string> _initPart = new List<string>();
    public List<string> _storylineActions = new List<string>();
    [SerializeField] private List<string> _curretActionSteps = new List<string>();
    [HideInInspector] public List<string> _totalStepsCount = new List<string>();

    //scene
    [HideInInspector] public Sprite _CGsprite;
    public Image _CGImage;
    [SerializeField] private GameObject _screenBorders;
    [SerializeField] private GameObject _phraseHolder;
    private RectTransform _phraseHolderRectTransform;
    [SerializeField] private TextMeshProUGUI _phraseHolderAuthor;
    [SerializeField] private TextMeshProUGUI _phraseHolderPhrase;
    private float _leftCameraBorderPosition;
    private float _rightCameraBorderPosition;
    private float[] _movingPoolPositions;
    [HideInInspector] public string _phrase;
    [HideInInspector] public string _phraseAuthor;
    //characters spawn
    public GameObject _ñharacter;
    private StrEditorCharacterSpawner _characterSpawner;
    [HideInInspector] public float _ñanvasMovingPool;
    [HideInInspector] public float _CGMovingPool;
    [HideInInspector] public float _cgPositionX;
    [HideInInspector] public RectTransform _CGRectTransform;
    [HideInInspector] public float _leftCGEdgePosition;
    [HideInInspector] public float _rightCGEdgePosition;
    [HideInInspector] public bool _readyForNextAction;

    public string _jumpMarker = null;
    private StrEditorEvents _StrEvents;
    private void OnEnable()
    {
        _StrEvents.StrEditorRootObjectRequested += OnStrEditorRootObjectRequested;
    }
    private void OnStrEditorRootObjectRequested()
    {
        if (_isStrEditorRootObjectInitialized == true)
        {
            _StrEvents.DeclareStrEditorRootObject(this);
        }
    }
    public void Init()
    {
        _characterSpawner = GetComponent<StrEditorCharacterSpawner>();
        _StrEvents = GetComponent<StrEditorEvents>();
        _tags = GetComponent<TaglistReader>();
        _folders = GetComponent<global_folders>();
        _replacer = GetComponent<StrEditorReplacer>();
        _CGRectTransform = _CGImage.GetComponent<RectTransform>();
        _phraseHolderRectTransform = _phraseHolder.GetComponent<RectTransform>();
        _phraseHolderRectTransform.localPosition = new Vector3(0f, 0f, 0f);
        _exeptions = GetComponent<ext_Storyline_exeptions>();
        _composer = GetComponent<StrEditorStorylineComposer>();
        _encryptor = GetComponent<StrEditorEncryptor>();
        _decomposer = GetComponent<StrEditorDecomposer>();
        if (_folders.Setup_folders() && _tags.Setup_tags() && GetCGPositionLimits() && _replacer.GetRequieredComponents() && _composer.GetRequieredComponents() && _encryptor.GetRequieredComponents() && _characterSpawner.GetRequieredComponents() && _decomposer.GetRequieredComponents())
        {
            _initStatus = "successful";
            _isStrEditorRootObjectInitialized = true;
            _StrEvents.EditorUpdated();
        }
        else
        {
            _isStrEditorRootObjectInitialized = false;
            _initStatus = "failed";
        }

    }
    private Boolean GetCGPositionLimits()
    {
        float CGWidht = _CGRectTransform.rect.width;
        _leftCameraBorderPosition = 0 - (_refereceResolutionWidht / 2);
        _rightCameraBorderPosition = 0 + (_refereceResolutionWidht / 2);
        float LeftX = _leftCameraBorderPosition + (CGWidht / 2);
        float RightX = _rightCameraBorderPosition - (CGWidht / 2);
        _ñanvasMovingPool = LeftX - RightX;
        _movingPoolPositions = new float[] { LeftX, RightX };
        return true;
    }
    public float GetReferenceCGResolutionWidht()
    {
        float resolution = _refereceResolutionWidht;
        return resolution;
    }
    public void SetCGPosition(float CGPositionX)
    {
        _CGRectTransform.localPosition = new Vector3(CGPositionX, _CGRectTransform.localPosition.y, _CGRectTransform.localPosition.z);
        _StrEvents.CGPositionChanged();
    }
    public void TranslocateCG(float CGPositionX)
    {
        float x = _movingPoolPositions[0] - CGPositionX;
        _CGRectTransform.localPosition = new Vector3(x, _CGRectTransform.localPosition.y, _CGRectTransform.localPosition.z);
    }
    private void TranslocatePhraseHolder(float phraseHolserPositionX)
    {
        _phraseHolderRectTransform.localPosition = new Vector3(phraseHolserPositionX, _phraseHolderRectTransform.localPosition.y, _phraseHolderRectTransform.localPosition.z);
    }
    private void SetPhraseHolderText(string phraseHolderAuthor, string phraseHolderPhrase)
    {
        _phraseHolderAuthor.text = phraseHolderAuthor;
        _phraseHolderPhrase.text = phraseHolderPhrase;
    }
    public void DefinePhraseHolderActivityState(string phraseHolderState)
    {
        switch (phraseHolderState)
        {
            case "True":
                _phraseHolder.SetActive(true);
                break;
            case "False":
                _phraseHolder.SetActive(false);
                break;
            case "Manual":
                {
                    if (_phraseHolder.activeInHierarchy == true)
                    {
                        _phraseHolder.SetActive(false);
                    }
                    else
                    {
                        _phraseHolder.SetActive(true);
                    }
                }
                break;
        }
    }
    public void DefineBordersActivityState()
    {
        if (_screenBorders.activeInHierarchy == true)
        {
            _screenBorders.SetActive(false);
        }
        else
        {
            _screenBorders.SetActive(true);
        }
    }
    public void ReadStoryline()
    {
        try
        {
            StreamReader SR = new StreamReader(_folders._storylines + "/" + _StorylineName);
            string line = SR.ReadLine();
            while (line != null)
            {
                line = SR.ReadLine();
            }
            SR.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
    }

    public void UpdateInitPart()
    {
        _initPart.Clear();
        StrStorylineParameters uncomposedStoryline = SetStrUncomposedStorylineParameters();
        _initPart = _composer.ComposeInitPart(uncomposedStoryline);
    }
    public void CreateNewStep()
    {
        if (_totalStepsCount.Count < 1)
        {
            StrStorylineParameters uncomposedStoryline = SetStrUncomposedStorylineParameters();
            _curretActionSteps = _composer.ComposeStep(uncomposedStoryline);
            _stepID += 1;
            _totalStepsCount.Add(_stepID.ToString());
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Cannot create more than 1 step in the current version", "OK");
        }
    }
    public void DeleteStep(int stepIndex)
    {
        if (_curretActionSteps.Count != 0)
        {
            _curretActionSteps.Clear();
            _totalStepsCount.Clear();
        }
    }
    public void CreateNewAction()
    {
        StrStorylineParameters uncomposedStoryline = SetStrUncomposedStorylineParameters();
        List<string> createdAction = _composer.ComposeAction(uncomposedStoryline);
        if (createdAction.Count != 0)
        {
            foreach (string actionUnit in createdAction)
            {
                _storylineActions.Add(actionUnit);
            }
            ClearActionAssociatedData();
            _actionID += 1;
            _stepID = 1;
            _totalActions += 1;
        }
    }
    public void ExportStorylineToStrFile()
    {
        StrUncomposedStorylineParts storylineParts = SetUncomposedStorylineParts();
        _composedStoryline = _composer.ComposeStoryline(storylineParts);
        string fileContent = "";
        foreach (string storylineLine in _composedStoryline)
        {
            fileContent = fileContent + storylineLine + _tags._lineSeparator;
        }
        _encryptor.ExportToFile(_StorylineName, fileContent);
    }
    private StrStorylineParameters SetStrUncomposedStorylineParameters()
    {
        StrStorylineParameters tempStrStruct;
        tempStrStruct.User = _editorUser;
        tempStrStruct.Version = _version;
        tempStrStruct.ActionID = _actionID;
        tempStrStruct.StepID = _stepID;
        tempStrStruct.Phrase = _phrase;
        tempStrStruct.PhraseAuthor = _phraseAuthor;
        tempStrStruct.IsPhraseHolderActive = GetPhraseHolderActivityState();
        tempStrStruct.PhraseHolder = _phraseHolder;
        tempStrStruct.CGImage = _CGImage;
        tempStrStruct.CGRectTransform = _CGRectTransform;
        tempStrStruct.ActiveCharacters = _activeCharacters;
        tempStrStruct.ActiveRectTransforms = _activeRectTransforms;
        tempStrStruct.StepsOfCurrentAction = _curretActionSteps;
        tempStrStruct.RequiredObjects = _requiredObjects;
        tempStrStruct.RequiredCG = _requiredCG;
        tempStrStruct.ChoiseOptions = _choiseOptions;
        tempStrStruct.JumpMarker = _jumpMarker;
        return tempStrStruct;
    }
    private bool GetPhraseHolderActivityState()
    {
        return _phraseHolder.activeInHierarchy;
    }
    private StrUncomposedStorylineParts SetUncomposedStorylineParts()
    {
        StrUncomposedStorylineParts storylineParts;
        storylineParts.InitPart = _initPart;
        storylineParts.StorylineActions = _storylineActions;
        return storylineParts;
    }
    private void ClearActionAssociatedData()
    {
        _curretActionSteps.Clear();
        _totalStepsCount.Clear();
        _jumpMarker = "";
    }
    public void AddCG(string CGPath, string CGName)
    {
        try
        {
            Debug.Log("add path "+ CGPath);
            Debug.Log("add name "+ CGName);
            Texture2D tempTexture = Resources.Load(CGPath) as Texture2D;
            _CGsprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(tempTexture.width / 2, tempTexture.height / 2));
            _CGsprite.name = CGName;
            if (_requiredCG.Count == 0)
            {
                _requiredCG.Add(_CGsprite);
                _CGImage.sprite = _CGsprite;
            }
            else
            {
                if (CheckCGExistence(CGName))
                {
                    _requiredCG.Add(_CGsprite);
                    _CGImage.sprite = _CGsprite;
                }
                else
                {
                    _CGImage.sprite = _CGsprite;
                }
            }
            UpdateInitPart();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);          
        }       
    }
    public void AddCharacter(string CharacterPath, string CharacterName)
    {
        try
        {
            if (_requiredObjects.Count == 0)
            {
                _ñharacter = _characterSpawner.Spawn(_canvas, CharacterPath, CharacterName);
                SetupCharacter(_ñharacter);
                UpdateInitPart();
                _ñharacter = null;
            }
            else
            {
                if (CheckCharacterExistence(CharacterName))
                {
                    _ñharacter = _characterSpawner.Spawn(_canvas, CharacterPath, CharacterName);
                    SetupCharacter(_ñharacter);
                    UpdateInitPart();
                    _ñharacter = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "This Character already exists.", "OK"); // to editor ui
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("" + ex.Message);
        }

    }
    private void SetupCharacter(GameObject Character)
    {
        _requiredObjects.Add(Character);
        RectTransform RT = Character.GetComponent<RectTransform>();
        RT.localPosition = new Vector3(458f, -121f, 0f);
        RT.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        Character.transform.SetParent(_CGRectTransform.transform, false);
        _activeRectTransforms.Add(RT);
        if (_activeCharacters.Count == 0)
        {
            _activeCharacters.Add(_ñharacter);
        }
        else
        {
            _ñharacter.SetActive(false);
        }
    }
    public void SelectAction(int TargetActionID)
    {
        _actionID = TargetActionID;
        List<string> selectedAction = _replacer.GetSelectedActionData(TargetActionID);
        SelectedActionSetup(selectedAction);
    }
    public void OverwriteSelectedAction()
    {
        DeleteStep(0);
        CreateNewStep();
        StrStorylineParameters uncomposedStoryline = SetStrUncomposedStorylineParameters();
        List<string> createdAction = _composer.ComposeAction(uncomposedStoryline);
        _storylineActions = _replacer.ReplaceSelectedAction(createdAction);
    }
    public void SelectedActionSetup(List<string> selectedActionData)
    {
        StrDecomposedAction decomposedAction = _decomposer.DecomposeSelectedAction(selectedActionData);
        SetPhrase(decomposedAction.Phrase);
        SetAuthor(decomposedAction.PhraseAuthor);
        DefinePhraseHolderActivityState(decomposedAction.IsPhraseHolderActive);
        TranslocatePhraseHolder(decomposedAction.PhraseHolderPosition.x);
        SetPhraseHolderText(_phraseAuthor, _phrase);
        SetCG(decomposedAction.CGImageName);
        DefineCharacterActivityState(decomposedAction.ActiveCharacters);
        foreach (KeyValuePair<string, Vector3> characterPosition in decomposedAction.ActiveCharactersPositions)
        {
            TranslocateCharacters(characterPosition.Key, characterPosition.Value);
        }
        foreach (KeyValuePair<string, Vector2> characterScale in decomposedAction.ActiveCharactersScales)
        {
            RescaleCharacters(characterScale.Key, characterScale.Value);
        }
        SetChoiseOptions(decomposedAction.ChoiseOptions);
        SetJumpMarker(decomposedAction.JumpToAction);
        TranslocateCG(decomposedAction.CGPosition.x);
        SetCGPosition(decomposedAction.CGPosition.x);
    }
    private Boolean TranslocateCharacters(string characterName, Vector3 characterPosition)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == characterName)
            {
                _requiredObjects[i].GetComponent<RectTransform>().localPosition = characterPosition;
            }
        }
        return true;
    }
    private void RescaleCharacters(string characterName, Vector2 characterScale)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == characterName)
            {
                _requiredObjects[i].GetComponent<RectTransform>().sizeDelta = characterScale;
            }
        }
    }
    private void SetChoiseOptions(List<string> choiseOptions)
    {
        if (!choiseOptions.Contains(_tags._null))
        {
            _choiseOptions = choiseOptions;
        }
        else
        {
            _choiseOptions.Clear();
        }
    }
    private void SetJumpMarker(string jumpMarker)
    {
        if (jumpMarker != _tags._null)
        {
            _jumpMarker = jumpMarker;
        }
        else
        {
            _jumpMarker = "";
        }
    }
    public void CreateChoiseOption(StrChoiseOption choiseOption)
    {
        if (_jumpMarker == "")
        {
            int optionNumber = _choiseOptions.Count + 1;
            string currencyType = choiseOption.CurrencyType;
            int costValue = choiseOption.CostValue;
            int jumpToActionID = choiseOption.JumpToActionID;
            int givedItemID = choiseOption.GivedItemID;
            string optionText = choiseOption.OptionText;
            string option = optionNumber.ToString() + _tags._separator + currencyType + _tags._separator + costValue + _tags._separator + jumpToActionID + _tags._separator + givedItemID + _tags._separator + optionText;
            _choiseOptions.Add(option);
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "This action already has a redirect.", "OK");
        }
    }
    public List<string> GetChoiseOptionsList()
    {
        return _choiseOptions;
    }
    public void DeleteChoiseOption(int optionIndex)
    {
        _choiseOptions.Remove(_choiseOptions[optionIndex]);
        RenumberChoiseOptionsList();
    }
    public void ChangeChoiseOptionPosition(int optionIndex, StrListDirection direction)
    {
        string ReplacedOption = null;
        int ReplacedID = 0;
        if (direction == StrListDirection.Up)
        {
            ReplacedID = optionIndex - 1;
        }
        if (direction == StrListDirection.Down)
        {
            ReplacedID = optionIndex + 1;
        }
        ReplacedOption = _choiseOptions[ReplacedID];
        _choiseOptions[ReplacedID] = _choiseOptions[optionIndex];
        _choiseOptions[optionIndex] = ReplacedOption;
        RenumberChoiseOptionsList();
    }
    public void RenumberChoiseOptionsList()
    {
        for (int i = 0; i < _choiseOptions.Count; i++)
        {
            string[] Units = _choiseOptions[i].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Units[0] = (i + 1).ToString();
            string temp = "";
            for (int r = 0; r < Units.Length; r++)
            {
                temp += Units[r] + _tags._separator;
            }
            _choiseOptions[i] = temp;
        }
    }
    public void CreateJumpMarker(int targetActionIndex)
    {
        if (_choiseOptions.Count == 0)
        {
            _jumpMarker = targetActionIndex.ToString();
        }
        else
        {
            _jumpMarker = null;
            EditorUtility.DisplayDialog("Notice", "This action already has a redirect.", "OK");
        }
    }
    private void DefineCharacterActivityState(List<string> actionActiveCharacters)
    {
        if (actionActiveCharacters.Count != 0)
        {
            List<int> activatedCharactersIndices = new List<int>();
            List<int> remainActiveCharactersIndices = new List<int>();
            List<int> deactivatedCharactersIndices = new List<int>();
            foreach (string characterName in actionActiveCharacters)
            {
                for (int i = 0; i < _requiredObjects.Count; i++)
                {
                    string tempName = _requiredObjects[i].ToString().Replace(" (UnityEngine.GameObject)", "");
                    if (tempName == characterName && characterName != _tags._null)
                    {
                        if (_activeCharacters.Count != 0)
                        {
                            if (_activeCharacters.Contains(_requiredObjects[i]))
                            {
                                remainActiveCharactersIndices.Add(i);
                            }
                            else
                            {
                                activatedCharactersIndices.Add(i);
                            }
                        }
                        else
                        {
                            activatedCharactersIndices.Add(i);
                        }
                    }
                }
            }
            deactivatedCharactersIndices = FormDeactivatedCharactersIndicesList(activatedCharactersIndices, remainActiveCharactersIndices);
            foreach (int activatedCharacterIndex in activatedCharactersIndices)
            {
                ActivateExistingCharacter(_requiredObjects[activatedCharacterIndex].name);
            }
            foreach (int deactivatedCharacterIndex in deactivatedCharactersIndices)
            {
                DeactivateCharacter(_requiredObjects[deactivatedCharacterIndex].name);
            }
            _StrEvents.EditorUpdated();
        }
    }
    private List<int> FormDeactivatedCharactersIndicesList(List<int> activated, List<int> remainActive)
    {
        List<int> tempList = new List<int>();
        for (int i = 0; i < _activeCharacters.Count; i++)
        {
            if (!activated.Contains(i) && !remainActive.Contains(i))
            {
                tempList.Add(i);
            }
        }
        return tempList;
    }
    Boolean CheckCharacterExistence(string characterName)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == characterName)
            {
                return false;
            }
        }
        return true;
    }
    Boolean CheckCGExistence(string CGName)
    {
        for (int i = 0; i < _requiredCG.Count; i++)
        {
            if (_requiredCG[i].name == CGName)
            {
                return false;
            }
        }
        return true;
    }
    public Boolean CheckStorylineExistence(string fileName)
    {
        try
        {
            StreamReader SR = new StreamReader(_folders._storylines + "/" + fileName);
            string line = SR.ReadLine();
            SR.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        return true;
    }
    public Boolean DeactivateCharacter(string CharacterName)
    {
        foreach (var Character in _activeCharacters)
        {
            if (Character.name == CharacterName)
            {
                string temp = Character.ToString().Replace(" (UnityEngine.GameObject)", "");
                Character.SetActive(false);
                _activeCharacters.Remove(Character);
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
    public Boolean ActivateExistingCharacter(string characterName)
    {
        foreach (var character in _requiredObjects)
        {
            if (character.name == characterName)
            {
                string tempName = character.ToString().Replace(" (UnityEngine.GameObject)", "");
                character.SetActive(true);
                _activeCharacters.Add(character);
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
    public Boolean CreateNewStoryline(string fileName, string user)
    {
        try
        {
            StreamWriter SW = new StreamWriter(_folders._storylines + "/" + fileName, true, encoding: System.Text.Encoding.Unicode);
            string t = "";
            SW.WriteLine(t);
            SW.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);

            return false;
        }
        _stepID = 1;
        _actionID = 1;
        _editorUser = user;
        _StorylineName = fileName;
        LoadFromFile(fileName);
        return true;
    }

    public void ResetEditor()
    {
        _phraseHolderRectTransform.localPosition = new Vector3(0f, 0f, 0f);
        SetCGPosition(0f);
        _phrase = string.Empty;
        _phraseHolderAuthor.text = StrConstantValues.PlaceholderText;
        _phraseHolderPhrase.text = StrConstantValues.PlaceholderText; ;
        DefinePhraseHolderActivityState(true.ToString());
        foreach (GameObject destroy in _requiredObjects)
        {
            DestroyImmediate(destroy);
        }
        _replacer._selectedActionData.Clear();
        _replacer._selectedActionSteps.Clear();
        _requiredObjects.Clear();
        _activeCharacters.Clear();
        _activeRectTransforms.Clear();
        _requiredCG.Clear();
        _storylineActions.Clear();
        _initPart.Clear();
        _totalStepsCount.Clear();
        _choiseOptions.Clear();
        _CGImage.sprite = null;
        _CGsprite = null;
        _phraseAuthor = string.Empty;
        _stepID = 1;
        _actionID = 1;
        _totalActions = 0;
        _StrEvents.EditorUpdated();
    }

    public Boolean ValidateStoryline()
    {
        if (_StorylineName == "" || _StorylineName != null)
        {
            return false;
        }
        return true;
    }
  public void SetUser(string user)
    {
        _editorUser = user;
    }
   public  void SetVersion(float version)
    {
        _version = version;
    }
    private void SetStorylineActions(List<string> storylineActions)
    {
        _storylineActions = storylineActions;
    }
    private void SetTotalActions(int totalActions)
    {
        _totalActions = totalActions;
    }
    public void SetActionID(int actionID)
    {
        _actionID = actionID;
    }
    private void SetCG(string CGName)
    {
        foreach (Sprite CGSprite in _requiredCG)
        {
            if (CGSprite.name == CGName)
            {
                _CGImage.sprite = CGSprite;
                SceneViewRepaintCrutch();
                _StrEvents.EditorUpdated();
            }
        }
    }
    public void SetPhrase(string phrase)
    {
        _phrase = phrase;
        SetPhraseHolderText(_phraseAuthor, _phrase);
        SceneViewRepaintCrutch();
    }
    public void SetAuthor(string authorObjectName)
    {
        string temp = authorObjectName.ToString().Replace(" (UnityEngine.GameObject)", "");
        _phraseAuthor = temp;
        SetPhraseHolderText(_phraseAuthor, _phrase);
        SceneViewRepaintCrutch();
    }
    public void SelectPhraseHolderInHierarchy()
    {
        Selection.activeGameObject = _phraseHolder;
    }
    public void SelectActiveCharacterInHierarchy(string characterName)
    {
        foreach (GameObject character in _activeCharacters)
        {
            if (character.name == characterName)
            {
                Selection.activeGameObject = character;
            }
        }
    }
    public void SceneViewRepaintCrutch()
    {
        DefineBordersActivityState();
        DefineBordersActivityState();
    }
    public Boolean DeleteCharacter(string characterName)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == characterName)
            {
                GameObject tempObject = _requiredObjects[i];
                tempObject.SetActive(true);
                _requiredObjects.Remove(_requiredObjects[i]);

                for (int i2 = 0; i2 < _activeCharacters.Count; i2++)
                {
                    if (_activeCharacters[i2] != null && _activeCharacters[i2].name == characterName)
                    {
                        _activeCharacters.Remove(_activeCharacters[i2]);
                    }
                }
                for (int i5 = 0; i5 < _activeRectTransforms.Count; i5++)
                {
                    if (_activeRectTransforms[i5] != null && _activeRectTransforms[i5].name == characterName)
                    {
                        _activeRectTransforms.Remove(_activeRectTransforms[i5]);
                    }
                }
                if (_initPart.Count != 0)
                {
                    for (int i8 = 0; i8 < _initPart.Count; i8++)
                    {
                        if (_initPart[i8] != null)
                        {
                            string y = characterName + _tags._separator;
                            string m = _initPart[i8].Replace(y, "");

                            _initPart[i8] = m;

                        }
                    }
                }
                if (_storylineActions.Count != 0)
                {
                    for (int i9 = 0; i9 < _storylineActions.Count; i9++)
                    {
                        if (_storylineActions[i9] != null)
                        {
                            string y = characterName + _tags._separator;
                            if (_storylineActions[i9] == characterName)
                            {
                                _storylineActions[i9] = "";
                            }
                            if (_storylineActions[i9].StartsWith("          " + y) && _storylineActions[i9 - 1] == _tags._characterRelocated)
                            {
                                _storylineActions.Remove(_storylineActions[i9]);
                            }
                            else
                            {
                                string m = _storylineActions[i9].Replace(y, "");
                                _storylineActions[i9] = m;
                            }
                        }
                    }
                }
                DestroyImmediate(tempObject);
                CheckForExeptions();
            }

        }

        return true;
    }
    private void CheckForExeptions()
    {
        for (int i = 0; i < _storylineActions.Count; i++)
        {

            if (_storylineActions[i].StartsWith(_tags._author))
            {
                if (_storylineActions[i + 1] == "")
                {
                    string[] units = _storylineActions[i].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int action_id = int.Parse(units[1]);
                    _exeptions.Set_no_author(action_id);
                }
            }
        }
    }
    public void SaveToFile()
    {        
        StrRawStr rawStr = new StrRawStr();
        rawStr.User = _editorUser;
        rawStr.Version = _version;
        rawStr.ActionID = _actionID;
        rawStr.StepID = _stepID;
        rawStr.Phrase = _phrase;
        rawStr.PhraseAuthor = _phraseAuthor;
        rawStr.IsPhraseHolderActive = GetPhraseHolderActivityState();
        rawStr.PhraseHolderPosition = _phraseHolderRectTransform.localPosition;
        rawStr.CGPosition = _CGRectTransform.localPosition;
         List<string> tempActiveCharacters = new List<string>();
        foreach (GameObject character in _activeCharacters)
        {
            tempActiveCharacters.Add(character.name);
        }
        rawStr.ActiveCharacters = tempActiveCharacters;
        List<string> tempRequiredObjects = new List<string>();
        Dictionary<string, Vector3> tempCharactersPositions = new Dictionary<string, Vector3>();
        Dictionary<string, Vector2> tempCharactersScales = new Dictionary<string, Vector2>();
        if (_requiredObjects.Count != 0)
        {
            foreach (GameObject gObject in _requiredObjects)
            {
                RectTransform gObjectRectTransform = gObject.GetComponent<RectTransform>();
                tempRequiredObjects.Add(gObject.name);
                tempCharactersPositions.Add(gObject.name, gObjectRectTransform.localPosition);
                tempCharactersScales.Add(gObject.name, new Vector2(gObjectRectTransform.rect.width, gObjectRectTransform.rect.height));
            }
        }
        rawStr.RequiredCharacters = tempRequiredObjects;
        rawStr.CharactersPositions = tempCharactersPositions;
        rawStr.CharactersScales = tempCharactersScales;
        List<string> tempRequiredCG = new List<string>();
        if (_requiredCG.Count != 0)
        {
            foreach (Sprite requiredCG in _requiredCG)
            {
                tempRequiredCG.Add(requiredCG.name);
            }
        }
        rawStr.RequiredCGs = tempRequiredCG;
        rawStr.ChoiseOptions = _choiseOptions;
        rawStr.JumpMarker = _jumpMarker;
        rawStr.StorylineName = _StorylineName;
        rawStr.InitPart = _initPart;
        rawStr.StorylineActions = _storylineActions;
        rawStr.CurretActionSteps = _curretActionSteps;
        rawStr.TotalStepsCount = _totalStepsCount;
        rawStr.CGSpriteName = _CGsprite.name;
        rawStr.IsReadyForNextAction = _readyForNextAction;
        rawStr.RefereceResolutionWidht = _refereceResolutionWidht;
        rawStr.TotalActions = _totalActions;      
       
        List<string> tempRawStr = _composer.ComposeRawStr(rawStr);
        try
        {
            string tempName = _StorylineName.Replace(StrExtensions.FinalStr, string.Empty);
            string fileName = tempName + StrExtensions.RawStr;
            StreamWriter SW = new StreamWriter(_folders._savedStorylines + "/" + fileName, true, encoding: System.Text.Encoding.Unicode);
            foreach (string unit in tempRawStr)
            {
                SW.WriteLine(unit);
            }
            SW.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }       
    }
    public void LoadFromFile(string filePath)
    {
        List<string> rstrContent = new List<string>();
        StreamReader SR = new StreamReader(filePath);
        string line = SR.ReadLine();
        rstrContent.Add(line);
        while (line != null)
        {
            line = SR.ReadLine();
            rstrContent.Add(line);
        }
        SR.Close();
        StrRawStr rawStr = _decomposer.DecomposeRawStr(rstrContent);
        SetupEditor(rawStr);            
    }
    private void SetupEditor(StrRawStr rawStr)
    {
        SetUser(rawStr.User);
        SetVersion(rawStr.Version);
        SetStorylineActions(rawStr.StorylineActions);
        SetTotalActions(rawStr.TotalActions);
        InitializeCharacters(rawStr.RequiredCharacters);
        InitializeRequiredCGs(rawStr.RequiredCGs);
        if (rawStr.ActionID <= rawStr.TotalActions)
        {
            SelectAction(rawStr.ActionID);
        }
        else      
        {
            SetActionID(rawStr.ActionID);
            SetPhrase(rawStr.Phrase);
            SetAuthor(rawStr.PhraseAuthor);
            DefinePhraseHolderActivityState(rawStr.IsPhraseHolderActive.ToString());
            TranslocatePhraseHolder(rawStr.PhraseHolderPosition.x);
            SetPhraseHolderText(_phraseAuthor, _phrase);
            SetCG(rawStr.CGSpriteName);
            DefineCharacterActivityState(rawStr.ActiveCharacters);
            SetChoiseOptions(rawStr.ChoiseOptions);
            SetJumpMarker(rawStr.JumpMarker);
            TranslocateCG(rawStr.CGPosition.x);
            SetCGPosition(rawStr.CGPosition.x);
            foreach (KeyValuePair<string, Vector3> characterPosition in rawStr.CharactersPositions)
            {
                TranslocateCharacters(characterPosition.Key, characterPosition.Value);
            }          
            foreach (KeyValuePair<string, Vector2> characterScale in rawStr.CharactersScales)
            {
                RescaleCharacters(characterScale.Key, characterScale.Value);
            }          
            _StrEvents.EditorUpdated();
            SceneViewRepaintCrutch();
        }
    }
    private void InitializeCharacters(List<string> requiredCharacters)
    {
        if (requiredCharacters.Count != 0)
        {
            foreach (string charcter in requiredCharacters)
            {
                string tempPath = _folders._characters + "/" + charcter + StrExtensions.Character;
                AddCharacter(tempPath, name);
            }
        }
    }   
    private void InitializeRequiredCGs(List<string> requiredCGs)
    {
        if (requiredCGs.Count != 0)
        {
            foreach (string CG in requiredCGs)
            {
                string tempPath = _folders._CG + "/" + CG;
                string path = tempPath.Replace(_folders._root + "/Resources/", string.Empty);            
                AddCG(path, CG);
            }
        }
    }
    private void OnDisable()
    {
        _StrEvents.StrEditorRootObjectRequested -= OnStrEditorRootObjectRequested;
    }
}
