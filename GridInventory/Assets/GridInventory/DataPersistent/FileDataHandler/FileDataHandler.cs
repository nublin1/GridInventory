using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{

    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {                       
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (dataToLoad != null)
                {
                    loadedData = new GameData();
                    loadedData.m_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataToLoad);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("load data error " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamWriter writer = File.CreateText(fullPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data.m_data);
            }
        }

        catch (Exception e)
        {
            Debug.LogError("save data error " + fullPath + "\n" + e);
        }
    }
}
