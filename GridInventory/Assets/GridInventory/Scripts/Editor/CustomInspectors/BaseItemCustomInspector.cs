using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseItem), true)]
public class BaseItemCustomInspector : Editor
{
    protected SerializedProperty m_ItemName;

    protected void OnEnable()
    {
        this.m_ItemName = serializedObject.FindProperty("m_ItemName");
        
    }

    private void OnValidate()
    {
        
    }

    public override void OnInspectorGUI()
    {       
        serializedObject.Update();
        //base.OnInspectorGUI();
        
        DrawPropertiesExcluding(serializedObject);
        serializedObject.ApplyModifiedProperties();

        ObjectNames.SetNameSmart(serializedObject.targetObject, m_ItemName.stringValue);

    }
}
