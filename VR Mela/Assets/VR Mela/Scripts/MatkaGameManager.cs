using UnityEngine;
using JMRSDK.InputModule;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MatkaGameManager : MonoBehaviour, ISelectHandler
{
    public GameStats gameStats;
    public GameObject potsPrefab;
    public GameObject pebbleCounterDisplay;
    public float patharCount;
    public GameObject gulel;
    public GameObject patharPrefab;
    public GameObject scoreTrackerDisplay;
    public GameObject gameOverParent;

    private Ray controllerRay;
    private RaycastHit hit;

    public float score;

    public int potCounter;

    private bool firstTime = true;

    private GameObject pots;

    public bool isPaused = false;

    private GameObject pebble;

    public bool firingAllowed = true;

    private bool enumRunning = false;

    public void Start()
    {
        pebble = null;
        // implicitly fixed for game restart
        patharCount = 20;
        potCounter = 5;
        enumRunning = false;
        firstTime = true;
        firingAllowed = true;
        JMRInputManager.Instance.RemoveGlobalListener(gameObject);
        JMRInputManager.Instance.AddGlobalListener(gameObject);
        updatePebbleCounter();
        GameObject pots = Instantiate(potsPrefab,new Vector3(0.02f,0f,2.46f),potsPrefab.transform.rotation);
        pots.transform.SetParent(this.transform.parent);

        GameObject[] potSet = GameObject.FindGameObjectsWithTag("PotConfiguration");
        if(potSet.Length > 1)
        {
            Destroy(potSet[0]);
        }
        score = 0;
        updateScoreTracker();
    }

    public void Update()
    {

        Debug.Log("Checking for EnumRunning! PotCounter Current Val :: " + potCounter + " PatharCount ::" + patharCount);
        if (!enumRunning)
        {
            Debug.Log("CoRoutine Not Running!");
            EndGameCheck();
        }
    }

    private void EndGameCheck()
    {
        Debug.Log("Inside EndGame check");
        if (patharCount <= 0 || potCounter <= 0)
        {
            
            pebble = GameObject.FindGameObjectWithTag("Pebble");
            // first time makes it so it only works once per game cycle as it is inside update
            Debug.Log("Pots Deleted or Pathar Count = 0" + pebble + "first time value = "+firstTime);
            if (pebble == null && firstTime)
            {
                firstTime = false;
                Debug.Log("Game Over");
                // Insert Display banner here
                gameOverParent.SetActive(true);
                Transform parent = gameOverParent.transform;
                //update global ticket counter
                gameStats.incrementPlayerTickets((int)score);
                Debug.Log("Saving User Stats with updated ticket count" + gameStats.playerTickets);
                // now save the new ticket count to device
                //save current ticket count and Object Stats to device storage
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GamesManager>().SaveGame();
                //find the UI Element and change its text
                Debug.Log("Saved Game");
                foreach (Transform tr in parent)
                {

                    if (tr.tag == "UIElement")
                    {
                        tr.GetComponentInChildren<TextMeshProUGUI>().text = "You Earned " + score + " tickets!! They have been added to your account! ";
                    }
                }
            }
        }
    }


    public void restartGame()
    {
        if (pots != null) Destroy(pots);
        this.Start();
    }


    public void togglePause()
    {
        this.isPaused = !isPaused;
        this.firingAllowed = true;
        
    }
    public void boomPot()
    {
        potCounter--;
    }

    public void updatePebbleCounter()
    {
        if(pebbleCounterDisplay != null)
        {
            TextMeshProUGUI textView = pebbleCounterDisplay.GetComponentInChildren<TextMeshProUGUI>();
            textView.text = patharCount.ToString() + " pebble left";
        }
    }

    public void updateScoreTracker()
    {
        if (scoreTrackerDisplay != null)
        {
            TextMeshProUGUI textView = scoreTrackerDisplay.GetComponentInChildren<TextMeshProUGUI>();
            textView.text = score + " tickets";
        }
    }

    public void OnSelectDown(SelectEventData eventData)
    {
        //check error if no object tag defined here
        if (eventData != null)
        {   
            if(eventData.selectedObject != null && eventData.selectedObject.transform != null)
            {
                string selectedObjectTag = eventData.selectedObject.transform.tag;


                if (selectedObjectTag == "UIElement" || selectedObjectTag == "MenuButton")
                {
                    Debug.Log("Pointer Target is a UI ELEMENT OR MENU BUTTON so no shooting!!");
                    return;
                }
            }
            
        }



        pebble = GameObject.FindGameObjectWithTag("Pebble");
    
        
        if (patharCount > 0 && pebble == null && !isPaused && firingAllowed)
        {
            firingAllowed = false;
            StartCoroutine(shotsFired());
        }

    }

    private IEnumerator shotsFired()
    {
        enumRunning = true;
        Debug.Log("ShotsFired Started Cannot check EndGame");
        
        // Get controller ray
        List<IInputSource>  controllers = JMRInteractionManager.Instance.GetSources();
        if (controllers.Count > 0)
        {
            // Debug.Log("Controller Source Found!!");
            IInputSource source = controllers[0];
        }
        else
        {
            Debug.Log("No Controller Source Found!!");
        }
        if (controllers[0].TryGetPointingRay(out controllerRay))
        {
            //ray found so extrapolate it to find the point target
            if (Physics.Raycast(controllerRay, out hit))
            {
                gulel.GetComponent<Animation>()["shootAnimation"].speed = 4f;
                gulel.GetComponent<Animation>().Play("shootAnimation");
                gulel.GetComponent<AudioSource>().Play();
                yield return new WaitForSeconds(0.9f);
                Debug.Log("Pathar Shot");
                GameObject pathar = Instantiate(patharPrefab);
                pathar.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
                pathar.transform.position = gulel.transform.position;
                //Vector3 rayDirection = JMRPointerManager.Instance.GetCurrentRay().direction;
                Vector3 shootDir = (hit.point - pathar.transform.position).normalized;
                pathar.transform.forward = shootDir;
                patharCount--;
                updatePebbleCounter();
            }


        }
        
        //yield return new WaitForSeconds(3f);      
        firingAllowed = true;
        enumRunning = false;
        Debug.Log("ShotsFired Ended Can check now");
    }

    public void OnSelectUp(SelectEventData eventData)
    {

    }

    

}
