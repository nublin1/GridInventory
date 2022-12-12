using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridInventoryInspector
{
    private const float LIST_RESIZE_WIDTH = 10f;

    private bool m_StartDrag;
    private Rect m_DragRect = Rect.zero;

    protected Rect m_SidebarRect = new Rect(0, 30, 200, 1000);
    protected Vector2 m_SidebarScrollPosition;

    private List<string> m_Items = new List<string>() { "One", "Two", "Three" };

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
        DrawSidebar(new Rect(position.x, position.y, m_SidebarRect.width, position.height));
        DrawContent(new Rect(m_SidebarRect.width, m_SidebarRect.y, position.width - m_SidebarRect.width, position.height));
        ResizeSidebar();
    }

    private void DoToolbar()
    {
        EditorGUILayout.Space();
        
    }

    private void DrawSidebar(Rect position)
    {
        m_SidebarRect = position;

        GUILayout.BeginArea(m_SidebarRect, "");
        GUILayout.BeginHorizontal();
        GUILayout.Space(1f);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        m_SidebarScrollPosition = GUILayout.BeginScrollView(m_SidebarScrollPosition);
        List<Rect> rects = new List<Rect>();
        for (int i = 0; i < m_Items.Count; i++) {

            var currentItem = m_Items[i];

            using (var h = new EditorGUILayout.HorizontalScope(Styles.selectButton, GUILayout.Height(25f)))
            {
                Color backgroundColor = GUI.backgroundColor;
                Color textColor = Styles.selectButtonText.normal.textColor;
                GUI.backgroundColor = Styles.normalColor;

                GUI.Label(h.rect, GUIContent.none, Styles.selectButton);
                Rect rect = h.rect;
                rect.width -= LIST_RESIZE_WIDTH * 0.5f;
                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    GUI.FocusControl("");
                    //Select(currentItem);
                    this.m_StartDrag = true;
                    Event.current.Use();
                }

                //DrawItemLabel(i, currentItem);
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
                rects.Add(rect);
            }
        }

        for (int j = 0; j < rects.Count; j++)
        {

            Rect rect = rects[j];
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
        GUILayout.BeginArea(position, "");

        GUILayout.EndArea();
    }

    private void ResizeSidebar()
    {
        Rect rect = new Rect(m_SidebarRect.width - LIST_RESIZE_WIDTH * 0.5f, m_SidebarRect.y, LIST_RESIZE_WIDTH, m_SidebarRect.height);
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
    }
}