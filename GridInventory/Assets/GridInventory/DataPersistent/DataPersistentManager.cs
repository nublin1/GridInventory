using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistentManager : MonoBehaviour
{
    private GameData gameData;
    private List<IDataPersistence> dataPersistencesObjects;

    public static DataPersistentManager instance { get; private set; }

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        dataPersistencesObjects = FindAllIDataPersistence();
    }

   

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void SaveGame()
    {

    }

    public void LoadGame()
    {
        if(gameData == null)
        {
            Debug.Log("No data was found");
            NewGame();
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistencesObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
        
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllIDataPersistence()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistencesObjects);
    }
}
