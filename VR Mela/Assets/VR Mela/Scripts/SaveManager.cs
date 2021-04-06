using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveManager
{
    static string basePath = Path.Combine(Application.persistentDataPath,"player.helixcry");

    public static void SaveState(GameStats gameStats) {

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(basePath, FileMode.Create);

        GameData gameData = new GameData(gameStats);

        formatter.Serialize(stream, gameData);

        stream.Close();
        
    }

    public static GameData LoadState() {

        if (File.Exists(basePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(basePath, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;

            stream.Close();
            return gameData;

        }
        else {
            Debug.LogError("Save data not found at path ::" + basePath);
            return null;
        }
    
    }


}
