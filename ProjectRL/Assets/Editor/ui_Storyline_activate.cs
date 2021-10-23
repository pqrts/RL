using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class ui_Storyline_activate : EditorWindow
{
    private ext_StorylineEditor _s_StorylineEditor;
    private ext_StorylineEventSystem _s_StrEvent;
    private Sprite _preview_Body;
    private Sprite _preview_Haircut;
    private Sprite _preview_Clothes;
    private Sprite _preview_Makeup;
    private string _CharacterName;
    private string _CharacterDescription;
    public List<GameObject> _list_CharactesListview = new List<GameObject>();
    public static ui_Storyline_activate ShowWindow()
    {
        ui_Storyline_activate window_activate = GetWindow<ui_Storyline_activate>();
        window_activate.titleContent = new GUIContent("Characters list");
        window_activate.minSize = new Vector2(170, 475f);
        window_activate.maxSize = new Vector2(170f, 475f);
        return window_activate;

    }
    private void OnEnable()
    {
        _s_StrEvent = (ext_StorylineEventSystem)FindObjectOfType(typeof(ext_StorylineEventSystem));
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent.OnStrEdUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
        Repaint();
    }

    private void CreateGUI()
    {
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/char_activation.uxml");
        VisualElement VTuxml = VT.Instantiate();
        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/charIconTemplate.uxml");
        VisualElement VTlistview_element = VTListview.Instantiate();
        rootVisualElement.Add(VTuxml);

        Label _l_Preview = VTuxml.Q<VisualElement>("preview") as Label;
        _l_Preview.text = "Character preview";
        Label _l_CharacterName = VTuxml.Q<VisualElement>("charname") as Label;
        _l_CharacterName.text = "Runtime Name";
        Label _l_CharactersList = VTuxml.Q<VisualElement>("list") as Label;
        _l_CharactersList.text = "Characters list";

        //charlist setup
        var _CharactersListviewItems = new List<GameObject>();

        for (int i = 0; i < _s_StorylineEditor._list_RequiredObjects.Count; i++)
            if (_s_StorylineEditor._list_RequiredObjects[i] != null)
            {
                _CharactersListviewItems.Add(_s_StorylineEditor._list_RequiredObjects[i]);
            }
        Func<VisualElement> makeItem = () => VTListview.CloneTree();
        Label element_name = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = _s_StorylineEditor._list_RequiredObjects[i].name;
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _s_StorylineEditor._temp_CharIcon.texture;
        };

        const int itemHeight = 30;
        var _listView_Characters = new ListView(_CharactersListviewItems, itemHeight, makeItem, bindItem);

        _listView_Characters.selectionType = SelectionType.Single;

        _listView_Characters.onItemsChosen += obj =>
        {

            Debug.Log(_listView_Characters.selectedItem);

            if (GetPreviewComponents(_listView_Characters.selectedIndex))
            {
                if (_preview_Body != null && _preview_Clothes != null && _preview_Haircut != null && _preview_Makeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _preview_Body.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _preview_Clothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _preview_Haircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _preview_Makeup.texture;
                    Label l_char_name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    l_char_name.text = _CharacterName;
                    Label l_char_descr = VTuxml.Q<VisualElement>("descrcontent") as Label;
                    l_char_descr.text = _CharacterDescription;
                }

            }
        };
        _listView_Characters.onSelectionChange += objects =>
        {
            if (GetPreviewComponents(_listView_Characters.selectedIndex))
            {
                if (_preview_Body != null && _preview_Clothes != null && _preview_Haircut != null && _preview_Makeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _preview_Body.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _preview_Clothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _preview_Haircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _preview_Makeup.texture;
                    Label _l_Character_Name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    _l_Character_Name.text = _CharacterName;

                }
            }
        };
        _listView_Characters.style.flexGrow = 1.0f;
        Button _b_CharacterActivate = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string TempCharacterName = _listView_Characters.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                Activate(TempCharacterName);
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_CharacterActivate.text = "Activate";
        Button _b_CharacterDelete = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (EditorUtility.DisplayDialog("Notice", " Are you sure about this?", "OK", "Cancel"))
                {
                    string p_char_name = _listView_Characters.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                    if (_s_StorylineEditor.DeleteCharacter(p_char_name))
                    {
                        _s_StrEvent.EditorUpdated();
                    }
                }

            }
        });

        _b_CharacterDelete.text = "Delete";
        //
        VTuxml.Q<VisualElement>("charlistBackgroung").Add(_listView_Characters);
        VTuxml.Q<VisualElement>("buttonHolder2").Add(_b_CharacterDelete);
        VTuxml.Q<VisualElement>("buttonHolder1").Add(_b_CharacterActivate);
    }
    private void Activate(string CharacterName)
    {
        _s_StorylineEditor.ActivatExistingCharacter(CharacterName);
    }
    public Boolean GetPreviewComponents(int SelectedCharacterID)
    {
        _preview_Body = _s_StorylineEditor._list_RequiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_body.sprite;
        _preview_Clothes = _s_StorylineEditor._list_RequiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_clothes.sprite;
        _preview_Haircut = _s_StorylineEditor._list_RequiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_haircut.sprite;
        _preview_Makeup = _s_StorylineEditor._list_RequiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_makeup.sprite;
        _CharacterName = _s_StorylineEditor._list_RequiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_runtime_name;
        return true;
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
    private void OnDisable()
    {
        _s_StrEvent.OnStrEdUpdated -= OnStrEdUpdated;
    }
}
