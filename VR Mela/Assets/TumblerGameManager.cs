using JMRSDK.InputModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TumblerGameManager : MonoBehaviour, ISelectHandler
{

    public Image UIFillImage;
    public float powerMultiplier = 3.0f;
    public GameObject ballPrefab;
    public GameObject shootingPosition;
    public int ballCount = 3;
    public float score = 0f;
    public float maxScore = 12f;
    public GameObject ballCounterDisplay;
    public GameObject scoreTrackerDisplay;
    public GameObject gameOverParent;
    public GameStats gameStats;


    public GameObject tumblerSetPrefab;

    private GameObject tumblersConfig;

    private Rigidbody ballCopyRB;
    private Ray controllerRay;
    private RaycastHit hit;
    private bool isPaused = false;
    private bool firstTime = true;
    private GameObject ball;

    // for controller rotation might delete later
    private IInputSource source;
    private List<IInputSource> controllers;


    public void Start()
    {
        if (!shootingPosition.activeSelf)
        {
            shootingPosition.SetActive(true);
        }
        ball = null;
        ballCount = 3;
        score = 0;
        isPaused = false;
        firstTime = true;
        JMRInputManager.Instance.RemoveGlobalListener(gameObject);
        JMRInputManager.Instance.AddGlobalListener(gameObject);
        updateBallCounter();
        updateScoreTracker();
        //Subscribe to tumbler fall events
        tumblersConfig = Instantiate(tumblerSetPrefab, new Vector3(-0.038f, -1.711f, 15.36f), tumblerSetPrefab.transform.rotation);
        tumblersConfig.transform.localScale = new Vector3(0.5f, 0.8f, 0.5f);
        tumblersConfig.transform.SetParent(this.transform.parent);

        GameObject[] tumblerSet = GameObject.FindGameObjectsWithTag("TumblerConfiguration");
        if (tumblerSet.Length > 1)
        {
            Destroy(tumblerSet[0]);
        }

        Debug.Log("Tumbler Set count :: "+tumblerSet.Length);
        registerTumberEvents();
    }


    public void registerTumberEvents()
    {
        GameObject[] tumblers = GameObject.FindGameObjectsWithTag("Tumbler");
        foreach (GameObject t in tumblers)
        {

            Tumbler script = t.GetComponent<Tumbler>();
            script.OnTumblerFell += EventHandlerOnTumblerFell;
        }
    }

    public void deRegisterTumberEvents()
    {
        GameObject[] tumblers = GameObject.FindGameObjectsWithTag("Tumbler");
        foreach (GameObject t in tumblers)
        {

            Tumbler script = t.GetComponent<Tumbler>();
            script.OnTumblerFell -= EventHandlerOnTumblerFell;
        }
    }


    public void restartGame()
    {
        deRegisterTumberEvents();
        // reset the tumblers here 
        if (tumblersConfig != null) Destroy(tumblersConfig);
        this.Start();
    }
    public void Update()
    {
        EndGameCheck();
    }

    private void EventHandlerOnTumblerFell(object sender, EventArgs e)
    {
        score += maxScore / 6;
        updateScoreTracker();
        Debug.Log("Event Fired !! New Score :: " + score);

    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }
    public void Fire(Vector3 targetPoint, Vector3 sourcePoint)
    {
        ballCount--;
        updateBallCounter();
        Debug.Log("Starting Fire Method");
        float firePower = UIFillImage.fillAmount * powerMultiplier;

        Debug.Log("Fill Amount: " + UIFillImage.fillAmount + "   TOtal Fire Power :: " + firePower);
        Vector3 shootDir = (targetPoint - sourcePoint).normalized;
        Debug.Log("shootDirection ::" + shootDir);
        GameObject ballCopy = Instantiate(ballPrefab, shootingPosition.transform.position, Quaternion.identity) as GameObject;
        ballCopy.transform.localScale = new Vector3(20.1f, 20f, 20f);
        ballCopyRB = ballCopy.GetComponent<Rigidbody>();
        Debug.Log("RigidBody" + ballCopyRB.name);
        ballCopyRB.AddForce(shootDir * firePower, ForceMode.Impulse);
        Destroy(ballCopy, 3.0f);
    }

    public void updateBallCounter()
    {
        if (ballCounterDisplay != null)
        {
            TextMeshProUGUI textView = ballCounterDisplay.GetComponentInChildren<TextMeshProUGUI>();
            textView.text = ballCount.ToString() + " REMAINING";
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
            if (eventData.selectedObject != null && eventData.selectedObject.transform != null)
            {
                string selectedObjectTag = eventData.selectedObject.transform.tag;


                if (selectedObjectTag == "UIElement" || selectedObjectTag == "MenuButton")
                {
                    Debug.Log("Pointer Target is a UI ELEMENT OR MENU BUTTON so no shooting!!");
                    return;
                }
            }

        }
        // Get controller ray
        controllers = JMRInteractionManager.Instance.GetSources();
        if (controllers.Count > 0)
        {
            // Debug.Log("Controller Source Found!!");
            source = controllers[0];
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
                if (hit.point != null && ballCount > 0 && !isPaused)
                {
                    Fire(hit.point, shootingPosition.transform.position);
                }
            }


        }



    }

    private void EndGameCheck()
    {
        Debug.Log("Inside EndGame check");
        if (ballCount <= 0 || score == maxScore)
        {
            
             ball = GameObject.FindGameObjectWithTag("Ball");
            // first time makes it so it only works once per game cycle as it is inside update
            Debug.Log("first time value = " + firstTime);
            if (ball == null && firstTime)
            {
                firstTime = false;
                shootingPosition.SetActive(false);
                Debug.Log("Game Over");
                // Insert Display banner here
                gameOverParent.SetActive(true);
                Transform parent = gameOverParent.transform;
                //update global ticket counter
                //add bonus points for saved balls
                score += (ballCount * 2);
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


    public void OnSelectUp(SelectEventData eventData)
    {
       
    }
}
