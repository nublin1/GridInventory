using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEditor 
{
    public string ToolbarName { get; set; }

    abstract public void OnEnable();
    abstract public void OnDisable();
    abstract public void OnDestroy();
    abstract public void CreateGUI();
    abstract public void OnGUI(Rect position);
}
