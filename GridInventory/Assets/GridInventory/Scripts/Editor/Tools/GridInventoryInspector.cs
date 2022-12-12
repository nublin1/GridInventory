using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridInventoryInspector
{
    private const float LIST_RESIZE_WIDTH = 10f;

    //private bool m_StartDrag;
    private Rect m_DragRect = Rect.zero;

    protected Rect m_SidebarRect = new Rect(0, 10, 200, 500);
    protected Vector2 m_SidebarScrollPosition;

    private List<string> m_Items = new List<string>() { "One", "Two", "Three" };

    List<Rect> fields_Rects;

    int m_SelectedItemIndex;
    string selectedItem
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

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    public void OnDestroy()
    {

    }

    public void OnGUI(Rect position)
    {
        DrawSidebar(new Rect(0, 0, m_SidebarRect.width, position.height));
        //DrawContent(new Rect(m_SidebarRect.width, 0, 350, position.height));
        //ResizeSidebar();
    }

    private void DoToolbar()
    {
        EditorGUILayout.Space();

    }

    private void DrawSidebar(Rect position)
    {
        m_SidebarRect = position;

        GUILayout.BeginArea(m_SidebarRect, "", EditorStyles.textArea);
        GUILayout.BeginHorizontal();
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

                DrawItemLabel(i);
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

        //GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    protected virtual void DrawContent(Rect position)
    {
        GUILayout.BeginArea(position, "");
        GUILayout.Label("Test Content");
        GUILayout.EndArea();
    }

    private void ResizeSidebar()
    {
        Rect rect = new Rect(m_SidebarRect.width - LIST_RESIZE_WIDTH * 0.5f, m_SidebarRect.y, LIST_RESIZE_WIDTH, m_SidebarRect.height);
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
    }

    void DrawItemLabel(int index)
    {
        GUILayout.BeginHorizontal();
        Color color = Color.green;
        //GUI.backgroundColor = color;
        GUILayout.Label("Test Name", Styles.selectButtonText);
        GUILayout.EndHorizontal();
    }

    void Select(string item)
    {
        int index = m_Items.IndexOf(item);
        if (this.m_SelectedItemIndex != index)
        {
            this.m_SelectedItemIndex = index;
            //this.m_ScrollPosition.y = 0f;
        }
    }
}