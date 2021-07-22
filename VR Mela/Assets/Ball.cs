using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip ballSound;
    public AudioClip tumblerSound;


    public void OnCollisionEnter(Collision collision)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = ballSound;
        source.spatialBlend = 1f;
        source.volume = 1f;
        source.Play();

        if (collision.gameObject.tag.Equals("Tumbler"))
        {
            AudioSource source2 = gameObject.AddComponent<AudioSource>();
            source.clip = tumblerSound;
            source.spatialBlend = 1f;
            source.volume = 1f;
            source.Play();
        }
    }
}
