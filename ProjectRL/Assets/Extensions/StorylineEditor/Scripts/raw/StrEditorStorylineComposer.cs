using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StorylineEditor;

[RequireComponent(typeof(StrEditorEvents))]
[RequireComponent(typeof(StrEditorGodObject))]
[RequireComponent(typeof(StrEditorReplacer))]
[RequireComponent(typeof(TaglistReader))]
[RequireComponent(typeof(StrEditorEncryptor))]
[ExecuteInEditMode]

public class StrEditorStorylineComposer : MonoBehaviour
{
    private TaglistReader _tags;
    private StrEditorGodObject _StrRootObject;
    public Boolean GetRequieredComponents()
    {
        StrEditorGodObject tempRootObject = GetComponent<StrEditorGodObject>();
        if (tempRootObject is IStrEditorRoot)
        {
            _StrRootObject = tempRootObject;
        }
        else
        {
            throw new ArgumentException("'StrEditorRoot' must implement the 'IStrEditorRoot' interface");
        }
        _tags = GetComponent<TaglistReader>();
        return true;
    }
    public List<string> ComposeStep(StrStorylineParameters uncomposedStoryline)
    {
        List<string> stepsOfCurrentAction = new List<string>();
        string _activeCharacters = "";
        string _charactersToInactivation = "";
        int stepID = uncomposedStoryline.StepID;
        RectTransform _CGRectTransform = uncomposedStoryline.CGRectTransform;
        List<GameObject> activeCharacters = uncomposedStoryline.ActiveCharacters;
        List<RectTransform> _activeRectTransforms = uncomposedStoryline.ActiveRectTransforms;
        for (int i = 0; i <= StrConstantValues.StepComposeStagesCount; i++)
        {
            switch (i)
            {
                case 0:
                    stepsOfCurrentAction.Add(_tags._step + _tags._separator + stepID);
                    break;
                case 1:

                    foreach (GameObject temp in uncomposedStoryline.ActiveCharacters)
                    {
                        _activeCharacters = _activeCharacters + temp.name + _tags._separator;
                    }

                    break;
                case 2:
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._CGposition);
                    float CGPositionX = Mathf.Round(_CGRectTransform.localPosition.x);
                    float CGPositionY = _CGRectTransform.localPosition.y;
                    float CGPositionZ = _CGRectTransform.localPosition.z;
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + CGPositionX + _tags._separator + CGPositionY + _tags._separator + CGPositionZ + _tags._separator);
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._activate);
                    if (_activeCharacters != "")
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _activeCharacters);
                    }
                    else
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._null);
                    }

                    break;
                case 3:
                    if (activeCharacters.Count != 0)
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._characterRelocated);
                        for (int e = 0; e < activeCharacters.Count; e++)
                        {
                            string characterName = activeCharacters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                            float positionX = Mathf.Round(_activeRectTransforms[e].localPosition.x);
                            float positionY = Mathf.Round(_activeRectTransforms[e].localPosition.y);
                            float positionZ = Mathf.Round(_activeRectTransforms[e].localPosition.z);
                            stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + characterName + _tags._separator + positionX + _tags._separator + positionY + _tags._separator + positionZ);
                        }
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._characterRescaled);
                        for (int e = 0; e < activeCharacters.Count; e++)
                        {
                            string characterName = activeCharacters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                            float scaleX = Mathf.Round(_activeRectTransforms[e].rect.width);
                            float scaleY = Mathf.Round(_activeRectTransforms[e].rect.height);
                            stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + characterName + _tags._separator + scaleX + _tags._separator + scaleY);
                        }
                    }
                    else
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._characterRelocated);
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._null);
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._characterRescaled);
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._null);
                    }
                    break;
            }
        }
        return stepsOfCurrentAction;
    }
    public List<string> ComposeInitPart(StrStorylineParameters uncomposedStoryline)
    {
        List<string> initPart = new List<string>();
        DateTime date = System.DateTime.Now;
        string user = uncomposedStoryline.User;
        float version = uncomposedStoryline.Version;
        string metaInfo = "Created by: " + user + " at " + date;
        List<GameObject> requiredObjects = uncomposedStoryline.RequiredObjects;
        List<Sprite> requiredCG = uncomposedStoryline.RequiredCG;

        initPart.Add(_tags._skip + _tags._separator + "**************************META**************************");
        initPart.Add(_tags._skip + _tags._separator + metaInfo);
        initPart.Add(_tags._skip + _tags._separator + "********************************************************");
        initPart.Add(_tags._init);
        initPart.Add(_tags._skip);
        initPart.Add(_tags._version);
        initPart.Add("" + version);
        initPart.Add(_tags._requiredObjects);
        string RequiredObjects = "";
        foreach (GameObject requiredObject in requiredObjects)
        {
            RequiredObjects = RequiredObjects + requiredObject.name + _tags._separator;
        }
        initPart.Add(RequiredObjects);
        initPart.Add(_tags._requiredCG);
        string RequiredCG = "";
        foreach (Sprite requiredSprite in requiredCG)
        {
            RequiredCG = RequiredCG + requiredSprite.name + _tags._separator;
        }
        initPart.Add(RequiredCG);
        initPart.Add(_tags._skip + _tags._separator + "********************************************************");
        initPart.Add(_tags._start);
        initPart.Add(_tags._skip);

        return initPart;
    }
    public List<string> ComposeAction(StrStorylineParameters uncomposedStoryline)
    {
        List<string> currentAction = new List<string>();
        List<string> stepsOfCurrentAction = uncomposedStoryline.StepsOfCurrentAction;
        List<string> choiseOptions = uncomposedStoryline.ChoiseOptions;
        int actionID = uncomposedStoryline.ActionID;
        string phrase = uncomposedStoryline.Phrase;
        string phraseAuthor = uncomposedStoryline.PhraseAuthor;
        bool isPhraseHolderActive = uncomposedStoryline.IsPhraseHolderActive;
        RectTransform phraseHolderRectTransform = uncomposedStoryline.PhraseHolder.GetComponent<RectTransform>();
        string CGSpriteName = uncomposedStoryline.CGImage.sprite.ToString().Replace(" (UnityEngine.Sprite)", "");
        string _jumpMarker = uncomposedStoryline.JumpMarker;
        for (int i = 0; i <= StrConstantValues.ActionComposeStagesCount; i++)
        {
            switch (i)
            {
                case 0:
                    currentAction.Add(_tags._action + _tags._separator + actionID);
                    currentAction.Add("{");
                    currentAction.Add(_tags._phrase + _tags._separator + actionID);
                    currentAction.Add(phrase);
                    currentAction.Add(_tags._author + _tags._separator + actionID);
                    currentAction.Add(phraseAuthor);
                    currentAction.Add(_tags._phraseHolderState + _tags._separator + actionID);
                    currentAction.Add(isPhraseHolderActive.ToString());
                    currentAction.Add(_tags._praseHolderPosition + _tags._separator + actionID);
                    if (phraseHolderRectTransform != null)
                    {
                        float positionX = phraseHolderRectTransform.localPosition.x;
                        float positionY = phraseHolderRectTransform.localPosition.y;
                        float positionZ = phraseHolderRectTransform.localPosition.z;
                        currentAction.Add(positionX + _tags._separator + positionY + _tags._separator + positionZ);
                    }
                    else
                    {
                        currentAction.Add(_tags._null);
                    }
                    currentAction.Add(_tags._CG);
                    currentAction.Add(CGSpriteName);
                    break;
                case 1:
                    if (stepsOfCurrentAction.Count != 0)
                    {
                        foreach (var step in stepsOfCurrentAction)
                        {
                            currentAction.Add(step);
                        }
                    }
                    currentAction.Add(_tags._stepsEnd);
                    break;
                case 2:
                    currentAction.Add(_tags._choise);
                    if (choiseOptions.Count != 0)
                    {
                        foreach (string choiseOption in choiseOptions)
                        {
                            currentAction.Add(choiseOption);
                        }
                    }
                    else
                    {
                        currentAction.Add(_tags._null);
                    }
                    if (_jumpMarker != "")
                    {
                        currentAction.Add(_tags._jumpMarker);
                        currentAction.Add(_jumpMarker);
                    }
                    else
                    {
                        currentAction.Add(_tags._jumpMarker);
                        currentAction.Add(_tags._null);
                    }
                    break;
                case 3:
                    currentAction.Add(_tags._actionEnd);
                    currentAction.Add("}");
                    currentAction.Add(_tags._skip);
                    break;

            }
        }
        return currentAction;
    }
    public List<string> ComposeStoryline(StrUncomposedStorylineParts storylineParts)
    {
        List<string> composedStoryline = new List<string>();
        List<string> initPart = storylineParts.InitPart;
        List<string> storylineActions = storylineParts.StorylineActions;
        if (initPart.Count != 0)
        {
            foreach (string initPartLine in initPart)
            {
                composedStoryline.Add(initPartLine);
            }
        }
        if (storylineActions.Count != 0)
        {
            foreach (string storylineActionsLine in storylineActions)
            {
                composedStoryline.Add(storylineActionsLine);
            }
        }
        return composedStoryline;
    }
    public List<string> ComposeRawStr(StrRawStr rawStr)
    {
        List<string> composedRawStr = new List<string>();
        composedRawStr.Add(rawStr.StorylineName);
        composedRawStr.Add(rawStr.User);
        composedRawStr.Add(rawStr.Version.ToString());
        composedRawStr.Add(rawStr.ActionID.ToString());
        composedRawStr.Add(rawStr.StepID.ToString());
        composedRawStr.Add(rawStr.Phrase);
        composedRawStr.Add(rawStr.PhraseAuthor);
        composedRawStr.Add(rawStr.IsPhraseHolderActive.ToString());
        composedRawStr.Add(rawStr.IsReadyForNextAction.ToString());
        composedRawStr.Add(rawStr.RefereceResolutionWidht.ToString());
        composedRawStr.Add(rawStr.JumpMarker);
        if (rawStr.PhraseHolderPosition != null)
        {
            float positionX = rawStr.PhraseHolderPosition.x;
            float positionY = rawStr.PhraseHolderPosition.y;
            float positionZ = rawStr.PhraseHolderPosition.z;
            composedRawStr.Add(positionX + _tags._separator + positionY + _tags._separator + positionZ);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }
        composedRawStr.Add(rawStr.CGspriteName);
        if (rawStr.CGPosition != null)
        {
            float CGpositionX = rawStr.CGPosition.x;
            float CGpositionY = rawStr.CGPosition.y;
            float CGpositionZ = rawStr.CGPosition.z;
            composedRawStr.Add(CGpositionX + _tags._separator + CGpositionY + _tags._separator + CGpositionZ);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }

        if (rawStr.ActiveCharacters.Count != 0)
        {
            string tempActiveCharacters = "";
            foreach (string character in rawStr.ActiveCharacters)
            {
                tempActiveCharacters = tempActiveCharacters + _tags._separator + character;
            }
            composedRawStr.Add(tempActiveCharacters);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }

        if (rawStr.RequiredObjects.Count != 0)
        {
            string tempRequiredObjects = "";
            foreach (string gObject in rawStr.RequiredObjects)
            {
                tempRequiredObjects = tempRequiredObjects + _tags._separator + gObject;
            }
            composedRawStr.Add(tempRequiredObjects);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }

        if (rawStr.RequiredCG.Count != 0)
        {
            string tempRequiredCG = "";
            foreach (string CG in rawStr.RequiredCG)
            {
                tempRequiredCG = tempRequiredCG + _tags._separator + CG;
            }
            composedRawStr.Add(tempRequiredCG);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }
        if (rawStr.ChoiseOptions.Count != 0)
        {
            string tempChoiseOptions = "";
            foreach (string option in rawStr.ChoiseOptions)
            {
                tempChoiseOptions = tempChoiseOptions + _tags._separatorVertical + option;
            }
            composedRawStr.Add(tempChoiseOptions);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }
        if (rawStr.TotalStepsCount.Count != 0)
        {
            string tempTotalStepsCount = "";
            foreach (string stepsCount in rawStr.TotalStepsCount)
            {
                tempTotalStepsCount = tempTotalStepsCount + _tags._separatorVertical + stepsCount;
            }
            composedRawStr.Add(tempTotalStepsCount);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }
        if (rawStr.CurretActionSteps.Count != 0)
        {
            string tempCurrentActionSteps = "";
            foreach (string step in rawStr.CurretActionSteps)
            {
                tempCurrentActionSteps = tempCurrentActionSteps + _tags._separatorVertical + step;
            }
            composedRawStr.Add(tempCurrentActionSteps);
        }
        else
        {
            composedRawStr.Add(_tags._null);
        }
        composedRawStr.Add(_tags._rawStrActions);
        if (rawStr.StorylineActions.Count != 0)
        {

            foreach (string actionLine in rawStr.StorylineActions)
            {
                composedRawStr.Add(actionLine);
            }
        }
        else
        {
            composedRawStr.Add(_tags._null);
        } 
        return composedRawStr;
    }
}
