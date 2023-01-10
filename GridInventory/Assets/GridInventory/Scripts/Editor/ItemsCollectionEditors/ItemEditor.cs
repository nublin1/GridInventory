using Codice.CM.Common;
using GridInventorySystem;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ItemEditor : BaseCollectionEditor<BaseItem>
{
    private static GUIStyle customStyle;
    float viewportScale = 1;
    float itemRot = 0;
    int fontSize = 10;

    private Vector2 m_ScrollPreviewPosition;

    Vector2 base_Size;
    private Texture2D backgroundImage;

    public ItemEditor(string title, ItemDatabase _database) : base(title, _database)
    {
        ToolbarName = title;
        m_Database = _database;
        m_Items = m_Database.items;
    }

    [InitializeOnLoadMethod]
    private static void CreateCustomStyle()
    {
        //// Create a new GUIStyle
        //customStyle = new GUIStyle();
        //
        //// Set the font size, color, and padding of the style
        //customStyle.fontSize = 14;
        //customStyle.normal.textColor = Color.white;
        //customStyle.padding = new RectOffset(5, 5, 3, 3);
        //
        //// Set the background image and text alignment of the style
        //customStyle.normal.background = Texture2D.whiteTexture;
        //customStyle.alignment = TextAnchor.MiddleCenter;
        //
        //// Add the style to the EditorStyles object
        ////EditorStyles.AddCustomStyle(customStyle);
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
        DrawContent(new Rect(m_SidebarRect.width, m_SidebarRect.y, CONTENT_WIDTH, position.height - 50f));

        DrawPrev(new Rect(m_SidebarRect.width + 450, m_SidebarRect.y, position.width - CONTENT_WIDTH - m_SidebarRect.width - 5f, position.height - 50f));

        ObjectNames.SetNameSmart(m_Items[m_SelectedItemIndex], m_Items[m_SelectedItemIndex].ItemName);
    }

    void DrawPrev(Rect position)
    {
        GUILayout.BeginArea(position, "", EditorStyles.helpBox);

        GUILayout.BeginHorizontal();
        viewportScale = EditorGUILayout.Slider(viewportScale, 1, 10, GUILayout.MaxWidth(250));
        //itemRot = EditorGUILayout.Slider(itemRot, 0, 90, GUILayout.MaxWidth(250));
        GUILayout.EndHorizontal();

       
        

        Texture backgroundOutlineImage = new Texture2D((int)base_Size.x, (int)base_Size.y);
        backgroundOutlineImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/GridInventory/GUI/Square Outline.png", typeof(Texture));
        var icon = m_Items[m_SelectedItemIndex].Icon;

        var scaled_Size = base_Size * viewportScale;

        // Draw preview  
        var s_rect = new Rect(0, 0, position.width - 10, position.height - 25);
        var pos = new Vector2(s_rect.width / 2 - scaled_Size.x / 2, s_rect.height / 2 - scaled_Size.y / 2);

        m_ScrollPreviewPosition = GUI.BeginScrollView(new Rect(10, 25, s_rect.width, s_rect.height), m_ScrollPreviewPosition, new Rect(pos.x, pos.y, s_rect.x + scaled_Size.x, s_rect.y + scaled_Size.y));

        //Vector2 pivotPoint = new Vector2(pos.x + scaled_Size.x/2, pos.y + scaled_Size.y/2);
        //GUIUtility.RotateAroundPivot(itemRot, pivotPoint);

        GUI.DrawTexture(new Rect(pos.x, pos.y, scaled_Size.x, scaled_Size.y), backgroundImage);
        var def_Color = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.6f);
        GUI.DrawTexture(new Rect(pos.x, pos.y, scaled_Size.x, scaled_Size.y), backgroundOutlineImage);
        GUI.color = def_Color;
        if (icon != null)
            GUI.DrawTexture(new Rect(pos.x, pos.y, scaled_Size.x, scaled_Size.y), icon.texture);

        // Draw text
        GUI.skin.label.fontSize = (int)(fontSize * viewportScale);
        GUI.skin.label.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(pos.x, pos.y, scaled_Size.x, scaled_Size.y), m_Items[m_SelectedItemIndex].ItemName);
        GUI.skin.label.alignment = TextAnchor.LowerRight;
        var stack_str = m_Items[m_SelectedItemIndex].Stack.ToString();
        if (m_Items[m_SelectedItemIndex].ShowMaxStack)
            stack_str = stack_str + "/" + m_Items[m_SelectedItemIndex].MaxStack.ToString();
        if (m_Items[m_SelectedItemIndex].Stack > 1)
            GUI.Label(new Rect(pos.x, pos.y, scaled_Size.x, scaled_Size.y), stack_str);

        //GUIUtility.RotateAroundPivot(-itemRot, pivotPoint);
        GUI.EndScrollView();

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

        RebuildPreviewImages();
    }

    protected override bool MatchesSearch(BaseItem item, string search)
    {
        if (item == null)
            return false;

        return (item.name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
    }

    private void RebuildPreviewImages()
    {
        base_Size = new Vector2((int)(50 * m_Items[m_SelectedItemIndex].Width), (int)(50 * m_Items[m_SelectedItemIndex].Height));
        backgroundImage = new Texture2D((int)base_Size.x, (int)base_Size.y);

        Color color;
        if (m_Items[m_SelectedItemIndex].IsCategoryBasedColor && m_Items[m_SelectedItemIndex].Category != null)
            color = m_Items[m_SelectedItemIndex].Category.Color;
        else
            color = m_Items[m_SelectedItemIndex].BackgroundColor;
         
        Color[] colors = Enumerable.Repeat(color, (int)base_Size.x * (int)base_Size.y).ToArray();
        // Fill the texture with the desired color
        backgroundImage.SetPixels(colors);
        backgroundImage.Apply();
    }
}
