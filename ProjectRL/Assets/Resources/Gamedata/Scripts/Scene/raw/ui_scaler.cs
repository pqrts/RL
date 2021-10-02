using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[ExecuteInEditMode]

public class ui_scaler : MonoBehaviour
{
    public GameObject[] _arr_ui_elements;
    public List<GameObject> _list_main_ui = new List<GameObject>();
    public List<GameObject> _list_wardrobe_ui = new List<GameObject>();
    public List<GameObject> _list_shop_ui = new List<GameObject>();
    public List<GameObject> _list_reward_ui = new List<GameObject>();
    public List<GameObject> _list_exeption_ui = new List<GameObject>();
    public List<GameObject> _list_story_ui = new List<GameObject>();
    public List<GameObject> _list_settings_ui = new List<GameObject>();
    public RectTransform[] ui_rectTransforms;
    private Vector2[] _r_ratioXY;
    private Vector3[] _s_ratioXYZ;
    [SerializeField] private float _standart_height;
    [SerializeField] private float _standart_wight;
    private float _coef_x;
    private float _coef_y;
    private float _height;
    private float _width;
    private float _ratio;
    private float _ratio_standart = 0.5625f;
    Scene _scene;
    //debug
    public bool _main;
    public bool _ward;
    public bool _shop;
    public bool _reward;
    public bool _exeption;
    public bool _story;
    public bool _settings;
    public Text ui_resolution;

    void Start()
    {
        _scene = SceneManager.GetActiveScene();
        _height = Screen.height;
        _width = Screen.width;
        _ratio = _width / _height;
        if (_ratio != _ratio_standart)
        {
            if (_width > _standart_wight)
            {
                _coef_x = 0.7399f;
                _coef_y = 0.7399f;
                Rescale();
                ui_resolution.text = "r: " + _width + ":" + _height + "ratio : " + _ratio + " rescaled " + ui_rectTransforms.Length + " units: " + _coef_x + " : " + _coef_y;
                Debug.Log("rescaled " + ui_rectTransforms.Length + " units: " + _coef_x + " : " + _coef_y); ;
            }
            else
            {
                _coef_x = 1f;
                _coef_y = 1f;
                Rescale();
                ui_resolution.text = "r: " + _width + ":" + _height + "ratio : " + _ratio + " rescaled " + ui_rectTransforms.Length + " units: " + _coef_x + " : " + _coef_y;
                Debug.Log("rescaled " + ui_rectTransforms.Length + " units: " + _coef_x + " : " + _coef_y);
            }
        }
        else
        {
            ui_resolution.text = "r: " + _width + ":" + _height + "ratio : " + _ratio + " no rescaling required";
            _coef_x = 1f;
            _coef_y = 1f;
            Rescale_standart();
            Debug.Log(" no rescaling required");
        }

    }

    public void Add_elements()
    {
        _list_main_ui.Clear();
        _list_wardrobe_ui.Clear();
        _arr_ui_elements = GameObject.FindGameObjectsWithTag("ui_element");
        ui_rectTransforms = new RectTransform[_arr_ui_elements.Length];
        _r_ratioXY = new Vector2[_arr_ui_elements.Length];
        _s_ratioXYZ = new Vector3[_arr_ui_elements.Length];

        foreach (GameObject element in _arr_ui_elements)
        {
            if (_scene.buildIndex == 0)
            {

                string name = element.name;
                if (name.StartsWith("main#"))
                {
                    _list_main_ui.Add(element);
                }
                if (name.StartsWith("wardrobe#"))
                {
                    _list_wardrobe_ui.Add(element);
                }
                if (name.StartsWith("shop#"))
                {
                    _list_shop_ui.Add(element);
                }
                if (name.StartsWith("reward#"))
                {
                    _list_reward_ui.Add(element);
                }
                if (name.StartsWith("exeption#"))
                {
                    _list_exeption_ui.Add(element);
                }
                if (name.StartsWith("story#"))
                {
                    _list_story_ui.Add(element);
                }
                if (name.StartsWith("settings#"))
                {
                    _list_settings_ui.Add(element);
                }
            }
        }
    }
    public void Standart_positions()
    {
        for (int i = 0; i < _arr_ui_elements.Length; i++)
        {
            ui_rectTransforms[i] = _arr_ui_elements[i].GetComponent<RectTransform>();
            float x = ui_rectTransforms[i].position.x / _width;
            float y = ui_rectTransforms[i].position.y / _height;
            _r_ratioXY[i] = new Vector2(x, y);
            float x1 = ui_rectTransforms[i].position.x / _width;
            float y1 = ui_rectTransforms[i].position.y / _height;
            float z = 1f;
        }
    }
    public void Rescale_standart()
    {
        for (int i = 0; i < _arr_ui_elements.Length; i++)
        {
            float x = _width * _r_ratioXY[i].x;
            float y = _height * _r_ratioXY[i].y;
            ui_rectTransforms[i].position = new Vector3(x, y, 0f);
            ui_rectTransforms[i].localScale = new Vector3(1f, 1f, 1f);
        }
    }
    public void Rescale()
    {

        for (int i = 0; i < _arr_ui_elements.Length; i++)
        {
            float x = _width * _r_ratioXY[i].x;
            float y = _height * _r_ratioXY[i].y;
            _s_ratioXYZ[i].x = _width / _standart_wight;
            _s_ratioXYZ[i].y = _height / _standart_height;
            float s_x = 1f * (_s_ratioXYZ[i].x);
            float s_y = 1f * (_s_ratioXYZ[i].y);
            ui_rectTransforms[i].position = new Vector3(x, y, 0f);
            ui_rectTransforms[i].localScale = new Vector3(s_x * _coef_x, s_y * _coef_x, 1f);
        }

    }
    //delete
    public void Hide_main()
    {
        if (_main && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_main_ui)
            {
                element.SetActive(false);
            }
            _main = false;
        }
        else
        {
            foreach (GameObject element in _list_main_ui)
            {
                element.SetActive(true);
            }
            _main = true;
        }
    }
    public void Hide_ward()
    {
        if (_ward && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_wardrobe_ui)
            {
                element.SetActive(false);
            }
            _ward = false;
        }
        else
        {
            foreach (GameObject element in _list_wardrobe_ui)
            {
                element.SetActive(true);
            }
            _ward = true;
        }
    }
    public void Hide_shop()
    {
        if (_shop && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_shop_ui)
            {

                element.SetActive(false);
            }
            _shop = false;
        }
        else
        {
            foreach (GameObject element in _list_shop_ui)
            {
                element.SetActive(true);
            }
            _shop = true;
        }
    }
    public void Hide_reward()
    {
        if (_reward && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_reward_ui)
            {
                element.SetActive(false);
            }
            _reward = false;
        }
        else
        {
            foreach (GameObject element in _list_reward_ui)
            {
                element.SetActive(true);
            }
            _reward = true;
        }
    }
    public void Hide_exeption()
    {
        if (_exeption && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_exeption_ui)
            {

                element.SetActive(false);
            }
            _exeption = false;
        }
        else
        {
            foreach (GameObject element in _list_exeption_ui)
            {
                element.SetActive(true);
            }
            _exeption = true;
        }
    }
    public void Hide_story()
    {
        if (_story && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_story_ui)
            {
                element.SetActive(false);
            }
            _story = false;
        }
        else
        {
            foreach (GameObject element in _list_story_ui)
            {
                element.SetActive(true);
            }
            _story = true;
        }
    }
    public void Hide_settings()
    {
        if (_settings && _scene.buildIndex == 0)
        {
            foreach (GameObject element in _list_settings_ui)
            {
                element.SetActive(false);
            }
            _settings = false;
        }
        else
        {
            foreach (GameObject element in _list_settings_ui)
            {
                element.SetActive(true);
            }
            _settings = true;
        }
    }
}
