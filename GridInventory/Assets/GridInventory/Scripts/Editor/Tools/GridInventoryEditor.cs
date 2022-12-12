using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridInventoryEditor : EditorWindow
{
    private GridInventoryInspector m_gridInventoryInspector;
    

    [MenuItem("Tools/TestEditor", false, 0)]
    public static void ShowWindow()
    {
        GridInventoryEditor[] objArray = Resources.FindObjectsOfTypeAll<GridInventoryEditor>();
        GridInventoryEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<GridInventoryEditor>() : objArray[0]);

        editor.minSize = new Vector2(690, 300);
        editor.titleContent = new GUIContent("Inventory System");

        editor.Show();
    }

    private void OnEnable()
    {
        m_gridInventoryInspector = new GridInventoryInspector();
    }

    private void OnDisable()
    {
        this.m_gridInventoryInspector.OnDisable();
    }

    private void OnDestroy()
    {
        this.m_gridInventoryInspector.OnDestroy();
    }

    private void OnGUI()
    {
        this.m_gridInventoryInspector.OnGUI(position);
    }
}
