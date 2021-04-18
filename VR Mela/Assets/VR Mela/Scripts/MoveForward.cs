using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private float speed = 2f;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.CompareTag("bullseye")){
            other.gameObject.transform.parent.gameObject.GetComponent<PotHealth>().InstantiateBrokenPot();
        }else if(other.gameObject.CompareTag("earthpot")){
            other.gameObject.GetComponent<PotHealth>().InstantiateCrackedPot();
        }else if(other.gameObject.CompareTag("crackedpot")){
            other.gameObject.GetComponent<PotHealth>().InstantiateBrokenPot();
        }
        Destroy(gameObject);
    }
}
