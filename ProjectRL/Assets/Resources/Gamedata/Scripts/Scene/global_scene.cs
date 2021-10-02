using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System;

public class global_scene : MonoBehaviour
{
    [SerializeField] private GameObject _Canvas;
    [SerializeField] private Image _CG_Image;
    [SerializeField] private local_moving _CG_LM;
    [SerializeField] private RectTransform _CG_RT;
    [SerializeField] private GameObject _phrase_holder;
    [SerializeField] private TextMeshProUGUI _phrase_text_name;
    [SerializeField] private TextMeshProUGUI _phrase_text_phrase;
    [SerializeField] private Sprite[] _arr_sprites_CG;
    //timer
    private DateTime _currentDate;
    private DateTime _oldDate;
    private TimeSpan _difference;
    private float _time_interval;
    private float _time_remaining_total;
    private float _time_remaining_hours;
    private float _time_remaining_minutes;
    private float _time_remaining_seconds;
    private float _time_remaining_interim;
    private float _time_ad_load;
    private float _to_ui_hours;
    private float _to_ui_minutes;
    private float _to_ui_seconds;
    //ids
    private int _id_action;
    private int _id_step;
    private int _id_decomposed_steps;
    private int _id_total_steps;
    private float _id_action_current;
    //states
    private bool _state_scene;
    //scripts
    private ui_scaler _s_ui_scaler;
    private global_stats _s_global_stats;
    private global_char_spawner _s_g_char_spawner;
    private global_str_reader _s_g_str_reader;
    private global_taglist _s_tag;
    private global_folders _s_folder;
    //for Initialization
    private GameObject _temp_character;
    //data
    private List<GameObject> _list_existing_characters = new List<GameObject>();
    private List<Sprite> _list_existing_CG = new List<Sprite>();
    private List<string> _list_action_raw = new List<string>();
    private List<string> _list_activated_objects = new List<string>();
    private List<string> _list_inactivated_objects = new List<string>();
    private List<string> _list_current_action_steps = new List<string>();
    private List<string> _list_step_objects_pos = new List<string>();
    private List<RectTransform> _list_existing_RT = new List<RectTransform>();
    private List<local_character> _list_existing_LC = new List<local_character>();
    //for scene
    private List<GameObject> _list_active_objects = new List<GameObject>();

    void Start()
    {
        _s_global_stats = GetComponent<global_stats>();
        _s_g_char_spawner = GetComponent<global_char_spawner>();
        _s_g_str_reader = GetComponent<global_str_reader>();
        _s_ui_scaler = GetComponent<ui_scaler>();
        _s_tag = GetComponent<global_taglist>();
        _s_folder = GetComponent<global_folders>();
        _CG_RT = _CG_Image.GetComponent<RectTransform>();
        _CG_LM = _CG_Image.GetComponent<local_moving>();
        _oldDate = DateTime.Now;
        _time_ad_load = _s_global_stats._time_remaining;
        _time_interval = _s_global_stats._time_interval;
        RectTransform TR = _phrase_holder.GetComponent<RectTransform>();
        TR.SetAsLastSibling();
        Initialization();
    }
    void Update()
    {
        _currentDate = DateTime.Now;
        _difference = _currentDate.Subtract(_oldDate);

        if (_time_remaining_total > 0f)
        {
            if (_time_ad_load > 0)
            {
                _time_remaining_total = Mathf.Round(_time_ad_load - ((float)_difference.TotalSeconds));
                _time_remaining_minutes = _time_remaining_total / 60f;
            }
            else
            {
                _time_remaining_total = Mathf.Round(_time_interval - ((float)_difference.TotalSeconds));
                _time_remaining_minutes = _time_remaining_total / 60f;
            }
            if (_time_remaining_minutes < 60f)
            {
                _to_ui_hours = 0f;
                _to_ui_minutes = (int)_time_remaining_minutes;
                _time_remaining_seconds = _time_remaining_total - ((_to_ui_hours * 3600f) + (_to_ui_minutes * 60f));
                _to_ui_seconds = Mathf.Round(_time_remaining_seconds);
            }
            else
            {
                _time_remaining_interim = _time_remaining_minutes / 60f;
                _to_ui_hours = (int)_time_remaining_interim;
                _to_ui_minutes = Mathf.Round((_time_remaining_interim - _to_ui_hours) * 60f);
                _time_remaining_seconds = _time_remaining_total - ((_to_ui_hours * 3600f) + (_to_ui_minutes * 60f));
                _to_ui_seconds = Mathf.Round(_time_remaining_seconds);

            }
        }
    }
    private Boolean Initialization()
    {
        if (_s_g_str_reader._read_complete == true)
        {
            Import_action();
            if (Init_characters())
            {
                if (Get_characters_components())
                {
                    if (Init_CG())
                    {
                        if (Decompose_action())
                        {
                            if (Decompose_steps(1))
                            {
                               
                                if (Execute_step(1))
                                {
                                    if (_id_total_steps == 1)
                                    {
                                        _id_action += 1;
                                        _id_step = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    private Boolean Init_characters()
    {
        if (_s_g_str_reader._list_required_objects.Count != 0)
        {
            foreach (string unit in _s_g_str_reader._list_required_objects)
            {
                _temp_character = _s_g_char_spawner.Spawn(_Canvas, _s_folder._root, _s_folder._body, _s_folder._haircut, _s_folder._clothes, _s_folder._makeup, _s_folder._characters, unit);
                _temp_character.transform.SetParent(_CG_Image.transform, false);
                _list_existing_characters.Add(_temp_character);
                _temp_character.SetActive(false);
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    private Boolean Init_CG()
    {
        if (_s_g_str_reader._list_required_CG.Count != 0)
        {

            foreach (string unit in _s_g_str_reader._list_required_CG)
            {
                string path = _s_folder._CG + "/" + unit + ".png";
                string t = path.Replace(_s_folder._root + "/Resources/", "");
                string t2 = t.Replace(".png", "");
                Texture2D tex;
                tex = Resources.Load(t2) as Texture2D;
                Debug.Log("Initialization   ");
                Debug.Log("Initialization cg  " + t2);
                Sprite sprite_CG = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                sprite_CG.name = unit;
                _list_existing_CG.Add(sprite_CG);

            }
        }
        else
        {
            return false;
        }
        return true;
    }
    private Boolean Get_characters_components()
    {
        for (int i = 0; i < _list_existing_characters.Count; i++)
        {
            RectTransform RT = _list_existing_characters[i].GetComponent<RectTransform>();
            _list_existing_RT.Add(RT);
            local_character LC = _list_existing_characters[i].GetComponent<local_character>();
            _list_existing_LC.Add(LC);

        }
        return true;
    }

    public void Next_part()
    {
           if (_id_step == 1)
        {
            if (Clear_data_total())
            {
                if (_s_g_str_reader.Read_action(_id_action))
                {
                    if (Import_action())
                    {
                        if (Decompose_action())
                        {
                            if (Decompose_steps(_id_step))
                            {
                                if (Execute_step(_id_step))
                                {
                                    if (_id_total_steps == 1)
                                    {
                                        _id_action += 1;
                                        _id_step = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (_id_total_steps > 1 && _id_step != 1)
            {
                if (Clear_data_step())
                {
                    if (Execute_step(_id_step))
                    {
                        if (_id_step == _id_total_steps)
                        {
                            _id_action += 1;
                            _id_step = 1;
                        }
                    }
                }
            }
        }
    }
    private Boolean Import_action()
    {
        foreach (string unit in _s_g_str_reader._list_action_data)
        {
            _list_action_raw.Add(unit);
        }
        return true;
    }
    private Boolean Clear_data_step()
    {
        _list_activated_objects.Clear();
        _list_inactivated_objects.Clear();
        return true;
    }
    private Boolean Clear_data_total()
    {
        _list_activated_objects.Clear();
        _list_inactivated_objects.Clear();
        _list_current_action_steps.Clear();
        _list_action_raw.Clear();
        return true;
    }
    private Boolean Decompose_action()
    {
        _id_total_steps = 0;
        _id_step = 1;
        for (int i = 0; i < _list_action_raw.Count; i++)
        {
            if (_list_action_raw[i] == _s_tag._skip)
            {
                continue;
            }
            else
            {
                if (_list_action_raw[i].StartsWith(_s_tag._phrase))
                {
                    string phrase = _list_action_raw[i + 1];
                    Set_phrase(phrase);
                }
                if (_list_action_raw[i].StartsWith(_s_tag._author))
                {
                    string author = _list_action_raw[i + 1];
                    Set_author(author);
                }
                if (_list_action_raw[i].StartsWith(_s_tag._CG))
                {
                    string CG = _list_action_raw[i + 1];
                    Set_CG(CG);
                }

                if (_list_action_raw[i].StartsWith(_s_tag._step))
                {
                    _id_total_steps += 1;
                }
            }
        }
        return true;
    }
    private Boolean Decompose_steps(int step_id)
    {
        _id_decomposed_steps = 1;
        if (_id_total_steps != 1)
        {
            for (int i = 0; i < _list_action_raw.Count; i++)
            {
                string step_unit = _s_tag._step + _s_tag._separator + _id_decomposed_steps;
                string step_unit_next = _s_tag._step + _s_tag._separator + (_id_decomposed_steps + 1);
                string action_unit_next = _s_tag._action + _s_tag._action + (_id_action + 1);
                if (_list_action_raw[i] == step_unit)
                {
                    string step_raw = "";
                    for (int e = i; e < _list_action_raw.Count; e++)
                    {
                        if (_list_action_raw[e] != step_unit_next || _list_action_raw[e] != "}")
                        {
                            string temp_tag_skip = "          " + _s_tag._skip;
                            if (_list_action_raw[e] != temp_tag_skip && _list_action_raw[e] != step_unit)
                            {
                                string t = _list_action_raw[e].Replace("          ", "");
                                step_raw = step_raw + t + _s_tag._separator_vert;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (_id_total_steps == 1)
                    {
                        _list_current_action_steps.Add(step_raw);
                        break;
                    }
                    else
                    {
                        _list_current_action_steps.Add(step_raw);
                    }
                    step_raw = "";
                    _id_decomposed_steps += 1;
                }
            }
        }
        else
        {
            bool filled = false;
            int r = 0;
            for (int i = 0; i < _list_action_raw.Count; i++)
            {
                string step_unit = _s_tag._step + _s_tag._separator + _id_step;
                string endstep = "/&endstep";
                if (_list_action_raw[i] == step_unit)
                {

                    string step_raw = "";
                    for (int e = i; e < _list_action_raw.Count; e++)
                    {
                        if (_list_action_raw[e] != endstep)
                        {
                            string t = _list_action_raw[e].Replace("          ", "");
                            step_raw = step_raw + t + _s_tag._separator_vert;
                        }
                        else
                        {
                            _list_current_action_steps.Add(step_raw);
                            r += 1;
                            filled = true;
                            break;
                        }
                    }
                }
                if (filled == true)
                {
                    break;
                }
            }
        }
        return true;
    }
    private Boolean Execute_step(int step_id)
    {
        string step_raw = _list_current_action_steps[step_id - 1];
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
                    Rescale_objects(p_char_name, p_sca_x, p_sca_y, p_sca_z);
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
                    _CG_LM.All_stop();
                    _CG_LM.Start_move(_CG_Image, _CG_RT, new_pos_cg);
                }
            }
            if (i == units.Length - 1)
            {
                Activate_objects();
                Inactivate_objects();
                _id_step += 1;
                break;
            }
        }
        return true;
    }
    private Boolean Activate_objects()
    {
        if (_list_activated_objects.Count != 0)
        {
            foreach (string unit in _list_activated_objects)
            {
                foreach (GameObject GO in _list_existing_characters)
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
                foreach (GameObject GO in _list_existing_characters)
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
        for (int i = 0; i < _list_existing_characters.Count; i++)
        {
            if (_list_existing_characters[i].name == char_name)
            {
                _list_existing_RT[i].localPosition = new Vector3(pos_x, pos_y, pos_z);
                _list_existing_RT[i].localScale = new Vector3(1.8f, 1.8f, 1.8f);
            }
        }
        return true;
    }
    private Boolean Rescale_objects(string char_name, float sca_x, float sca_y, float sca_z)
    {
        for (int i = 0; i < _list_existing_characters.Count; i++)
        {
            if (_list_existing_characters[i].name == char_name)
            {
                _list_existing_RT[i].localScale = new Vector3(sca_x, sca_y, sca_z);
            }
        }
        return true;
    }
    private Boolean Set_phrase(string phrase)
    {
        _phrase_text_phrase.text = phrase;
        return true;
    }
    private Boolean Set_author(string author)
    {
        for (int i = 0; i < _list_existing_characters.Count; i++)
        {
            if (_list_existing_characters[i].name == author)
            {
                string author_name = _list_existing_LC[i]._char_runtime_name;
                _phrase_text_name.text = author_name;
            }
        }
        return true;
    }
    private Boolean Set_CG(string CG)
    {
        foreach (Sprite unit in _list_existing_CG)
        {
            if (unit.name == CG)
            {
                _CG_Image.sprite = unit;
            }
        }
        return true;
    }
    private void OnApplicationQuit()
    {
        _s_global_stats._time_remaining = _time_remaining_total;

        _s_global_stats.SaveToFile();
    }
}
