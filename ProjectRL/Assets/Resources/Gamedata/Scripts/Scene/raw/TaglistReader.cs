
using UnityEngine;
using System.IO;
using System;
[ExecuteAlways]
public class TaglistReader : MonoBehaviour
{
    private global_folders _s_folder;
    public string _version;
    public string _start;
    public string _action;
    public string _separator;
    public string _phrase;
    public string _CG;
    public string _init;
    public string _requiredObjects;
    public string _requiredCG;
    public string _skip;
    public string _step;
    public string _characterRelocated;
    public string _activate;
    public string _rawStrActions;
    public string _author;
    public string _rescale;
    public string _null;
    public string _separatorVertical;
    public string _CGposition;
    public string _stepsEnd;
    public string _choise;
    public string _actionEnd;
    public string _lineSeparator;
    public string _jumpMarker;
    public string _characterRescaled;
    public string _phraseHolderState;
    public string _praseHolderPosition;

    void Start()
    {
        _s_folder = GetComponent<global_folders>();
        Setup_tags();
    }
    public Boolean Setup_tags()
    {
        string taglistPath = _s_folder._configs + "/tag_list.cfg";
        StreamReader SR = new StreamReader(taglistPath);
        string line = SR.ReadLine();
        _version = line;
        int count = 1;
        while (line != null)
        {
            line = SR.ReadLine();
            switch (count)
            {
                case 0:
                    count += 1;
                    break;
                case 1:
                    _start = line;
                    count += 1;
                    break;
                case 2:
                    _action = line;
                    count += 1;
                    break;
                case 3:
                    _separator = line;
                    count += 1;
                    break;
                case 4:
                    _phrase = line;
                    count += 1;
                    break;
                case 5:
                    _CG = line;
                    count += 1;
                    break;
                case 6:
                    _init = line;
                    count += 1;
                    break;
                case 7:
                    _requiredObjects = line;
                    count += 1;
                    break;
                case 8:
                    _requiredCG = line;
                    count += 1;
                    break;
                case 9:
                    _skip = line;
                    count += 1;
                    break;
                case 10:
                    _step = line;
                    count += 1;
                    break;
                case 11:
                    _characterRelocated = line;
                    count += 1;
                    break;
                case 12:
                    _activate = line;
                    count += 1;
                    break;
                case 13:
                    _rawStrActions = line;
                    count += 1;
                    break;
                case 14:
                    _author = line;
                    count += 1;
                    break;
                case 15:
                    _rescale = line;
                    count += 1;
                    break;
                case 16:
                    _null = line;
                    count += 1;
                    break;
                case 17:
                    _separatorVertical = line;
                    count += 1;
                    break;
                case 18:
                    _CGposition = line;
                    count += 1;
                    break;
                case 19:
                    _stepsEnd = line;
                    count += 1 ;
                    break;
                case 20:
                    _choise = line;
                    count += 1;
                    break;
                case 21:
                    _actionEnd = line;
                    count += 1;
                    break;
                case 22:
                    _lineSeparator = line;
                    count += 1;
                    break;
                case 23:
                    _jumpMarker = line;
                    count +=1;
                    break;
                case 24:
                    _characterRescaled = line;
                    count += 1;
                    break;
                case 25:
                    _phraseHolderState = line;
                    count += 1;
                    break;
                case 26:
                    _praseHolderPosition = line;
                    count = 0;
                    break;
            }
        }
        SR.Close();
        return true;
    }
}
