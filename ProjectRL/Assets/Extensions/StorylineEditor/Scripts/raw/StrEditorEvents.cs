using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorylineEditor;
using System;
using UnityEngine.UI;
[RequireComponent(typeof(StrEditorGodObject))]
[ExecuteInEditMode]
public class StrEditorEvents : MonoBehaviour, IStrEventSystem
{
    public delegate void OnStrEditorUpdated();
    public event OnStrEditorUpdated StrEditorUpdated;
    public delegate void OnStrEditorRootObjectRequested();
    public event OnStrEditorRootObjectRequested StrEditorRootObjectRequested;
    public delegate void OnStrEditorRootObjectDeclared(StrEditorGodObject StrEditorRootObject);
    public event OnStrEditorRootObjectDeclared StrEditorRootObjectDeclared;
    public void EditorUpdated()
    {
        StrEditorUpdated?.Invoke();
    }
    public void RequestStrEditorRootObject()
    {
        StrEditorRootObjectRequested?.Invoke();
    }
    public void DeclareStrEditorRootObject(StrEditorGodObject StrEditorRootObject)
    {
        if (StrEditorRootObject is IStrEditorRoot)
        {
            StrEditorRootObjectDeclared?.Invoke(StrEditorRootObject);
        }
        else
        {
            throw new ArgumentException("'StrEditorRootObject' must implement the 'IStrEditor' interface");
        }
    }
}

namespace StorylineEditor
{

    interface IStrEditorRoot
    {
        public Boolean ValidateStoryline();
        public List<string> GetChoiseOptionsList();
        public void CreateChoiseOption(StrChoiseOption choiseOption);
        public void DeleteChoiseOption(int optionIndex);
        public void ChangeChoiseOptionPosition(int optionIndex, StrListDirection direction);
        public void RenumberChoiseOptionsList();
    }
    public interface IStrEventSystem
    {

        public void DeclareStrEditorRootObject(StrEditorGodObject StrEditorRootObject);
    }

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
        public const string StrFileStepGap = "          ";
        public const int StandartListviewItemHeight = 30;
        public const int StepComposeStagesCount = 5;
        public const int ActionComposeStagesCount = 3;

    }
    public struct StrExtensions
    {
        public const string FinalStr = ".str";
        public const string RawStr = ".rsrtr";
        public const string Key = ".strk";
        public const string iv = ".striv";
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
    public struct StrStorylineParameters
    {
        public string User;
        public float Version;
        public int ActionID;
        public int StepID;
        public string Phrase;
        public string PhraseAuthor;
        public Image CGImage;
        public RectTransform CGRectTransform;
        public List<GameObject> ActiveCharacters;
        public List<RectTransform> ActiveRectTransforms;
        public List<string> ActivatedCharacters;
        public List<string> InactivatedCharacters;
        public List<string> StepsOfCurrentAction;
        public List<GameObject> RequiredObjects;
        public List<Sprite> RequiredCG;
        public List<string> ChoiseOptions;
    }
    public struct StrUncomposedStorylineParts
    {
        public List<string> InitPart;
        public List<string> StorylineActions;
    }
}
