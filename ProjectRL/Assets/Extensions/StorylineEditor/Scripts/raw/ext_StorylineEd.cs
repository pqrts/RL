using System;
using System.Globalization;

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEditor;
[ExecuteInEditMode]
public class ext_StorylineEd : MonoBehaviour
{

    public Sprite _temp_CharIcon;
    public GameObject _Canvas;
    [SerializeField] private int _ref_resolution_widht;
    [HideInInspector] public string _init_status;
    public int _id_action;
    public int _id_action_total;
    public int _id_step;
    [SerializeField] private int _id_assembly_stages = 4;
    //scripts
    global_taglist _s_tag;
    public global_folders _s_folder;
    ext_Storyline_replacer _s_replacer;
    //metadata
    private string _meta;
    [SerializeField] public string _user;
    [SerializeField] private float _version;
    private DateTime _date;
    //for Initialization part
    public List<GameObject> _list_required_objects = new List<GameObject>();
    [HideInInspector] public List<Sprite> _list_required_CG = new List<Sprite>();
    private string req_objects;
    //steps parameters
    private List<RectTransform> _list_active_RectTransforms = new List<RectTransform>();
    public List<GameObject> _list_active_characters = new List<GameObject>();
    private List<string> _list_activated_characters = new List<string>();
    private List<string> _list_inactivated_characters = new List<string>();
    public List<string> _list_activated_objects = new List<string>();
    public List<string> _list_inactivated_objects = new List<string>();

    private string _to_activation;
    private string _to_inactivation;
    public string _str_name;
    //for str form
    public List<string> _init_to_str = new List<string>();
    public List<string> _actions_to_str = new List<string>();
    private List<string> _steps_to_action = new List<string>();
    [HideInInspector] public List<string> _steps_total = new List<string>();
    [HideInInspector] public List<List<string>> _actions_total = new List<List<string>>();
    //scene
    [HideInInspector] public Sprite _CG_sprite;
    public Image _CG_image;
    private float _cam_border_left;
    private float _cam_border_right;
    private Vector2 _moving_pool_pos;
    private Vector2 _cg_moving_pool_pos;
    [HideInInspector] public string _phrase;
    [HideInInspector] public  string _phrase_author;
    //characters spawn
    public GameObject _character;
    ext_CharacterSp _s_CharacterSp;
    [HideInInspector] public float _canvas_moving_pool;
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
        _s_tag = GetComponent<global_taglist>();
        _s_folder = GetComponent<global_folders>();
        _s_replacer = GetComponent<ext_Storyline_replacer>();
        _CG_RectTransform = _CG_image.GetComponent<RectTransform>();
        if (_s_folder.Setup_folders() && _s_tag.Setup_tags() && Get_pos_limits() && _s_replacer.Get_scripts())
        {
            _init_status = "successful";
        }
        else
        {
            _init_status = "failed";
        }
    }

    Boolean Get_pos_limits()
    {
        float cg_widht = _CG_RectTransform.rect.width;
        _cam_border_left = 0 - (_ref_resolution_widht / 2);
        _cam_border_right = 0 + (_ref_resolution_widht / 2);
        Debug.Log("pool: " + _cam_border_left);
        float left_x = _cam_border_left + (cg_widht / 2);
        float right_x = _cam_border_right - (cg_widht / 2);
        _canvas_moving_pool = left_x - right_x;
        _moving_pool_pos = new Vector2(left_x, right_x);
        return true;
    }
    public Boolean Get_pos_limits_cg(RectTransform character_RT)
    {
        float cg_widht = _CG_RectTransform.rect.width;
        float character_widht = character_RT.rect.width;

        float cg_border_left = _CG_RectTransform.localPosition.x - (cg_widht / 2);
        float cg_border_right = _CG_RectTransform.localPosition.x + (cg_widht / 2);

        float left_x = cg_border_left + (character_widht / 2);
        float right_x = cg_border_right - (character_widht / 2);
        _cg_moving_pool = left_x - right_x;
        _cg_moving_pool_pos = new Vector2(left_x, right_x);
        return true;
    }
    public void Move_CG(float cg_pos_x)
    {
        float x = _moving_pool_pos.x - cg_pos_x;
        _CG_RectTransform.localPosition = new Vector3(x, _CG_RectTransform.localPosition.y, _CG_RectTransform.localPosition.z);
    }
    public Boolean Move_character(float character_pos_x, RectTransform char_RT)
    {
        float x = _cg_moving_pool_pos.x - character_pos_x;
        char_RT.localPosition = new Vector3(x, char_RT.localPosition.y, char_RT.localPosition.z);
        return true;
    }
    public void Read_str()
    {
        try
        {
            StreamReader SR = new StreamReader(_s_folder._storylines + "/" + _str_name);
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
    public Boolean Form_str()
    {
        try
        {
            StreamWriter SW = new StreamWriter(_s_folder._storylines + "/" + _str_name, true, encoding: System.Text.Encoding.Unicode);
            for (int i = 0; i < _actions_to_str.Count; i++)
            {
                string t = _actions_to_str[i];
                SW.WriteLine(t);
            }
            SW.Close();
            _actions_to_str.Clear();
        }
        catch (Exception ex)
        {

        }
        return true;
    }
    public Boolean Create_step()
    {
        int step_stages = 5;
        _to_activation = "";
        _to_inactivation = "";
        for (int i = 0; i <= step_stages; i++)
        {
            switch (i)
            {
                case 0:
                    _steps_to_action.Add(_s_tag._step + _s_tag._separator + _id_step);
                    _steps_to_action.Add("          " + _s_tag._skip);
                    break;
                case 1:
                    foreach (string line in _list_activated_characters)
                    {
                        if (line != null)
                        {
                            _to_activation = _to_activation + line + _s_tag._separator;
                        }
                        else
                        {
                            _steps_to_action.Add("          " + _s_tag._skip);
                        }
                    }
                    foreach (string line2 in _list_inactivated_characters)
                    {
                        if (line2 != null)
                        {
                            _to_inactivation = _to_inactivation + line2 + _s_tag._separator;
                        }
                        else
                        {
                            _steps_to_action.Add("          " + _s_tag._skip);
                        }
                    }
                    break;
                case 2:
                    _steps_to_action.Add("          " + _s_tag._cg_position);
                    float cg_pos_x = Mathf.Round(_CG_RectTransform.localPosition.x);
                    float cg_pos_y = _CG_RectTransform.localPosition.y;
                    float cg_pos_z = _CG_RectTransform.localPosition.z;
                    _steps_to_action.Add("          " + cg_pos_x + _s_tag._separator + cg_pos_y + _s_tag._separator + cg_pos_z + _s_tag._separator);
                    _steps_to_action.Add("          " + _s_tag._activate);
                    if (_to_activation != "")
                    {
                        _steps_to_action.Add("          " + _to_activation);
                    }
                    else
                    {
                        _steps_to_action.Add("          " + _s_tag._null);
                    }
                    _steps_to_action.Add("          " + _s_tag._inactivate);
                    if (_to_inactivation != "")
                    {
                        _steps_to_action.Add("          " + _to_inactivation);
                    }
                    else
                    {
                        _steps_to_action.Add("          " + _s_tag._null);
                    }
                    break;
                case 3:
                    _steps_to_action.Add("          " + _s_tag._character_relocated);
                    for (int e = 0; e < _list_active_characters.Count; e++)
                    {
                        string char_name = _list_active_characters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                        float pos_x = _list_active_RectTransforms[e].localPosition.x;
                        float pos_y = _list_active_RectTransforms[e].localPosition.y;
                        float pos_z = _list_active_RectTransforms[e].localPosition.z;
                        _steps_to_action.Add("          " + char_name + _s_tag._separator + pos_x + _s_tag._separator + pos_y + _s_tag._separator + pos_z + _s_tag._separator);
                    }
                    break;
                case 4:
                    _steps_to_action.Add("          " + _s_tag._skip);
                    _list_activated_characters.Clear();
                    _list_inactivated_characters.Clear();
                    _to_activation = "";
                    _to_inactivation = "";
                    _id_step += 1;
                    _steps_total.Add(_id_step.ToString());
                    break;
            }
        }
        return true;
    }
    public Boolean New_action()
    {
        if (Action_assembly())
        {
            _id_action += 1;
            _id_step = 1;
            _id_action_total += 1;

            _steps_to_action.Clear();
            _steps_total.Clear();
        }
        return true;
    }
    void Init_update()
    {
        _init_to_str.Clear();
        _date = System.DateTime.Now;
        _meta = "Created by: " + _user + " at " + _date;
        _init_to_str.Add(_s_tag._skip + _s_tag._separator + "**************************META**************************");
        _init_to_str.Add(_s_tag._skip + _s_tag._separator + _meta);
        _init_to_str.Add(_s_tag._skip + _s_tag._separator + "********************************************************");
        _init_to_str.Add(_s_tag._init);
        _init_to_str.Add(_s_tag._skip);
        _init_to_str.Add(_s_tag._version);
        _init_to_str.Add("" + _version);
        _init_to_str.Add(_s_tag._required_objects);
        string req_obj = "";
        foreach (GameObject unit in _list_required_objects)
        {
            req_obj = req_obj + unit.name + _s_tag._separator;
        }
        _init_to_str.Add(req_obj);
        _init_to_str.Add(_s_tag._required_cg);
        string req_cg = "";
        foreach (Sprite unit2 in _list_required_CG)
        {
            req_cg = req_cg + unit2.name + _s_tag._separator;
        }
        _init_to_str.Add(req_cg);
        _init_to_str.Add(_s_tag._skip + _s_tag._separator + "********************************************************");
        _init_to_str.Add(_s_tag._start);
        _init_to_str.Add(_s_tag._skip);
    }
    Boolean Action_assembly()
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
                    _actions_to_str.Add(_s_tag._action + _s_tag._separator + _id_action);
                    _actions_to_str.Add("{");
                    _actions_to_str.Add(_s_tag._phrase + _s_tag._separator + _id_action);
                    _actions_to_str.Add(_phrase);
                    _actions_to_str.Add(_s_tag._author);
                    _actions_to_str.Add(_phrase_author);
                    _actions_to_str.Add(_s_tag._CG);
                    string t = _CG_image.sprite.ToString().Replace(" (UnityEngine.Sprite)", "");
                    _actions_to_str.Add(t);
                    break;
                case 3:
                    foreach (var step in _steps_to_action)
                    {
                        _actions_to_str.Add(step);
                    }
                    string endstep = "/&endstep";
                    _actions_to_str.Add(endstep);
                    break;
                case 4:
                    _actions_to_str.Add("}");
                    _actions_to_str.Add(_s_tag._skip);
                    _steps_to_action.Clear();
                    break;
            }
        }
        return true;
    }
    public Boolean Add_CG(string cg_path, string cg_name)
    {
        try
        {
            Texture2D tex;
            tex = Resources.Load(cg_path) as Texture2D;
            _CG_sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            _CG_sprite.name = cg_name;
            if (_list_required_CG.Count == 0)
            {
                _list_required_CG.Add(_CG_sprite);
                _CG_image.sprite = _CG_sprite;
            }
            else
            {
                if (Check_CG_existence(cg_name))
                {
                    _list_required_CG.Add(_CG_sprite);
                    _CG_image.sprite = _CG_sprite;
                }
                else
                {
                    _CG_image.sprite = _CG_sprite;
                }
            }
            Init_update();
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
    public void Add_character(string character_path, string character_name)
    {
        try
        {
            if (_list_required_objects.Count == 0)
            {

                _character = _s_CharacterSp.Spawn(_Canvas, _s_folder._root, _s_folder._body, _s_folder._haircut, _s_folder._clothes, _s_folder._makeup, character_path, character_name);

                _list_required_objects.Add(_character);
                RectTransform RT = _character.GetComponent<RectTransform>();
                RT.localPosition = new Vector3(458f, -121f, 0f);
                RT.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                _character.transform.SetParent(_CG_RectTransform.transform, false);

                _list_active_RectTransforms.Add(RT);
                if (_list_active_characters.Count == 0)
                {
                    _list_activated_characters.Add(character_name);
                    _list_active_characters.Add(_character);

                }
                else
                {
                    _list_inactivated_characters.Add(character_name);
                    _character.SetActive(false);
                }
                Init_update();
                _character = null;
            }
            else
            {
                if (Check_character_existence(character_name))
                {
                    _character = _s_CharacterSp.Spawn(_Canvas, _s_folder._root, _s_folder._body, _s_folder._haircut, _s_folder._clothes, _s_folder._makeup, character_path, character_name);
                    _list_required_objects.Add(_character);
                    RectTransform RT = _character.GetComponent<RectTransform>();
                    RT.localPosition = new Vector3(458f, -121f, 0f);
                    RT.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    _character.transform.SetParent(_CG_RectTransform.transform, false);
                    _list_active_RectTransforms.Add(RT);
                    if (_list_active_characters.Count == 0)
                    {
                        _list_activated_characters.Add(character_name);
                        _list_active_characters.Add(_character);
                    }
                    else
                    {
                        _list_inactivated_characters.Add(character_name);
                        _character.SetActive(false);
                    }
                    Init_update();
                    _character = null;
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
    public void Select_action(int target_action_id)
    {
        _id_action = target_action_id;
        _list_activated_characters.Clear();
        _list_inactivated_characters.Clear();
        _s_replacer.Get_selected_action_data();

    }

    public void Selected_action_setup()
    {
        string step_raw = _s_replacer._list_selected_action_steps[_id_step - 1];
        string[] units = step_raw.Split(_s_tag._separator_vert.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == _s_tag._activate)
            {
                if (units[i + 1] != _s_tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit2 in units2)
                    {
                        _list_activated_objects.Add(unit2);
                    }
                }
            }
            if (units[i] == _s_tag._inactivate)
            {
                if (units[i + 1] != _s_tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string unit2 in units2)
                    {
                        _list_inactivated_objects.Add(unit2);
                    }
                }
            }

            if (units[i] == _s_tag._character_relocated)
            {
                if (units[i + 1] != _s_tag._null)
                {
                    for (int k = (i + 1); k < units.Length; k++)
                    {
                        if (units[k] != _s_tag._skip)
                        {
                            string line = units[k];
                            string[] units2 = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            string p_char_name = units2[0];
                            float p_pos_char_x = float.Parse(units2[1], CultureInfo.InvariantCulture);
                            float p_pos_char_y = float.Parse(units2[2], CultureInfo.InvariantCulture);
                            float p_pos_char_z = float.Parse(units2[3], CultureInfo.InvariantCulture);
                            Relocate_objects(p_char_name, p_pos_char_x, p_pos_char_y, p_pos_char_z);
                        }
                    }
                }
            }
            if (units[i] == _s_tag._rescale)
            {
                if (units[i + 1] != _s_tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string p_char_name = units2[0];
                    float p_sca_x = float.Parse(units2[1], CultureInfo.InvariantCulture);
                    float p_sca_y = float.Parse(units2[2], CultureInfo.InvariantCulture);
                    float p_sca_z = float.Parse(units2[3], CultureInfo.InvariantCulture);
                    // Rescale_objects(p_char_name, p_sca_x, p_sca_y, p_sca_z);
                }
            }
            if (units[i] == _s_tag._cg_position)
            {
                if (units[i + 1] != _s_tag._null)
                {
                    string line = units[i + 1];
                    string[] units2 = line.Split(_s_tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    float p_pos_cg_x = float.Parse(units2[0], CultureInfo.InvariantCulture);
                    float p_pos_cg_y = float.Parse(units2[1], CultureInfo.InvariantCulture);
                    float p_pos_cg_z = float.Parse(units2[2], CultureInfo.InvariantCulture);
                    Vector3 new_pos_cg = new Vector3(p_pos_cg_x, p_pos_cg_y, p_pos_cg_z);
                    _CG_image.GetComponent<RectTransform>().localPosition = new_pos_cg;
                }
            }
            if (i == units.Length - 1)
            {
                Activate_objects();
                Inactivate_objects();

                break;
            }
        }

    }
    private Boolean Activate_objects()
    {
        if (_list_activated_objects.Count != 0)
        {
            foreach (string unit in _list_activated_objects)
            {
                foreach (GameObject GO in _list_required_objects)
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
    private Boolean Inactivate_objects()
    {
        if (_list_inactivated_objects.Count != 0)
        {
            foreach (string unit in _list_inactivated_objects)
            {
                foreach (GameObject GO in _list_required_objects)
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
    private Boolean Relocate_objects(string char_name, float pos_x, float pos_y, float pos_z)
    {
        for (int i = 0; i < _list_required_objects.Count; i++)
        {
            if (_list_required_objects[i].name == char_name)
            {
                _list_required_objects[i].GetComponent<RectTransform>().localPosition = new Vector3(pos_x, pos_y, pos_z);
                _list_required_objects[i].GetComponent<RectTransform>().localScale = new Vector3(1.8f, 1.8f, 1.8f);
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
    Boolean Check_character_activation(string character_name)
    {
        if (_list_activated_characters.Count != 0)
        {
            foreach (string unit in _list_activated_characters)
            {
                if (unit == character_name)
                {
                    return false;
                }
            }
            _list_activated_characters.Add(character_name);
        }
        else
        {
            _list_activated_characters.Add(character_name);
        }
        return true;
    }
    Boolean Check_character_existence(string char_name)
    {

        for (int i = 0; i < _list_required_objects.Count; i++)
        {
            if (_list_required_objects[i].name == char_name)
            {
                return false;
            }
        }
        return true;
    }
    Boolean Check_CG_existence(string name)
    {
        for (int i = 0; i < _list_required_CG.Count; i++)
        {
            if (_list_required_CG[i].name == name)
            {
                return false;
            }
        }
        return true;
    }
    public Boolean Check_str_existence(string file_name)
    {
        try
        {
            StreamReader SR = new StreamReader(_s_folder._storylines + "/" + file_name);
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
    public Boolean Deactivate_character(string char_name)
    {
        foreach (var character in _list_active_characters)
        {
            if (character.name == char_name)
            {
                string t = character.ToString().Replace(" (UnityEngine.GameObject)", "");
                _list_inactivated_characters.Add(t);
                character.SetActive(false);
                _list_active_characters.Remove(character);
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
    public Boolean Activate_existing_character(string char_name)
    {
        foreach (var character in _list_required_objects)
        {
            if (character.name == char_name)
            {
                string t = character.ToString().Replace(" (UnityEngine.GameObject)", "");
                _list_activated_characters.Add(t);
                character.SetActive(true);
                _list_active_characters.Add(character);
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
    public Boolean Create_new_str(string file_name, string t_user)
    {
        try
        {
            StreamWriter SW = new StreamWriter(_s_folder._storylines + "/" + file_name, true, encoding: System.Text.Encoding.Unicode);
            string t = "";
            SW.WriteLine(t);
            SW.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);

            return false;
        }
        _id_step = 1;
        _id_action = 1;
        _user = t_user;
        _str_name = file_name;
        open_str(file_name);
        return true;
    }
    public Boolean open_str(string file_name)
    {
        // >> clearing method
        _str_name = file_name;
        _s_replacer._list_selected_action_data.Clear();
        _s_replacer._list_selected_action_steps.Clear();
        _list_required_objects.Clear();
        _list_active_characters.Clear();
        _list_active_RectTransforms.Clear();
        _list_inactivated_characters.Clear();
        _list_activated_characters.Clear();
        _list_required_CG.Clear();
        _actions_to_str.Clear();
        _init_to_str.Clear();
        _actions_total.Clear();
        _steps_total.Clear();
        _CG_image.sprite = null;
        _CG_sprite = null;
        _phrase_author = "";
        _id_step = 1;
        _id_action = 1;
        foreach (GameObject destroy in _list_required_objects)
        {
            DestroyImmediate(destroy);
        }
        return true;
    }
    public void Set_author(string author_obj)
    {
        string t = author_obj.ToString().Replace(" (UnityEngine.GameObject)", "");
        _phrase_author = t;
    }
    public void LOG()
    {
        Debug.Log(_list_active_RectTransforms[0].anchoredPosition);
        Debug.Log(_list_active_RectTransforms[0].localPosition);
    }
}
