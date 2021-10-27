using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorylineEditor;
using System;

public class StrEditorEvents : MonoBehaviour
{
    public delegate void OnStrEditorUpdated();
    public event OnStrEditorUpdated StrEditorUpdated;

    public delegate void OnChoiseOptionCreated(StrChoiseOption choiseOption);
    public event OnChoiseOptionCreated StrEditorChoiseOptionCreated;

    public delegate StrChoiseOption OnChoiseOptionRequested(int choiseOptionIndex);
    public event OnChoiseOptionRequested StrEditorChoiseOptionRequested;
    
 
    public void EditorUpdated()
    {
        StrEditorUpdated?.Invoke();
    }
    public void CreateChoiseOption(StrChoiseOption choiseOption)
    {
        StrEditorChoiseOptionCreated?.Invoke(choiseOption);
            }
   
 
}

namespace StorylineEditor
{
    public class StrPreviewElementType
    {
        private string _typeIndex;
        private string _typeAssociatedFolder;


        public StrPreviewElementType(string index, string associatedFolder)
        {
            _typeIndex = index;
            _typeAssociatedFolder = associatedFolder;
        }
        public string GetFieldTypeIndex()
        {
            return _typeIndex;
        }
        public string GetFieldTypeAssociatedFolder()
        {
            return _typeAssociatedFolder;
        }
        public static StrPreviewElementType Body = new StrPreviewElementType("PreviewElementBody", "Char_body");
        public static StrPreviewElementType Clothes = new StrPreviewElementType("PreviewElementClothes", "Char_clothes");
        public static StrPreviewElementType Haircut = new StrPreviewElementType("PreviewElementHaircut", "Char_haircut");
        public static StrPreviewElementType Makeup = new StrPreviewElementType("PreviewElementMakeup", "Char_makeup");
    }
    public class StrFieldType
    {
        private string _typeIndex;
        public StrFieldType(string index)
        {
            _typeIndex = index;
        }
        public string GetFieldTypeIndex()
        {
            return _typeIndex;
        }
        public static StrFieldType RuntimeNameField = new StrFieldType("FieldTypeRuntimeName");
        public static StrFieldType TechNameField = new StrFieldType("FieldTypeTechName");
        public static StrFieldType CostField = new StrFieldType("FieldTypeCost");
        public static StrFieldType JumpToField = new StrFieldType("FieldTypeJumpTo");
        public static StrFieldType OptionTextField = new StrFieldType("FieldTypeOptionText");
        public static StrFieldType ItemIDField = new StrFieldType("FieldTypeItemId");
        public static StrFieldType CharacterDescriptionField = new StrFieldType("FieldTypeCharacterDescription");
    }
    public class StrListDirection
    {
        private string _typeIndex;
        public StrListDirection(string index)
        {
            _typeIndex = index;
        }
        public static StrListDirection Up = new StrListDirection("DirectionUp");
        public static StrListDirection Down = new StrListDirection("DirectionDown");

    }
    public struct StrConstantValues
    {
        public const string PlaceholderText = "----";
    }

    public struct StrUXMLElementsNames
    {
        public const string PreviewHolder = "previewHolder";
        public const string StatusLabel = "labelStatus";
    }
    public struct StrChoiseOption
    {
        public string CurrencyType;
        public int CostValue;
        public int JumpToActionID;
        public int GivedItemID;
        public string OptionText;
    }
}
