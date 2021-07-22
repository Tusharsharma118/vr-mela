using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 2f;
    public float lifeTime = 3f;
    private float timer;
    private bool alreadyTriggered = false;

    private GameObject matkaGameManagerObject;

    private MatkaGameManager matkaGameManager;




    private void Start()
    {
        alreadyTriggered = false;
        timer = lifeTime;
        matkaGameManagerObject = GameObject.FindGameObjectWithTag("MatkaGameManager");
        matkaGameManager = matkaGameManagerObject.GetComponent<MatkaGameManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            Destroy(gameObject);
        }
    }


    // full matka score 10  5-5 each step  and bullseye = +5
    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered)
        {
            alreadyTriggered = true;
            Debug.Log(other.gameObject.tag);
            Debug.Log(other.gameObject.name);
            if (other.gameObject.CompareTag("BullsEye"))
            {
                matkaGameManager.score += 15;
                matkaGameManager.updateScoreTracker();
                matkaGameManager.boomPot();
                other.gameObject.transform.parent.gameObject.GetComponent<PotHealth>().InstantiateBrokenPot();
            }
            else if (other.gameObject.CompareTag("EarthPot"))
            {
                matkaGameManager.score += 5;
                matkaGameManager.updateScoreTracker();
                other.gameObject.GetComponent<PotHealth>().InstantiateCrackedPot();
            }
            else if (other.gameObject.CompareTag("CrackedPot"))
            {
                matkaGameManager.score += 5;
                matkaGameManager.updateScoreTracker();
                matkaGameManager.boomPot();
                other.gameObject.GetComponent<PotHealth>().InstantiateBrokenPot();
            }
            Destroy(gameObject);
        }
        
    }
}
