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
        if (editor != null)
        {
            editor.OnInspectorGUI();
        }
    }

    protected override void Create()
    {
        BaseItem item = ScriptableObject.CreateInstance<BaseItem>();
        item.name = item.itemName;
        item.hideFlags = HideFlags.HideInHierarchy;

        AssetDatabase.AddObjectToAsset(item, m_Database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        m_Items.Add(item);
        Select(item);

        EditorUtility.SetDirty(m_Database);
    }

    protected override void Remove(BaseItem item)
    {
        int index = m_Items.IndexOf(item);

        if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + item.name + "?", "Yes", "No"))
        {
            GameObject.DestroyImmediate(item, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            m_Items.Remove(item);
        }
    }

    protected override void AddContextItem(GenericMenu menu)
    {
        base.AddContextItem(menu);
    }

    protected override void Select(BaseItem item)
    {
        base.Select(item);
        if (editor != null)
            ScriptableObject.DestroyImmediate(editor);

        editor = Editor.CreateEditor(item);
    }
}
