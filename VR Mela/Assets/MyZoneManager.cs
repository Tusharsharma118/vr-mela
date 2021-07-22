using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyZoneManager : MonoBehaviour
{
    public GameStats gameStats;
    public GameObject instructions;
    public GameObject sceneBackButton;

    public GameObject[] gameObjects;
    

    private int activeNumber;
    private bool firstTime;


    void Start()
    {
        activeNumber = 0;
        firstTime = true;
        PopulateObjects();

    }

    public void PopulateObjects()
    {
        string rewardString = "";
        foreach (string key in gameStats.CollectedRewards)
        {
            rewardString += key;
        }

        foreach (GameObject reward in gameObjects)
        {
            if (rewardString.Contains(reward.name))
            {
                activeNumber++;
                reward.SetActive(true);
            }
            else
            {
                reward.SetActive(false);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activeNumber == 0 && firstTime)
        {
            //set instructions as active
            firstTime = false;
            instructions.gameObject.SetActive(true);
            sceneBackButton.gameObject.SetActive(false);
        }
    }
}
