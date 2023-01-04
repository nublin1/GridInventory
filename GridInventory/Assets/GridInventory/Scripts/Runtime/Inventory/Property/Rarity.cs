using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rarity : ScriptableObject
{
    [SerializeField]
    private string m_RarityName = "New Rarity";

    public string Name { get => m_RarityName; set => m_RarityName = value; }

    [SerializeField]
    private Color m_Color = Color.gray;
    public Color Color { get => m_Color; set => m_Color = value; }

    private void Awake()
    {
        name = m_RarityName;
    }
}
