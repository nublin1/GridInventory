using GridInventorySystem;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ItemEditor : BaseCollectionEditor<BaseItem>
{
    private static GUIStyle customStyle;
    float cellSize = 50;

    private Vector2 m_ScrollPreviewPosition;

    public ItemEditor(string title, ItemDatabase _database) : base(title, _database)
    {
        ToolbarName = title;
        m_Database = _database;
        m_Items = m_Database.items;
    }

    [InitializeOnLoadMethod]
    private static void CreateCustomStyle()
    {
        // Create a new GUIStyle
        customStyle = new GUIStyle();

        // Set the font size, color, and padding of the style
        customStyle.fontSize = 14;
        customStyle.normal.textColor = Color.white;
        customStyle.padding = new RectOffset(5, 5, 3, 3);

        // Set the background image and text alignment of the style
        customStyle.normal.background = Texture2D.whiteTexture;
        customStyle.alignment = TextAnchor.MiddleCenter;

        // Add the style to the EditorStyles object
        //EditorStyles.AddCustomStyle(customStyle);
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
        DrawSidebar(new Rect(0, m_SidebarRect.y, m_SidebarRect.width, position.height - 30f));
        DrawContent(new Rect(m_SidebarRect.width, m_SidebarRect.y, m_contentWidth, position.height - 50f));

        DrawPrev(new Rect(m_SidebarRect.width + 450, m_SidebarRect.y, position.width - m_contentWidth - m_SidebarRect.width - 5f, position.height - 50f));

        ObjectNames.SetNameSmart(m_Items[m_SelectedItemIndex], m_Items[m_SelectedItemIndex].ItemName);
    }

    void DrawPrev(Rect position)
    {
        GUILayout.BeginArea(position, "", EditorStyles.helpBox);

        GUILayout.BeginHorizontal();
        cellSize = EditorGUILayout.Slider(cellSize, 1, 10, GUILayout.MaxWidth(250));
        GUILayout.EndHorizontal();

        var size = new Vector2Int((int)(50 * m_Items[m_SelectedItemIndex].Width), (int)(50 * m_Items[m_SelectedItemIndex].Height));

        Texture2D backgroundImage = new Texture2D(size.x, size.y);        
        // Fill the texture with the desired color
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
                backgroundImage.SetPixel(x, y, new Color(0, 0, 0, 0.15f));
        }
        backgroundImage.Apply();

        Texture backgroundOutlineImage = new Texture2D(size.x, size.y);
        backgroundOutlineImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Texture));

        var source = m_Items[m_SelectedItemIndex].Icon;

        //
            var pos = new Vector2(position.width / 2 - size.x/2, position.height / 2 - size.y / 2);
        //
        //m_ScrollPreviewPosition = GUILayout.BeginScrollView(m_ScrollPreviewPosition, GUIStyle.none);
        GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), backgroundImage);
        var def_Color = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.6f);
        GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), backgroundOutlineImage);
        GUI.color = def_Color;
        if (source != null)
            GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), source.texture);


       // GUILayout.EndScrollView();
        GUILayout.EndArea();
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
        if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + item.name + "?", "Yes", "No"))
        {
            GameObject.DestroyImmediate(item, true);
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
        contextSortMenu.AddItem(new GUIContent("Sort A->Z"), false, delegate
        {
            var selected = m_Items[m_SelectedItemIndex];
            m_Items.Sort(delegate (BaseItem a, BaseItem b) { return a.ItemName.CompareTo(b.ItemName); });
            Select(selected);
        });
        contextSortMenu.AddItem(new GUIContent("Sort Z->A"), false, delegate
        {
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

        return (item.name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
    }
}
