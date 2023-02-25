using GridInventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Serialization;
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
            var target = property.serializedObject.targetObject as BaseItem;
            Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects = new();
            if (target != null)
                selectableObjects = BuildSelectableObjects(property.serializedObject);
            else
                selectableObjects = BuildSelectableObjects();

            ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), selectableObjects,
                     (UnityEngine.Object obj) => {
                         property.serializedObject.Update();
                         property.objectReferenceValue = obj;
                         property.serializedObject.ApplyModifiedProperties();
                     },
                     () => {
                         //ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
                     });
        }
    }

    private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> BuildSelectableObjects()
    {        
        Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects = new();

        string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase));
            List<UnityEngine.Object> items = GetItems(obj as ItemDatabase).Cast<UnityEngine.Object>().ToList();
            
            selectableObjects.Add(obj, items);
        }
        return selectableObjects;
    }

    private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> BuildSelectableObjects(SerializedObject so)
    {
        Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects = new();
        var target = so.targetObject as BaseItem;

        var db = target.DatabaseParent;
        List<UnityEngine.Object> items = GetItems(db).Cast<UnityEngine.Object>().ToList();
        selectableObjects.Add(db, items);
        return selectableObjects;
    }

    protected abstract List<T> GetItems(ItemDatabase database);

}
