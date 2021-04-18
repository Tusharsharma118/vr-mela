using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotHealth : MonoBehaviour
{
    public GameObject brokenPot;
    public GameObject crackedPot;

    public void InstantiateBrokenPot(){
        Instantiate(brokenPot,transform.position,transform.rotation);
        Destroy(gameObject);
    }

    public void InstantiateCrackedPot(){
        Instantiate(crackedPot,transform.position,transform.rotation);
        Destroy(gameObject);
    }

}
