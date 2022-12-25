using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PickerWindow : EditorWindow
{
    private static Style style;

    private string m_SearchString = string.Empty;
    private bool isSearching
    {
        get
        {
            return !string.IsNullOrEmpty(m_SearchString);
        }
    }

    private UnityEngine.Object m_Root;
    private Vector2 m_ScrollPosition;
    private Type m_Type;
    private bool m_SelectChildren = false;
    private bool m_AcceptNull;
    public delegate void SelectCallbackDelegate(UnityEngine.Object obj);
    public SelectCallbackDelegate onSelectCallback;
    public delegate void CreateCallbackDelegate();
    public CreateCallbackDelegate onCreateCallback;
    private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> m_SelectableObjects;

    public static void ShowWindow(Rect buttonRect, Type type, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull = false)
    {
        PickerWindow window = CreateInstance<PickerWindow>();
        buttonRect = GUIToScreenRect(buttonRect);
        window.m_Type = type;
        window.BuildSelectableObjects(type);
        window.onSelectCallback = selectCallback;
        window.onCreateCallback = createCallback;
        window.m_AcceptNull = acceptNull;
        window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
    }

    private static Rect GUIToScreenRect(Rect guiRect)
    {
        Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
        guiRect.x = vector.x;
        guiRect.y = vector.y;
        return guiRect;
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {        
        Header();
        DrawSelectableObjects();
    }

    private void Header()
    {
        if (style == null)
            style = new Style();

        GUIContent content = new GUIContent(this.m_Root == null ? "Select " + ObjectNames.NicifyVariableName(this.m_Type.Name) : this.m_Root.name);
        Rect headerRect = GUILayoutUtility.GetRect(content, style.header, GUILayout.Height(30));
        if (GUI.Button(headerRect, content, style.header))
        {
            this.m_Root = null;
        }

        // Draw a horizontal line
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, Color.gray);
    }

    private void DrawSelectableObjects()
    {
        List<UnityEngine.Object> selectableObjects = this.m_Root == null ? this.m_SelectableObjects.Keys.ToList() : this.m_SelectableObjects[this.m_Root];

        this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition);

        foreach (UnityEngine.Object obj in selectableObjects)
        {
            Color backgroundColor = GUI.backgroundColor;
            Color textColor = Styles.selectButtonText.normal.textColor;
            GUI.backgroundColor = Styles.normalColor;

            GUIContent label = new GUIContent(obj.name);
            Rect rect = GUILayoutUtility.GetRect(label, Styles.elementButton, GUILayout.Height(20f));
            GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0.2f, 0.5f, 0, 0.0f));

            if (GUI.Button(rect, label, style.elementButton))
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
           
        }

        if (this.m_Root == null)
        {
            GUI.backgroundColor = Styles.normalColor;

            GUIContent createContent = new GUIContent("Create New " + this.m_Type.Name);
            Rect rect1 = GUILayoutUtility.GetRect(createContent, Styles.elementButton, GUILayout.Height(20f));
            GUI.backgroundColor = (rect1.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0.2f, 0.5f, 0, 0.0f));

            if (GUI.Button(rect1, createContent, style.elementButton))
            {
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void BuildSelectableObjects(Type type)
    {
        this.m_SelectableObjects = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();

        string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, type);
            this.m_SelectableObjects.Add(obj, new List<UnityEngine.Object>());
        }

        //Debug.Log(m_SelectableObjects);
    }

    private class Style
    {
        public GUIStyle header = new GUIStyle("HeaderStyle");
        public GUIStyle elementButton = new GUIStyle("MeTransitionSelectHead");

        public Style()
        {
            header.alignment = TextAnchor.MiddleCenter;
            header.fontSize = 16;
            header.fontStyle = FontStyle.Bold;
            header.stretchWidth = true;
            header.margin = new RectOffset(1, 1, 0, 4);
            header.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
            header.border = new RectOffset(1, 1, 1, 1);

            elementButton.alignment = TextAnchor.MiddleLeft;
            elementButton.padding.left = 22;
            elementButton.margin = new RectOffset(1, 1, 0, 0);
            elementButton.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
        }
    }

}
