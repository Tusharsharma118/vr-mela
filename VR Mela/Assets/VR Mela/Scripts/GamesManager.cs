using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesManager : MonoBehaviour
{
    public GameStats gameStats;
  
    public void QuitGame() {
        Debug.Log("QuitGameInvoked!!");
        Application.Quit();
    }

    public void SaveGame() {
        SaveManager.SaveState(gameStats);
    }

    public void LoadGame()
    {
        GameData gameData = SaveManager.LoadState();
        gameStats.playerTickets = gameData.playerTickets;
    }

}
