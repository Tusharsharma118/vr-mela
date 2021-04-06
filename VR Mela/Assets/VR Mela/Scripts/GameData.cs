[System.Serializable]
public class GameData
{
    public float playerTickets;

    public GameData(GameStats gameStats) {
        this.playerTickets = gameStats.playerTickets;
    }

}
