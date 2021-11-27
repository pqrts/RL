using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StorylineEditor;

[RequireComponent(typeof(TaglistReader))]
[RequireComponent(typeof(StrEditorGodObject))]
[ExecuteInEditMode]
public class StrEditorDecomposer : MonoBehaviour
{
    private TaglistReader _tags;
    private StrEditorGodObject _StrEditorRoot;

    public Boolean GetRequieredComponents()
    {
        _StrEditorRoot = GetComponent<StrEditorGodObject>();
        _tags = GetComponent<TaglistReader>();
        return true;
    }
    public StrDecomposedAction DecomposeSelectedAction(List<string> selectedAction)
    {
        StrDecomposedAction decomposedAction = new StrDecomposedAction();
        string[] actionIdRaw = selectedAction[0].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int actionID = int.Parse(actionIdRaw[1]);
        decomposedAction.Phrase = ReadPhrase(selectedAction, actionID);
        decomposedAction.PhraseAuthor = ReadPhraseAuthor(selectedAction, actionID);
        decomposedAction.CGImageName = ReadCGName(selectedAction, actionID);
        decomposedAction.CGPosition = ReadCGPosition(selectedAction);
        decomposedAction.ActiveCharacters = ReadActiveCharacters(selectedAction);
        decomposedAction.ChoiseOptions = ReadChoiseOptions(selectedAction);
        decomposedAction.ActiveCharactersPositions = ReadActiveCharacterPositions(selectedAction);
        decomposedAction.JumpToAction = ReadJumpMarker(selectedAction);
        return decomposedAction;
    }
    private string ReadPhrase(List<string> selectedAction, int actionID)
    {
        string phrase = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._phrase + _tags._separator + actionID)
            {
                phrase = selectedAction[i + 1];
            }
        }
        return phrase;
    }
    private string ReadPhraseAuthor(List<string> selectedAction, int actionID)
    {
        string phraseAuthor = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._author + _tags._separator + actionID)
            {
                phraseAuthor = selectedAction[i + 1];
            }
        }
        return phraseAuthor;
    }
    private string ReadCGName(List<string> selectedAction, int actionID)
    {
        string CGName = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._author + _tags._separator + actionID)
            {
                CGName = selectedAction[i + 1];
            }
        }
        return CGName;
    }
    private Vector3 ReadCGPosition(List<string> selectedAction)
    {
        Vector3 CGPosition;
        string CGPositionRaw = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == StrConstantValues.StrFileStepGap + _tags._CGposition)
            {
                CGPositionRaw = selectedAction[i + 1];
            }
        }
        string[] CGPositionValues = CGPositionRaw.Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int positionX = int.Parse(CGPositionValues[0]);
        int positionY = int.Parse(CGPositionValues[1]);
        int positionZ = int.Parse(CGPositionValues[2]);
        CGPosition = new Vector3(positionX, positionY, positionZ);
        return CGPosition;
    }
    private List<string> ReadActiveCharacters(List<string> selectedAction)
    {
        List<string> activeCharacters = new List<string>();
        string activeCharacterRaw = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == StrConstantValues.StrFileStepGap + _tags._activate)
            {
                activeCharacterRaw = selectedAction[i + 1];
            }
        }
        string[] characters = activeCharacterRaw.Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        foreach (string character in characters)
        {
            activeCharacters.Add(character);
        }
        return activeCharacters;
    }
    private Dictionary<string, Vector3> ReadActiveCharacterPositions(List<string> selectedAction)
    {
        Dictionary<string, Vector3> activeCharactersPositions = new Dictionary<string, Vector3>();
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == StrConstantValues.StrFileStepGap + _tags._characterRelocated)
            {
                if (selectedAction[i + 1] != _tags._null)
                {
                    int m = i + 1;
                    for (int k = m; k < selectedAction.Count; k++)
                    {
                        if (selectedAction[k] != StrConstantValues.StrFileStepGap + _tags._skip)
                        {
                            string[] characterPositionRaw = selectedAction[k].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            int characterPositionX = int.Parse(characterPositionRaw[1]);
                            int characterPositionY = int.Parse(characterPositionRaw[2]);
                            int characterPositionZ = int.Parse(characterPositionRaw[3]);
                            Vector3 characterPosition = new Vector3(characterPositionX, characterPositionY, characterPositionZ);
                            activeCharactersPositions.Add(characterPositionRaw[0], characterPosition);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    activeCharactersPositions.Add(_tags._null, Vector3.one);
                }

            }
        }
        return activeCharactersPositions;
    }
    private List<string> ReadChoiseOptions(List<string> selectedAction)
    {
        List<string> choiseOptions = new List<string>();
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._choise)
            {
                if (selectedAction[i + 1] != _tags._null)
                {
                    int m = i + 1;
                    for (int k = m; k < selectedAction.Count; k++)
                    {
                        if (selectedAction[k] != _tags._jumpMarker)
                        {
                            choiseOptions.Add(selectedAction[k]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    choiseOptions.Add(_tags._null);
                }
            }
        }
        return choiseOptions;
    }
    private string ReadJumpMarker(List<string> selectedAction)
    {
        string jumpMarker = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._jumpMarker)
            {
                if (selectedAction[i + 1] != _tags._null)
                {
                    jumpMarker = selectedAction[i + 1];
                }
                else
                {
                    jumpMarker = _tags._null;
                }
            }
        }
        return jumpMarker;
    }
}
