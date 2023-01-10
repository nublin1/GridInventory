using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseCollectionEditor<T> : BaseEditor
{
    public string m_ToolbarName;
    protected ItemDatabase m_Database;
    
    private const float LIST_RESIZE_WIDTH = 10f;
    protected Rect m_SidebarRect = new Rect(0, 40, 200, 500);
    protected const float CONTENT_WIDTH = 450;

    protected Vector2 m_ScrollPosition;
    protected string m_SearchString = string.Empty;
    protected bool m_SearchStringShow = false;


    //private bool m_StartDrag;
    private Rect m_DragRect = Rect.zero;

    List<Rect> fields_Rects;

    protected List<T> m_Items = new List<T>();
    protected int m_SelectedItemIndex;
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

    protected Editor editor;
    private Vector2 m_SidebarScrollPosition;

    public BaseCollectionEditor(string title, ItemDatabase database)
    {
        this.m_ToolbarName = title;
        m_Database = database;
    }

    protected void DrawSidebar(Rect position)
    {
        m_SidebarRect = position;
        GUILayout.BeginArea(m_SidebarRect, "", EditorStyles.textArea);

        #region icons
        GUILayout.BeginHorizontal();

        GUIContent content = EditorGUIUtility.TrIconContent("CreateAddNew", "Create new item");
        if (GUILayout.Button(content, GUILayout.Width(35f), GUILayout.Height(35f)))
        {
            Create();
        }

        GUILayout.Space(1f);
        GUIContent contentFind = EditorGUIUtility.TrIconContent("BillboardRenderer Icon", "Find item");
        if (GUILayout.Button(contentFind, GUILayout.Width(35f), GUILayout.Height(35f)))
        {
            m_SearchStringShow = !m_SearchStringShow;
        }

        GUILayout.Space(1f);
        GUIContent contentSort = EditorGUIUtility.TrIconContent("AlphabeticalSorting", "Sort items");
        if (GUILayout.Button(contentSort, GUILayout.Width(35f), GUILayout.Height(35f)))
        {
            ShowSortMenu();
        }

        GUILayout.EndHorizontal();
        #endregion

        DoSearchGUI();
        EditorGUILayout.Space();

        m_SidebarScrollPosition = GUILayout.BeginScrollView(m_SidebarScrollPosition);

        fields_Rects = new List<Rect>();
        for (int i = 0; i < m_Items.Count; i++)
        {
            var currentItem = m_Items[i];

            if (!MatchesSearch(currentItem, m_SearchString) && m_SearchStringShow == true)
            {
                continue;
            }

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
                    GUI.FocusControl("");
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

        switch (Event.current.rawType)
        {
            case EventType.MouseDown:
                if (Event.current.button == 1)
                    for (int j = 0; j < fields_Rects.Count; j++)
                    {
                        if (fields_Rects[j].Contains(Event.current.mousePosition))
                        {
                            ShowContextMenu(m_Items[j]);
                            break;
                        }
                    }
                break;
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

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    protected virtual void DrawContent(Rect position)
    {
        GUILayout.BeginArea(position, "", EditorStyles.helpBox);
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUIStyle.none);
        if (selectedItem != null)
        {
            DrawItem(selectedItem);
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    protected virtual void Select(T item)
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
        if (m_SearchStringShow == false)
        {
            return;
        }

        m_SearchString = GUILayout.TextField(m_SearchString);
        Rect rect = GUILayoutUtility.GetLastRect();

        if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
        }
    }

    private void ShowContextMenu(T currentItem)
    {
        GenericMenu contextMenu = new GenericMenu();
        contextMenu.AddItem(new GUIContent("Delete"), false, delegate { Remove(currentItem); });

        AddContextItem(contextMenu);
        contextMenu.ShowAsContext();
    }

    protected virtual void ShowSortMenu()    {  }

    protected virtual void DrawItemLabel(int index, T currentItem)
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

    /// <summary>
    /// Checks for search.
    /// </summary>
    /// <returns><c>true</c>, if search was matchesed, <c>false</c> otherwise.</returns>
    /// <param name="item">Item.</param>
    /// <param name="search">Search.</param>
    protected abstract bool MatchesSearch(T item, string search);
}
