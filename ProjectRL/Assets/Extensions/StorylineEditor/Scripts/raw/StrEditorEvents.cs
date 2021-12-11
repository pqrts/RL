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
    public delegate void OnStrCGPositionChanged();
    public event OnStrCGPositionChanged StrCGPositionChanged;
    public delegate void OnStrEditorRootObjectRequested();
    public event OnStrEditorRootObjectRequested StrEditorRootObjectRequested;
    public delegate void OnStrEditorRootObjectDeclared(StrEditorGodObject StrEditorRootObject);
    public event OnStrEditorRootObjectDeclared StrEditorRootObjectDeclared;
    public void EditorUpdated()
    {
        StrEditorUpdated?.Invoke();
    }
    public void CGPositionChanged()
    {
        StrCGPositionChanged?.Invoke();
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

    public class StrCharacterElementType
    {
        private string _typeIndex;
        private string _typeAssociatedFolder;
        public StrCharacterElementType(string index, string associatedFolder)
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
        public static StrCharacterElementType Body = new StrCharacterElementType("PreviewElementBody", "Char_body");
        public static StrCharacterElementType Clothes = new StrCharacterElementType("PreviewElementClothes", "Char_clothes");
        public static StrCharacterElementType Haircut = new StrCharacterElementType("PreviewElementHaircut", "Char_haircut");
        public static StrCharacterElementType Makeup = new StrCharacterElementType("PreviewElementMakeup", "Char_makeup");
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
        public const float StandartCharacterHeight = 1280f;
        public const float StandartCharacterWidht = 720f;

    }
    public struct StrExtensions
    {
        public const string FinalStr = ".str";
        public const string RawStr = ".rstr";
        public const string Key = ".strk";
        public const string IV = ".striv";
        public const string Character = ".char";
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
        public bool IsPhraseHolderActive;
        public GameObject PhraseHolder;
        public Image CGImage;
        public RectTransform CGRectTransform;
        public List<GameObject> ActiveCharacters;
        public List<RectTransform> ActiveRectTransforms;
        public List<string> StepsOfCurrentAction;
        public List<GameObject> RequiredObjects;
        public List<Sprite> RequiredCG;
        public List<string> ChoiseOptions;
        public string JumpMarker;
    }
    public struct StrUncomposedStorylineParts
    {
        public List<string> InitPart;
        public List<string> StorylineActions;
    }
    public struct StrDecomposedAction
    {
        public string Phrase;
        public string PhraseAuthor;
        public string IsPhraseHolderActive;
        public Vector3 PhraseHolderPosition;
        public string CGImageName;
        public Vector3 CGPosition;
        public List<string> ActiveCharacters;
        public Dictionary<string, Vector3> ActiveCharactersPositions;
        public Dictionary<string, Vector2> ActiveCharactersScales;
        public List<string> ChoiseOptions;
        public string JumpToAction;
    }
    public struct StrCharacter
    {
        public string CharacterTechName;
        public string CharacterRuntimeName;
        public string CharacterDescription;
        public Image CharacterBody;
        public Image CharacterHaircut;
        public Image CharacterClothes;
        public Image CharacterMakeup;
    }
    public struct StrRawStr
    {
        public string User;
        public float Version;
        public int ActionID;
        public int StepID;
        public string Phrase;
        public string PhraseAuthor;
        public bool IsPhraseHolderActive;
        public Vector3 PhraseHolderPosition;
        public Vector3 CGPosition;
        public List<string> ActiveCharacters;
        public List<string> RequiredCharacters;
        public List<string> RequiredCGs;
        public List<string> ChoiseOptions;
        public string JumpMarker;
        public string StorylineName;
        public List<string> InitPart;
        public List<string> StorylineActions;
        public List<string> CurretActionSteps;
        public List<string> TotalStepsCount;
        public string CGSpriteName;
        public bool IsReadyForNextAction;
        public int RefereceResolutionWidht;
        public int TotalActions;
        public Dictionary<string, Vector3> CharactersPositions;
        public Dictionary<string, Vector2> CharactersScales;
    }
}
