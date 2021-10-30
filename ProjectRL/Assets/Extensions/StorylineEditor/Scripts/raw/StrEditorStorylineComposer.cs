using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StorylineEditor;

[RequireComponent(typeof(StrEditorEvents))]
[RequireComponent(typeof(StrEditorGodObject))]
[RequireComponent(typeof(extStrEditorReplacer))]
[RequireComponent(typeof(global_taglist))]
[ExecuteInEditMode]

public class StrEditorStorylineComposer : MonoBehaviour
{
    private global_taglist _tags;
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
        _tags = GetComponent<global_taglist>();
        return true;
    }
    public List<string> ComposeStep(StrUncomposedStorylineParameters uncomposedStoryline)
    {
        List<string> stepsOfCurrentAction = new List<string>();
        string _charactersToActivation = "";
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
                    foreach (string activatedCharacter in activatedCharacters)
                    {
                        if (activatedCharacter != null)
                        {
                            _charactersToActivation = _charactersToActivation + activatedCharacter + _tags._separator;
                        }
                        else
                        {
                            stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._skip);
                        }
                    }
                    foreach (string inactivatedCharacter in inactivatedCharacters)
                    {
                        if (inactivatedCharacter != null)
                        {
                            _charactersToInactivation = _charactersToInactivation + inactivatedCharacter + _tags._separator;
                        }
                        else
                        {
                            stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._skip);
                        }
                    }
                    break;
                case 2:
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._cg_position);
                    float CGPositionX = Mathf.Round(_CGRectTransform.localPosition.x);
                    float CGPositionY = _CGRectTransform.localPosition.y;
                    float CGPositionZ = _CGRectTransform.localPosition.z;
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + CGPositionX + _tags._separator + CGPositionY + _tags._separator + CGPositionZ + _tags._separator);
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._activate);
                    if (_charactersToActivation != "")
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _charactersToActivation);
                    }
                    else
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._null);
                    }
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._inactivate);
                    if (_charactersToInactivation != "")
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _charactersToInactivation);
                    }
                    else
                    {
                        stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._null);
                    }
                    break;
                case 3:
                    stepsOfCurrentAction.Add(StrConstantValues.StrFileStepGap + _tags._character_relocated);
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
    public List<string> ComposeInitPart(StrUncomposedStorylineParameters uncomposedStoryline)
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
        initPart.Add(_tags._required_objects);
        string RequiredObjects = "";
        foreach (GameObject requiredObject in requiredObjects)
        {
            RequiredObjects = RequiredObjects + requiredObject.name + _tags._separator;
        }
        initPart.Add(RequiredObjects);
        initPart.Add(_tags._required_cg);
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
    public List<string> ComposeAction(StrUncomposedStorylineParameters uncomposedStoryline)
    {
        List<string> currentAction = new List<string>();
        List<string> stepsOfCurrentAction = uncomposedStoryline.StepsOfCurrentAction;
        int actionID = uncomposedStoryline.ActionID;
        string phrase = uncomposedStoryline.Phrase;
        string phraseAuthor = uncomposedStoryline.PhraseAuthor;
        string CGSpriteName = uncomposedStoryline.CGImage.sprite.ToString().Replace(" (UnityEngine.Sprite)", "");
        string endstep = "/&endstep";
        for (int i = 0; i <= StrConstantValues.ActionComposeStagesCount; i++)
        {
            switch (i)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:
                    currentAction.Add(_tags._action + _tags._separator + actionID);
                    currentAction.Add("{");
                    currentAction.Add(_tags._phrase + _tags._separator + actionID);
                    currentAction.Add(phrase);
                    currentAction.Add(_tags._author + _tags._separator + actionID);
                    currentAction.Add(phraseAuthor);
                    currentAction.Add(_tags._CG);
                    currentAction.Add(CGSpriteName);
                    break;
                case 3:
                    if (stepsOfCurrentAction.Count != 0)
                    {
                        foreach (var step in stepsOfCurrentAction)
                        {
                            currentAction.Add(step);
                        }
                    }
                    currentAction.Add(endstep);
                    break;
                case 4:
                    currentAction.Add("}");
                    currentAction.Add(_tags._skip);
                    break;
            }
        }
        return currentAction;
    }
}