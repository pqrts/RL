using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEditor;
using System;

public class ext_Storyline_exeptions : MonoBehaviour
{
    public  List<int> _no_author_in_action = new List<int>();

    public void Set_no_author( int id_action)
    {
        _no_author_in_action.Add(id_action);
    }


}
