using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StorylineEditor;
using System.Globalization;

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
        decomposedAction.IsPhraseHolderActive = ReadIsPhraseHolderActive(selectedAction, actionID);
        decomposedAction.PhraseHolderPosition = ReadPhraseHolderPosition(selectedAction, actionID);
        decomposedAction.CGImageName = ReadCGName(selectedAction, actionID);
        decomposedAction.CGPosition = ReadCGPosition(selectedAction);
        decomposedAction.ActiveCharacters = ReadActiveCharacters(selectedAction);
        decomposedAction.ChoiseOptions = ReadChoiseOptions(selectedAction);
        decomposedAction.ActiveCharactersPositions = ReadActiveCharacterPositions(selectedAction);
        decomposedAction.ActiveCharactersScales = ReadActiveCharacterScales(selectedAction);
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
    private string ReadIsPhraseHolderActive(List<string> selectedAction, int actionID)
    {
        string isPhraseHolderActive = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._phraseHolderState + _tags._separator + actionID)
            {
                isPhraseHolderActive = selectedAction[i + 1];
            }
        }
        return isPhraseHolderActive;
    }
    private Vector3 ReadPhraseHolderPosition(List<string> selectedAction, int actionID)
    {
        Vector3 phraseHolderPosition;
        string phraseHolderPositionRaw = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._praseHolderPosition + _tags._separator + actionID)
            {
                phraseHolderPositionRaw = selectedAction[i + 1];
            }
        }
        string[] phraseHolderPositionValues = phraseHolderPositionRaw.Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int positionX = int.Parse(phraseHolderPositionValues[0]);
        int positionY = int.Parse(phraseHolderPositionValues[1]);
        int positionZ = int.Parse(phraseHolderPositionValues[2]);
        phraseHolderPosition = new Vector3(positionX, positionY, positionZ);
        return phraseHolderPosition;
    }

    private string ReadCGName(List<string> selectedAction, int actionID)
    {
        string CGName = "";
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == _tags._CG)
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
                activeCharacterRaw = selectedAction[i + 1].Replace(StrConstantValues.StrFileStepGap, "");

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
                        if (selectedAction[k] != StrConstantValues.StrFileStepGap + _tags._characterRescaled)
                        {
                            string tempRawPosition = selectedAction[k].Replace(StrConstantValues.StrFileStepGap, "");
                            string[] characterPositionRaw = tempRawPosition.Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            int characterPositionX = int.Parse(characterPositionRaw[1], CultureInfo.InvariantCulture);
                            int characterPositionY = int.Parse(characterPositionRaw[2], CultureInfo.InvariantCulture);
                            int characterPositionZ = int.Parse(characterPositionRaw[3], CultureInfo.InvariantCulture);
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
    private Dictionary<string, Vector2> ReadActiveCharacterScales(List<string> selectedAction)
    {
        Dictionary<string, Vector2> activeCharactersScales = new Dictionary<string, Vector2>();
        for (int i = 0; i < selectedAction.Count; i++)
        {
            if (selectedAction[i] == StrConstantValues.StrFileStepGap + _tags._characterRescaled)
            {
                if (selectedAction[i + 1] != _tags._null)
                {
                    int m = i + 1;
                    for (int k = m; k < selectedAction.Count; k++)
                    {
                        if (selectedAction[k] != _tags._stepsEnd)
                        {
                            string tempRawScale = selectedAction[k].Replace(StrConstantValues.StrFileStepGap, "");
                            string[] characterScaleRaw = tempRawScale.Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            float characterScaleX = float.Parse(characterScaleRaw[1]);
                            float characterScaleY = float.Parse(characterScaleRaw[2]);
                            Vector2 characterScale = new Vector2(characterScaleX, characterScaleY);
                            activeCharactersScales.Add(characterScaleRaw[0], characterScale);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    activeCharactersScales.Add(_tags._null, Vector3.one);
                }

            }
        }
        return activeCharactersScales;
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
    public StrRawStr DecomposeRawStr(List<string> rstrContent)
    {
        StrRawStr decomposedRawStr = new StrRawStr();
        decomposedRawStr.StorylineName = rstrContent[0];
        decomposedRawStr.User = rstrContent[1];
        decomposedRawStr.Version = float.Parse(rstrContent[2]);
        decomposedRawStr.ActionID = int.Parse(rstrContent[3]);
        decomposedRawStr.StepID = int.Parse(rstrContent[4]);
        decomposedRawStr.Phrase = rstrContent[5];
        decomposedRawStr.PhraseAuthor = rstrContent[6];
        if (rstrContent[7] == true.ToString())
        {
            decomposedRawStr.IsPhraseHolderActive = true;
        }
        else
        {
            decomposedRawStr.IsPhraseHolderActive = false;
        }
        if (rstrContent[8] == true.ToString())
        {
            decomposedRawStr.IsReadyForNextAction = true;
        }
        else
        {
            decomposedRawStr.IsReadyForNextAction = false;
        }
        decomposedRawStr.RefereceResolutionWidht = int.Parse(rstrContent[9]);
        decomposedRawStr.JumpMarker = rstrContent[10];
        string[] phraseHolderPositionValues = rstrContent[11].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int positionX = int.Parse(phraseHolderPositionValues[0]);
        int positionY = int.Parse(phraseHolderPositionValues[1]);
        int positionZ = int.Parse(phraseHolderPositionValues[2]);
        decomposedRawStr.PhraseHolderPosition = new Vector3(positionX, positionY, positionZ);
        decomposedRawStr.CGSpriteName = rstrContent[12];
        string[] CGPositionValues = rstrContent[13].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("line 12: " + rstrContent[13]);
        int CGpositionX = int.Parse(CGPositionValues[0]);
        int CGpositionY = int.Parse(CGPositionValues[1]);
        int CGpositionZ = int.Parse(CGPositionValues[2]);
        decomposedRawStr.CGPosition = new Vector3(CGpositionX, CGpositionY, CGpositionZ);
        string[] tempActiveCharacters = rstrContent[14].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTempActiveCharacters = new List<string>();
        if (tempActiveCharacters.Length != 0)
        {
            foreach (string unit in tempActiveCharacters)
            {
                decomposedTempActiveCharacters.Add(unit);
            }
        }
        decomposedRawStr.ActiveCharacters = decomposedTempActiveCharacters;
        string[] tempRequiredObjects = rstrContent[15].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTempRequiredObjects = new List<string>();
        if (tempRequiredObjects.Length != 0)
        {
            foreach (string unit in tempRequiredObjects)
            {
                decomposedTempRequiredObjects.Add(unit);
            }
        }
        decomposedRawStr.RequiredCharacters = decomposedTempRequiredObjects;
        string[] tempRequiredCG = rstrContent[16].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTempRequiredCG = new List<string>();
        if (tempRequiredCG.Length != 0)
        {
            foreach (string unit in tempRequiredCG)
            {
                decomposedTempRequiredCG.Add(unit);
            }
        }
        decomposedRawStr.RequiredCGs = decomposedTempRequiredCG;
        string[] tempChoiseOptions = rstrContent[17].Split(_tags._separatorVertical.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTempChoiseOptions = new List<string>();
        if (tempChoiseOptions.Length != 0)
        {
            foreach (string choiseOption in tempChoiseOptions)
            {
                decomposedTempChoiseOptions.Add(choiseOption);
            }
        }
        decomposedRawStr.ChoiseOptions = decomposedTempChoiseOptions;
        string[] tempTotalStepsCount = rstrContent[18].Split(_tags._separatorVertical.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTotalStepsCount = new List<string>();
        if (tempTotalStepsCount.Length != 0)
        {
            foreach (string count in tempTotalStepsCount)
            {
                decomposedTotalStepsCount.Add(count);
            }
        }
        decomposedRawStr.TotalStepsCount = decomposedTotalStepsCount;
        string[] tempCurrentActionSteps = rstrContent[19].Split(_tags._separatorVertical.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        List<string> decomposedTempCurrentActionSteps = new List<string>();
        if (tempCurrentActionSteps.Length != 0)
        {
            foreach (string step in tempCurrentActionSteps)
            {
                decomposedTempCurrentActionSteps.Add(step);
            }
        }
        decomposedRawStr.CurretActionSteps = decomposedTempCurrentActionSteps;
        decomposedRawStr.TotalActions = int.Parse(rstrContent[20]);
        decomposedRawStr.CharactersPositions = ReadRawStrCharactersPositions(rstrContent);
        decomposedRawStr.CharactersScales = ReadRawStrCharactersScales(rstrContent);
        decomposedRawStr.StorylineActions = ReadRawStrActions(rstrContent);
        return decomposedRawStr;
    }
    private Dictionary<string, Vector3> ReadRawStrCharactersPositions(List<string> rstrContent)
    {
        Dictionary<string, Vector3> tempPositions = new Dictionary<string, Vector3>();
        if (rstrContent.Count != 0)
        {
            for (int i = 0; i < rstrContent.Count; i++)
            {
                if (rstrContent[i] == _tags._rstrCharctersPositions)
                {
                    for (int k = i + 1; k < rstrContent.Count; k++)
                    {
                        if (rstrContent[k] == _tags._rstrCharctersScales)
                        {
                            break;
                        }
                        else
                        {
                            string[] decomposedTempPositions = rstrContent[k].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in decomposedTempPositions)
                            {
                                Debug.Log(line);
                            }
                            Vector3 positions = new Vector3(float.Parse(decomposedTempPositions[1]), float.Parse(decomposedTempPositions[2]), float.Parse(decomposedTempPositions[3]));
                            tempPositions.Add(decomposedTempPositions[0], positions);
                        }
                    }
                    break;
                }
            }
        }
        return tempPositions;
    }
    private Dictionary<string, Vector2> ReadRawStrCharactersScales(List<string> rstrContent)
    {
        Dictionary<string, Vector2> tempScales = new Dictionary<string, Vector2>();
        if (rstrContent.Count != 0)
        {
            for (int i = 0; i < rstrContent.Count; i++)
            {
                if (rstrContent[i] == _tags._rstrCharctersScales)
                {
                    for (int k = i + 1; k < rstrContent.Count; k++)
                    {
                        if (rstrContent[k] == _tags._rstrActions)
                        {
                            break;
                        }
                        else
                        {
                            string[] decomposedTempScales = rstrContent[k].Split(_tags._separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            Vector2 scales = new Vector2(float.Parse(decomposedTempScales[1]), float.Parse(decomposedTempScales[2]));
                            tempScales.Add(decomposedTempScales[0], scales);
                        }
                    }
                    break;
                }
            }
        }
        return tempScales;
    }
    private List<string> ReadRawStrActions(List<string> rstrContent)
    {
        List<string> tempStorylineActions = new List<string>();
        for (int i = 0; i < rstrContent.Count; i++)
        {
            if (rstrContent[i] == _tags._rstrActions)
            {
                for (int k = i + 1; k < rstrContent.Count; k++)
                {
                    tempStorylineActions.Add(rstrContent[k]);
                }
            }
        }
        return tempStorylineActions;
    }
}
