using GridInventorySystem;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ItemGenerator), true)]
public class ItemGeneratorCustomInspector : Editor
{
    private SerializedProperty script;
    private SerializedProperty m_Items;
    private ReorderableList m_ItemList;


    private void OnEnable()
    {
        this.script = serializedObject.FindProperty("m_Script");       
        this.m_Items = serializedObject.FindProperty("m_Pools");

        CreateItemList(serializedObject, m_Items);
    }

    private void CreateItemList(SerializedObject serializedObject, SerializedProperty elements)
    {
        this.m_ItemList = new ReorderableList(serializedObject, elements, true, true, true, true);

        m_ItemList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Items (Item, Amount)");
        };

        m_ItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y = rect.y + verticalOffset;
            rect.width = rect.width;


            SerializedProperty element = elements.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);

            /*
            SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
            if (amounts.arraySize < this.m_Items.arraySize)
            {
                for (int i = amounts.arraySize; i < this.m_Items.arraySize; i++)
                {
                    amounts.InsertArrayElementAtIndex(i);
                    amounts.GetArrayElementAtIndex(i).intValue = 1;
                }
            }
            SerializedProperty amount = amounts.GetArrayElementAtIndex(index);
            
            rect.x += rect.width + 2f;
            rect.width = 50f;
            EditorGUI.PropertyField(rect, amount, GUIContent.none);
            */
        };

        m_ItemList.onAddCallback = (ReorderableList list) =>
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
        };

        m_ItemList.onSelectCallback = (ReorderableList list) =>
        {

        };

        m_ItemList.onChangedCallback = (ReorderableList list) =>
        {

        };
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(script);
        EditorGUI.EndDisabledGroup();

        serializedObject.Update();        

        GUILayout.Space(3f);
        m_ItemList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
