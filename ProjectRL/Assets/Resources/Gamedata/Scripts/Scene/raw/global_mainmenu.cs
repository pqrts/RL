
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

[RequireComponent(typeof(ui_scaler))]
public class global_mainmenu : MonoBehaviour
{
    [SerializeField] private Image _CG_mainmenu;
    [SerializeField] private Sprite[] _arr_sprites_CG;
    [SerializeField] private Button[] _arr_buttons;
    [SerializeField] private Text[] _arr_text;
    [SerializeField] private TextMeshProUGUI _storyname;
    private RectTransform CG_1_RT;
    private RectTransform CG_2_RT;
    private GameObject[] _arr_ui_elements;
    private List<GameObject> _list_main_ui = new List<GameObject>();
    private ui_scaler _s_ui_scaler;
    private global_stats _s_global_stats;
    private bool _state_mainmenu;
    private bool _state_wardrobe;
    private bool _state_shop;
    private bool _state_reward;
    private bool _state_exeption;
    private bool _state_story;
    private bool _state_settings;
    private bool _ui_ad_show;
    private RewardedAd _rew_Ad;
    public string _RewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
    //timer
    private DateTime _currentDate;
    private DateTime _oldDate;
    private TimeSpan _difference;
    public float _time_interval;
    public float _time_remaining_total;
    private float _time_remaining_hours;
    private float _time_remaining_minutes;
    private float _time_remaining_seconds;
    private float _time_remaining_interim;
    private float _to_ui_hours;
    private float _to_ui_minutes;
    private float _to_ui_seconds;
    //story change
    private bool _cg_state;
    private int _id_cg;
    private int _id_cg_wardrobe;


    private void Awake()
    {
        MobileAds.Initialize(InitializationStatus => { });
    }
    void Start()
    {
        _s_global_stats = this.GetComponent<global_stats>();
        _s_ui_scaler = this.GetComponent<ui_scaler>();
        _ui_ad_show = false;
        _arr_buttons[0].interactable = false;
        _oldDate = System.DateTime.Now;
        To_mainmenu();
    }
    private void OnEnable()
    {
        _rew_Ad = new RewardedAd(_RewardedUnitId);
        AdRequest adrequest = new AdRequest.Builder().Build();
        _rew_Ad.LoadAd(adrequest);
    }
    void Update()
    {
        if (_time_remaining_total == 0f)
        {
            _ui_ad_show = true;
            _arr_buttons[0].interactable = true;

        }
        else
        {
            _arr_buttons[0].interactable = false;
        }
        _currentDate = System.DateTime.Now;
        _difference = _currentDate.Subtract(_oldDate);
        if (_time_remaining_total > 0f)
        {
            _time_remaining_total = Mathf.Round(_time_interval - ((float)_difference.TotalSeconds));
            _time_remaining_minutes = _time_remaining_total / 60f;
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
        _arr_text[0].text = ("" + _to_ui_hours + "h:" + _to_ui_minutes + "m:" + _to_ui_seconds + "s");
    }

    public void Next_story()
    {
        /////////////////////////////////////////////////
        if (_cg_state == true)
        {

            _id_cg = 5;
            _id_cg_wardrobe = 2;
            _storyname.text = "Агнец";
            _CG_mainmenu.sprite = _arr_sprites_CG[5];
            _cg_state = false;
        }
        else
        {
            _id_cg = 0;
            _id_cg_wardrobe = 4;
            _storyname.text = "Оглянись вокруг";
            _CG_mainmenu.sprite = _arr_sprites_CG[0];
            _cg_state = true;
        }
        //////////////////////////////////////////////////
    }
    public void Show_ad()
    {
        _oldDate = System.DateTime.Now;
        _time_remaining_total = 1f;
        Rebuild_ui();
        if (_rew_Ad.IsLoaded())
        {
            _rew_Ad.Show();
            _rew_Ad.OnUserEarnedReward += HandleUserEarnedReward;
            _rew_Ad = new RewardedAd(_RewardedUnitId);
            AdRequest adrequest = new AdRequest.Builder().Build();
            _rew_Ad.LoadAd(adrequest);
        }
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        To_reward();
    }
    public void Get_reward()
    {
        _s_global_stats._items_diamond += 2;
        To_mainmenu();
    }
    private void Rebuild_ui()
    {
        foreach (GameObject element in _s_ui_scaler._list_main_ui)
        {
            if (_state_mainmenu)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_wardrobe_ui)
        {
            if (_state_wardrobe)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_shop_ui)
        {
            if (_state_shop)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_reward_ui)
        {
            if (_state_reward)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_exeption_ui)
        {
            if (_state_exeption)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_story_ui)
        {
            if (_state_story)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        foreach (GameObject element in _s_ui_scaler._list_settings_ui)
        {
            if (_state_settings)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }

        }
        _arr_text[1].text = _s_global_stats._items_diamond.ToString();
        _arr_text[2].text = _s_global_stats._items_cups.ToString();

    }
    public void To_story_scene()
    {
        _s_global_stats._time_remaining = _time_remaining_total;
        _s_global_stats._time_interval = _time_interval;
        _s_global_stats.SaveToFile();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void To_mainmenu()
    {
        _CG_mainmenu.sprite = _arr_sprites_CG[_id_cg];
        _state_mainmenu = true;
        _state_wardrobe = false;
        _state_shop = false;
        _state_reward = false;
        _state_exeption = false;
        _state_story = false;
        _state_settings = false;
        Rebuild_ui();
    }
    public void To_wardrobe()
    {
        _CG_mainmenu.sprite = _arr_sprites_CG[_id_cg_wardrobe];
        _state_mainmenu = false;
        _state_wardrobe = true;
        _state_shop = false;
        _state_reward = false;
        _state_exeption = false;
        _state_story = false;
        _state_settings = false;
        Rebuild_ui();
    }
    public void To_shop()
    {
        _CG_mainmenu.sprite = _arr_sprites_CG[1];
        _state_mainmenu = false;
        _state_wardrobe = false;
        _state_shop = true;
        _state_reward = false;
        _state_exeption = false;
        _state_story = false;
        _state_settings = false;
        Rebuild_ui();
    }
    public void To_reward()
    {
        _state_reward = true;
        _state_exeption = false;
        Rebuild_ui();
    }
    public void To_exeption()
    {
        _state_exeption = true;
        Rebuild_ui();
    }
    public void To_story()
    {
        _CG_mainmenu.sprite = _arr_sprites_CG[3];
        _state_mainmenu = false;
        _state_wardrobe = false;
        _state_shop = false;
        _state_reward = false;
        _state_exeption = false;
        _state_story = true;
        _state_settings = false;

        Rebuild_ui();
    }
    public void To_settings()
    {
        _CG_mainmenu.sprite = _arr_sprites_CG[1];
        _state_mainmenu = false;
        _state_wardrobe = false;
        _state_shop = false;
        _state_reward = false;
        _state_exeption = false;
        _state_story = false;
        _state_settings = true;
        Rebuild_ui();
    }
    private void OnApplicationQuit()
    {
        _s_global_stats._time_remaining = _time_remaining_total;
        _s_global_stats._time_interval = _time_interval;
        _s_global_stats.SaveToFile();
    }
}

