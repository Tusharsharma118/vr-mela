using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotHealth : MonoBehaviour
{
    public GameObject brokenPot;
    public GameObject crackedPot;
    public AudioClip hitSound;
    public AudioClip breakSound;

    public void InstantiateBrokenPot(){
        GameObject brokenPotInstance = Instantiate(brokenPot,transform.position,transform.rotation);
        brokenPotInstance.transform.parent = gameObject.transform.parent;
        AudioSource source = brokenPotInstance.AddComponent<AudioSource>();
        source.clip = breakSound;
        source.spatialBlend = 1f;
        source.volume = 1f;
        source.Play();
        Destroy(gameObject);
    }

    public void InstantiateCrackedPot(){
        GameObject crackedPotInstance = Instantiate(crackedPot,transform.position,transform.rotation);
        crackedPotInstance.transform.parent = gameObject.transform.parent;
        AudioSource source = crackedPotInstance.AddComponent<AudioSource>();
        source.clip = hitSound;
        source.spatialBlend = 1f;
        source.volume = 1f;
        source.Play();
        Destroy(gameObject);
    }

}
