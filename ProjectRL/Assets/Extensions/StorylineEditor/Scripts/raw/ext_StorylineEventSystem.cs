using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ext_StorylineEventSystem : MonoBehaviour
{
    public delegate void OnStrEditorUpdated();
    public event OnStrEditorUpdated OnStrEdUpdated;

    public void EditorUpdated()
    {
        OnStrEdUpdated?.Invoke();
        Debug.Log("event");
    }
}

