using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesManager : MonoBehaviour
{
    public GameStats gameStats;
    public GameData gameData;
    public GameObject mainMenu;

    public GameObject introPlayer;
    public float timer = 12f;

    private bool firstTime = true;

    public void Awake() {
        if (SaveExists("stats")) {
            LoadGame();
        }
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0 && firstTime)
        {
            firstTime = false;
            introPlayer.SetActive(false);
            mainMenu.SetActive(true);
        }

       
    }
    public void QuitGame() {
        Debug.Log("QuitGameInvoked!!");
        Application.Quit();
    }

    public void SaveGame() {
        gameData.playerTickets = gameStats.playerTickets;
        gameData.collectedRewards = gameStats.CollectedRewards;
        Debug.Log("Saving Following Data: PlayerTickets:: " + gameData.playerTickets);
        foreach (string key in gameData.collectedRewards) {
            Debug.Log("HashSet Element:: " + key);
        }
        SaveManager.Save(gameData,"stats");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void LoadGame()
    {
        gameData = SaveManager.Load<GameData>("stats");
        Debug.Log("Loading Following Data: PlayerTickets:: " + gameData.playerTickets);
        foreach (string key in gameData.collectedRewards)
        {
            Debug.Log("HashSet Element:: " + key);
        }
        gameStats.playerTickets = gameData.playerTickets;
        gameStats.CollectedRewards = gameData.collectedRewards;
    }

    public bool SaveExists(string key) {
       return  SaveManager.SaveExists("stats");
    }

}
