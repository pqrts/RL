using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class ui_Storyline_choise : EditorWindow
{
    private ext_StorylineEd s_target;
    private int _cost_value;
    private string _option_text_value;
    private int _jump_to_value;
    private int _item_id_value;
    private List<string> _currency_choices = new List<string> { "Free", "Diamonds", "Hearts" };

    private TextField _cost_value_field;
    private TextField _jump_to_field;
    private TextField _item_id_field;
    private TextField _option_text_field;
    private DropdownField _d_currency_type;
    private string _type_cost = "cost";
    private string _type_opt_text = "option_text";
    private string _type_jump_to = "jump_to";
    private string _type_item_id = "item_id";
    private List<string> _list_options_temp = new List<string>();
    public static ui_Storyline_choise ShowWindow()
    {
        ui_Storyline_choise window_choise = GetWindow<ui_Storyline_choise>();
        window_choise.titleContent = new GUIContent("Choise Constructor");
        window_choise.minSize = new Vector2(340, 475f);
        window_choise.maxSize = new Vector2(340f, 475f);

        return window_choise;

    }
    private void OnEnable()
    {
        s_target = (ext_StorylineEd)FindObjectOfType(typeof(ext_StorylineEd));
    }
    private void Update()
    {
        if (s_target._update_ui_choise == true)
        {
            CreateGUI();
        }
    }
    public void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/choise_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();

        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ChoiseOptionTemplate.uxml");
        VisualElement VTlistview_element = VTListview.Instantiate();
        /// optionslist setup
        var items = new List<string>();

        for (int i = 0; i < s_target._list_choise_options.Count; i++)
            if (s_target._list_choise_options[i] != null)
            {
                items.Add(s_target._list_choise_options[i]);
            }
        Func<VisualElement> makeItem = () => VTListview.CloneTree();

        Label l_element_number = VTlistview_element.Q<VisualElement>("number") as Label;
        Label l_element_status_number = VTlistview_element.Q<VisualElement>("status_number") as Label;

        Label l_element_currency = VTlistview_element.Q<VisualElement>("currency_type") as Label;
        Label l_element_status_currency = VTlistview_element.Q<VisualElement>("status_currency_type") as Label;

        Label l_element_jump_to = VTlistview_element.Q<VisualElement>("jump_to") as Label;
        Label l_element_status_jump_to = VTlistview_element.Q<VisualElement>("status_jump_to") as Label;

        Label l_element_itemID = VTlistview_element.Q<VisualElement>("item_ID") as Label;
        Label l_element_status_itemID = VTlistview_element.Q<VisualElement>("status_itemID") as Label;

        Label l_element_status_opt_text = VTlistview_element.Q<VisualElement>("status_opt_text") as Label;


        Action<VisualElement, int> bindItem = (e, i) =>
        {
            string[] _option = s_target.Get_choise_option(i);
            foreach (string unit in _option)
            {
                Debug.Log(unit);
            }

            (e.Q<VisualElement>("number") as Label).text = "¹: ";
            (e.Q<VisualElement>("status_number") as Label).text = _option[0];

            (e.Q<VisualElement>("currency_type") as Label).text = "Currency type: ";
            (e.Q<VisualElement>("status_currency_type") as Label).text = _option[1];

            (e.Q<VisualElement>("cost") as Label).text = "Cost: ";
            (e.Q<VisualElement>("status_cost") as Label).text = _option[2];

            (e.Q<VisualElement>("jump_to") as Label).text = "Jump to: ";
            (e.Q<VisualElement>("status_jump_to") as Label).text = _option[3];

            (e.Q<VisualElement>("item_ID") as Label).text = "Item ID: ";
            (e.Q<VisualElement>("status_itemID") as Label).text = _option[4];

            (e.Q<VisualElement>("status_opt_text") as Label).text = _option[5];

        };

        const int itemHeight = 30;
        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Single;

        listView.onItemsChosen += obj =>
        {
          listView.contentContainer.
          
      

        };
        listView.onSelectionChange += objects =>
        {
            int r = listView.selectedIndex;
            Action<VisualElement, int> bindItem = (e, r) =>
            {
                (e.Q<VisualElement>(" Background") as VisualElement).style.backgroundColor = Color.cyan;
            };

        };
        listView.style.flexGrow = 1.0f;
        /// 


        _cost_value_field = new TextField();
        _cost_value_field.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_value(_cost_value_field.value, _type_cost, true));

        _option_text_field = new TextField();
        _option_text_field.style.height = 40f;
        _option_text_field.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_value(_option_text_field.value, _type_opt_text, false));

        _jump_to_field = new TextField();
        _jump_to_field.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_value(_jump_to_field.value, _type_jump_to, true));

        _item_id_field = new TextField();
        _item_id_field.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => Set_value(_item_id_field.value, _type_item_id, true));


        _d_currency_type = new DropdownField("", _currency_choices, 0);



        Label l_options = VTuxml.Q<VisualElement>("options") as Label;
        l_options.text = "Options: ";

        Label l_currency = VTuxml.Q<VisualElement>("currency") as Label;
        l_currency.text = "Currency type: ";

        Label l_cost = VTuxml.Q<VisualElement>("cost") as Label;
        l_cost.text = "Cost: ";

        Label l_cost_value = VTuxml.Q<VisualElement>("cost_label") as Label;
        l_cost_value.text = "Cost: ";

        Label l_opt_text = VTuxml.Q<VisualElement>("opt_text") as Label;
        l_opt_text.text = "Text: ";

        Label l_jump_to = VTuxml.Q<VisualElement>("jump_to") as Label;
        l_jump_to.text = "Jump to action: ";

        Label l_jump_number = VTuxml.Q<VisualElement>("jump_label") as Label;
        l_jump_number.text = "Action ¹: ";

        Label l_give_item = VTuxml.Q<VisualElement>("give_item") as Label;
        l_give_item.text = "Give item: ";

        Label l_item_id = VTuxml.Q<VisualElement>("give_label") as Label;
        l_item_id.text = "Item ID: ";

        Label l_edit_opt = VTuxml.Q<VisualElement>("edit_opt") as Label;
        l_edit_opt.text = "Editing options: ";

        Button add_option = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                Create_option();
                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        add_option.text = "Add option";

        Button delete_option = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {
                if (listView.selectedItem != null)
                {
                    Delete_option(listView.selectedIndex);
                    s_target.Update_editor_windows();
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        delete_option.text = "Delete selected option";

        Button add_option_to_action = new Button(() =>
        {
            if (s_target.Check_str_existence(s_target._str_name))
            {

                s_target.Update_editor_windows();
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Create new storyline first", "OK");
            }
        });
        add_option_to_action.text = "Add choise to Action";

        VTuxml.Q<VisualElement>("buttonHolder1").Add(add_option);
        VTuxml.Q<VisualElement>("buttonHolder2").Add(delete_option);
        VTuxml.Q<VisualElement>("buttonHolder3").Add(add_option_to_action);
        VTuxml.Q<VisualElement>("cost_fieldHolder").Add(_cost_value_field);
        VTuxml.Q<VisualElement>("options_text_Holder").Add(_option_text_field);
        VTuxml.Q<VisualElement>("jump_fieldHolder").Add(_jump_to_field);
        VTuxml.Q<VisualElement>("give_fieldHolder").Add(_item_id_field);
        VTuxml.Q<VisualElement>("currency_DropHolder").Add(_d_currency_type);
        VTuxml.Q<VisualElement>("options_list_Holder").Add(listView);


        rootVisualElement.Add(VTuxml);
        s_target._update_ui_choise = false;
    }

    private void Set_value(string field_value, string field_type, bool need_parsing)
    {
        int out_value = 0;
        if (need_parsing == false)
        {
            if (field_type == _type_opt_text)
            {
                _option_text_value = field_value;
            }
        }
        else
        {
            if (int.TryParse(field_value, out out_value))
            {
                if (field_type == _type_cost)
                {

                    _cost_value = out_value;
                }
                if (field_type == _type_jump_to)
                {
                    if (out_value > 0 && out_value <= s_target._id_action_total)
                    {
                        _jump_to_value = out_value;
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Notice", "Action ID out of range", "OK"))
                        {
                            _jump_to_field.value = "";
                            Repaint();
                        }
                    }
                }
                if (field_type == _type_item_id)
                {
                    _item_id_value = out_value;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Incorrect value", "OK");
                if (field_type == _type_cost)
                {
                    _cost_value_field.value = "";

                }
                if (field_type == _type_jump_to)
                {
                    _jump_to_field.value = "";
                }
                if (field_type == _type_item_id)
                {
                    _item_id_field.value = "";
                }
                Repaint();
            }
        }
    }
    private void Create_option()
    {

        if (_cost_value_field.value != "" && _jump_to_field.value != "" && _item_id_field.value != "" && _option_text_field.value != "")
        {
            if (_d_currency_type.value == _currency_choices[0])
            {
                _cost_value = 0;
                _cost_value_field.value = "0";
                s_target.Create_choise_option(_d_currency_type.value, _cost_value, _jump_to_value, _item_id_value, _option_text_value);
            }
            else
            {
                if (_cost_value != 0)
                {
                    s_target.Create_choise_option(_d_currency_type.value, _cost_value, _jump_to_value, _item_id_value, _option_text_value);
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Option type 'Paid', cost value cant be '0'", "OK");
                }
            }

        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Fill all fields", "OK");
        }
    }
    private void Delete_option(int selected_id)
    {
        s_target.Delete_choise_option(selected_id);
    }

}
