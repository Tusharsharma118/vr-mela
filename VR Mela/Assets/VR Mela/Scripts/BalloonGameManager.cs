using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonGameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] balloonPrefabs;
    public GameObject[] spawnLocations;
    

    void Start()
    {

        InvokeRepeating("Spawn", 0.2f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        Instantiate(balloonPrefabs[0], transform.position, transform.rotation);
        Instantiate(balloonPrefabs[0], transform.position, transform.rotation);
        Instantiate(balloonPrefabs[0], transform.position, transform.rotation);
    }


}
