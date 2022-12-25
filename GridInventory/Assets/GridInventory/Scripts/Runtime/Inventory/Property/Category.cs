using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Category : ScriptableObject
{
    [SerializeField]
    private string m_CategoryName = "New Category";

    public string Name { get => m_CategoryName; set => m_CategoryName = value; }

    [SerializeField]
    private Color m_Color = Color.gray;
    public Color Color { get => m_Color; set => m_Color = value; }

    private void Awake()
    {
        name = m_CategoryName;
    }
}
