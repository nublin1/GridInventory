using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
{
    void LoadData(Dictionary<string, object> data);

    void SaveData(ref Dictionary<string, object> data);
}
