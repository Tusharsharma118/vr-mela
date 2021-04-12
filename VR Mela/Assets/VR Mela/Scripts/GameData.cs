using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public float playerTickets;
    public HashSet<string> collectedRewards { get; private set; } = new HashSet<string>();

    public GameData(GameStats gameStats) {
        this.playerTickets = gameStats.playerTickets;
        this.collectedRewards = gameStats.CollectedRewards;
    }

}
