using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesManager : MonoBehaviour
{
    public GameStats gameStats;

    public void Awake() {
        if (SaveExists("stats")) {
            LoadGame();
        }
    }
    public void QuitGame() {
        Debug.Log("QuitGameInvoked!!");
        Application.Quit();
    }

    public void SaveGame() {
        SaveManager.Save(gameStats,"stats");
    }

    public void LoadGame()
    {
        GameData gameData = SaveManager.Load<GameData>("stats");
        gameStats.playerTickets = gameData.playerTickets;
        gameStats.CollectedRewards = gameData.collectedRewards;
    }

    public bool SaveExists(string key) {
       return  SaveManager.SaveExists("stats");
    }

}
