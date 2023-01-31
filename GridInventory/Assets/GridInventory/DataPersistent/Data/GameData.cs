using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{   
    public Dictionary<string, object> m_data ;

    public GameData()
    {
        m_data = new Dictionary<string, object>();
    }
}
