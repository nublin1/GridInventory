using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemCollectionInspector : EditorWindow
{
    private static ItemDatabase m_Database;
    private static List<BaseEditor> m_ChildEditors;

    private string[] toolbarNames = { "Items", "Rarity", "Categories" };
    private int toolbarIndex;

    [MenuItem("Tools/Items Editor", false, 1)]
    public static void ShowWindow()
    {
        var exited = Resources.FindObjectsOfTypeAll<ItemCollectionInspector>();
        ItemCollectionInspector editor;

        if (exited.Length <= 0)
        {
            editor = CreateInstance<ItemCollectionInspector>();
            editor.titleContent = new GUIContent("Inventory System");
            editor.Show();
        }
        else
        {
            editor = exited[0];
            editor.Show();
        }
    }

    private void OnEnable()
    {
        m_Database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(EditorPrefs.GetString("ItemDatabasePath"));
        UpdateChildEditors();        
    }

    private void OnDisable()
    {
        if (m_Database != null)
        {
            EditorPrefs.SetString("ItemDatabasePath", AssetDatabase.GetAssetPath(m_Database));
        }
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

        if (m_ChildEditors != null)
        {
            m_ChildEditors[toolbarIndex].OnGUI(position);
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
                    ItemDatabase db = GridInventorySystem.Utilities.CreateAsset<ItemDatabase>(true);
                    if (db != null)
                    {
                        CreateDefaultCategory(db);
                        m_Database = db;
                        UpdateChildEditors();
                    }
                });
        }
    }
    private void CreateDefaultCategory(ItemDatabase database)
    {  
        //category.hideFlags = HideFlags.HideInHierarchy;
        //AssetDatabase.AddObjectToAsset(category, database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();        
        EditorUtility.SetDirty(database);
    }

    private void UpdateChildEditors()
    {
        if (m_Database != null)
        {
            EditorUtility.SetDirty(m_Database);
            m_ChildEditors = new List<BaseEditor>
            {
                new ItemEditor("Test1", m_Database),
                new RariryEditor("Test2", m_Database),
                new CategoryEditor("Test3", m_Database)
            };

            foreach (BaseEditor editor in m_ChildEditors)
            {
                editor.OnEnable();
            }
        }
    }
}
