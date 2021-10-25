using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrEditorEvents : MonoBehaviour
{
    public delegate void OnStrEditorUpdated();
    public event OnStrEditorUpdated StrEditorUpdated;

    public void EditorUpdated()
    {
        StrEditorUpdated?.Invoke();

    }
}

namespace StorylineEditor
{
    public class StrPreviewElementType
    {
        private string _typeIndex;
        public StrPreviewElementType(string index)
        {
            _typeIndex = index;
                }
        public static StrPreviewElementType Body = new StrPreviewElementType("type_body");
        public static StrPreviewElementType Clothes = new StrPreviewElementType("type_clothes");
        public static StrPreviewElementType Haircut = new StrPreviewElementType("type_haircut");
        public static StrPreviewElementType Makeup = new StrPreviewElementType("type_makeup");
    }
    public class StrFieldType
    {
        private string _typeIndex;
        public StrFieldType(string index)
        {
            _typeIndex = index;
        }
        public static StrFieldType RuntimeNameField = new StrFieldType("type_runtime_name_field");
        public static StrFieldType TechNameField = new StrFieldType("type_tech_name_field");
        public static StrFieldType CostField = new StrFieldType("type_cost_field");
        public static StrFieldType JumpToField = new StrFieldType("type_jump_to_field");
        public static StrFieldType OptionTextField = new StrFieldType("type_option_text_field");
        public static StrFieldType ItemIDField = new StrFieldType("type_item_id_field");
        public static StrFieldType CharacterDescriptionField = new StrFieldType("type_character_description_field");
        
    }
    public class StrListDirection
    {
        private string _typeIndex;
        public StrListDirection(string index)
        {
            _typeIndex = index;
        }
        public static StrListDirection Up = new StrListDirection("direction_up");
        public static StrListDirection Down = new StrListDirection("direction_down");

    }

    public  class StrPreviewElementsFolders
    {
        private static Dictionary<StrPreviewElementType, string> _elementsAndFolderInterrelation;
        public StrPreviewElementsFolders()
        {
            _elementsAndFolderInterrelation = new Dictionary<StrPreviewElementType, string>
            {
                [StrPreviewElementType.Body] = "Char_body",
                [StrPreviewElementType.Clothes] = "Char_clothes",
                [StrPreviewElementType.Haircut] = "Char_haircut",
                [StrPreviewElementType.Makeup] = "Char_makeup"
            };
        }
        public static string GetPreviewElementFolder(StrPreviewElementType key)
        {
            string folderName = _elementsAndFolderInterrelation[key];
            return folderName;
        }
        public static StrPreviewElementsFolders PreviewFolders = new StrPreviewElementsFolders();

    }
}
