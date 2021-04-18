using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crackedPot : MonoBehaviour
{
    public GameObject brokenPot;
    public int hp = 1;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        Instantiate(brokenPot,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
