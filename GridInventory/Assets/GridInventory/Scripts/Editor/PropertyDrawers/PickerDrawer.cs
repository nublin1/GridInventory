using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PickerAttribute), true)]
public abstract class PickerDrawer <T> : PropertyDrawer where T : ScriptableObject
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        T current = (T)Utilities.GetValue(property);
        position = EditorGUI.PrefixLabel(position, label);
        DoSelection(position, property, label, current);
        EditorGUI.EndProperty();
    }

    protected virtual void DoSelection(Rect buttonRect, SerializedProperty property, GUIContent label, T current)
    {
        GUIStyle buttonStyle = EditorStyles.objectField;
        GUIContent buttonContent = new GUIContent(current != null ? current.name : "Null");
        if (GUI.Button(buttonRect, buttonContent, buttonStyle))
        {
            Debug.Log("Selectopn");
        }
    }

    protected abstract List<T> GetItems(ItemDatabase database);

}
