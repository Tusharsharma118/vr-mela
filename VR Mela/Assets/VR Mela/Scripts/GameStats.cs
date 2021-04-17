using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public int playerTickets;
    public HashSet<string> CollectedRewards { get; set; } = new HashSet<string>();
    private Reward[] rewards;
    public GameObject rewardsCarousel;

    private void Awake() {
        rewards = rewardsCarousel.GetComponent<Carousel>().rewards;
        CollectedRewards = new HashSet<string>();
        updateCollectedRewards();
    }

    private void updateCollectedRewards()
    {
        rewards = rewardsCarousel.GetComponent<Carousel>().rewards;
        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i].isBought)
            {
                CollectedRewards.Add(rewards[i].getId());
            }
        }
    }

    public void addRewardtoOwned(String id) {
        if (CollectedRewards == null) {
            CollectedRewards = new HashSet<string>();
        }
        CollectedRewards.Add(id);
    }

    //UI Methods
    public float getPlayerTickets() {
        return playerTickets;
     }

    public void incrementPlayerTickets(int amount) {
        this.playerTickets += amount;
    }

    public void decrementPlayerTickets(int amount) {
        this.playerTickets -= amount;
    }
}
