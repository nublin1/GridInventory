using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseCollectionEditor<T> : BaseEditor
{
    public string m_ToolbarName;
    protected ItemDatabase m_Database;

    private const float LIST_MIN_WIDTH = 200f;
    private const float LIST_MAX_WIDTH = 400f;
    private const float LIST_RESIZE_WIDTH = 10f;

    protected Rect m_SidebarRect = new Rect(0, 50, 200, 500);

    protected Vector2 m_ScrollPosition;
    protected string m_SearchString = string.Empty;



    //private bool m_StartDrag;
    private Rect m_DragRect = Rect.zero;

    List<Rect> fields_Rects;

    protected List<T> m_Items = new List<T>();
    int m_SelectedItemIndex;
    T selectedItem
    {
        get
        {
            if (m_SelectedItemIndex > -1 && m_SelectedItemIndex < m_Items.Count)
            {
                return m_Items[m_SelectedItemIndex];
            }
            return default;
        }
    }

    public BaseCollectionEditor(string title, ItemDatabase database)
    {
        this.m_ToolbarName = title;
        m_Database = database;
        m_Items = m_Database.items as List<T>;
    }

    protected void DrawSidebar(Rect position)
    {
        m_SidebarRect = position;

        GUILayout.BeginArea(m_SidebarRect, "", EditorStyles.textArea);
        GUILayout.BeginHorizontal();

        GUIContent content = EditorGUIUtility.IconContent("CreateAddNew");
        if (GUILayout.Button(content, GUILayout.Width(35f)))
        {
            Create();
        }

        GUILayout.Space(1f);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        fields_Rects = new List<Rect>();
        for (int i = 0; i < m_Items.Count; i++)
        {
            var currentItem = m_Items[i];

            using (var h = new EditorGUILayout.HorizontalScope(Styles.selectButton, GUILayout.Height(25f)))
            {
                Color backgroundColor = GUI.backgroundColor;
                Color textColor = Styles.selectButtonText.normal.textColor;
                GUI.backgroundColor = Styles.normalColor;

                if (selectedItem != null && selectedItem.Equals(currentItem))
                {
                    GUI.backgroundColor = Styles.activeColor;
                    Styles.selectButtonText.normal.textColor = Color.white;
                    Styles.selectButtonText.fontStyle = FontStyle.Bold;
                }
                else if (h.rect.Contains(Event.current.mousePosition))
                {
                    GUI.backgroundColor = Styles.hoverColor;
                    Styles.selectButtonText.normal.textColor = textColor;
                    Styles.selectButtonText.fontStyle = FontStyle.Normal;
                }

                GUI.Label(h.rect, GUIContent.none, Styles.selectButton);
                Rect rect = h.rect;
                rect.width -= LIST_RESIZE_WIDTH * 0.5f;
                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Select(currentItem);
                }

                DrawItemLabel(i, currentItem);
                //string error = HasConfigurationErrors(currentItem);
                //if (!string.IsNullOrEmpty(error))
                //{
                //    GUI.backgroundColor = Styles.warningColor;
                //    Rect errorRect = new Rect(h.rect.width - 20f, h.rect.y + 4.5f, 16f, 16f);
                //    GUI.Label(errorRect, new GUIContent("", error), (GUIStyle)"CN EntryWarnIconSmall");
                //}

                GUI.backgroundColor = backgroundColor;
                Styles.selectButtonText.normal.textColor = textColor;
                Styles.selectButtonText.fontStyle = FontStyle.Normal;
                fields_Rects.Add(rect);
            }
        }

        for (int j = 0; j < fields_Rects.Count; j++)
        {

            Rect rect = fields_Rects[j];
            Rect rect1 = new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
            Rect rect2 = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width, rect.height * 0.5f);

            if (rect1.Contains(Event.current.mousePosition))
            {
                m_DragRect = rect;
                m_DragRect.y = rect.y + 10f - 25f;
                m_DragRect.x = rect.x + 5f;
                break;
            }
            else if (rect2.Contains(Event.current.mousePosition))
            {
                m_DragRect = rect;
                m_DragRect.y = rect.y + 10f;
                m_DragRect.x = rect.x + 5f;

                break;
            }
            else
            {
                m_DragRect = Rect.zero;
            }
        }

        GUILayout.EndArea();
    }

    protected virtual void DrawContent(Rect position)
    {
        GUILayout.BeginArea(position, "", EditorStyles.helpBox);
        if (selectedItem != null)
        {
            DrawItem(selectedItem);
        }
        GUILayout.EndArea();
    }

    void Select(T item)
    {
        int index = m_Items.IndexOf(item);
        if (this.m_SelectedItemIndex != index)
        {
            this.m_SelectedItemIndex = index;
            this.m_ScrollPosition.y = 0f;
        }
    }

    protected virtual void DoSearchGUI()
    {
        //m_SearchString = EditorTools.SearchField(m_SearchString);
    }

    private void ShowContextMenu(T currentItem)
    {
        GenericMenu contextMenu = new GenericMenu();

        contextMenu.AddItem(new GUIContent("Delete"), false, delegate { Remove(currentItem); });

        AddContextItem(contextMenu);
        contextMenu.ShowAsContext();
    }

    void DrawItemLabel(int index, T currentItem)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(index + ": " + GetSidebarLabel(currentItem), Styles.selectButtonText, GUILayout.Width(17), GUILayout.Height(17));
        GUILayout.EndHorizontal();

    }

    protected abstract string GetSidebarLabel(T item);

    protected abstract void DrawItem(T item);

    /// <summary>
    /// Create an item.
    /// </summary>
    protected virtual void Create() { }

    /// <summary>
    /// Remove the specified item from collection.
    /// </summary>
    /// <param name="item">Item.</param>
    protected virtual void Remove(T item) { }

    protected virtual void AddContextItem(GenericMenu menu) { }
}
