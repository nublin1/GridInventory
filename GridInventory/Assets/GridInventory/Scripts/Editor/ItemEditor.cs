using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemEditor : BaseCollectionEditor<BaseItem>
{
    protected SerializedObject m_SerializedObject;
    protected SerializedProperty m_SerializedProperty;

    public ItemEditor(string title, ItemDatabase _database) : base(title, _database)
    {
        ToolbarName = title;
        m_Database = _database;

        //var obj = ScriptableObject.CreateInstance<ItemDatabase>();
        m_SerializedObject = new SerializedObject(_database);
        m_SerializedProperty= m_SerializedObject.FindProperty("items");
    }

    public override void OnEnable()
    {
        
    }

    public override void OnDisable()
    {
        
    }

    public override void OnDestroy()
    {
        
    }
    

    public override void CreateGUI()
    {
       
    }

    public override void OnGUI(Rect position)
    {
        DrawSidebar(new Rect(0, m_SidebarRect.y, m_SidebarRect.width, position.height));
        DrawContent(new Rect(m_SidebarRect.width, m_SidebarRect.y, 450, position.height));
    }

    protected override string GetSidebarLabel(BaseItem item)
    {
        return item.name;
    }

    protected override void DrawItem(BaseItem item)
    {
        // Get all fields in this script
        FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // Iterate through the fields and print their names and values
        foreach (FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false))
            {
                //Debug.Log(field.Name + ": " + field.GetValue(this));
                //EditorGUILayout.ObjectField(field.Name);
            }
        }



        int index = m_Items.IndexOf(item);
        m_SerializedObject.Update();

        SerializedProperty element = this.m_SerializedProperty.GetArrayElementAtIndex(index);
        foreach (var child in Utilities.EnumerateChildProperties(element))
        {
            Debug.Log(child);
            EditorGUILayout.PropertyField(
                child,
                includeChildren: true
            );
            EditorGUI.EndDisabledGroup();

        }

        m_SerializedObject.ApplyModifiedProperties();
    }

    protected override void Create()
    {
        BaseItem item = ScriptableObject.CreateInstance<BaseItem>();

        AssetDatabase.AddObjectToAsset(item, m_Database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        m_Items.Add(item);
        
        EditorUtility.SetDirty(m_Database);
    }

    protected override void Remove(BaseItem item)
    {
        int index = m_Items.IndexOf(item);
        
    }

    protected override void AddContextItem(GenericMenu menu)
    {
        base.AddContextItem(menu);
    }
}
