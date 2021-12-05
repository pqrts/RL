using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using StorylineEditor;
[RequireComponent(typeof(global_folders))]
public class StrEditorCharacterSpawner : MonoBehaviour
{
    private global_folders _folders;
    private string _characterTechName;
    private string _characterRuntimeName;
    private string _characterDescription;
    private GameObject _characterGameObject;
    private Image _characterBody;
    private Image _characterHaircut;
    private Image _characterClothes;
    private Image _characterMakeup;
    private Dictionary<StrCharacterElementType, Image> _characterElements = new Dictionary<StrCharacterElementType, Image>();
    private string _bodyFileName;
    private string _haircutFileName;
    private string _clothesFileName;
    private string _makeupFileName;
    private StrEditorGodObject _StrEditorRoot;
    public Boolean GetRequieredComponents()
    {
        _folders = GetComponent<global_folders>();

        return true;
    }
    public GameObject Spawn(GameObject Canvas, string charactersPath, string characterName)
    {
        try
        {
            _characterElements.Clear();
            SetupElemetsDictionary();
            ReadFromFile(charactersPath);
            SetupCharacter(Canvas);
            StrCharacter StrCharacter = SetParametersStruct();
            _characterGameObject.GetComponent<Character>().SetCharacterParameters(StrCharacter);
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
        return _characterGameObject;
    }
    private void SetupCharacter(GameObject canvas)
    {
        for (int i = 0; i < _characterElements.Count; i++)
        {
            switch (i)
            {
                case 0:
                    string tempPathBody = ComposeElementResourcesPath(StrCharacterElementType.Body);
                    CreateCharacterElements(canvas, tempPathBody,_bodyFileName, StrCharacterElementType.Body);
                    break;
                case 1:
                    string tempPathClothes = ComposeElementResourcesPath(StrCharacterElementType.Clothes);
                    CreateCharacterElements(canvas, tempPathClothes,_clothesFileName, StrCharacterElementType.Clothes);
                    break;
                case 2:
                    string tempPathHaircut = ComposeElementResourcesPath(StrCharacterElementType.Haircut);
                    CreateCharacterElements(canvas, tempPathHaircut, _haircutFileName,StrCharacterElementType.Haircut);
                    break;
                case 3:
                    string tempPathMakeup = ComposeElementResourcesPath(StrCharacterElementType.Makeup);
                    CreateCharacterElements(canvas, tempPathMakeup,_makeupFileName, StrCharacterElementType.Makeup);
                    break;
            }
        }

    }
    private void ReadFromFile(string filePath)
    {
        StreamReader SR = new StreamReader(filePath, encoding: System.Text.Encoding.GetEncoding("windows-1251"));
        string line = SR.ReadLine();
        _characterTechName = line;

        int count = 1;
        while (line != null)
        {
            line = SR.ReadLine();
            switch (count)
            {
                case 1:
                    _characterRuntimeName = line;
                    count += 1;
                    break;
                case 2:
                    _characterDescription = line;
                    count += 1;
                    break;
                case 3:
                    _bodyFileName = line;
                    count += 1;
                    break;
                case 4:
                    _clothesFileName = line;
                    count += 1;
                    break;
                case 5:
                    _haircutFileName = line;
                    count += 1;
                    break;
                case 6:
                    _makeupFileName = line;
                    count += 1;
                    break;
            }
        }
        SR.Close();
    }
    private StrCharacter SetParametersStruct()
    {
        StrCharacter tempStrCharacter = new StrCharacter();
        tempStrCharacter.CharacterTechName = _characterTechName;
        tempStrCharacter.CharacterRuntimeName = _characterRuntimeName;
        tempStrCharacter.CharacterDescription = _characterDescription;
        tempStrCharacter.CharacterBody = _characterElements[StrCharacterElementType.Body];
        tempStrCharacter.CharacterClothes = _characterElements[StrCharacterElementType.Clothes];
        tempStrCharacter.CharacterHaircut = _characterElements[StrCharacterElementType.Haircut];
        tempStrCharacter.CharacterMakeup = _characterElements[StrCharacterElementType.Makeup];
        return tempStrCharacter;
    }
    private string ComposeElementResourcesPath(StrCharacterElementType type)
    {
        string tempResourcesPath = "";
        if (type == StrCharacterElementType.Body)
        {
            tempResourcesPath = _folders._body.Replace(_folders._root + "/Resources/", "") + "/" + _bodyFileName;
        }
        if (type == StrCharacterElementType.Clothes)
        {
            tempResourcesPath = _folders._clothes.Replace(_folders._root + "/Resources/", "") + "/" + _clothesFileName;
        }
        if (type == StrCharacterElementType.Haircut)
        {
            tempResourcesPath = _folders._haircut.Replace(_folders._root + "/Resources/", "") + "/" + _haircutFileName;
        }
        if (type == StrCharacterElementType.Makeup)
        {
            tempResourcesPath = _folders._makeup.Replace(_folders._root + "/Resources/", "") + "/" + _makeupFileName;
        }
        return tempResourcesPath;
    }
    private void SetupElemetsDictionary()
    {
        if (_characterElements.Count == 0)
        {
            _characterElements.Add(StrCharacterElementType.Body, _characterBody);
            _characterElements.Add(StrCharacterElementType.Clothes, _characterClothes);
            _characterElements.Add(StrCharacterElementType.Haircut, _characterHaircut);
            _characterElements.Add(StrCharacterElementType.Makeup, _characterMakeup);
        }
    }

    private void CreateCharacterElements(GameObject canvas, string elementResourcesPath, string spriteName, StrCharacterElementType elementType)
    {
        GameObject tempElementGameObject = new GameObject();
        RectTransform tempElementRectTransform = tempElementGameObject.AddComponent<RectTransform>();
        tempElementRectTransform.localScale = Vector3.one;
        tempElementRectTransform.anchoredPosition = new Vector2(0f, 0f);
        tempElementRectTransform.sizeDelta = new Vector2(StrConstantValues.StandartCharacterWidht, StrConstantValues.StandartCharacterHeight);
        if (elementType == StrCharacterElementType.Body)
        {
            tempElementGameObject.name = _characterTechName;
            tempElementGameObject.transform.SetParent(canvas.transform, false);
            tempElementGameObject.AddComponent<Character>();
            _characterGameObject = tempElementGameObject;
            SetupCharacterElement(tempElementGameObject, elementResourcesPath,spriteName, elementType);
        }
        else
        {
            SetupCharacterElement(tempElementGameObject, elementResourcesPath,spriteName, elementType);
            tempElementGameObject.transform.SetParent(_characterElements[StrCharacterElementType.Body].transform, false);
            tempElementGameObject.name = elementType.GetFieldTypeAssociatedFolder();
        }
    }
    private void SetupCharacterElement(GameObject tempElement, string resourcesPath,string name, StrCharacterElementType elementType)
    {
        Texture2D tempTexture;
        tempTexture = Resources.Load(resourcesPath) as Texture2D;
        _characterElements[elementType] = tempElement.AddComponent<Image>();
        _characterElements[elementType].sprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(tempTexture.width / 2, tempTexture.height / 2));
        _characterElements[elementType].sprite.name = name;
    }


}
