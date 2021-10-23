using System;
using System.Globalization;

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEditor;
using StorylineEditor;

[ExecuteInEditMode]
public class ext_StorylineEditor : MonoBehaviour
{
    public Sprite _temp_CharIcon;
    public GameObject _Canvas;
    [SerializeField] private int _RefereceResolutionWidht;
    [HideInInspector] public string _init_status;
    public int _IDAction;
    public int _IDActionsTotal;
    public int _IDStep;
    [SerializeField] private int _id_assembly_stages = 4;
    //scripts
    global_taglist _s_Tag;
    public global_folders _s_Folder;
    ext_Storyline_replacer _s_replacer;
    ext_Storyline_exeptions _s_exeption;
    //metadata
    private string _Meta;
    [SerializeField] public string _user;
    [SerializeField] private float _version;
    private DateTime _Date;
    //for Initialization part
    public List<GameObject> _list_RequiredObjects = new List<GameObject>();
    [HideInInspector] public List<Sprite> _list_RequiredCG = new List<Sprite>();
    private string req_objects;
    //steps parameters
    private List<RectTransform> _list_ActiveRectTransforms = new List<RectTransform>();
    public List<GameObject> _list_ActiveCharacters = new List<GameObject>();
    private List<string> _list_ActivatedCharacters = new List<string>();
    private List<string> _list_InactivatedCharacters = new List<string>();
    public List<string> _list_ActivatedObjects = new List<string>();
    public List<string> _list_InactivatedObjects = new List<string>();
    public List<string> _list_ChoiseOptions = new List<string>();

    private string _ToActivation;
    private string _ToInactivation;
    public string _StorylineName;
    //for str form
    public List<string> _InitPartToStr = new List<string>();
    public List<string> _ActionsToStr = new List<string>();
    private List<string> _StepsToAction = new List<string>();
    [HideInInspector] public List<string> _IDStepsTotal = new List<string>();
    [HideInInspector] public List<List<string>> _actions_total = new List<List<string>>();
    //scene
    [HideInInspector] public Sprite _CGSprite;
    public Image _CGImage;
    private float _CameraBorderLeft;
    private float _CameraBorderRight;
    private float[] _MovingPoolPositions;
    private Vector2 _cg_moving_pool_pos;
    [HideInInspector] public string _Phrase;
    [HideInInspector] public string _PhraseAuthor;
    //characters spawn
    public GameObject _Character;
    ext_CharacterSp _s_CharacterSp;
    [HideInInspector] public float _CanvasMovingPool;
    [HideInInspector] public float _cg_moving_pool;
    [HideInInspector] public float _cg_pos_x;
    [HideInInspector] public RectTransform _CG_RectTransform;
    [HideInInspector] public float _cg_edge_left;
    [HideInInspector] public float _cg_edge_right;
    ///?
    private int _id_decomposed_steps;
    [HideInInspector] public bool _ready_for_next_action;
    ///


    public void Init()
    {

        _s_CharacterSp = GetComponent<ext_CharacterSp>();
        _s_Tag = GetComponent<global_taglist>();
        _s_Folder = GetComponent<global_folders>();
        _s_replacer = GetComponent<ext_Storyline_replacer>();
        _CG_RectTransform = _CGImage.GetComponent<RectTransform>();
        _s_exeption = GetComponent<ext_Storyline_exeptions>();
        if (_s_Folder.Setup_folders() && _s_Tag.Setup_tags() && GetCGPositionLimits() && _s_replacer.Get_scripts())
        {
            _init_status = "successful";
        }
        else
        {
            _init_status = "failed";
        }
    }
    Boolean GetCGPositionLimits()
    {
        float CGWidht = _CG_RectTransform.rect.width;
        _CameraBorderLeft = 0 - (_RefereceResolutionWidht / 2);
        _CameraBorderRight = 0 + (_RefereceResolutionWidht / 2);
        float LeftX = _CameraBorderLeft + (CGWidht / 2);
        float RightX = _CameraBorderRight - (CGWidht / 2);
        _CanvasMovingPool = LeftX - RightX;
        _MovingPoolPositions = new float[] { LeftX, RightX };
        return true;
    }
    public void MoveCG(float CGPositionX)
    {
        float x = _MovingPoolPositions[0] - CGPositionX;
        _CG_RectTransform.localPosition = new Vector3(x, _CG_RectTransform.localPosition.y, _CG_RectTransform.localPosition.z);
    }

    public void Read_str()
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
    public Boolean FormStoryline()
    {
        try
        {
            StreamWriter SW = new StreamWriter(_s_Folder._storylines + "/" + _StorylineName, true, encoding: System.Text.Encoding.Unicode);
            for (int i = 0; i < _ActionsToStr.Count; i++)
            {
                string t = _ActionsToStr[i];
                SW.WriteLine(t);
            }
            SW.Close();
            _ActionsToStr.Clear();
        }
        catch (Exception ex)
        {

        }
        return true;
    }
    public Boolean CreateStep()
    {
        int StepAssemblyStagesCount = 5;
        _ToActivation = "";
        _ToInactivation = "";
        for (int i = 0; i <= StepAssemblyStagesCount; i++)
        {
            switch (i)
            {
                case 0:
                    _StepsToAction.Add(_s_Tag._step + _s_Tag._separator + _IDStep);
                    _StepsToAction.Add("          " + _s_Tag._skip);
                    break;
                case 1:
                    foreach (string line in _list_ActivatedCharacters)
                    {
                        if (line != null)
                        {
                            _ToActivation = _ToActivation + line + _s_Tag._separator;
                        }
                        else
                        {
                            _StepsToAction.Add("          " + _s_Tag._skip);
                        }
                    }
                    foreach (string line2 in _list_InactivatedCharacters)
                    {
                        if (line2 != null)
                        {
                            _ToInactivation = _ToInactivation + line2 + _s_Tag._separator;
                        }
                        else
                        {
                            _StepsToAction.Add("          " + _s_Tag._skip);
                        }
                    }
                    break;
                case 2:
                    _StepsToAction.Add("          " + _s_Tag._cg_position);
                    float CGPositionX = Mathf.Round(_CG_RectTransform.localPosition.x);
                    float CGPositionY = _CG_RectTransform.localPosition.y;
                    float CGPositionZ = _CG_RectTransform.localPosition.z;
                    _StepsToAction.Add("          " + CGPositionX + _s_Tag._separator + CGPositionY + _s_Tag._separator + CGPositionZ + _s_Tag._separator);
                    _StepsToAction.Add("          " + _s_Tag._activate);
                    if (_ToActivation != "")
                    {
                        _StepsToAction.Add("          " + _ToActivation);
                    }
                    else
                    {
                        _StepsToAction.Add("          " + _s_Tag._null);
                    }
                    _StepsToAction.Add("          " + _s_Tag._inactivate);
                    if (_ToInactivation != "")
                    {
                        _StepsToAction.Add("          " + _ToInactivation);
                    }
                    else
                    {
                        _StepsToAction.Add("          " + _s_Tag._null);
                    }
                    break;
                case 3:
                    _StepsToAction.Add("          " + _s_Tag._character_relocated);
                    for (int e = 0; e < _list_ActiveCharacters.Count; e++)
                    {
                        string char_name = _list_ActiveCharacters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                        float pos_x = _list_ActiveRectTransforms[e].localPosition.x;
                        float pos_y = _list_ActiveRectTransforms[e].localPosition.y;
                        float pos_z = _list_ActiveRectTransforms[e].localPosition.z;
                        _StepsToAction.Add("          " + char_name + _s_Tag._separator + pos_x + _s_Tag._separator + pos_y + _s_Tag._separator + pos_z + _s_Tag._separator);
                    }
                    break;
                case 4:
                    _StepsToAction.Add("          " + _s_Tag._skip);
                    _list_ActivatedCharacters.Clear();
                    _list_InactivatedCharacters.Clear();
                    _ToActivation = "";
                    _ToInactivation = "";
                    _IDStep += 1;
                    _IDStepsTotal.Add(_IDStep.ToString());
                    break;
            }
        }
        return true;
    }
    public Boolean NewAction()
    {
        if (ActionAssembly())
        {
            _IDAction += 1;
            _IDStep = 1;
            _IDActionsTotal += 1;
            _StepsToAction.Clear();
            _IDStepsTotal.Clear();
        }
        return true;
    }
    void InitPartUpdate()
    {
        _InitPartToStr.Clear();
        _Date = System.DateTime.Now;
        _Meta = "Created by: " + _user + " at " + _Date;
        _InitPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "**************************META**************************");
        _InitPartToStr.Add(_s_Tag._skip + _s_Tag._separator + _Meta);
        _InitPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "********************************************************");
        _InitPartToStr.Add(_s_Tag._init);
        _InitPartToStr.Add(_s_Tag._skip);
        _InitPartToStr.Add(_s_Tag._version);
        _InitPartToStr.Add("" + _version);
        _InitPartToStr.Add(_s_Tag._required_objects);
        string RequairedObjects = "";
        foreach (GameObject unit in _list_RequiredObjects)
        {
            RequairedObjects = RequairedObjects + unit.name + _s_Tag._separator;
        }
        _InitPartToStr.Add(RequairedObjects);
        _InitPartToStr.Add(_s_Tag._required_cg);
        string RequairedCG = "";
        foreach (Sprite unit2 in _list_RequiredCG)
        {
            RequairedCG = RequairedCG + unit2.name + _s_Tag._separator;
        }
        _InitPartToStr.Add(RequairedCG);
        _InitPartToStr.Add(_s_Tag._skip + _s_Tag._separator + "********************************************************");
        _InitPartToStr.Add(_s_Tag._start);
        _InitPartToStr.Add(_s_Tag._skip);
    }
    Boolean ActionAssembly()
    {
        for (int i = 0; i <= _id_assembly_stages; i++)
        {
            switch (i)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:
                    _ActionsToStr.Add(_s_Tag._action + _s_Tag._separator + _IDAction);
                    _ActionsToStr.Add("{");
                    _ActionsToStr.Add(_s_Tag._phrase + _s_Tag._separator + _IDAction);
                    _ActionsToStr.Add(_Phrase);
                    _ActionsToStr.Add(_s_Tag._author + _s_Tag._separator + _IDAction);
                    _ActionsToStr.Add(_PhraseAuthor);
                    _ActionsToStr.Add(_s_Tag._CG);
                    string t = _CGImage.sprite.ToString().Replace(" (UnityEngine.Sprite)", "");
                    _ActionsToStr.Add(t);
                    break;
                case 3:
                    foreach (var step in _StepsToAction)
                    {
                        _ActionsToStr.Add(step);
                    }
                    string endstep = "/&endstep";
                    _ActionsToStr.Add(endstep);
                    break;
                case 4:
                    _ActionsToStr.Add("}");
                    _ActionsToStr.Add(_s_Tag._skip);
                    _StepsToAction.Clear();
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
            _CGSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            _CGSprite.name = CGName;
            if (_list_RequiredCG.Count == 0)
            {
                _list_RequiredCG.Add(_CGSprite);
                _CGImage.sprite = _CGSprite;
            }
            else
            {
                if (CheckCGExistence(CGName))
                {
                    _list_RequiredCG.Add(_CGSprite);
                    _CGImage.sprite = _CGSprite;
                }
                else
                {
                    _CGImage.sprite = _CGSprite;
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
            if (_list_RequiredObjects.Count == 0)
            {

                _Character = _s_CharacterSp.Spawn(_Canvas, _s_Folder._root, _s_Folder._body, _s_Folder._haircut, _s_Folder._clothes, _s_Folder._makeup, CharacterPath, CharacterName);

                _list_RequiredObjects.Add(_Character);
                RectTransform RT = _Character.GetComponent<RectTransform>();
                RT.localPosition = new Vector3(458f, -121f, 0f);
                RT.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                _Character.transform.SetParent(_CG_RectTransform.transform, false);

                _list_ActiveRectTransforms.Add(RT);
                if (_list_ActiveCharacters.Count == 0)
                {
                    _list_ActivatedCharacters.Add(CharacterName);
                    _list_ActiveCharacters.Add(_Character);

                }
                else
                {
                    _list_InactivatedCharacters.Add(CharacterName);
                    _Character.SetActive(false);
                }
                InitPartUpdate();
                _Character = null;
            }
            else
            {
                if (CheckCharacterExistence(CharacterName))
                {
                    _Character = _s_CharacterSp.Spawn(_Canvas, _s_Folder._root, _s_Folder._body, _s_Folder._haircut, _s_Folder._clothes, _s_Folder._makeup, CharacterPath, CharacterName);
                    _list_RequiredObjects.Add(_Character);
                    RectTransform RT = _Character.GetComponent<RectTransform>();
                    RT.localPosition = new Vector3(458f, -121f, 0f);
                    RT.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    _Character.transform.SetParent(_CG_RectTransform.transform, false);
                    _list_ActiveRectTransforms.Add(RT);
                    if (_list_ActiveCharacters.Count == 0)
                    {
                        _list_ActivatedCharacters.Add(CharacterName);
                        _list_ActiveCharacters.Add(_Character);
                    }
                    else
                    {
                        _list_InactivatedCharacters.Add(CharacterName);
                        _Character.SetActive(false);
                    }
                    InitPartUpdate();

                    _Character = null;
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
    public void SelectAction(int TargetActionID)
    {
        _IDAction = TargetActionID;
        _list_ActivatedCharacters.Clear();
        _list_InactivatedCharacters.Clear();
        _s_replacer.Get_selected_action_data();

    }

    public void SelectedActionSetup()
    {
        string StepRaw = _s_replacer._list_selected_action_steps[_IDStep - 1];
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
                        _list_ActivatedObjects.Add(unit2);
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
                        _list_InactivatedObjects.Add(unit2);
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
        if (_list_ActivatedObjects.Count != 0)
        {
            foreach (string unit in _list_ActivatedObjects)
            {
                foreach (GameObject GO in _list_RequiredObjects)
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
        if (_list_InactivatedObjects.Count != 0)
        {
            foreach (string unit in _list_InactivatedObjects)
            {
                foreach (GameObject GO in _list_RequiredObjects)
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
        for (int i = 0; i < _list_RequiredObjects.Count; i++)
        {
            if (_list_RequiredObjects[i].name == char_name)
            {
                _list_RequiredObjects[i].GetComponent<RectTransform>().localPosition = new Vector3(pos_x, pos_y, pos_z);
                _list_RequiredObjects[i].GetComponent<RectTransform>().localScale = new Vector3(1.8f, 1.8f, 1.8f);
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

    public void CreateChoiseOption(string CurrencyType, int CostValue, int JumpToActionID, int GiveItemID, string OptionText)
    {
        int OptionNumber = _list_ChoiseOptions.Count + 1;
        string option = OptionNumber.ToString() + _s_Tag._separator + CurrencyType + _s_Tag._separator + CostValue + _s_Tag._separator + JumpToActionID + _s_Tag._separator + GiveItemID + _s_Tag._separator + OptionText;
        _list_ChoiseOptions.Add(option);
    }
    public string[] GetChoiseOption(int OptionID)
    {
        string[] units = _list_ChoiseOptions[OptionID].Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        return units;
    }
    public void DeleteChoiseOption(int option_id)
    {
        _list_ChoiseOptions.Remove(_list_ChoiseOptions[option_id]);
        RenumberChoiseOptionsList();
    }
    public void MoveChoiseOption(int OptionID, StrListDirection Direction)
    {
        string ReplacedOption = null;
        int ReplacedID = 0;
        if (Direction == StrListDirection.Up)
        {
            ReplacedID = OptionID - 1;
        }
        if (Direction == StrListDirection.Down)
        {
            ReplacedID = OptionID + 1;
        }
        ReplacedOption = _list_ChoiseOptions[ReplacedID];
        _list_ChoiseOptions[ReplacedID] = _list_ChoiseOptions[OptionID];
        _list_ChoiseOptions[OptionID] = ReplacedOption;
        RenumberChoiseOptionsList();

    }
    public void RenumberChoiseOptionsList()
    {
        for (int i = 0; i < _list_ChoiseOptions.Count; i++)
        {
            string[] Units = _list_ChoiseOptions[i].Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Units[0] = (i + 1).ToString();
            string t = "";
            for (int r = 0; r < Units.Length; r++)
            {
                t += Units[r] + _s_Tag._separator;
            }
            _list_ChoiseOptions[i] = t;
            t = "";
        }
    }
    Boolean CheckCharacterActivation(string CharacterName)
    {
        if (_list_ActivatedCharacters.Count != 0)
        {
            foreach (string unit in _list_ActivatedCharacters)
            {
                if (unit == CharacterName)
                {
                    return false;
                }
            }
            _list_ActivatedCharacters.Add(CharacterName);
        }
        else
        {
            _list_ActivatedCharacters.Add(CharacterName);
        }
        return true;
    }
    Boolean CheckCharacterExistence(string char_name)
    {

        for (int i = 0; i < _list_RequiredObjects.Count; i++)
        {
            if (_list_RequiredObjects[i].name == char_name)
            {
                return false;
            }
        }
        return true;
    }
    Boolean CheckCGExistence(string CGName)
    {
        for (int i = 0; i < _list_RequiredCG.Count; i++)
        {
            if (_list_RequiredCG[i].name == CGName)
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
        foreach (var Character in _list_ActiveCharacters)
        {
            if (Character.name == CharacterName)
            {
                string temp = Character.ToString().Replace(" (UnityEngine.GameObject)", "");
                _list_InactivatedCharacters.Add(temp);
                Character.SetActive(false);
                _list_ActiveCharacters.Remove(Character);
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
        foreach (var character in _list_RequiredObjects)
        {
            if (character.name == char_name)
            {
                string t = character.ToString().Replace(" (UnityEngine.GameObject)", "");
                _list_ActivatedCharacters.Add(t);
                character.SetActive(true);
                _list_ActiveCharacters.Add(character);
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
        _IDStep = 1;
        _IDAction = 1;
        _user = User;
        _StorylineName = FileName;
        OpenStoryline(FileName);
        return true;
    }
    public Boolean OpenStoryline(string file_name)
    {
        // >> clearing method
        _StorylineName = file_name;
        _s_replacer._list_selected_action_data.Clear();
        _s_replacer._list_selected_action_steps.Clear();
        _list_RequiredObjects.Clear();
        _list_ActivatedObjects.Clear();
        _list_ActiveCharacters.Clear();
        _list_ActiveRectTransforms.Clear();
        _list_InactivatedCharacters.Clear();
        _list_ActivatedCharacters.Clear();
        _list_RequiredCG.Clear();
        _ActionsToStr.Clear();
        _InitPartToStr.Clear();
        _actions_total.Clear();
        _IDStepsTotal.Clear();
        _CGImage.sprite = null;
        _CGSprite = null;
        _PhraseAuthor = "";
        _IDStep = 1;
        _IDAction = 1;
        foreach (GameObject destroy in _list_RequiredObjects)
        {
            DestroyImmediate(destroy);
        }
        return true;
    }
    public void SetAuthor(string AuthorObjectName)
    {
        string temp = AuthorObjectName.ToString().Replace(" (UnityEngine.GameObject)", "");
        _PhraseAuthor = temp;
    }
    public Boolean DeleteCharacter(string CharacterName)
    {
        for (int i = 0; i < _list_RequiredObjects.Count; i++)
        {
            if (_list_RequiredObjects[i].name == CharacterName)
            {
                GameObject tempObject = _list_RequiredObjects[i];
                tempObject.SetActive(true);
                _list_RequiredObjects.Remove(_list_RequiredObjects[i]);

                for (int i2 = 0; i2 < _list_ActiveCharacters.Count; i2++)
                {
                    if (_list_ActiveCharacters[i2] != null && _list_ActiveCharacters[i2].name == CharacterName)
                    {
                        _list_ActiveCharacters.Remove(_list_ActiveCharacters[i2]);
                    }
                }
                for (int i3 = 0; i3 < _list_ActivatedObjects.Count; i3++)
                {
                    if (_list_ActivatedObjects[i3] != null && _list_ActivatedObjects[i3] == CharacterName)
                    {
                        _list_ActivatedObjects.Remove(_list_ActivatedObjects[i3]);
                    }
                }
                for (int i4 = 0; i4 < _list_InactivatedObjects.Count; i4++)
                {
                    if (_list_InactivatedObjects[i4] != null && _list_InactivatedObjects[i4] == CharacterName)
                    {
                        _list_InactivatedObjects.Remove(_list_InactivatedObjects[i4]);
                    }
                }
                for (int i5 = 0; i5 < _list_ActiveRectTransforms.Count; i5++)
                {
                    if (_list_ActiveRectTransforms[i5] != null && _list_ActiveRectTransforms[i5].name == CharacterName)
                    {
                        _list_ActiveRectTransforms.Remove(_list_ActiveRectTransforms[i5]);
                    }
                }
                for (int i6 = 0; i6 < _list_InactivatedCharacters.Count; i6++)
                {
                    if (_list_InactivatedCharacters[i6] != null && _list_InactivatedCharacters[i6] == CharacterName)
                    {
                        _list_InactivatedCharacters.Remove(_list_InactivatedCharacters[i6]);
                    }
                }
                for (int i7 = 0; i7 < _list_ActivatedCharacters.Count; i7++)
                {
                    if (_list_ActivatedCharacters[i7] != null && _list_ActivatedCharacters[i7] == CharacterName)
                    {
                        _list_ActivatedCharacters.Remove(_list_ActivatedCharacters[i7]);
                    }
                }
                if (_InitPartToStr.Count != 0)
                {
                    for (int i8 = 0; i8 < _InitPartToStr.Count; i8++)
                    {
                        if (_InitPartToStr[i8] != null)
                        {
                            string y = CharacterName + _s_Tag._separator;
                            string m = _InitPartToStr[i8].Replace(y, "");

                            _InitPartToStr[i8] = m;

                        }
                    }
                }
                if (_ActionsToStr.Count != 0)
                {
                    for (int i9 = 0; i9 < _ActionsToStr.Count; i9++)
                    {
                        if (_ActionsToStr[i9] != null)
                        {
                            string y = CharacterName + _s_Tag._separator;
                            if (_ActionsToStr[i9] == CharacterName)
                            {
                                _ActionsToStr[i9] = "";
                            }
                            if (_ActionsToStr[i9].StartsWith("          " + y) && _ActionsToStr[i9 - 1] == _s_Tag._character_relocated)
                            {
                                _ActionsToStr.Remove(_ActionsToStr[i9]);
                            }
                            else
                            {
                                string m = _ActionsToStr[i9].Replace(y, "");
                                _ActionsToStr[i9] = m;
                            }


                        }
                    }
                }

                DestroyImmediate(tempObject);
                CheckForExeptions();
                tempObject = null;
            }

        }

        return true;
    }
    private void CheckForExeptions()
    {
        for (int i = 0; i < _ActionsToStr.Count; i++)
        {

            if (_ActionsToStr[i].StartsWith(_s_Tag._author))
            {
                if (_ActionsToStr[i + 1] == "")
                {
                    string[] units = _ActionsToStr[i].Split(_s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int action_id = int.Parse(units[1]);
                    _s_exeption.Set_no_author(action_id);
                }
            }
        }
    }
}
namespace StorylineEditor
{
    public class StrPreviewComponentType
    {
        private string _type_index;
        public StrPreviewComponentType(string index)
        {
            _type_index = index;
        }
        public static StrPreviewComponentType Body = new StrPreviewComponentType("type_body");
        public static StrPreviewComponentType Clothes = new StrPreviewComponentType("type_clothes");
    }
    public class StrFieldType
    {
        private string _type_index;
        public StrFieldType(string index)
        {
            _type_index = index;
        }
        public static StrFieldType RuntimeName = new StrFieldType("type_runtime_name");
        public static StrFieldType TechName = new StrFieldType("type_tech_name");
        public static StrFieldType Cost = new StrFieldType("type_cost");
        public static StrFieldType JumpTo = new StrFieldType("type_jump_to");
        public static StrFieldType OptionText = new StrFieldType("type_option_text");
        public static StrFieldType ItemID = new StrFieldType("type_item_id");
    }
    public class StrListDirection
    {
        private string _type_index;
        public StrListDirection(string index)
        {
            _type_index = index;
        }
        public static StrListDirection Up = new StrListDirection("direction_up");
        public static StrListDirection Down = new StrListDirection("direction_down");

    }
}
