using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int playerTickets;
    public HashSet<string> collectedRewards { get; set; } = new HashSet<string>();

    public GameData(GameStats gameStats) {
        this.playerTickets = gameStats.playerTickets;
        this.collectedRewards = gameStats.CollectedRewards;
    }

}
