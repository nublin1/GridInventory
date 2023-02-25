using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CategoryEditor : BaseCollectionEditor<Category>
{
    public CategoryEditor(string title, ItemDatabase database) : base(title, database)
    {
        ToolbarName = title;
        m_Database = database;
        m_Items = m_Database.categories;
    }

    public override void OnEnable()
    {
        if (m_Items.Count > 0)
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

        if (m_Items.Count == 0) return;
        ObjectNames.SetNameSmart(m_Items[m_SelectedItemIndex], m_Items[m_SelectedItemIndex].Name);
    }

    protected override string GetSidebarLabel(Category item)
    {
        return item.Name;
    }

    protected override void DrawItem(Category item)
    {
        if (editor != null)
        {
            editor.OnInspectorGUI();
        }
    }

    protected override void Create()
    {
        Category item = ScriptableObject.CreateInstance<Category>();
        item.name = item.name;
        item.hideFlags = HideFlags.HideInHierarchy;

        AssetDatabase.AddObjectToAsset(item, m_Database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        m_Items.Add(item);
        Select(item);

        EditorUtility.SetDirty(m_Database);
    }

    protected override void Remove(Category item)
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

    protected override void Select(Category item)
    {
        base.Select(item);
        if (editor != null)
            ScriptableObject.DestroyImmediate(editor);

        editor = Editor.CreateEditor(item);

    }

    protected override bool MatchesSearch(Category item, string search)
    {
        if (item == null)
            return false;

        return (item.name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
    }
}
