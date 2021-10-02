using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ui_scaler))]
public class ui_scaler_editor : Editor
{
    public override void OnInspectorGUI()
    {
        ui_scaler s_ui_scaler = (ui_scaler)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Set standart ratio"))
        {
            s_ui_scaler.Standart_positions();
        }
        if (GUILayout.Button("rescale"))
        {
            s_ui_scaler.Rescale();
        }
        if (GUILayout.Button("test rescale"))
        {
            s_ui_scaler.Rescale_standart();
        }
        if (GUILayout.Button("Add elements"))
        {
            s_ui_scaler.Add_elements();
        }
        if (GUILayout.Button("Show/hide main#"))
        {
            s_ui_scaler.Hide_main();
        }
        if (GUILayout.Button("Show/hide wardrobe#"))
        {
            s_ui_scaler.Hide_ward();
        }
        if (GUILayout.Button("Show/hide shop#"))
        {
            s_ui_scaler.Hide_shop();
        }
        if (GUILayout.Button("Show/hide award#"))
        {
            s_ui_scaler.Hide_reward();
        }
        if (GUILayout.Button("Show/hide exeption#"))
        {
            s_ui_scaler.Hide_exeption();
        }
        if (GUILayout.Button("Show/hide story#"))
        {
            s_ui_scaler.Hide_story();
        }
        if (GUILayout.Button("Show/hide settings#"))
        {
            s_ui_scaler.Hide_settings();
        }

    }

}
