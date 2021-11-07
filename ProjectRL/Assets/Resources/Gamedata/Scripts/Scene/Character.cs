using UnityEngine;
using UnityEngine.UI;
using StorylineEditor;
public class Character : MonoBehaviour
{
    private string _characterTechName;
    private string _characterRuntimeName;
    private string _characterDescription;
    private Image _characterBody;
    private Image _characterHaircut;
    private Image _characterClothes;
    private Image _characterMakeup;
    public void SetCharacterParameters(StrCharacter tempStrCharacter)
    {
        _characterTechName = tempStrCharacter.CharacterTechName;
        _characterRuntimeName = tempStrCharacter.CharacterRuntimeName;
        _characterDescription = tempStrCharacter.CharacterDescription;
        _characterBody = tempStrCharacter.CharacterBody;
        _characterClothes = tempStrCharacter.CharacterClothes;
        _characterHaircut = tempStrCharacter.CharacterHaircut;
        _characterMakeup = tempStrCharacter.CharacterMakeup;
    }
    public StrCharacter GetCharacterParameters()
    {
        StrCharacter tempStrCharacter = new StrCharacter();
        tempStrCharacter.CharacterTechName = _characterTechName;
        tempStrCharacter.CharacterRuntimeName = _characterRuntimeName;
        tempStrCharacter.CharacterDescription = _characterDescription;
        tempStrCharacter.CharacterBody = _characterBody;
        tempStrCharacter.CharacterClothes = _characterClothes;
        tempStrCharacter.CharacterHaircut = _characterHaircut;
        tempStrCharacter.CharacterMakeup = _characterMakeup;
        return tempStrCharacter;
    }
}