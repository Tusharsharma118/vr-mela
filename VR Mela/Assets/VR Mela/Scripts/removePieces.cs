using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removePieces : MonoBehaviour
{
    void Update(){
        Destroy(gameObject, 2.0f);
    }
}
