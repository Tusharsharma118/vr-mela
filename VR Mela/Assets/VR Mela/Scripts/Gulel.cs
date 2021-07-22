using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;

public class Gulel : MonoBehaviour
{
   
    
    private IInputSource source;
    private List<IInputSource> controllers;
    private Quaternion controllerRotation;
    private float speed = 2f;
    
  

    // Update is called once per frame
    void Update()
    {
       // controllers = JMRInteractionManager.Instance.GetSources();
       // if (controllers.Count > 0)
       // {
           // Debug.Log("Controller Source Found!!");
       //     source = controllers[0];
       // }
       // else {
       //     Debug.Log("No Controller Source Found!!");
        //}
       // if (controllers[0].TryGetRotation(out controllerRotation))
        //{
           // gameObject.transform.rotation = controllerRotation;
        //}
    }

   
   

}
