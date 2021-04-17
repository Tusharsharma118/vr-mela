using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveManager
{
    static string basePath = Path.Combine(Application.persistentDataPath,"helixcry");

    public static void Save<T>(T objectToSave, string fileName)
    {
        Directory.CreateDirectory(basePath);
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(Path.Combine(basePath, fileName), FileMode.Create))
        {
            formatter.Serialize(fileStream, objectToSave);
        }
        Debug.Log("saved!");
    }

    public static T Load<T>(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        T returnValue = default(T);
        using (FileStream fileStream = new FileStream(Path.Combine(basePath, fileName), FileMode.Open))
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }
        return returnValue;
    }

    public static bool SaveExists(string fileName)
    {
        return File.Exists(Path.Combine(basePath, fileName));
    }

    public static void SeriouslyDeleteAllSaveFiles()
    {
        DirectoryInfo directory = new DirectoryInfo(basePath);
        directory.Delete(true);
        Directory.CreateDirectory(basePath);
    }


}
