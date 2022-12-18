using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemCollectionInspector : EditorWindow
{
    private static ItemDatabase m_Database;
    private static List<BaseEditor> m_ChildEditors;


    private string[] toolbarNames = {"Items", "Rarity", "Categories" };
    private int toolbarIndex;

    [MenuItem("Tools/Items Editor", false, 1)]
    public static void ShowWindow()
    {

        ItemCollectionInspector editor = CreateInstance<ItemCollectionInspector>();

        editor.minSize = new Vector2(600, 300);
        editor.titleContent = new GUIContent("Inventory System");

        editor.Show();
    }

    private void OnEnable()
    {
        UpdateChildEditors();
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }

    private void Update()
    {
        if (EditorWindow.mouseOverWindow == this)
            Repaint();
    }

    private void CreateGUI()
    {

    }

    private void OnGUI()
    {
        Toolbar();

        if (m_ChildEditors == null)
            return;

        foreach (BaseEditor editor in m_ChildEditors)
        {
            editor.OnGUI(position);
        }
    }

    private void Toolbar()
    {
        GUILayout.BeginHorizontal();
        Button_SelectDatabase();

        GUILayout.EndHorizontal();
        if (m_ChildEditors != null)
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarNames, GUILayout.MinWidth(200));
    }

    private void Button_SelectDatabase()
    {
        GUIStyle buttonStyle = EditorStyles.objectField;
        GUIContent buttonContent = new GUIContent(m_Database != null ? m_Database.name : "Null");
        Rect buttonRect = GUILayoutUtility.GetRect(180f, 18f);

        if (GUI.Button(buttonRect, buttonContent, buttonStyle))
        {
            PickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase),
                (UnityEngine.Object obj) =>
                {
                    m_Database = obj as ItemDatabase;
                    UpdateChildEditors();
                },
                () =>
                {
                    //ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
                    //if (db != null)
                    //{
                    //    CreateDefaultCategory(db);
                    //    this.m_Database = db;
                    //    ResetChildEditors();
                    //}
                });
        }
    }

    private void UpdateChildEditors()
    {
        if (m_Database != null)
        {
            EditorUtility.SetDirty(m_Database);
            m_ChildEditors = new List<BaseEditor>();
            m_ChildEditors.Add(new ItemEditor("Test1", m_Database));
            
            foreach(BaseEditor editor in m_ChildEditors)
            {
                editor.OnEnable();
            }
        }
    }
}
