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
    private List<string> _choiseOptions = new List<string>();
    private VisualElement _choiseConstructorMainVE;
    private VisualTreeAsset _choiseConstructorVTAsset;
    private VisualElement _choiseOptionListviewItemVE;
    private VisualTreeAsset _choiseOptionListviewItemVTAsset;
    private StrEditorGodObject StrEditorRoot;
    private StrEditorEvents StrEvents;
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
    private Button _addOptionButton;
    private Button _deleteOptionButton;
    private Button _addOptionToActionButton;
    private Button _moveOptionUpButton;
    private Button _moveOptionDownButton;
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
        SetupStrEventsComponent();
        StrEvents.StrEditorUpdated += OnStrEdUpdated;
        StrEvents.StrEditorRootObjectDeclared += OnStrEditorRootObjectDeclared;
        StrEvents.RequestStrEditorRootObject();
    }
    private void SetupStrEventsComponent()
    {
        StrEditorEvents tempStrEvents = (StrEditorEvents)FindObjectOfType(typeof(StrEditorEvents));
        if (tempStrEvents is IStrEventSystem)
        {
            StrEvents = tempStrEvents;
        }
        else
        {
            throw new ArgumentException("'StrEditorEvents' must implement the 'IStrEventSystem' interface");
        }
    }
    private void OnStrEditorRootObjectDeclared(StrEditorGodObject StrEditorRootObject)
    {
        StrEditorRoot = StrEditorRootObject;
    }
    private void OnStrEdUpdated()
    {
        CreateGUI();
    }
    public void CreateGUI()
    {
        InstantiateMainVisualElement();
        InstatiateChoiseOptionsListviewItemVE();
        SetChoiseOptionsList();
        InstatiateTextFields();
        RegisterTextFieldsCallback();
        InstatiateDropDownFields();
        InstatiateChoiseOptionsListview();
        InstatiateButtons();
        AddInstatiatedUIElementsToMainVE();
    }
    private void InstantiateMainVisualElement()
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
        }
    }
    private void InstatiateChoiseOptionsListviewItemVE()
    {
        try
        {
            _choiseOptionListviewItemVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/ChoiseOptionTemplate.uxml");
            _choiseOptionListviewItemVE = _choiseOptionListviewItemVTAsset.Instantiate();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    private void SetChoiseOptionsList()
    {
        _choiseOptions = StrEditorRoot.GetChoiseOptionsList();
    }
    private void InstatiateTextFields()
    {
        _costField = new TextField();
        _optionTextField = new TextField();
        _optionTextField.style.height = 20f;
        _jumpToActionField = new TextField();
        _itemIDField = new TextField();
    }
    private void RegisterTextFieldsCallback()
    {
        _costField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_costField.value, StrFieldType.CostField, true));
        _optionTextField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_optionTextField.value, StrFieldType.OptionTextField, false));
        _jumpToActionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_jumpToActionField.value, StrFieldType.JumpToField, true));
        _itemIDField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetValue(_itemIDField.value, StrFieldType.ItemIDField, true));
    }
    private void InstatiateButtons()
    {
        if (ValidateStoryline())
        {
            _addOptionButton = new Button(() => CreateOption());
            _deleteOptionButton = new Button(() => DeleteOption());
            _addOptionToActionButton = new Button(() => StrEvents.EditorUpdated());
            _moveOptionUpButton = new Button(() => MoveChoiseOption(StrListDirection.Up));
            _moveOptionDownButton = new Button(() => MoveChoiseOption(StrListDirection.Down));
            _addOptionButton.text = "Add option";
            _deleteOptionButton.text = "Delete selected option";
            _addOptionToActionButton.text = "Add choise to Action";
            _moveOptionUpButton.text = "Move up";
            _moveOptionDownButton.text = "Move down";
        }
    }
    private void InstatiateDropDownFields()
    {
        _currencyDropdown = new DropdownField("", _currencyDropdownChoises, 0);
    }
    private void InstatiateChoiseOptionsListview()
    {
        List<string> choiseOptionsListviewItems = SetupChoiseOptionsListviewItemsList();
        Func<VisualElement> makeItem = () => _choiseOptionListviewItemVTAsset.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            string[] decomposedChoiseOption = DecomposeChoiseOption(i);
            SetupUIElementsOfChoiseOptionTemplate(e, decomposedChoiseOption);
        };
        _optionsListview = new ListView(choiseOptionsListviewItems, StrConstantValues.StandartListviewItemHeight, makeItem, bindItem);
        _optionsListview.selectionType = SelectionType.Single;
        _optionsListview.style.flexGrow = 1.0f;
        _optionsListview.onItemsChosen += obj =>
        {

        };
        _optionsListview.onSelectionChange += objects =>
        {

        };
    }
    private List<string> SetupChoiseOptionsListviewItemsList()
    {
        List<string> itemsList = new List<string>();
        if (_choiseOptions.Count != 0)
        {
            foreach (string choiseOption in _choiseOptions)
            {
                itemsList.Add(choiseOption);
            }
        }
        return itemsList;
    }
    private string[] DecomposeChoiseOption(int OptionIndex)
    {
        string[] decomposedChoiseOption = _choiseOptions[OptionIndex].Split(StrEditorRoot._s_Tag._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        return decomposedChoiseOption;
    }
    private void SetupUIElementsOfChoiseOptionTemplate(VisualElement choiseOptionTemplateVE, string[] decomposedOption)
    {
        (choiseOptionTemplateVE.Q<VisualElement>("number") as Label).text = "¹: ";
        (choiseOptionTemplateVE.Q<VisualElement>("currency_type") as Label).text = "Currency type: ";
        (choiseOptionTemplateVE.Q<VisualElement>("cost") as Label).text = "Cost: ";
        (choiseOptionTemplateVE.Q<VisualElement>("jump_to") as Label).text = "Jump to: ";
        (choiseOptionTemplateVE.Q<VisualElement>("item_ID") as Label).text = "Item ID: ";
        (choiseOptionTemplateVE.Q<VisualElement>("status_number") as Label).text = decomposedOption[0];
        (choiseOptionTemplateVE.Q<VisualElement>("status_currency_type") as Label).text = decomposedOption[1];
        (choiseOptionTemplateVE.Q<VisualElement>("status_cost") as Label).text = decomposedOption[2];
        (choiseOptionTemplateVE.Q<VisualElement>("status_jump_to") as Label).text = decomposedOption[3];
        (choiseOptionTemplateVE.Q<VisualElement>("status_itemID") as Label).text = decomposedOption[4];
        (choiseOptionTemplateVE.Q<VisualElement>("status_opt_text") as Label).text = decomposedOption[5];
    }
    private void AddInstatiatedUIElementsToMainVE()
    {
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder1").Add(_addOptionButton);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder2").Add(_deleteOptionButton);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder3").Add(_addOptionToActionButton);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder4").Add(_moveOptionUpButton);
        _choiseConstructorMainVE.Q<VisualElement>("buttonHolder5").Add(_moveOptionDownButton);
        _choiseConstructorMainVE.Q<VisualElement>("cost_fieldHolder").Add(_costField);
        _choiseConstructorMainVE.Q<VisualElement>("options_text_Holder").Add(_optionTextField);
        _choiseConstructorMainVE.Q<VisualElement>("jump_fieldHolder").Add(_jumpToActionField);
        _choiseConstructorMainVE.Q<VisualElement>("give_fieldHolder").Add(_itemIDField);
        _choiseConstructorMainVE.Q<VisualElement>("currency_DropHolder").Add(_currencyDropdown);
        _choiseConstructorMainVE.Q<VisualElement>("options_list_Holder").Add(_optionsListview);
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
                    if (out_value > 0 && out_value <= StrEditorRoot._actionsTotalID)
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
                StrEditorRoot.CreateChoiseOption(choiseOption);
                StrEvents.EditorUpdated();
            }
            else
            {
                if (_costFieldValue != 0)
                {
                    StrChoiseOption choiseOption = SetChoiseOptionValues();
                    StrEditorRoot.CreateChoiseOption(choiseOption);
                    StrEvents.EditorUpdated();
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
        StrEvents.EditorUpdated();
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
    private void DeleteOption()
    {
        if (_optionsListview.selectedItem != null)
        {
            StrEditorRoot.DeleteChoiseOption(_optionsListview.selectedIndex);
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
        }
        StrEvents.EditorUpdated();
    }
    private void MoveChoiseOption(StrListDirection moveDirection)
    {
        if (_optionsListview.selectedItem != null)
        {
            if (moveDirection == StrListDirection.Up)
            {
                if (_optionsListview.selectedIndex != 0)
                {
                    StrEditorRoot.ChangeChoiseOptionPosition(_optionsListview.selectedIndex, StrListDirection.Up);
                    _optionsListview.selectedIndex = _optionsListview.selectedIndex - 1;
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Option already on top", "OK");
                }
            }
            if (moveDirection == StrListDirection.Down)
            {
                int selected_option_id = _optionsListview.selectedIndex;
                if ((_optionsListview.selectedIndex + 1) != _choiseOptions.Count)
                {
                    StrEditorRoot.ChangeChoiseOptionPosition(_optionsListview.selectedIndex, StrListDirection.Down);
                    _optionsListview.selectedIndex = _optionsListview.selectedIndex + 1;
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Option already on top", "OK");
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Select option first", "OK");
        }
        StrEvents.EditorUpdated();
    }
    private Boolean ValidateStoryline()
    {
        if (StrEditorRoot.CheckStorylineExistence(StrEditorRoot._StorylineName))
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
        StrEvents.StrEditorUpdated -= OnStrEdUpdated;

    }
}
