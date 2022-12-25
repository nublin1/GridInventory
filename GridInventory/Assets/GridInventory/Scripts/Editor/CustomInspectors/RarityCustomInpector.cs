using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rarity), true)]
public class RarityCustomInpector : Editor
{
    protected SerializedProperty m_ItemName;

    protected void OnEnable()
    {
        this.m_ItemName = serializedObject.FindProperty("m_RarityName");
    }
    private void OnValidate()
    {

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        ObjectNames.SetNameSmart(serializedObject.targetObject, m_ItemName.stringValue);
    }
}
