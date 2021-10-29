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
    private VisualElement _rootVisualElement;
    private VisualTreeAsset _rootVTasset;
    private Sprite _previewBody;
    private Sprite _previewHaircut;
    private Sprite _previewClothes;
    private Sprite _previewMakeup;
    private string _characterName;
    private string _characterDescription;
    private string _characterSprite;
    private string _storylineTitle;
    private string _phraseFieldValue;
     
    private RectTransform _SelectedCharacterRectTransform;

    private StrEditorEvents _s_StrEvent;
    private StrEditorGodObject _s_StorylineEditor;

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
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StorylineEditor = (StrEditorGodObject)FindObjectOfType(typeof(StrEditorGodObject));
        _s_StrEvent.StrEditorUpdated += OnStrEdUpdated;
        if (_s_StorylineEditor != null)
        {
            _s_StorylineEditor.Init();
        }
    }

    private void OnStrEdUpdated()
    {
        CreateGUI();
       
    }
    private void CreateGUI()
    {
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/mainwindow.uxml");
        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/charIconTemplate.uxml");
        VisualElement VTuxml = VT.Instantiate();
        VisualElement VTlistview_element = VTListview.Instantiate();

        var SS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/mainwindowUSS.uss");
        rootVisualElement.styleSheets.Add(SS);
        var SS2 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ñhariconTemplateUSS.uss");
        rootVisualElement.styleSheets.Add(SS2);

        Scroller _slider_CGPosition = new Scroller(0, 100, (v) => { }, SliderDirection.Horizontal);
        _slider_CGPosition.style.height = 20f;
        //Charlist setup
        var Characters_listview_items = new List<GameObject>();

        for (int i = 0; i < _s_StorylineEditor._activeCharacters.Count; i++)
            if (_s_StorylineEditor._activeCharacters[i] != null)
            {
                Characters_listview_items.Add(_s_StorylineEditor._activeCharacters[i]);
            }
        Func<VisualElement> makeItem = () => VTListview.CloneTree();
        Label element_name = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e.Q<VisualElement>("name") as Label).text = _s_StorylineEditor._activeCharacters[i].name;
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _s_StorylineEditor._tempCharIcon.texture;
        };

        const int itemHeight = 30;
        var _listview_Characters = new ListView(Characters_listview_items, itemHeight, makeItem, bindItem);
        _listview_Characters.selectionType = SelectionType.Single;

        _listview_Characters.onItemsChosen += obj =>
        {

            string p_char_name = _listview_Characters.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
            SetSelectedCharacterRectTransform(p_char_name);

            if (GetPreviewComponents(_listview_Characters.selectedIndex))
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
        _listview_Characters.onSelectionChange += objects =>
        {

            string temp_CharacterName = _listview_Characters.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
            SetSelectedCharacterRectTransform(temp_CharacterName);
            if (GetPreviewComponents(_listview_Characters.selectedIndex))
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
        _listview_Characters.style.flexGrow = 1.0f;
        // steplist setup
        var items2 = new List<int>();

        for (int i = 0; i < _s_StorylineEditor._stepsTotal.Count; i++)
            items2.Add(i);
        Func<VisualElement> makeItem2 = () => VTListview.CloneTree();
        Label element_name2 = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon2 = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem2 = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = "Step: " + i.ToString();
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = _s_StorylineEditor._tempCharIcon.texture;
        };

        const int itemHeight2 = 30;
        var _listview_Steps = new ListView(items2, itemHeight2, makeItem2, bindItem2);

        _listview_Steps.selectionType = SelectionType.Single;

        _listview_Steps.onItemsChosen += obj =>
        {

            Debug.Log(_listview_Steps.selectedItem);


        };
        _listview_Steps.onSelectionChange += objects =>
        {

            Debug.Log(_listview_Steps.selectedItem);

        };
        _listview_Steps.style.flexGrow = 1.0f;
        /////////////////////////////////////////////////////////////
        //////
        Label _l_Charlist = VTuxml.Q<VisualElement>("charlist") as Label;
        _l_Charlist.text = "Active Characters";
        Label _l_Preview = VTuxml.Q<VisualElement>("preview") as Label;
        _l_Preview.text = "Character preview";
        Label _l_CharacterDescription = VTuxml.Q<VisualElement>("description") as Label;
        _l_CharacterDescription.text = "Description";
        Label _l_Tools = VTuxml.Q<VisualElement>("toolbar1") as Label;
        _l_Tools.text = "Tools";

        Label _l_FileOptions = VTuxml.Q<VisualElement>("toolbar2") as Label;
        _l_FileOptions.text = "File options";

        Label _l_EditingOptions = VTuxml.Q<VisualElement>("toolbar3") as Label;
        _l_EditingOptions.text = "Editing options";

        Label _l_CharacterName = VTuxml.Q<VisualElement>("charname") as Label;
        _l_CharacterName.text = "Runtime Name";
        Label _l_CharaterSprite = VTuxml.Q<VisualElement>("sprites") as Label;
        _l_CharaterSprite.text = "Character options";
        // Label l_phrase = VTuxml.Q<VisualElement>("_phrase") as Label;
        //  l_phrase.text = "Character _phrase";
        Label _l_Status = VTuxml.Q<VisualElement>("status") as Label;
        _l_Status.text = "Initialization : " + _s_StorylineEditor._initStatus + "      Current file : " + _s_StorylineEditor._StorylineName;
        Label _l_Status2 = VTuxml.Q<VisualElement>("status2") as Label;
        _l_Status2.text = "Action: " + _s_StorylineEditor._actionID + " (Total: " + _s_StorylineEditor._actionsTotal.Count + ") / Step: " + _s_StorylineEditor._stepID + " (Total: " + _s_StorylineEditor._stepsTotal.Count + ")";

        Label _l_StepsList = VTuxml.Q<VisualElement>("steplist") as Label;
        _l_StepsList.text = "Steps list";
        Label _l_CGSlider = VTuxml.Q<VisualElement>("CGrlabel") as Label;
        _l_CGSlider.text = "CG position";

        Label _l_CGPreview = VTuxml.Q<VisualElement>("CGpreview") as Label;
        _l_CGPreview.text = "CG preview";


        // CG preview
        if (_s_StorylineEditor._CGsprite != null)
        {
            VTuxml.Q<VisualElement>("CGpreviewArea").style.backgroundImage = _s_StorylineEditor._CGsprite.texture;
        }

        //buttons

        Button _b_CharacterAdd = new Button(() =>
        {
            if (ValidateStoryline())
            {
                SelectCharacter();
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_CharacterAdd.text = "Add character";

        Button _b_CharacterDeactivate = new Button(() =>
        {
            if (ValidateStoryline())
            {
                string temp = _listview_Characters.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                if (_s_StorylineEditor.DeactivateCharacter(temp))
                {
                    EditorUtility.DisplayDialog("Notice", "Character deactivated", "OK");
                    _s_StrEvent.EditorUpdated();
                }
            }
        });
        _b_CharacterDeactivate.text = "Deactivate Character";

        Button _b_CharacterActivate = new Button(() =>
        {
            if (ValidateStoryline())
            {
                StrEditorCharactersListWindow.ShowWindow();
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_CharacterActivate.text = "Open Characters list";

        Button _b_ControlPanel = new Button(() =>
        {
            if (ValidateStoryline())
            {
               StrEditorControlPanelWindow.ShowWindow();
                _s_StrEvent.EditorUpdated();
            }

        });
        _b_ControlPanel.text = "Control panel";

        Button _b_SetAuthor = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_listview_Characters.selectedItem != null)
                {
                    _s_StorylineEditor.SetAuthor(_listview_Characters.selectedItem.ToString());
                    _s_StrEvent.EditorUpdated();
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "No character selected", "OK");
                }
            }
        });
        _b_SetAuthor.text = "Set as author";

        Button _b_NewAction = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_s_StorylineEditor._readyForNextAction == true)
                {
                    if (_s_StorylineEditor.NewAction())
                    {
                        EditorUtility.DisplayDialog("Notice", "New action created", "OK");
                        _s_StrEvent.EditorUpdated();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Unable to create, check required conditions", "OK");
                }
            }
        });
        _b_NewAction.text = "New Action";

        Button _b_SelectCG = new Button(() =>
        {
            if (ValidateStoryline())
            {
                SelectCG();
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_SelectCG.text = "Select CG";

        Button _b_NewStep = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_s_StorylineEditor.CreateStep())
                {
                    _s_StrEvent.EditorUpdated();
                }
            }
        });
        _b_NewStep.text = "New Step";

        Button _b_CharacterEditor = new Button(() =>
        {
            if (ValidateStoryline())
            {
                StrEditorCharacterConstructorWindow.ShowWindow();
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_CharacterEditor.text = "Character Editor";

        Button _b_Save = new Button(() =>
        {
            if (ValidateStoryline())
            {
                Debug.Log(" doing nothing");
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_Save.text = "Save";

        Button _b_ExportToStr = new Button(() =>
        {
            if (ValidateStoryline())
            {
                _s_StrEvent.EditorUpdated();
                EditorUtility.DisplayDialog("Notice", ".str writed.", "OK");
            }
        });
        _b_ExportToStr.text = "Export to .str";

        Button _b_AddChoise = new Button(() =>
        {
            if (ValidateStoryline())
            {
                StrEditorChoiseConstructorWindow.ShowWindow();
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_AddChoise.text = "Add choise";

        Button _b_JumpTo = new Button(() =>
        {
            if (ValidateStoryline())
            {
                Debug.Log(" doing nothing");
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_JumpTo.text = "Add jump marker";

        Button _b_DeleteCharacter = new Button(() =>
        {
            if (ValidateStoryline())
            {
                Debug.Log(" doing nothing");
                _s_StrEvent.EditorUpdated();
            }
        });

        _b_DeleteCharacter.text = "Delete character";

        Button _b_NewFile = new Button(() =>
        {

            StrEditorStorylineCreatorWindow.ShowWindow();
            _s_StrEvent.EditorUpdated();

        });
        _b_NewFile.text = "New .str";

        Button _b_AddEffect = new Button(() =>
        {
            if (ValidateStoryline())
            {
                Debug.Log(" doing nothing");
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_AddEffect.text = "Add effect";

        Button _b_OpenFile = new Button(() =>
        {
            SelectStoryline();
            _s_StrEvent.EditorUpdated();
        });
        _b_OpenFile.text = "Open .str";

        Button _b_DeleteCharacterFromStoryline = new Button(() =>
        {
            if (ValidateStoryline())
            {
                Debug.Log(" doing nothing");
                _s_StrEvent.EditorUpdated();
            }
            SelectStoryline();
        });
        _b_DeleteCharacterFromStoryline.text = "Delete from current";

        TextField _field_Phrase = new TextField();

        var SS4 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/InputFieldCustom.uss");

        // rootVisualElement.styleSheets.Add(SS4);
        _field_Phrase.Q(TextField.textInputUssName).AddToClassList("TextField-Editor");
        _field_Phrase.multiline = true;
        _field_Phrase.style.whiteSpace = WhiteSpace.Normal;

        _field_Phrase.style.height = 180;
        _field_Phrase.maxLength = 150;
        //
        rootVisualElement.Add(VTuxml);
        VTuxml.Q<VisualElement>("leftToolbar").Add(_b_ControlPanel);
        VTuxml.Q<VisualElement>("leftToolbar").Add(_b_CharacterEditor);

        VTuxml.Q<VisualElement>("leftToolbar2").Add(_b_NewFile);
        VTuxml.Q<VisualElement>("leftToolbar2").Add(_b_OpenFile);
        VTuxml.Q<VisualElement>("leftToolbar2").Add(_b_Save);

        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_SelectCG);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_CharacterAdd);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_DeleteCharacter);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_NewStep);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_JumpTo);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_AddChoise);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_AddEffect);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_NewAction);
        VTuxml.Q<VisualElement>("leftToolbar3").Add(_b_ExportToStr);

        VTuxml.Q<VisualElement>("phraseHolder2").Add(_field_Phrase);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(_b_CharacterDeactivate);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(_b_CharacterActivate);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(_b_SetAuthor);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(_b_DeleteCharacterFromStoryline);
        VTuxml.Q<VisualElement>("charlistBackgroung").Add(_listview_Characters);
        VTuxml.Q<VisualElement>("steplistArea").Add(_listview_Steps);
        VTuxml.Q<VisualElement>("CG_sliderHolder").Add(_slider_CGPosition);

        _slider_CGPosition.valueChanged += (e => ConvertSliderToCGPosition(_slider_CGPosition.value));
        _field_Phrase.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SelectPhrase(_field_Phrase.value));
        _field_Phrase.value = _s_StorylineEditor._phrase;

    }
    void SetSelectedCharacterRectTransform(string CharacterName)
    {

        foreach (GameObject unit in _s_StorylineEditor._activeCharacters)
        {
            if (unit.name == CharacterName)
            {
                _SelectedCharacterRectTransform = unit.GetComponent<RectTransform>();

            }
        }
    }
    void ConvertSliderToCGPosition(float CGSliderValue)
    {
        float PoolX = _s_StorylineEditor._ñanvasMovingPool;
        float SliderValueOfDivision = PoolX / 100;
        float CGPosisitionX = CGSliderValue * SliderValueOfDivision;
        _s_StorylineEditor.MoveCG(CGPosisitionX);

    }
    
    void SetSelectedCharacterParent()
    {
        _SelectedCharacterRectTransform.transform.SetParent(_s_StorylineEditor._CGRectTransform.transform, true);
    }
    void SelectCG()
    {
        Texture2D tex = new Texture2D(1, 1);
        string Path = EditorUtility.OpenFilePanel("Select CG", _s_StorylineEditor._s_Folder._CG, "png");
        if (Path.Length != 0)
        {
            string temp = Path.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
            string temp2 = temp.Replace(".png", "");
            string temp3 = temp2.Replace("Gamedata/Textures/CG/", "");
            if (_s_StorylineEditor.AddCG(temp2, temp3))
            {
                CreateGUI();
            }

        }
    }
    void SelectStoryline()
    {
        if (EditorUtility.DisplayDialog("Notice", "Usaved progress will be lost. Continue?", "OK", "Cancel"))
        {
            string Path = EditorUtility.OpenFilePanel("Select storyline", _s_StorylineEditor._s_Folder._storylines, "str");
            if (Path.Length != 0)
            {
                string temp = Path.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
                string temp2 = temp.Replace(".str", "");
                string temp3 = temp.Replace("Gamedata/Storylines/", "");
                if (_s_StorylineEditor.OpenStoryline(temp3))
                {
                    CreateGUI();
                }
            }
        }
    }
    private void SelectCharacter()
    {
        string Path = EditorUtility.OpenFilePanel("Select Character", _s_StorylineEditor._s_Folder._characters, "char");
        if (Path.Length != 0)
        {
            string temp = Path.Replace(_s_StorylineEditor._s_Folder._root + "/Resources/", "");
            string temp2 = temp.Replace(".char", "");
            string temp3 = temp2.Replace("Gamedata/Ñharacters/", "");
            _s_StorylineEditor.AddCharacter(Path, temp3);
             }
    }
    private string SelectPhrase(string PhraseText)
    {
        _s_StorylineEditor._phrase = PhraseText;
        return (PhraseText);
    }
    private Boolean GetPreviewComponents(int SelectedCharacterID)
    {
        _previewBody = _s_StorylineEditor._activeCharacters[SelectedCharacterID].GetComponent<local_character>()._char_body.sprite;
        _previewClothes = _s_StorylineEditor._activeCharacters[SelectedCharacterID].GetComponent<local_character>()._char_clothes.sprite;
        _previewHaircut = _s_StorylineEditor._activeCharacters[SelectedCharacterID].GetComponent<local_character>()._char_haircut.sprite;
        _previewMakeup = _s_StorylineEditor._activeCharacters[SelectedCharacterID].GetComponent<local_character>()._char_makeup.sprite;
        _characterName = _s_StorylineEditor._activeCharacters[SelectedCharacterID].GetComponent<local_character>()._char_runtime_name;
        _characterDescription = "Ïåðñîíàæ, èñïîëüçóåìûé äëÿ ðàçðàáîòêè ðåäàêòîðà";
        _characterSprite = "get sprites list";
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
