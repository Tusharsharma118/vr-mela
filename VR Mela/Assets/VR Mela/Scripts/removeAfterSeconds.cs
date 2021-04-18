using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeAfterSeconds : MonoBehaviour
{
    public float seconds;
    void Update(){  
        Destroy(gameObject, seconds);
    }
}
