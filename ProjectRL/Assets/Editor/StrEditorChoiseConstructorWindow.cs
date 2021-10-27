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
    private VisualElement _choiseConstructorMainVE;
    private VisualTreeAsset _choiseConstructorVTAsset;

    private VisualElement _choiseOptionListviewItemVE;
    private VisualTreeAsset _choiseOptionListviewItemVTAsset;

    private ext_StorylineEditor _s_StorylineEditor;
    private StrEditorEvents _s_StrEvent;
    private int _costFieldValue;
    private string _optionTextFieldValue;
    private int _jumpToActionFieldValue;
    private int _itemIDFieldValue;
    private List<string> _currencyDropdownChoises = new List<string> { "Free", "Diamonds", "Hearts" };

    private ListView _optionsListview;
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
        _s_StrEvent = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        _s_StorylineEditor = (ext_StorylineEditor)FindObjectOfType(typeof(ext_StorylineEditor));
        _s_StrEvent.StrEditorUpdated += OnStrEdUpdated;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
        Repaint();
    }
    public void CreateGUI()
    {
        if (InstantiateMainVisualElement())
        {
            if (InstatiateChoiseOptionsListviewItemVE())
            {
                InstatiateChoiseOptionsListview();
            }
        }



        /// optionslist setup


        _costField = new TextField();
        _costField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_costField.value, StrFieldType.CostField, true));

        _optionTextField = new TextField();
        _optionTextField.style.height = 20f;
        _optionTextField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_optionTextField.value, StrFieldType.OptionTextField, false));

        _jumpToActionField = new TextField();
        _jumpToActionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_jumpToActionField.value, StrFieldType.JumpToField, true));

        _itemIDField = new TextField();
        _itemIDField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_itemIDField.value, StrFieldType.ItemIDField, true));

        _currencyDropdown = new DropdownField("", _currencyDropdownChoises, 0);

        Label _l_Options = _choiseConstructorMainVE.Q<VisualElement>("options") as Label;
        _l_Options.text = "Options: ";

        Label _l_Currency = _choiseConstructorMainVE.Q<VisualElement>("currency") as Label;
        _l_Currency.text = "Currency type: ";

        Label _l_Cost = _choiseConstructorMainVE.Q<VisualElement>("cost") as Label;
        _l_Cost.text = "Cost: ";

        Label _l_CostValue = _choiseConstructorMainVE.Q<VisualElement>("cost_label") as Label;
        _l_CostValue.text = "Cost: ";

        Label _l_OptionText = _choiseConstructorMainVE.Q<VisualElement>("opt_text") as Label;
        _l_OptionText.text = "Text: ";

        Label _l_JumpToAction = _choiseConstructorMainVE.Q<VisualElement>("jump_to") as Label;
        _l_JumpToAction.text = "Jump to action: ";

        Label _l_JumpActionNumber = _choiseConstructorMainVE.Q<VisualElement>("jump_label") as Label;
        _l_JumpActionNumber.text = "Action ¹: ";

        Label _l_GiveItem = _choiseConstructorMainVE.Q<VisualElement>("give_item") as Label;
        _l_GiveItem.text = "Give item: ";

        Label _l_ItemID = _choiseConstructorMainVE.Q<VisualElement>("give_label") as Label;
        _l_ItemID.text = "Item ID: ";

        Label _l_EditingOptions = _choiseConstructorMainVE.Q<VisualElement>("edit_opt") as Label;
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
                if (_optionsListview.selectedItem != null)
                {
                    DeleteOption(_optionsListview.selectedIndex);
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
                if (_optionsListview.selectedItem != null)
                {
                    int selected_option_id = _optionsListview.selectedIndex;

                    if (selected_option_id != 0)
                    {
                        _s_StorylineEditor.ChangeChoiseOptionPosition(selected_option_id, StrListDirection.Up);
                        _optionsListview.selectedIndex = selected_option_id - 1;
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
                if (_optionsListview.selectedItem != null)
                {
                    int selected_option_id = _optionsListview.selectedIndex;
                    if ((selected_option_id + 1) != _s_StorylineEditor._choiseOptions.Count)
                    {
                        _s_StorylineEditor.ChangeChoiseOptionPosition(selected_option_id, StrListDirection.Down);

                        _optionsListview.selectedIndex = selected_option_id + 1;
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

        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder1").Add(_b_AddOption);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder2").Add(_b_DeleteOption);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder3").Add(_b_AddOptionToAction);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder4").Add(_b_MoveOptionUp);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder5").Add(_b_MoveOptionDown);
        _choiseConstructorMainVE.Q<VisualElement>("cost_fieldHolder").Add(_costField);
        _choiseConstructorMainVE.Q<VisualElement>("options_text_Holder").Add(_optionTextField);
        _choiseConstructorMainVE.Q<VisualElement>("jump_fieldHolder").Add(_jumpToActionField);
        _choiseConstructorMainVE.Q<VisualElement>("give_fieldHolder").Add(_itemIDField);
        _choiseConstructorMainVE.Q<VisualElement>("currency_DropHolder").Add(_currencyDropdown);
        _choiseConstructorMainVE.Q<VisualElement>("options_list_Holder").Add(_optionsListview);
        rootVisualElement.Add(_choiseConstructorMainVE);

    }
    private Boolean InstantiateMainVisualElement()
    {
        try
        {
            _choiseConstructorVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/ChoiseConstructorWindow.uxml");
            _choiseConstructorMainVE = _choiseConstructorVTAsset.Instantiate();
            rootVisualElement.Add(_choiseConstructorMainVE);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
        return true;
    }
    private Boolean InstatiateChoiseOptionsListviewItemVE()
    {
        try
        {
            _choiseOptionListviewItemVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ChoiseOptionTemplate.uxml");
            _choiseOptionListviewItemVE = _choiseOptionListviewItemVTAsset.Instantiate();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
        return true;
    }
    private void InstatiateChoiseOptionsListview()
    {
        var choiseOptionsListviewItems = new List<string>();

        for (int i = 0; i < _s_StorylineEditor._choiseOptions.Count; i++)
            if (_s_StorylineEditor._choiseOptions[i] != null)
            {
                choiseOptionsListviewItems.Add(_s_StorylineEditor._choiseOptions[i]);
            }
        Func<VisualElement> makeItem = () => _choiseOptionListviewItemVTAsset.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            string[] _option = _s_StorylineEditor.GetChoiseOption(i);

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
        const int ItemHeight = 30;
        _optionsListview = new ListView(choiseOptionsListviewItems, ItemHeight, makeItem, bindItem);
        _optionsListview.selectionType = SelectionType.Single;
        _optionsListview.onItemsChosen += obj =>
        {

        };
        _optionsListview.onSelectionChange += objects =>
        {
        };
        _optionsListview.style.flexGrow = 1.0f;
    }
    private void SetValue(string FieldValue, StrFieldType FieldType, bool NeedParsing)
    {
        int out_value = 0;
        if (NeedParsing == false)
        {
            if (FieldType == StrFieldType.OptionTextField)
            {
                _optionTextFieldValue = FieldValue;
            }
        }
        else
        {
            if (int.TryParse(FieldValue, out out_value))
            {
                if (FieldType == StrFieldType.CostField)
                {

                    _costFieldValue = out_value;
                }
                if (FieldType == StrFieldType.JumpToField)
                {
                    if (out_value > 0 && out_value <= _s_StorylineEditor._actionsTotalID)
                    {
                        _jumpToActionFieldValue = out_value;
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
                if (FieldType == StrFieldType.ItemIDField)
                {
                    _itemIDFieldValue = out_value;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Incorrect value", "OK");
                if (FieldType == StrFieldType.CostField)
                {
                    _costField.value = "";

                }
                if (FieldType == StrFieldType.JumpToField)
                {
                    _jumpToActionField.value = "";
                }
                if (FieldType == StrFieldType.ItemIDField)
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
                StrChoiseOption choiseOption = SetChoiseOptionValues();
                _s_StrEvent.CreateChoiseOption(choiseOption);
                _s_StrEvent.EditorUpdated();
            }
            else
            {
                if (_costFieldValue != 0)
                {
                    StrChoiseOption choiseOption = SetChoiseOptionValues();
                    _s_StrEvent.CreateChoiseOption(choiseOption);
                    _s_StrEvent.EditorUpdated();
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
    private StrChoiseOption SetChoiseOptionValues()
    {
        StrChoiseOption option = new StrChoiseOption();
        option.CurrencyType = _currencyDropdown.value;
        option.CostValue = _costFieldValue;
        option.JumpToActionID = _jumpToActionFieldValue;
        option.GivedItemID = _itemIDFieldValue;
        option.OptionText = _optionTextFieldValue;
        return option;
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
        _s_StrEvent.StrEditorUpdated -= OnStrEdUpdated;
    }
}
