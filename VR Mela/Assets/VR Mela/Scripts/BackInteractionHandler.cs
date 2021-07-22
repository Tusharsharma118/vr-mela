using JMRSDK.InputModule;
using System;
using UnityEngine;

public class BackInteractionHandler : MonoBehaviour, IBackHandler
{

    // can only have objects that are off by default and one may be active
    public GameObject[] screens;
    public GameObject[] floatMenus;
    
    public void OnBackAction()
    {

        //backActionViewPort.SetActive(true);

        foreach(GameObject screen in screens)
        {
            // find better logic later
            //order matters here so make sure to put overlay screens first before anything else
            //i.e. instruction overlay comes before the actual screen its the instruction for

            if (screen.activeSelf)
            {
                Debug.Log("Currently Active Screen is ::" + screen.name);
                processScreen(screen);
                break;
            }
        }
        
    }

    private void processScreen(GameObject screen)
    {
        //shitty logic i know but i gotta do what i gotta do to get the project to work 
        if (screen.name.Equals("MainMenu"))
        {
            Debug.Log("Main Menu Page currently open");

           Array.Find(floatMenus, alert =>alert.name.Contains("QuitBox")).SetActive(true);
            return;
        }
        else if (screen.name.Equals("MatkaInstructions") || screen.name.Equals("TumblerInstructions"))
        {   
            Debug.Log("Matka Instructions Page currently open ;" + screen.gameObject.name);
            screen.transform.parent.gameObject.SetActive(false);
            screen.SetActive(false);
            Debug.Log("Set " + screen.gameObject.name);

            GameObject selector = Array.Find(floatMenus, menu => menu.name.Equals("Selector"));
            Debug.Log(selector.name);
            selector.SetActive(true);
            
            return;
        }
        else if (screen.name.Equals("GameModeSelector") || screen.name.Equals("RewardScreen") || screen.name.Equals("MyZone"))
        {
            Debug.Log("Game Mode Page currently open");

            screen.SetActive(false);
            Array.Find(screens, menu => menu.name.Contains("MainMenu")).SetActive(true);
            return;
        }
        else if (screen.name.Equals("MatkaGameOver"))
        {
            Debug.Log("Matka Game over screen currently open. Back does nothing!");
            return;
        }
        else if (screen.name.Equals("MatkaGame") )
        {
            Debug.Log("Matka Game Page currently open");

            //screen.SetActive(false);
            Array.Find(screens, menu => menu.name.Contains("MatkaPauseMenu")).SetActive(true);
            MatkaGameManager matkaManager = Array.Find(floatMenus, menu => menu.name.Contains("MatkaGameManager")).GetComponent<MatkaGameManager>();
            matkaManager.togglePause();
            return;
        }
        else if (screen.name.Equals("MatkaPauseMenu"))
        {
            Debug.Log("MatkaPauseMenu Page currently open");

            screen.SetActive(false);
            MatkaGameManager matkaManager = Array.Find(floatMenus, menu => menu.name.Contains("MatkaGameManager")).GetComponent<MatkaGameManager>();
            matkaManager.togglePause();
            return;
        }
        else if (screen.name.Equals("TumblerGameOver"))
        {
            Debug.Log("Tumbler Game over screen currently open. Back does nothing!");
            return;
        }
        else if (screen.name.Equals("TumblerGame"))
        {
            Debug.Log("TumblerGame  Page currently open");

            //screen.SetActive(false);
            Array.Find(screens, menu => menu.name.Contains("TumblerPauseMenu")).SetActive(true);
            TumblerGameManager tumblerManager = Array.Find(floatMenus, menu => menu.name.Contains("TumblerGameManager")).GetComponent<TumblerGameManager>();
            tumblerManager.TogglePause();
            return;
        }
        else if (screen.name.Equals("TumblerPauseMenu"))
        {
            Debug.Log("TumblerPauseMenu Page currently open");

            screen.SetActive(false);
            TumblerGameManager tumblerManager = Array.Find(floatMenus, menu => menu.name.Contains("TumblerGameManager")).GetComponent<TumblerGameManager>();
            tumblerManager.TogglePause();
            return;
        }
        else if (screen.name.Equals("QuitBox"))
        {
            Debug.Log("QuitBox Page currently open");
            screen.SetActive(false);
            return;
        }

        else if (screen.name.Equals("RewardInstructions"))
        {
            Debug.Log("Reward Instructions currently open. Now closing!");
            screen.SetActive(false);
            Array.Find(floatMenus, menu => menu.name.Contains("ShopBackButton")).SetActive(true);
            return;
        }

        else if (screen.name.Equals("MyZoneInstructions"))
        {
            Debug.Log("Reward Instructions currently open. Now closing!");
            screen.SetActive(false);
            Array.Find(floatMenus, menu => menu.name.Contains("MyZoneBackButton")).SetActive(true);
            return;
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        JMRInputManager.Instance.AddGlobalListener(gameObject);

    }

  
}
