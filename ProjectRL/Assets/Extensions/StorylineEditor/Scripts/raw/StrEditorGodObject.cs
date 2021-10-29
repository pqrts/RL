using StorylineEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(StrEditorEvents))]
public class StrEditorGodObject : MonoBehaviour, IStrEditorRoot
{
    private bool _isStrEditorRootObjectInitialized;
    public Sprite _tempCharIcon;
    public GameObject _canvas;
    [SerializeField] private int _refereceResolutionWidht;
    [HideInInspector] public string _initStatus;
    public int _actionID;
    public int _actionsTotalID;
    public int _stepID;
    [SerializeField] private int _assemblyStagesCount = 4;
    //scripts
    public global_taglist _s_Tag;
    public global_folders _s_Folder;
    extStrEditorReplacer _s_replacer;
    ext_Storyline_exeptions _s_exeption;
    //metadata
    private string _metaToStr;
    [SerializeField] public string _editorUser;
    [SerializeField] private float _version;
    private DateTime _dateToStr;
    //for Initialization part
    public List<GameObject> _requiredObjects = new List<GameObject>();
    [HideInInspector] public List<Sprite> _requiredCG = new List<Sprite>();
    //steps parameters
    private List<RectTransform> _activeRectTransforms = new List<RectTransform>();
    public List<GameObject> _activeCharacters = new List<GameObject>();
    private List<string> _activatedCharacters = new List<string>();
    private List<string> _inactivatedCharacters = new List<string>();
    private List<string> _activatedObjects = new List<string>();
    public List<string> _inactivatedObjects = new List<string>();
    public List<string> _choiseOptions = new List<string>();

    private string _toActivation;
    private string _toInactivation;
    public string _StorylineName;
    //for str form
    public List<string> _initPartToStr = new List<string>();
    public List<string> _actionsToStr = new List<string>();
    private List<string> _stepsToAction = new List<string>();
    [HideInInspector] public List<string> _stepsTotal = new List<string>();
    [HideInInspector] public List<List<string>> _actionsTotal = new List<List<string>>();
    //scene
    [HideInInspector] public Sprite _CGsprite;
    public Image _CGImage;
    private float _leftCameraBorderPosition;
    private float _rightCameraBorderPosition;
    private float[] _movingPoolPositions;
    [HideInInspector] public string _phrase;
    [HideInInspector] public string _phraseAuthor;
    //characters spawn
    public GameObject _ñharacter;
    ext_CharacterSp _s_CharacterSp;
    [HideInInspector] public float _ñanvasMovingPool;
    [HideInInspector] public float _cgMovingPool;
    [HideInInspector] public float _cgPositionX;
    [HideInInspector] public RectTransform _CGRectTransform;
    [HideInInspector] public float _leftCGEdgePosition;
    [HideInInspector] public float _rightCGEdgePosition;
    [HideInInspector] public bool _readyForNextAction;

    private StrEditorEvents _s_StrEvent;
    private void OnEnable()
    {
        _s_StrEvent.StrEditorRootObjectRequested += OnStrEditorRootObjectRequested;
    }
    private void OnStrEditorRootObjectRequested()
    {
        if (_isStrEditorRootObjectInitialized == true)
        {
            _s_StrEvent.DeclareStrEditorRootObject(this);
        }
    }
    public void Init()
    {
        _s_CharacterSp = GetComponent<ext_CharacterSp>();
        _s_StrEvent = GetComponent<StrEditorEvents>();
        _s_Tag = GetComponent<global_taglist>();
        _s_Folder = GetComponent<global_folders>();
        _s_replacer = GetComponent<extStrEditorReplacer>();
        _CGRectTransform = _CGImage.GetComponent<RectTransform>();
        _s_exeption = GetComponent<ext_Storyline_exeptions>();
        if (_s_Folder.Setup_folders() && _s_Tag.Setup_tags() && GetCGPositionLimits() && _s_replacer.GetScripts())
        {
            _initStatus = "successful";
            _isStrEditorRootObjectInitialized = true;
            _s_StrEvent.EditorUpdated();
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
    public void MoveCG(float CGPositionX)
    {
        float x = _movingPoolPositions[0] - CGPositionX;
        _CGRectTransform.localPosition = new Vector3(x, _CGRectTransform.localPosition.y, _CGRectTransform.localPosition.z);
    }

    public void ReadStoryline()
    {
        try
        {
            StreamReader SR = new StreamReader(_s_Folder._storylines + "/" + _StorylineName);
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
    public Boolean WriteStorylineToStr()
    {
        try
        {
            StreamWriter SW = new StreamWriter(_s_Folder._storylines + "/" + _StorylineName, true, encoding: System.Text.Encoding.Unicode);
            for (int i = 0; i < _actionsToStr.Count; i++)
            {
                string t = _actionsToStr[i];
                SW.WriteLine(t);
            }
            SW.Close();
            _actionsToStr.Clear();
        }
        catch (Exception ex)
        {

        }
        return true;
    }
    public Boolean CreateStep()
    {
        int StepAssemblyStagesCount = 5;
        _toActivation = "";
        _toInactivation = "";
        for (int i = 0; i <= StepAssemblyStagesCount; i++)
        {
            switch (i)
            {
                case 0:
                    _stepsToAction.Add(_s_Tag._step + _s_Tag._separator + _stepID);
                    _stepsToAction.Add("          " + _s_Tag._skip);
                    break;
                case 1:
                    foreach (string line in _activatedCharacters)
                    {
                        if (line != null)
                        {
                            _toActivation = _toActivation + line + _s_Tag._separator;
                        }
                        else
                        {
                            _stepsToAction.Add("          " + _s_Tag._skip);
                        }
                    }
                    foreach (string line2 in _inactivatedCharacters)
                    {
                        if (line2 != null)
                        {
                            _toInactivation = _toInactivation + line2 + _s_Tag._separator;
                        }
                        else
                        {
                            _stepsToAction.Add("          " + _s_Tag._skip);
                        }
                    }
                    break;
                case 2:
                    _stepsToAction.Add("          " + _s_Tag._cg_position);
                    float CGPositionX = Mathf.Round(_CGRectTransform.localPosition.x);
                    float CGPositionY = _CGRectTransform.localPosition.y;
                    float CGPositionZ = _CGRectTransform.localPosition.z;
                    _stepsToAction.Add("          " + CGPositionX + _s_Tag._separator + CGPositionY + _s_Tag._separator + CGPositionZ + _s_Tag._separator);
                    _stepsToAction.Add("          " + _s_Tag._activate);
                    if (_toActivation != "")
                    {
                        _stepsToAction.Add("          " + _toActivation);
                    }
                    else
                    {
                        _stepsToAction.Add("          " + _s_Tag._null);
                    }
                    _stepsToAction.Add("          " + _s_Tag._inactivate);
                    if (_toInactivation != "")
                    {
                        _stepsToAction.Add("          " + _toInactivation);
                    }
                    else
                    {
                        _stepsToAction.Add("          " + _s_Tag._null);
                    }
                    break;
                case 3:
                    _stepsToAction.Add("          " + _s_Tag._character_relocated);
                    for (int e = 0; e < _activeCharacters.Count; e++)
                    {
                        string char_name = _activeCharacters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                        float pos_x = _activeRectTransforms[e].localPosition.x;
                        float pos_y = _activeRectTransforms[e].localPosition.y;
                        float pos_z = _activeRectTransforms[e].localPosition.z;
                        _stepsToAction.Add("          " + char_name + _s_Tag._separator + pos_x + _s_Tag._separator + pos_y + _s_Tag._separator + pos_z + _s_Tag._separator);
                    }
                    break;
                case 4:
                    _stepsToAction.Add("          " + _s_Tag._skip);
                    _activatedCharacters.Clear();
                    _inactivatedCharacters.Clear();
                    _toActivation = "";
                    _toInactivation = "";
                    _stepID += 1;
                    _stepsTotal.Add(_stepID.ToString());
                    break;
            }
        }
        return true;
    }
    public Boolean NewAction()
    {
        if (ActionAssembly())
        {
            _actionID += 1;
            _stepID = 1;
            _actionsTotalID += 1;
            _stepsToAction.Clear();
            _stepsTotal.Clear();
        }
        return true;
    }
    void InitPartUpdate()
    {
        _initPartToStr.Clear();
        _dateToStr = System.DateTime.Now;
        _metaToStr = "Created by: " + _editorUser + " at " + _dateToStr;
        _initPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "**************************META**************************");
        _initPartToStr.Add(_s_Tag._skip + _s_Tag._separator + _metaToStr);
        _initPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "********************************************************");
        _initPartToStr.Add(_s_Tag._init);
        _initPartToStr.Add(_s_Tag._skip);
        _initPartToStr.Add(_s_Tag._version);
        _initPartToStr.Add("" + _version);
        _initPartToStr.Add(_s_Tag._required_objects);
        string RequiredObjects = "";
        foreach (GameObject unit in _requiredObjects)
        {
            RequiredObjects = RequiredObjects + unit.name + _s_Tag._separator;
        }
        _initPartToStr.Add(RequiredObjects);
        _initPartToStr.Add(_s_Tag._required_cg);
        string RequiredCG = "";
        foreach (Sprite unit2 in _requiredCG)
        {
            RequiredCG = RequiredCG + unit2.name + _s_Tag._separator;
        }
        _initPartToStr.Add(RequiredCG);
        _initPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "********************************************************");
        _initPartToStr.Add(_s_Tag._start);
        _initPartToStr.Add(_s_Tag._skip);
    }
    Boolean ActionAssembly()
    {
        for (int i = 0; i <= _assemblyStagesCount; i++)
        {
            switch (i)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:
                    _actionsToStr.Add(_s_Tag._action + _s_Tag._separator + _actionID);
                    _actionsToStr.Add("{");
                    _actionsToStr.Add(_s_Tag._phrase + _s_Tag._separator + _actionID);
                    _actionsToStr.Add(_phrase);
                    _actionsToStr.Add(_s_Tag._author + _s_Tag._separator + _actionID);
                    _actionsToStr.Add(_phraseAuthor);
                    _actionsToStr.Add(_s_Tag._CG);
                    string t = _CGImage.sprite.ToString().Replace(" (UnityEngine.Sprite)", "");
                    _actionsToStr.Add(t);
                    break;
                case 3:
                    foreach (var step in _stepsToAction)
                    {
                        _actionsToStr.Add(step);
                    }
                    string endstep = "/&endstep";
                    _actionsToStr.Add(endstep);
                    break;
                case 4:
                    _actionsToStr.Add("}");
                    _actionsToStr.Add(_s_Tag._skip);
                    _stepsToAction.Clear();
                    break;
            }
        }
        return true;
    }
    public Boolean AddCG(string CGPath, string CGName)
    {
        try
        {
            Texture2D tex;
            tex = Resources.Load(CGPath) as Texture2D;
            _CGsprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
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
            InitPartUpdate();
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
    public void AddCharacter(string CharacterPath, string CharacterName)
    {
        try
        {
            if (_requiredObjects.Count == 0)
            {
                _ñharacter = _s_CharacterSp.Spawn(_canvas, _s_Folder._root, _s_Folder._body, _s_Folder._haircut, _s_Folder._clothes, _s_Folder._makeup, CharacterPath, CharacterName);
                SetupCharacter(_ñharacter);
                InitPartUpdate();
                _ñharacter = null;
            }
            else
            {
                if (CheckCharacterExistence(CharacterName))
                {
                    _ñharacter = _s_CharacterSp.Spawn(_canvas, _s_Folder._root, _s_Folder._body, _s_Folder._haircut, _s_Folder._clothes, _s_Folder._makeup, CharacterPath, CharacterName);
                    SetupCharacter(_ñharacter);
                    InitPartUpdate();
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
            _activatedCharacters.Add(Character.name);
            _activeCharacters.Add(_ñharacter);
        }
        else
        {
            _inactivatedCharacters.Add(Character.name);
            _ñharacter.SetActive(false);
        }
    }
    public void SelectAction(int TargetActionID)
    {
        _actionID = TargetActionID;
        _activatedCharacters.Clear();
        _inactivatedCharacters.Clear();
        _s_replacer.GetSelectedActionData();

    }

    public void SelectedActionSetup()
    {
        string StepRaw = _s_replacer._selectedActionSteps[_stepID - 1];
        string[] units = StepRaw.Split(_s_Tag._separator_vert.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == _s_Tag._activate)
            {
                if (units[i + 1] != _s_Tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit2 in units2)
                    {
                        _activatedObjects.Add(unit2);
                    }
                }
            }
            if (units[i] == _s_Tag._inactivate)
            {
                if (units[i + 1] != _s_Tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit2 in units2)
                    {
                        _inactivatedObjects.Add(unit2);
                    }
                }
            }

            if (units[i] == _s_Tag._character_relocated)
            {
                if (units[i + 1] != _s_Tag._null)
                {
                    for (int k = (i + 1); k < units.Length; k++)
                    {
                        if (units[k] != _s_Tag._skip)
                        {
                            string line = units[k];
                            string[] units2 = line.Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            string TempCharacterName = units2[0];
                            float TempCharacterPositionX = float.Parse(units2[1], CultureInfo.InvariantCulture);
                            float TempCharacterPositionY = float.Parse(units2[2], CultureInfo.InvariantCulture);
                            float TempCharacterPositionZ = float.Parse(units2[3], CultureInfo.InvariantCulture);
                            RelocateObjects(TempCharacterName, TempCharacterPositionX, TempCharacterPositionY, TempCharacterPositionZ);
                        }
                    }
                }
            }
            if (units[i] == _s_Tag._rescale)
            {
                if (units[i + 1] != _s_Tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string TempCharacterName = units2[0];
                    float TempCharacterScaleX = float.Parse(units2[1], CultureInfo.InvariantCulture);
                    float TempCharacterScaleY = float.Parse(units2[2], CultureInfo.InvariantCulture);
                    float TempCharacterScaleZ = float.Parse(units2[3], CultureInfo.InvariantCulture);
                    // Rescale_objects(p_char_name, p_sca_x, p_sca_y, p_sca_z);
                }
            }
            if (units[i] == _s_Tag._cg_position)
            {
                if (units[i + 1] != _s_Tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    float TempCGPositionX = float.Parse(units2[0], CultureInfo.InvariantCulture);
                    float TempCGPositionY = float.Parse(units2[1], CultureInfo.InvariantCulture);
                    float TempCGPositionZ = float.Parse(units2[2], CultureInfo.InvariantCulture);
                    Vector3 NewCGPosition = new Vector3(TempCGPositionX, TempCGPositionY, TempCGPositionZ);
                    _CGImage.GetComponent<RectTransform>().localPosition = NewCGPosition;
                }
            }
            if (i == units.Length - 1)
            {
                ActivateObjects();
                InactivateObjects();
                break;
            }
        }

    }
    private Boolean ActivateObjects()
    {
        if (_activatedObjects.Count != 0)
        {
            foreach (string unit in _activatedObjects)
            {
                foreach (GameObject GO in _requiredObjects)
                {
                    if (GO.name == unit)
                    {
                        GO.SetActive(true);
                    }
                }
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    private Boolean InactivateObjects()
    {
        if (_inactivatedObjects.Count != 0)
        {
            foreach (string unit in _inactivatedObjects)
            {
                foreach (GameObject GO in _requiredObjects)
                {
                    if (GO.name == unit)
                    {
                        GO.SetActive(false);
                    }
                }
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    private Boolean RelocateObjects(string char_name, float pos_x, float pos_y, float pos_z)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == char_name)
            {
                _requiredObjects[i].GetComponent<RectTransform>().localPosition = new Vector3(pos_x, pos_y, pos_z);
                _requiredObjects[i].GetComponent<RectTransform>().localScale = new Vector3(1.8f, 1.8f, 1.8f);
            }
        }
        return true;
    }
    //   private Boolean Rescale_objects(string char_name, float sca_x, float sca_y, float sca_z)
    //   {
    //       for (int i = 0; i < _list_existing_characters.Count; i++)
    //       {
    //           if (_list_existing_characters[i].name == char_name)
    //           {
    //              _list_existing_RT[i].localScale = new Vector3(sca_x, sca_y, sca_z);
    //         }
    //    }
    //     return true;
    //  }

    // >> check class

    public void CreateChoiseOption(StrChoiseOption choiseOption)
    {
        int optionNumber = _choiseOptions.Count + 1;
        string currencyType = choiseOption.CurrencyType;
        int costValue = choiseOption.CostValue;
        int jumpToActionID = choiseOption.JumpToActionID;
        int givedItemID = choiseOption.GivedItemID;
        string optionText = choiseOption.OptionText;
        string option = optionNumber.ToString() + _s_Tag._separator + currencyType + _s_Tag._separator + costValue + _s_Tag._separator + jumpToActionID + _s_Tag._separator + givedItemID + _s_Tag._separator + optionText;
        _choiseOptions.Add(option);
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
            string[] Units = _choiseOptions[i].Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Units[0] = (i + 1).ToString();
            string t = "";
            for (int r = 0; r < Units.Length; r++)
            {
                t += Units[r] + _s_Tag._separator;
            }
            _choiseOptions[i] = t;
        }
    }
    Boolean CheckCharacterActivation(string CharacterName)
    {
        if (_activatedCharacters.Count != 0)
        {
            foreach (string unit in _activatedCharacters)
            {
                if (unit == CharacterName)
                {
                    return false;
                }
            }
            _activatedCharacters.Add(CharacterName);
        }
        else
        {
            _activatedCharacters.Add(CharacterName);
        }
        return true;
    }
    Boolean CheckCharacterExistence(string char_name)
    {

        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == char_name)
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
    public Boolean CheckStorylineExistence(string file_name)
    {
        try
        {
            StreamReader SR = new StreamReader(_s_Folder._storylines + "/" + file_name);
            string line = SR.ReadLine();
            SR.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message + " existance check");
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
                _inactivatedCharacters.Add(temp);
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
    public Boolean ActivatExistingCharacter(string char_name)
    {
        foreach (var character in _requiredObjects)
        {
            if (character.name == char_name)
            {
                string t = character.ToString().Replace(" (UnityEngine.GameObject)", "");
                _activatedCharacters.Add(t);
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
    public Boolean CreateNewStoryline(string FileName, string User)
    {
        try
        {
            StreamWriter SW = new StreamWriter(_s_Folder._storylines + "/" + FileName, true, encoding: System.Text.Encoding.Unicode);
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
        _editorUser = User;
        _StorylineName = FileName;
        OpenStoryline(FileName);
        return true;
    }
    public Boolean OpenStoryline(string file_name)
    {
        // >> clearing method
        _StorylineName = file_name;
        _s_replacer._selectedActionData.Clear();
        _s_replacer._selectedActionSteps.Clear();
        _requiredObjects.Clear();
        _activatedObjects.Clear();
        _activeCharacters.Clear();
        _activeRectTransforms.Clear();
        _inactivatedCharacters.Clear();
        _activatedCharacters.Clear();
        _requiredCG.Clear();
        _actionsToStr.Clear();
        _initPartToStr.Clear();
        _actionsTotal.Clear();
        _stepsTotal.Clear();
        _CGImage.sprite = null;
        _CGsprite = null;
        _phraseAuthor = "";
        _stepID = 1;
        _actionID = 1;
        foreach (GameObject destroy in _requiredObjects)
        {
            DestroyImmediate(destroy);
        }
        return true;
    }
    public Boolean ValidateStoryline()
    {
        if (_StorylineName == "" || _StorylineName != null)
        {
            return false;
        }
        return true;
    }
    public void SetAuthor(string AuthorObjectName)
    {
        string temp = AuthorObjectName.ToString().Replace(" (UnityEngine.GameObject)", "");
        _phraseAuthor = temp;
    }
    public Boolean DeleteCharacter(string CharacterName)
    {
        for (int i = 0; i < _requiredObjects.Count; i++)
        {
            if (_requiredObjects[i].name == CharacterName)
            {
                GameObject tempObject = _requiredObjects[i];
                tempObject.SetActive(true);
                _requiredObjects.Remove(_requiredObjects[i]);

                for (int i2 = 0; i2 < _activeCharacters.Count; i2++)
                {
                    if (_activeCharacters[i2] != null && _activeCharacters[i2].name == CharacterName)
                    {
                        _activeCharacters.Remove(_activeCharacters[i2]);
                    }
                }
                for (int i3 = 0; i3 < _activatedObjects.Count; i3++)
                {
                    if (_activatedObjects[i3] != null && _activatedObjects[i3] == CharacterName)
                    {
                        _activatedObjects.Remove(_activatedObjects[i3]);
                    }
                }
                for (int i4 = 0; i4 < _inactivatedObjects.Count; i4++)
                {
                    if (_inactivatedObjects[i4] != null && _inactivatedObjects[i4] == CharacterName)
                    {
                        _inactivatedObjects.Remove(_inactivatedObjects[i4]);
                    }
                }
                for (int i5 = 0; i5 < _activeRectTransforms.Count; i5++)
                {
                    if (_activeRectTransforms[i5] != null && _activeRectTransforms[i5].name == CharacterName)
                    {
                        _activeRectTransforms.Remove(_activeRectTransforms[i5]);
                    }
                }
                for (int i6 = 0; i6 < _inactivatedCharacters.Count; i6++)
                {
                    if (_inactivatedCharacters[i6] != null && _inactivatedCharacters[i6] == CharacterName)
                    {
                        _inactivatedCharacters.Remove(_inactivatedCharacters[i6]);
                    }
                }
                for (int i7 = 0; i7 < _activatedCharacters.Count; i7++)
                {
                    if (_activatedCharacters[i7] != null && _activatedCharacters[i7] == CharacterName)
                    {
                        _activatedCharacters.Remove(_activatedCharacters[i7]);
                    }
                }
                if (_initPartToStr.Count != 0)
                {
                    for (int i8 = 0; i8 < _initPartToStr.Count; i8++)
                    {
                        if (_initPartToStr[i8] != null)
                        {
                            string y = CharacterName + _s_Tag._separator;
                            string m = _initPartToStr[i8].Replace(y, "");

                            _initPartToStr[i8] = m;

                        }
                    }
                }
                if (_actionsToStr.Count != 0)
                {
                    for (int i9 = 0; i9 < _actionsToStr.Count; i9++)
                    {
                        if (_actionsToStr[i9] != null)
                        {
                            string y = CharacterName + _s_Tag._separator;
                            if (_actionsToStr[i9] == CharacterName)
                            {
                                _actionsToStr[i9] = "";
                            }
                            if (_actionsToStr[i9].StartsWith("          " + y) && _actionsToStr[i9 - 1] == _s_Tag._character_relocated)
                            {
                                _actionsToStr.Remove(_actionsToStr[i9]);
                            }
                            else
                            {
                                string m = _actionsToStr[i9].Replace(y, "");
                                _actionsToStr[i9] = m;
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
        for (int i = 0; i < _actionsToStr.Count; i++)
        {

            if (_actionsToStr[i].StartsWith(_s_Tag._author))
            {
                if (_actionsToStr[i + 1] == "")
                {
                    string[] units = _actionsToStr[i].Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int action_id = int.Parse(units[1]);
                    _s_exeption.Set_no_author(action_id);
                }
            }
        }
    }
    private void OnDisable()
    {


    }
}
