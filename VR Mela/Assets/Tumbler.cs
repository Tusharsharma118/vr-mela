using System;
using UnityEngine;

public class Tumbler : MonoBehaviour
{
    public event EventHandler OnTumblerFell;
    private Boolean first = true;
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TumblerTriggerArea" && first)
        {
            Debug.Log("Trigger Fired for Tumbler");
            first = false;
            OnTumblerFell?.Invoke(this, EventArgs.Empty);
        }
    }
}
