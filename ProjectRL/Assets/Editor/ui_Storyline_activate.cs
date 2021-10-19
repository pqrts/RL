using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;


public class ui_Storyline_activate : EditorWindow
{
    Sprite preview_body;
    Sprite preview_haircut;
    Sprite preview_clothes;
    Sprite preview_makeup;
    string char_name;
    string char_descr;
    public List<GameObject> _list_listview_content = new List<GameObject>();
    public static ui_Storyline_activate ShowWindow()
    {
        ui_Storyline_activate window_activate = GetWindow<ui_Storyline_activate>();
        window_activate.titleContent = new GUIContent("Characters list");
        window_activate.minSize = new Vector2(170, 475f);
        window_activate.maxSize = new Vector2(170f, 475f);
        return window_activate;

    }

    private void Update()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        if (s_target._update_ui_activate == true)
        {
            CreateGUI();
        }
    }
    public void bbbb()
    {
        CreateGUI();
       
    }
    public  void CreateGUI()
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/char_activation.uxml");
        VisualElement VTuxml = VT.Instantiate();
        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/charIconTemplate.uxml");
        VisualElement VTlistview_element = VTListview.Instantiate();
        rootVisualElement.Add(VTuxml);
        Label l_preview = VTuxml.Q<VisualElement>("preview") as Label;
        l_preview.text = "Character preview";

        Label l_charname = VTuxml.Q<VisualElement>("charname") as Label;
        l_charname.text = "Runtime Name";
        Label l_charlist = VTuxml.Q<VisualElement>("list") as Label;
        l_charlist.text = "Characters list";
        //charlist setup
        var items = new List<GameObject>();

        for (int i = 0; i < s_target._list_required_objects.Count; i++)
            if (s_target._list_required_objects[i] != null)
            {
                items.Add(s_target._list_required_objects[i]);
            }
        Func<VisualElement> makeItem = () => VTListview.CloneTree();
        Label element_name = VTlistview_element.Q<VisualElement>("name") as Label;
        VisualElement element_icon = VTlistview_element.Q<VisualElement>("icon") as VisualElement;
        Action<VisualElement, int> bindItem = (e, i) =>
        {

            (e.Q<VisualElement>("name") as Label).text = s_target._list_required_objects[i].name;
            (e.Q<VisualElement>("icon") as VisualElement).style.backgroundImage = s_target._temp_CharIcon.texture;
        };

        const int itemHeight = 30;
        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Single;

        listView.onItemsChosen += obj =>
        {

            Debug.Log(listView.selectedItem);

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
            Debug.Log(objects);
            Debug.Log(listView.selectedItem);
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
              
                }
            }
        };
        listView.style.flexGrow = 1.0f;
        Button character_activate = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                string p_char_name = listView.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                Activate(p_char_name);
                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        character_activate.text = "Activate";
        Button character_delete = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

                if (EditorUtility.DisplayDialog("Notice", " Are you sure about this?", "OK", "Cancel"))
                {
                    string p_char_name = listView.selectedItem.ToString().Replace(" (UnityEngine.GameObject)", "");
                    if (s_target.Delete_character(p_char_name))
                    {
                        CreateGUI();
                        s_target.Update_editor_windows();
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });

        character_delete.text = "Delete";
        //
        VTuxml.Q<VisualElement>("charlistBackgroung").Add(listView);
        VTuxml.Q<VisualElement>("buttonHolder2").Add(character_delete);
        VTuxml.Q<VisualElement>("buttonHolder1").Add(character_activate);

        s_target._update_ui_activate = false;
    }
    private void Activate(string char_name)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));

        s_target.Activate_existing_character(char_name);
    }

    public Boolean Get_preview_components(int id)
    {
        ext_StorylineEd s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
        preview_body = s_target._list_required_objects[id].GetComponent<local_character>()._char_body.sprite;
        preview_clothes = s_target._list_required_objects[id].GetComponent<local_character>()._char_clothes.sprite;
        preview_haircut = s_target._list_required_objects[id].GetComponent<local_character>()._char_haircut.sprite;
        preview_makeup = s_target._list_required_objects[id].GetComponent<local_character>()._char_makeup.sprite;
        char_name = s_target._list_required_objects[id].GetComponent<local_character>()._char_runtime_name;


        return true;
    }

}
