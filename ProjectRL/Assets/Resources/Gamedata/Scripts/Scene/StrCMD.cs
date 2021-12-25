using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using RLClient;

public class StrCMD : MonoBehaviour
{
    private TMP_InputField _cmdInputField;
    [SerializeField] private GameObject _mainCamera;
    private string _load = "/load";
    [SerializeField] private List<IStrLoader> _StrLoaders = new List<IStrLoader>();
    void Start()
    {
        _cmdInputField = GetComponent<TMP_InputField>();
        if (_mainCamera != null)
        {
            Component[] tempComponents = _mainCamera.GetComponents(typeof( IStrLoader));
            foreach (Component tempLoader in tempComponents)
            {
                _StrLoaders.Add(tempLoader as IStrLoader);
                Debug.Log(_StrLoaders.Count);
            }
        }
        AddListeners();
    }
    private void AddListeners()
    {
        if (_cmdInputField != null)
        {
            _cmdInputField.onEndEdit.AddListener(delegate { CMDValueChanged(_cmdInputField); });
        }
    }
    private void CMDValueChanged(TMP_InputField cmdInputField)
    {
        if (cmdInputField.text.Length != 0)
        {
            ParseCommand(cmdInputField.text, cmdInputField);
        }
    }
    private void ParseCommand(string command, TMP_InputField cmdInputField)
    {
        string space = " ";
        string[] splitedCommand = command.Split(space.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (command.StartsWith(_load))
        {
            if (splitedCommand.Length>1)
            {
                if (_StrLoaders.Count == 1)
                {                   
                    _StrLoaders[0].LoadStoryline(splitedCommand[1]);
                    cmdInputField.text = ">Loading file: " + splitedCommand[1];
                }
                else 
                {
                    cmdInputField.text = ">Loading exeption: multiple loaders_"+_StrLoaders.Count;
                }               
            }
            else
            {
                cmdInputField.text = ">Loading exeption: no file name";
            }
        }
    }  
    void Update()
    {

    }
    private void OnDisable()
    {
        _cmdInputField.onValueChanged.RemoveAllListeners();
    }
}
