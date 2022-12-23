using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPickerWindow : EditorWindow
{
    private static Styles m_Styles;

    public delegate void SelectCallbackDelegate(UnityEngine.Object obj);
    public SelectCallbackDelegate onSelectCallback;
    public delegate void CreateCallbackDelegate();
    public CreateCallbackDelegate onCreateCallback;
    private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> m_SelectableObjects;
    private Type m_Type;
    private bool m_SelectChildren = false;
    private bool m_AcceptNull;
    private UnityEngine.Object m_Root;

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
    }

    private void Header()
    {
        if (m_Styles == null)        
            m_Styles = new Styles();
        

        GUIContent content = new GUIContent(this.m_Root == null ? "Select " + ObjectNames.NicifyVariableName(this.m_Type.Name) : this.m_Root.name);
        Rect headerRect = GUILayoutUtility.GetRect(content, m_Styles.header);
        if (GUI.Button(headerRect, content, m_Styles.header))
        {
            this.m_Root = null;
        }
    }

    private static Rect GUIToScreenRect(Rect guiRect)
    {
        Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
        guiRect.x = vector.x;
        guiRect.y = vector.y;
        return guiRect;
    }

    private class Styles
    {
        public GUIStyle header = new GUIStyle("DD HeaderStyle");
        public GUIStyle rightArrow = "AC RightArrow";
        public GUIStyle leftArrow = "AC LeftArrow";
        public GUIStyle elementButton = new GUIStyle("MeTransitionSelectHead");
        public GUIStyle background = "grey_border";

        public Styles()
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
