using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistentManager : MonoBehaviour
{
    [Header("Fole Storage Config")]
    [SerializeField]
    private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler fileDataHandler;

    public static DataPersistentManager instance { get; private set; }

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistencesObjects = FindAllIDataPersistence();
        LoadGame();
    }
   

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref gameData.m_data);
        }        

        fileDataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        gameData= fileDataHandler.Load(); 

        if(gameData == null)
        {
            Debug.Log("No data was found");
            NewGame();
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistencesObjects) {
            dataPersistenceObj.LoadData(gameData.m_data);
        }

        gameData.m_data.Clear();
        
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
