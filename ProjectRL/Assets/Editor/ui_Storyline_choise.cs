using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using StorylineEditor;


public class ui_Storyline_choise : EditorWindow
{
    private ext_StorylineEditor _s_StorylineEditor;
    private ext_StorylineEventSystem _s_StrEvent;
    private int _value_Cost;
    private string _value_OptionText;
    private int _value_JumToAction;
    private int _value_ItemID;
    private List<string> _dropChoises_Currency = new List<string> { "Free", "Diamonds", "Hearts" };

    private TextField _field_CostValue;
    private TextField _field_JumpToAction;
    private TextField _field_ItemID;
    private TextField _field_OptionText;
    private DropdownField _drop_Currency;

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
        _s_StrEvent = (ext_StorylineEventSystem)FindObjectOfType(typeof(ext_StorylineEventSystem));
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent.OnStrEdUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
        Repaint();
    }
        public void CreateGUI()
    {

        var VT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/choise_constructor.uxml");
        VisualElement VTuxml = VT.Instantiate();

        var VTListview = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ChoiseOptionTemplate.uxml");
        VisualElement VTlistview_element = VTListview.Instantiate();
        /// optionslist setup
        var items = new List<string>();

        for (int i = 0; i < _s_StorylineEditor._list_ChoiseOptions.Count; i++)
            if (_s_StorylineEditor._list_ChoiseOptions[i] != null)
            {
                items.Add(_s_StorylineEditor._list_ChoiseOptions[i]);
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
            string[] _option = _s_StorylineEditor.GetChoiseOption(i);
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
        var _listview_Options = new ListView(items, itemHeight, makeItem, bindItem);

        _listview_Options.selectionType = SelectionType.Single;

        _listview_Options.onItemsChosen += obj =>
        {

        };
        _listview_Options.onSelectionChange += objects =>
        {
        };
        _listview_Options.style.flexGrow = 1.0f;

        _field_CostValue = new TextField();
        _field_CostValue.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_field_CostValue.value, StrFieldType.Cost, true));

        _field_OptionText = new TextField();
        _field_OptionText.style.height = 20f;
        _field_OptionText.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_field_OptionText.value, StrFieldType.OptionText, false));

        _field_JumpToAction = new TextField();
        _field_JumpToAction.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_field_JumpToAction.value, StrFieldType.JumpTo, true));

        _field_ItemID = new TextField();
        _field_ItemID.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_field_ItemID.value, StrFieldType.ItemID, true));

        _drop_Currency = new DropdownField("", _dropChoises_Currency, 0);

        Label _l_Options = VTuxml.Q<VisualElement>("options") as Label;
        _l_Options.text = "Options: ";

        Label _l_Currency = VTuxml.Q<VisualElement>("currency") as Label;
        _l_Currency.text = "Currency type: ";

        Label _l_Cost = VTuxml.Q<VisualElement>("cost") as Label;
        _l_Cost.text = "Cost: ";

        Label _l_CostValue = VTuxml.Q<VisualElement>("cost_label") as Label;
        _l_CostValue.text = "Cost: ";

        Label _l_OptionText = VTuxml.Q<VisualElement>("opt_text") as Label;
        _l_OptionText.text = "Text: ";

        Label _l_JumpToAction = VTuxml.Q<VisualElement>("jump_to") as Label;
        _l_JumpToAction.text = "Jump to action: ";

        Label _l_JumpActionNumber = VTuxml.Q<VisualElement>("jump_label") as Label;
        _l_JumpActionNumber.text = "Action ¹: ";

        Label _l_GiveItem = VTuxml.Q<VisualElement>("give_item") as Label;
        _l_GiveItem.text = "Give item: ";

        Label _l_ItemID = VTuxml.Q<VisualElement>("give_label") as Label;
        _l_ItemID.text = "Item ID: ";

        Label _l_EditingOptions = VTuxml.Q<VisualElement>("edit_opt") as Label;
        _l_EditingOptions.text = "Editing options: ";

        Button _b_AddOption = new Button(() =>
        {
            if (ValidateStoryline())
            {
                CreateOption();
                _s_StrEvent.EditorUpdated();
            }

        });
        _b_AddOption.text = "Add option";

        Button _b_DeleteOption = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_listview_Options.selectedItem != null)
                {
                    DeleteOption(_listview_Options.selectedIndex);
                    _s_StrEvent.EditorUpdated();
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
                }
            }

        });
        _b_DeleteOption.text = "Delete selected option";

        Button _b_AddOptionToAction = new Button(() =>
        {
            if (ValidateStoryline())
            {

                _s_StrEvent.EditorUpdated();
            }
        });
        _b_AddOptionToAction.text = "Add choise to Action";

        Button _b_MoveOptionUp = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_listview_Options.selectedItem != null)
                {
                    int selected_option_id = _listview_Options.selectedIndex;

                    if (selected_option_id != 0)
                    {
                        _s_StorylineEditor.MoveChoiseOption(selected_option_id, StrListDirection.Up);
                        _listview_Options.selectedIndex = selected_option_id - 1;
                                           }
                    else
                    {
                        EditorUtility.DisplayDialog("Notice", "Option already on top", "OK");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
                }
                _s_StrEvent.EditorUpdated();
            }
        });
        _b_MoveOptionUp.text = "Move up";

        Button _b_MoveOptionDown = new Button(() =>
        {
            if (ValidateStoryline())
            {
                if (_listview_Options.selectedItem != null)
                {
                    int selected_option_id = _listview_Options.selectedIndex;
                    if ((selected_option_id + 1) != _s_StorylineEditor._list_ChoiseOptions.Count)
                    {
                        _s_StorylineEditor.MoveChoiseOption(selected_option_id, StrListDirection.Down);

                        _listview_Options.selectedIndex = selected_option_id + 1;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Notice", "Option already on top", "OK");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
                }
                _s_StrEvent.EditorUpdated();
            }

        });
        _b_MoveOptionDown.text = "Move down";

        VTuxml.Q<VisualElement>("buttonHolder1").Add(_b_AddOption);
        VTuxml.Q<VisualElement>("buttonHolder2").Add(_b_DeleteOption);
        VTuxml.Q<VisualElement>("buttonHolder3").Add(_b_AddOptionToAction);
        VTuxml.Q<VisualElement>("buttonHolder4").Add(_b_MoveOptionUp);
        VTuxml.Q<VisualElement>("buttonHolder5").Add(_b_MoveOptionDown);
        VTuxml.Q<VisualElement>("cost_fieldHolder").Add(_field_CostValue);
        VTuxml.Q<VisualElement>("options_text_Holder").Add(_field_OptionText);
        VTuxml.Q<VisualElement>("jump_fieldHolder").Add(_field_JumpToAction);
        VTuxml.Q<VisualElement>("give_fieldHolder").Add(_field_ItemID);
        VTuxml.Q<VisualElement>("currency_DropHolder").Add(_drop_Currency);
        VTuxml.Q<VisualElement>("options_list_Holder").Add(_listview_Options);
        rootVisualElement.Add(VTuxml);
       
    }

    private void SetValue(string FieldValue, StrFieldType FieldType, bool NeedParsing)
    {
        int out_value = 0;
        if (NeedParsing == false)
        {
            if (FieldType == StrFieldType.OptionText)
            {
                _value_OptionText = FieldValue;
            }
        }
        else
        {
            if (int.TryParse(FieldValue, out out_value))
            {
                if (FieldType == StrFieldType.Cost)
                {

                    _value_Cost = out_value;
                }
                if (FieldType == StrFieldType.JumpTo)
                {
                    if (out_value > 0 && out_value <= _s_StorylineEditor._IDActionsTotal)
                    {
                        _value_JumToAction = out_value;
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Notice", "Action ID out of range", "OK"))
                        {
                            _field_JumpToAction.value = "";
                            Repaint();
                        }
                    }
                }
                if (FieldType == StrFieldType.ItemID)
                {
                    _value_ItemID = out_value;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Incorrect value", "OK");
                if (FieldType == StrFieldType.Cost)
                {
                    _field_CostValue.value = "";

                }
                if (FieldType == StrFieldType.JumpTo)
                {
                    _field_JumpToAction.value = "";
                }
                if (FieldType == StrFieldType.ItemID)
                {
                    _field_ItemID.value = "";
                }
                Repaint();
            }
        }
    }
    private void CreateOption()
    {

        if (_field_CostValue.value != "" && _field_JumpToAction.value != "" && _field_ItemID.value != "" && _field_OptionText.value != "")
        {
            if (_drop_Currency.value == _dropChoises_Currency[0])
            {
                _value_Cost = 0;
                _field_CostValue.value = "0";
                _s_StorylineEditor.CreateChoiseOption(_drop_Currency.value, _value_Cost, _value_JumToAction, _value_ItemID, _value_OptionText);
            }
            else
            {
                if (_value_Cost != 0)
                {
                    _s_StorylineEditor.CreateChoiseOption(_drop_Currency.value, _value_Cost, _value_JumToAction, _value_ItemID, _value_OptionText);
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
    private void DeleteOption(int SelectedOptionID)
    {
        _s_StorylineEditor.DeleteChoiseOption(SelectedOptionID);
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
