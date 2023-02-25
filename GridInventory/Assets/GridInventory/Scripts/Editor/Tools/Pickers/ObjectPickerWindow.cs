using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectPickerWindow : EditorWindow
{
    private static Style m_Styles;
    private string m_SearchString = string.Empty;
    private bool isSearching
    {
        get
        {
            return !string.IsNullOrEmpty(m_SearchString);
        }
    }

    public delegate void SelectCallbackDelegate(UnityEngine.Object obj);
    public SelectCallbackDelegate onSelectCallback;
    public delegate void CreateCallbackDelegate();
    public CreateCallbackDelegate onCreateCallback;
    private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> m_SelectableObjects;
    private Type m_Type;
    private bool m_SelectChildren = false;
    private bool m_AcceptNull;
    private UnityEngine.Object m_Root;
    private Vector2 m_ScrollPosition;

    public static void ShowWindow(Rect buttonRect, Type type, Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects,
        ObjectPickerWindow.SelectCallbackDelegate selectCallback, ObjectPickerWindow.CreateCallbackDelegate createCallback, bool acceptNull = false)
    {
        ObjectPickerWindow window = ScriptableObject.CreateInstance<ObjectPickerWindow>();
        buttonRect = GUIToScreenRect(buttonRect);
        window.m_SelectableObjects = selectableObjects;
        window.m_Type = type;
        window.m_SelectChildren = true;
        window.onSelectCallback = selectCallback;
        window.onCreateCallback = createCallback;
        window.m_AcceptNull = acceptNull;
        window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
    }    

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Space(5f);
        Header();
        DrawSelectableObjects();
    }

    private void Header()
    {
        if (m_Styles == null)        
            m_Styles = new Style();
        

        GUIContent content = new GUIContent(this.m_Root == null ? "Select " + ObjectNames.NicifyVariableName(this.m_Type.Name) : this.m_Root.name);
        Rect headerRect = GUILayoutUtility.GetRect(content, m_Styles.header);
        if (GUI.Button(headerRect, content, m_Styles.header))
        {
            this.m_Root = null;
        }
    }

    private void DrawSelectableObjects()
    {       

        List<UnityEngine.Object> selectableObjects = m_Root == null ? m_SelectableObjects.Keys.ToList() : m_SelectableObjects[m_Root];        

        m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition);
        foreach (UnityEngine.Object obj in selectableObjects)
        {
            if (!SearchMatch(obj))
                continue;

            Color backgroundColor = GUI.backgroundColor;
            Color textColor = ObjectPickerWindow.m_Styles.elementButton.normal.textColor;
            int padding = ObjectPickerWindow.m_Styles.elementButton.padding.left;
            GUIContent label = new GUIContent(obj.name);
            Rect rect = GUILayoutUtility.GetRect(label, ObjectPickerWindow.m_Styles.elementButton, GUILayout.Height(20f));
            GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));
            ObjectPickerWindow.m_Styles.elementButton.normal.textColor = (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);

            //Texture2D icon = EditorGUIUtility.LoadRequired("d_ScriptableObject Icon") as Texture2D;
            //IconAttribute iconAttribute = obj.GetType().GetCustomAttribute<IconAttribute>();
            //if (iconAttribute != null)
            //{
            //    if (iconAttribute.type != null)
            //    {
            //        icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.type);
            //    }
            //    else
            //    {
            //        icon = Resources.Load<Texture2D>(iconAttribute.path);
            //    }
            //}

            // ObjectPickerWindow.m_Styles.elementButton.padding.left = (icon != null ? 22 : padding);
             ObjectPickerWindow.m_Styles.elementButton.padding.left = ( padding);

            if (GUI.Button(rect, label, ObjectPickerWindow.m_Styles.elementButton))
            {
                if (this.m_Root != null && this.m_SelectableObjects[this.m_Root].Count > 0)
                {
                    onSelectCallback?.Invoke(obj);
                    Close();
                }
                this.m_Root = obj;
                if (!this.m_SelectChildren)
                {
                    onSelectCallback?.Invoke(this.m_Root);
                    Close();
                }

            }
            GUI.backgroundColor = backgroundColor;
            ObjectPickerWindow.m_Styles.elementButton.normal.textColor = textColor;
            ObjectPickerWindow.m_Styles.elementButton.padding.left = padding;

            //if (icon != null)
            //{
                GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), EditorGUIUtility.LoadRequired("ScriptableObject Icon") as Texture2D);
            //}
        }

        if (this.m_Root == null)
        {
            if (this.m_AcceptNull)
            {
                GUIContent nullContent = new GUIContent("Null");
                Rect rect2 = GUILayoutUtility.GetRect(nullContent, ObjectPickerWindow.m_Styles.elementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect2.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

                if (GUI.Button(rect2, nullContent, ObjectPickerWindow.m_Styles.elementButton))
                {
                    onSelectCallback?.Invoke(null);
                    Close();
                }
                GUI.Label(new Rect(rect2.x, rect2.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);
            }

            GUIContent createContent = new GUIContent("Create New " + this.m_Type.Name);
            Rect rect1 = GUILayoutUtility.GetRect(createContent, ObjectPickerWindow.m_Styles.elementButton, GUILayout.Height(20f));
            GUI.backgroundColor = (rect1.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

            if (GUI.Button(rect1, createContent, ObjectPickerWindow.m_Styles.elementButton))
            {
                onCreateCallback?.Invoke();
                Close();
            }
            GUI.Label(new Rect(rect1.x, rect1.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);


        }

        if (selectableObjects.Count == 1 && m_Root == null)
        {
            m_Root = selectableObjects[0];
        }
        EditorGUILayout.EndScrollView();
    }

    private bool SearchMatch(UnityEngine.Object element)
    {
        if (isSearching && (element == null || !element.name.ToLower().Contains(this.m_SearchString.ToLower())))
        {
            return false;
        }
        return true;
    }

    private static Rect GUIToScreenRect(Rect guiRect)
    {
        Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
        guiRect.x = vector.x;
        guiRect.y = vector.y;
        return guiRect;
    }

    private class Style
    {
        public GUIStyle header = new GUIStyle("DD HeaderStyle");        
        public GUIStyle elementButton = new GUIStyle("MeTransitionSelectHead");        

        public Style()
        {

            this.header.stretchWidth = true;
            this.header.margin = new RectOffset(1, 1, 0, 4);

            this.elementButton.alignment = TextAnchor.MiddleLeft;
            this.elementButton.padding.left = 22;
            this.elementButton.margin = new RectOffset(1, 1, 0, 0);
            elementButton.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
        }
    }
}
