using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using StorylineEditor;


public class StrEditorChoiseConstructorWindow : EditorWindow
{
    private ext_StorylineEditor _s_StorylineEditor;
    private ext_StorylineEventSystem _s_StrEvent;
    private int _costFieldValue;
    private string _optionTextFieldValue;
    private int _jumToActionFieldValue;
    private int _itemIDFieldValue;
    private List<string> _currencyDropdownChoises = new List<string> { "Free", "Diamonds", "Hearts" };

    private TextField _costField;
    private TextField _jumpToActionField;
    private TextField _itemIDField;
    private TextField _optionTextField;
    private DropdownField _currencyDropdown;
        
    public static StrEditorChoiseConstructorWindow ShowWindow()
    {
        StrEditorChoiseConstructorWindow window_choise = GetWindow<StrEditorChoiseConstructorWindow>();
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

        for (int i = 0; i < _s_StorylineEditor._choiseOptions.Count; i++)
            if (_s_StorylineEditor._choiseOptions[i] != null)
            {
                items.Add(_s_StorylineEditor._choiseOptions[i]);
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

        _costField = new TextField();
        _costField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_costField.value, StrFieldType.Cost, true));

        _optionTextField = new TextField();
        _optionTextField.style.height = 20f;
        _optionTextField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_optionTextField.value, StrFieldType.OptionText, false));

        _jumpToActionField = new TextField();
        _jumpToActionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_jumpToActionField.value, StrFieldType.JumpTo, true));

        _itemIDField = new TextField();
        _itemIDField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_itemIDField.value, StrFieldType.ItemID, true));

        _currencyDropdown = new DropdownField("", _currencyDropdownChoises, 0);

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
                        _s_StorylineEditor.ChangeChoiseOptionPosition(selected_option_id, StrListDirection.Up);
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
                    if ((selected_option_id + 1) != _s_StorylineEditor._choiseOptions.Count)
                    {
                        _s_StorylineEditor.ChangeChoiseOptionPosition(selected_option_id, StrListDirection.Down);

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
        VTuxml.Q<VisualElement>("cost_fieldHolder").Add(_costField);
        VTuxml.Q<VisualElement>("options_text_Holder").Add(_optionTextField);
        VTuxml.Q<VisualElement>("jump_fieldHolder").Add(_jumpToActionField);
        VTuxml.Q<VisualElement>("give_fieldHolder").Add(_itemIDField);
        VTuxml.Q<VisualElement>("currency_DropHolder").Add(_currencyDropdown);
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
                _optionTextFieldValue = FieldValue;
            }
        }
        else
        {
            if (int.TryParse(FieldValue, out out_value))
            {
                if (FieldType == StrFieldType.Cost)
                {

                    _costFieldValue = out_value;
                }
                if (FieldType == StrFieldType.JumpTo)
                {
                    if (out_value > 0 && out_value <= _s_StorylineEditor._actionsTotalID)
                    {
                        _jumToActionFieldValue = out_value;
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Notice", "Action ID out of range", "OK"))
                        {
                            _jumpToActionField.value = "";
                            Repaint();
                        }
                    }
                }
                if (FieldType == StrFieldType.ItemID)
                {
                    _itemIDFieldValue = out_value;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Incorrect value", "OK");
                if (FieldType == StrFieldType.Cost)
                {
                    _costField.value = "";

                }
                if (FieldType == StrFieldType.JumpTo)
                {
                    _jumpToActionField.value = "";
                }
                if (FieldType == StrFieldType.ItemID)
                {
                    _itemIDField.value = "";
                }
                Repaint();
            }
        }
    }
    private void CreateOption()
    {

        if (_costField.value != "" && _jumpToActionField.value != "" && _itemIDField.value != "" && _optionTextField.value != "")
        {
            if (_currencyDropdown.value == _currencyDropdownChoises[0])
            {
                _costFieldValue = 0;
                _costField.value = "0";
                _s_StorylineEditor.CreateChoiseOption(_currencyDropdown.value, _costFieldValue, _jumToActionFieldValue, _itemIDFieldValue, _optionTextFieldValue);
            }
            else
            {
                if (_costFieldValue != 0)
                {
                    _s_StorylineEditor.CreateChoiseOption(_currencyDropdown.value, _costFieldValue, _jumToActionFieldValue, _itemIDFieldValue, _optionTextFieldValue);
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
