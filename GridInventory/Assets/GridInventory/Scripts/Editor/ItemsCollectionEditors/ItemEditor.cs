using GridInventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemEditor : BaseCollectionEditor<BaseItem>
{    
    public ItemEditor(string title, ItemDatabase _database) : base(title, _database)
    {
        ToolbarName = title;
        m_Database = _database;
        m_Items = m_Database.items;
    }

    public override void OnEnable()
    {
        Select(m_Items[0]);
        GUI.FocusControl("");
    }

    public override void OnDisable()
    {
        m_SearchString = "";
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
        return item.ItemName;
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
        item.name = item.ItemName;
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
            GameObject.DestroyImmediate(item,true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            m_Items.Remove(item);
            if (editor != null)
                ScriptableObject.DestroyImmediate(editor);
        }
    }

    protected override void AddContextItem(GenericMenu menu)
    {
        base.AddContextItem(menu);
    }

    protected override void ShowSortMenu()
    {
        base.ShowSortMenu();
        GenericMenu contextSortMenu = new GenericMenu();
        contextSortMenu.AddItem(new GUIContent("Sort A->Z"), false, delegate {
            var selected = m_Items[m_SelectedItemIndex];
            m_Items.Sort(delegate (BaseItem a, BaseItem b) { return a.ItemName.CompareTo(b.ItemName); });
            Select(selected);
        });
        contextSortMenu.AddItem(new GUIContent("Sort Z->A"), false, delegate {
            var selected = m_Items[m_SelectedItemIndex];
            m_Items.Sort(delegate (BaseItem a, BaseItem b) { return b.ItemName.CompareTo(a.ItemName); });
            Select(selected);
        });
        contextSortMenu.ShowAsContext();
    }

    protected override void Select(BaseItem item)
    {
        base.Select(item);
        if (editor != null)
            ScriptableObject.DestroyImmediate(editor);

        editor = Editor.CreateEditor(item);
        
    }

    protected override bool MatchesSearch(BaseItem item, string search)
    {
       if (item == null)
            return false;

        return (item.name.ToLower().Contains(search.ToLower()) ||search.ToLower() == item.GetType().Name.ToLower());
    }
}
