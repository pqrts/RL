using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using StorylineEditor;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class StrEditorJumpMarkerWindow : EditorWindow
{
    private StrEditorGodObject StrEditorRoot;
    private StrEditorEvents StrEvents;
    private VisualElement _JumpMarkerVE;
    private VisualTreeAsset _JumpMarkerVTAsset;
    private TextField _jumpToActionField;
    private Button _createMarker;
    private int _jumpFieldValue;
    public static StrEditorJumpMarkerWindow ShowWindow()
    {
        StrEditorJumpMarkerWindow window_jump_marker = GetWindow<StrEditorJumpMarkerWindow>();
        window_jump_marker.titleContent = new GUIContent("Jump to");
        window_jump_marker.minSize = new Vector2(170, 60f);
        window_jump_marker.maxSize = new Vector2(170f, 60f);
        return window_jump_marker;
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
        InstatiateTextFields();
        RegisterTextFieldsCallback();
        InstatiateButtons();
        AddInstatiatedUIElementsToMainVE();



    }
    private void InstantiateMainVisualElement()
    {
        try
        {
            _JumpMarkerVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Extensions/StorylineEditor/UXML/JumpMarkerWindow.uxml");
            _JumpMarkerVE = _JumpMarkerVTAsset.Instantiate();
            rootVisualElement.Add(_JumpMarkerVE);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    private void InstatiateTextFields()
    {
        _jumpToActionField = new TextField();
    }
    private void RegisterTextFieldsCallback()
    {

        _jumpToActionField.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(e => SetJumpFieldValue(_jumpToActionField.value));

    }
    private void InstatiateButtons()
    {
       
       
        if (ValidateStoryline())
        {
            _createMarker = new Button(() => CreateJumpMarker());
            _createMarker.text = "Create Marker";
        }
       
    }
    private void AddInstatiatedUIElementsToMainVE()
    {
        _JumpMarkerVE.Q<VisualElement>("jump_fieldHolder").Add(_jumpToActionField);
        _JumpMarkerVE.Q<VisualElement>("buttonHolder1").Add(_createMarker);
    }
        private void SetJumpFieldValue(string fieldValue)
    {
        int out_value = 0;
        if (int.TryParse(fieldValue, out out_value))
        {
            if (out_value > 0 && out_value <= StrEditorRoot._totalActions)
            {
                _jumpFieldValue = out_value;
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
        else
        {
            EditorUtility.DisplayDialog("Notice", "Incorrect value", "OK");
            _jumpToActionField.value = "";
        }
    }
    private void CreateJumpMarker()
    {
        if (_jumpToActionField.value != "")
        {
            StrEditorRoot.CreateJumpMarker(_jumpFieldValue);
        }
        else 
        { 
        EditorUtility.DisplayDialog("Notice", "Fill field", "OK");

        }
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
        StrEvents.StrEditorRootObjectDeclared -= OnStrEditorRootObjectDeclared;
    }
}
