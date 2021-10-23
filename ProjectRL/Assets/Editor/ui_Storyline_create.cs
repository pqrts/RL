using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
public class ui_Storyline_create : EditorWindow
{
    public static ui_Storyline_create ShowWindow()
    {
        ui_Storyline_create window_new = GetWindow<ui_Storyline_create>();
        window_new.titleContent = new GUIContent("New storyline");
        window_new.minSize = new Vector2(390f, 135f);
        window_new.maxSize = new Vector2(390f, 135f);
        return window_new;

    }
  
    private void CreateGUI()
    {
        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/new_storyline.uxml");
        VisualElement VTuxml = VT.Instantiate();
        var SS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/new_storyline_USS.uss");
        rootVisualElement.styleSheets.Add(SS);
        rootVisualElement.Add(VTuxml);
        Label l_title = VTuxml.Q<VisualElement>("title") as Label;
        l_title.text = "Title";
        Label l_number = VTuxml.Q<VisualElement>("number") as Label;
        l_number.text = "Number: ";
        Label l_part = VTuxml.Q<VisualElement>("part") as Label;
        l_part.text = "Part: ";
        Label l_user = VTuxml.Q<VisualElement>("user") as Label;
        l_user.text = "Author";
       
        TextField number = new TextField();
        number.style.height = 20;
        number.style.width = 40;
        number.maxLength = 4;
        TextField part = new TextField();
        part.style.height = 20;
        part.style.width = 40;
        part.maxLength = 4;
        TextField t_user = new TextField();
        t_user.style.height = 20;
        t_user.style.width = 170;
        t_user.maxLength = 32;

        Button create = new Button(() =>
        {
            if (number.value != "" && part.value != "" & t_user.value != "")
            {
                string file_name = "storyline_" + number.value + "_" + "part_" + part.value + ".str";
                string s_user = t_user.value;
                ext_StorylineEditor s_target = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));

                if (!s_target.CheckStorylineExistence(file_name))
                {
                    if (s_target.CreateNewStoryline(file_name, s_user))
                    {
                        EditorUtility.DisplayDialog("Notice", "Storyline created", "OK");
                      
                        this.Close();
                    }
                    else
                    {

                        EditorUtility.DisplayDialog("Notice", " The file was not created. Check 'Storylines' folder", "OK");
                    }
                }
                else 
                {
                    EditorUtility.DisplayDialog("Notice", " This storyline already exists", "OK");
                }
            }
            else 
            {
                EditorUtility.DisplayDialog("Notice", "Required fields are not filled", "OK");
            }

        });
        create.text = "Create";


        VTuxml.Q<VisualElement>("fieldHolder1").Add(number);
        VTuxml.Q<VisualElement>("fieldHolder2").Add(part);
        VTuxml.Q<VisualElement>("fieldHolder3").Add(t_user);
        VTuxml.Q<VisualElement>("buttonHolder").Add(create);
    }
}
