using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

using System.IO;

public class ui_StorylineEd_editor : EditorWindow
{
    public int id_action = 0;
    public int id_step;
    VisualElement rootVE;
    VisualTreeAsset VTasset;
    Sprite preview_body;
    Sprite preview_haircut;
    Sprite preview_clothes;
    Sprite preview_makeup;
    string char_name;
    string char_descr;
    string char_sprite;
    string storyline_title;
    float p_value;
    string phrase_str;
    float char_slider_value;
    List<GameObject> charlist_GO = new List<GameObject>();
    private RectTransform selected_character_RT;

    [MenuItem("Storyline Editor/Open")]
    public static ui_StorylineEd_editor ShowWindow()
    {
        ui_StorylineEd_editor window = GetWindow<ui_StorylineEd_editor>();
        window.titleContent = new GUIContent("Storyline Editor");
        window.minSize = new Vector2(845f, 565f);
        window.maxSize = new Vector2(845f, 565f);
        return window;
    }
    void OnEnable()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        if (s_target != null)
        {
            s_target.Init();
        }
    }

    public void CreateGUI()
    {

        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/mainwindow.uxml");
        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/charIconTemplate.uxml");
        VisualElement VTuxml = VT.Instantiate();
        VisualElement VTlistview_element = VTListview.Instantiate();

        var SS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/mainwindowUSS.uss");
        rootVisualElement.styleSheets.Add(SS);
        var SS2 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ñhariconTemplateUSS.uss");
        rootVisualElement.styleSheets.Add(SS2);
        //Charlist setup
        Scroller CG_positioning = new Scroller(0, 100, (v) => { }, SliderDirection.Horizontal);
        CG_positioning.style.height = 20f;
        Scroller Character_positioning = new Scroller(0, 100, (v) => { }, SliderDirection.Horizontal);
        Character_positioning.style.height = 20f;
        Scroller Phrase_positioning = new Scroller(0, 100, (v) => { }, SliderDirection.Horizontal);
        Phrase_positioning.style.height = 20f;
        var items = new List<GameObject>();

        for (int i = 0; i < s_target._list_active_characters.Count; i++)
            items.Add(s_target._list_active_characters[i]);
        Func<VisualElement> makeItem = () => VTListview.CloneTree();
        Label element_name = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e.Q<VisualElement>("name") as Label).text = s_target._list_active_characters[i].name;
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = s_target._temp_CharIcon.texture;
        };

        const int itemHeight = 30;
        var listView = new ListView(items, itemHeight, makeItem, bindItem);
        listView.selectionType = SelectionType.Single;

        listView.onItemsChosen += obj =>
        {

            string p_char_name = listView.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
            Set_selected_character_RT(p_char_name);

            if (Get_preview_components(listView.selectedIndex))
            {
                if (preview_body != null && preview_clothes != null && preview_haircut != null && preview_makeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = preview_body.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = preview_clothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = preview_haircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = preview_makeup.texture;
                    Label l_char_name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    l_char_name.text = char_name;
                    Label l_char_descr = VTuxml.Q<VisualElement>("descrcontent") as Label;
                    l_char_descr.text = char_descr;
                }
            }
        };
        listView.onSelectionChange += objects =>
        {

            string p_char_name = listView.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
            Set_selected_character_RT(p_char_name);
            if (Get_preview_components(listView.selectedIndex))
            {
                if (preview_body != null && preview_clothes != null && preview_haircut != null && preview_makeup != null)
                {
                    VTuxml.Q<VisualElement>("previewHolder").style.backgroundImage = preview_body.texture;
                    VTuxml.Q<VisualElement>("previewHolder2").style.backgroundImage = preview_clothes.texture;
                    VTuxml.Q<VisualElement>("previewHolder3").style.backgroundImage = preview_haircut.texture;
                    VTuxml.Q<VisualElement>("previewHolder4").style.backgroundImage = preview_makeup.texture;
                    Label l_char_name = VTuxml.Q<VisualElement>("namecontent") as Label;
                    l_char_name.text = char_name;
                    Label l_char_descr = VTuxml.Q<VisualElement>("descrcontent") as Label;
                    l_char_descr.text = char_descr;
                }
            }
        };
        listView.style.flexGrow = 1.0f;
        // steplist setup
        var items2 = new List<int>();

        for (int i = 0; i < s_target._steps_total.Count; i++)
            items2.Add(i);
        Func<VisualElement> makeItem2 = () => VTListview.CloneTree();
        Label element_name2 = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon2 = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem2 = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = "Step: " + i.ToString();
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = s_target._temp_CharIcon.texture;
        };

        const int itemHeight2 = 30;
        var listView2 = new ListView(items2, itemHeight2, makeItem2, bindItem2);

        listView2.selectionType = SelectionType.Single;

        listView2.onItemsChosen += obj =>
        {

            Debug.Log(listView2.selectedItem);


        };
        listView2.onSelectionChange += objects =>
        {

            Debug.Log(listView2.selectedItem);

        };
        listView2.style.flexGrow = 1.0f;
        /////////////////////////////////////////////////////////////
        //////
        Label l_charlist = VTuxml.Q<VisualElement>("charlist") as Label;
        l_charlist.text = "Active Characters";
        Label l_preview = VTuxml.Q<VisualElement>("preview") as Label;
        l_preview.text = "Character preview";
        Label l_description = VTuxml.Q<VisualElement>("description") as Label;
        l_description.text = "Description";
        Label l_tools = VTuxml.Q<VisualElement>("tools") as Label;
        l_tools.text = "Options";
        Label l_charname = VTuxml.Q<VisualElement>("charname") as Label;
        l_charname.text = "Runtime Name";
        Label l_charsprite = VTuxml.Q<VisualElement>("sprites") as Label;
        l_charsprite.text = "Character options";
       // Label l_phrase = VTuxml.Q<VisualElement>("_phrase") as Label;
       // l_phrase.text = "Character _phrase";
        Label l_status = VTuxml.Q<VisualElement>("status") as Label;
        l_status.text = "Initialization : " + s_target._init_status + "      Current file : " + s_target._str_name;
        Label l_status2 = VTuxml.Q<VisualElement>("status2") as Label;
        l_status2.text = "Action: " + s_target._id_action + " (Total: " + s_target._actions_total.Count + ") / Step: " + s_target._id_step + " (Total: " + s_target._steps_total.Count + ")";

        Label l_steplist = VTuxml.Q<VisualElement>("steplist") as Label;
        l_steplist.text = "Steps list";
        Label l_CG_slider = VTuxml.Q<VisualElement>("CGrlabel") as Label;
        l_CG_slider.text = "CG position";

        Label l_character_slider = VTuxml.Q<VisualElement>("characterlabel") as Label;
        l_character_slider.text = "Selected character position";

        Label l_phrase_slider = VTuxml.Q<VisualElement>("phraselabel") as Label;
        l_phrase_slider.text = "Phrase holder position";
        Label l_cgprev = VTuxml.Q<VisualElement>("CGpreview") as Label;
        l_cgprev.text = "CG preview";


        // CG preview
        if (s_target._CG_sprite != null)
        {
            VTuxml.Q<VisualElement>("CGpreviewArea").style.backgroundImage = s_target._CG_sprite.texture;
        }

        //buttons

        Button character_add = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                Select_character();
                CreateGUI();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        character_add.text = "Add character";

        Button character_deactivate = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                string t = listView.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                if (s_target.Deactivate_character(t))
                {
                    EditorUtility.DisplayDialog("Notice", "Character deactivated", "OK");
                    CreateGUI();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        character_deactivate.text = "Deactivate Character";

        Button character_activate = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                ui_Storyline_activate.ShowWindow();

            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        character_activate.text = "Activate existing";
        Button character_set_author = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                if (listView.selectedItem != null)
                {
                    s_target.Set_author(listView.selectedItem.ToString());
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "No character selected", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        character_set_author.text = "Set as author";

        Button new_action = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                if (s_target.New_action())
                {
                    EditorUtility.DisplayDialog("Notice", "New action created", "OK");

                    CreateGUI();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        new_action.text = "New action";


        Button select_cg = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                Select_cg();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        select_cg.text = "Select CG";


        Button new_step = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                if (s_target.Create_step())
                {
                    CreateGUI();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }

        });
        new_step.text = "New Step";

        Button save_action = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                s_target.Form_str();
                EditorUtility.DisplayDialog("Notice", ".str writed.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });

        save_action.text = "Write action to str";

        Button new_file = new Button(() =>
        {

            ui_Storyline_create.ShowWindow();

        });

        new_file.text = "New .str";

        Button open_file = new Button(() =>
        {

            Select_str();
        });

        open_file.text = "Open .str";

        TextField phrase = new TextField();

        var SS4 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/InputFieldCustom.uss");

        rootVisualElement.styleSheets.Add(SS4);
        phrase.Q(TextField.textInputUssName).AddToClassList("TextField-Editor");
        phrase.multiline = true;
        phrase.style.whiteSpace = WhiteSpace.Normal;

        phrase.style.height = 205;
        phrase.maxLength = 2000;
        //
        rootVisualElement.Add(VTuxml);
        VTuxml.Q<VisualElement>("leftToolbar").Add(new_file);
        VTuxml.Q<VisualElement>("leftToolbar").Add(open_file);
        VTuxml.Q<VisualElement>("leftToolbar").Add(character_add);
        VTuxml.Q<VisualElement>("leftToolbar").Add(new_action);
        VTuxml.Q<VisualElement>("leftToolbar").Add(select_cg);
        VTuxml.Q<VisualElement>("leftToolbar").Add(new_step);
        VTuxml.Q<VisualElement>("leftToolbar").Add(save_action);

        VTuxml.Q<VisualElement>("phraseHolder2").Add(phrase);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(character_deactivate);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(character_activate);
        VTuxml.Q<VisualElement>("CharspriteHolder").Add(character_set_author);
        VTuxml.Q<VisualElement>("charlistBackgroung").Add(listView);
        VTuxml.Q<VisualElement>("steplistArea").Add(listView2);
        VTuxml.Q<VisualElement>("CG_sliderHolder").Add(CG_positioning);
        VTuxml.Q<VisualElement>("character_sliderHolder").Add(Character_positioning);
        VTuxml.Q<VisualElement>("phrase_sliderHolder").Add(Phrase_positioning);

        //events 

        CG_positioning.valueChanged += (e => CG_moving(CG_positioning.value));
        phrase.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Select_phrase(phrase.value));
        phrase.value = s_target._phrase;
    }
    void Set_selected_character_RT(string char_name)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        foreach (GameObject unit in s_target._list_active_characters)
        {
            if (unit.name == char_name)
            {
                selected_character_RT = unit.GetComponent<RectTransform>();

            }
        }
    }
    void CG_moving(float p_value_cg)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        float pool_x = s_target._canvas_moving_pool;
        float coord_unit = pool_x / 100;
        float cg_pos_x = p_value_cg * coord_unit;
        s_target.Move_CG(cg_pos_x);

    }
    void Character_moving(float p_value_character)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        selected_character_RT.transform.SetParent(s_target._Canvas.transform, false);
        if (s_target.Get_pos_limits_cg(selected_character_RT))
        {
            float pool_x = s_target._cg_moving_pool;
            float coord_unit = pool_x / 100;
            float character_pos_x = p_value_character * coord_unit;
            s_target.Move_character(character_pos_x, selected_character_RT);

        }
    }
    void Set_selecter_char_parent()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        selected_character_RT.transform.SetParent(s_target._CG_RectTransform.transform, true);
    }
    void Select_cg()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        Texture2D tex = new Texture2D(1, 1);
        string path1 = EditorUtility.OpenFilePanel("Select CG", s_target._s_folder._CG, "png");
        if (path1.Length != 0)
        {
            string t = path1.Replace(s_target._s_folder._root + "/Resources/", "");
            string t2 = t.Replace(".png", "");
            string t3 = t2.Replace("Gamedata/Textures/CG/", "");

            if (s_target.Add_CG(t2, t3))
            {
                CreateGUI();
            }

        }
    }
    void Select_str()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        if (EditorUtility.DisplayDialog("Notice", "Usaved progress will be lost. Continue?", "OK", "Cancel"))
        {
            string path = EditorUtility.OpenFilePanel("Select storyline", s_target._s_folder._storylines, "str");
            if (path.Length != 0)
            {
                string t = path.Replace(s_target._s_folder._root + "/Resources/", "");
                string t2 = t.Replace(".str", "");
                string t3 = t.Replace("Gamedata/Storylines/", "");
                if (s_target.open_str(t3))
                {
                    CreateGUI();
                }

            }
        }
    }
    void Select_character()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        string path = EditorUtility.OpenFilePanel("Select Character", s_target._s_folder._characters, "char");
        if (path.Length != 0)
        {
            string t = path.Replace(s_target._s_folder._root + "/Resources/", "");
            string t2 = t.Replace(".char", "");
            string t3 = t2.Replace("Gamedata/Ñharacters/", "");
            s_target.Add_character(path, t3);
        }
    }
    string Select_phrase(string phrase_text)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        s_target._phrase = phrase_text;

        return (phrase_text);
    }
    public Boolean Get_preview_components(int id)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        preview_body = s_target._list_active_characters[id].GetComponent<local_character>()._char_body.sprite;
        preview_clothes = s_target._list_active_characters[id].GetComponent<local_character>()._char_clothes.sprite;
        preview_haircut = s_target._list_active_characters[id].GetComponent<local_character>()._char_haircut.sprite;
        preview_makeup = s_target._list_active_characters[id].GetComponent<local_character>()._char_makeup.sprite;
        char_name = s_target._list_active_characters[id].GetComponent<local_character>()._char_runtime_name;
        char_descr = "Ïåðñîíàæ, èñïîëüçóåìûé äëÿ ðàçðàáîòêè ðåäàêòîðà";
        char_sprite = "get sprites list";
        return true;
    }

}
