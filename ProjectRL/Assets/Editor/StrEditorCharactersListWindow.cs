using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class StrEditorCharactersListWindow : EditorWindow
{
    private StrEditorGodObject _s_StorylineEditor;
    private StrEditorEvents _s_StrEvent;
    private Sprite _previewBody;
    private Sprite _previewHaircut;
    private Sprite _previewClothes;
    private Sprite _previewMakeup;
    private string _characterName;
    private string _characterDescription;
    public List<GameObject> _CharactesListviewElements = new List<GameObject>();
    public static StrEditorCharactersListWindow ShowWindow()
    {
        StrEditorCharactersListWindow window_activate = GetWindow<StrEditorCharactersListWindow>();
        window_activate.titleContent = new GUIContent("Characters list");
        window_activate.minSize = new Vector2(170, 475f);
        window_activate.maxSize = new Vector2(170f, 475f);
        return window_activate;

    }
    private void OnEnable()
    {
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StorylineEditor = (StrEditorGodObject)FindObjectOfType(typeof(StrEditorGodObject));
        _s_StrEvent.StrEditorUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
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

        for (int i = 0; i < _s_StorylineEditor._requiredObjects.Count; i++)
            if (_s_StorylineEditor._requiredObjects[i] != null)
            {
                _CharactersListviewItems.Add(_s_StorylineEditor._requiredObjects[i]);
            }
        Func<VisualElement> makeItem = () => VTListview.CloneTree();
        Label element_name = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = _s_StorylineEditor._requiredObjects[i].name;
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _s_StorylineEditor._tempCharIcon.texture;
        };

        const int itemHeight = 30;
        var _listView_Characters = new ListView(_CharactersListviewItems, itemHeight, makeItem, bindItem);

        _listView_Characters.selectionType = SelectionType.Single;

        _listView_Characters.onItemsChosen += obj =>
        {

            Debug.Log(_listView_Characters.selectedItem);

            if (GetPreviewComponents(_listView_Characters.selectedIndex))
            {
                if (_previewBody != null && _previewClothes != null && _previewHaircut != null && _previewMakeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _previewBody.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _previewClothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _previewHaircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _previewMakeup.texture;
                    Label l_char_name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    l_char_name.text = _characterName;
                    Label l_char_descr = VTuxml.Q<VisualElement>("descrcontent") as Label;
                    l_char_descr.text = _characterDescription;
                }

            }
        };
        _listView_Characters.onSelectionChange += objects =>
        {
            if (GetPreviewComponents(_listView_Characters.selectedIndex))
            {
                if (_previewBody != null && _previewClothes != null && _previewHaircut != null && _previewMakeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = _previewBody.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = _previewClothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = _previewHaircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = _previewMakeup.texture;
                    Label _l_Character_Name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    _l_Character_Name.text = _characterName;

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
        _previewBody = _s_StorylineEditor._requiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_body.sprite;
        _previewClothes = _s_StorylineEditor._requiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_clothes.sprite;
        _previewHaircut = _s_StorylineEditor._requiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_haircut.sprite;
        _previewMakeup = _s_StorylineEditor._requiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_makeup.sprite;
        _characterName = _s_StorylineEditor._requiredObjects[SelectedCharacterID].GetComponent<local_character>()._char_runtime_name;
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
        _s_StrEvent.StrEditorUpdated -= OnStrEdUpdated;
    }
}
