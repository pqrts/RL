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
        List<string> activatedCharacters = uncomposedStoryline.ActivatedCharacters;
        List<string> inactivatedCharacters = uncomposedStoryline.InactivatedCharacters;
        for (int i = 0; i <= StrConstantValues.StepComposeStagesCount; i++)
        {
            switch (i)
            {
                case 0:
                    stepsOfCurrentAction.Add(_tags._step + _tags._separator + stepID);
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._skip);
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
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._characterRelocated);
                    for (int e = 0; e < activeCharacters.Count; e++)
                    {
                        string char_name = activeCharacters[e].ToString().Replace(" (UnityEngine.GameObject)", "");
                        float pos_x = _activeRectTransforms[e].localPosition.x;
                        float pos_y = _activeRectTransforms[e].localPosition.y;
                        float pos_z = _activeRectTransforms[e].localPosition.z;
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + char_name + _tags._separator + pos_x + _tags._separator + pos_y + _tags._separator + pos_z + _tags._separator);
                    }
                    break;
                case 4:
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._skip);
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
}
