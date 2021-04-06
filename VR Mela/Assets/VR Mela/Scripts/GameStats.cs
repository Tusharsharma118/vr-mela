using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public float playerTickets;

    //UI Methods
    public float getPlayerTickets() {
        return playerTickets;
     }

    public void incrementPlayerTickets(float amount) {
        this.playerTickets += amount;
    }

    public void decrementPlayerTickets(float amount) {
        this.playerTickets -= amount;
    }
}
