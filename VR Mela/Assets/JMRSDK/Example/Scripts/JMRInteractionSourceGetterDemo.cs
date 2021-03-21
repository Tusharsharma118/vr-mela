using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;
using JMRSDK.InputModule;
using UnityEngine.UI;

public class JMRInteractionSourceGetterDemo : MonoBehaviour
{
    IInputSource CurrentSource;

    IInputSource GetSourceByHandedness;

    IInputSource GetCurrentSource;

    List<IInputSource> GetSources = new List<IInputSource>();


    [SerializeField]
    private Text J_CurrentSelectTryGetButton;
    [SerializeField]
    private Text J_CurrentBackTryGetButton;
    [SerializeField]
    private Text J_CurrentHomeTryGetButton;
    [SerializeField]
    private Text J_CurrentFunctionTryGetButton;


    [SerializeField]
    private Text J_CurrentSourceRotation;


    [SerializeField]
    private Text J_CurrentSourcePosition;

    Quaternion SourceRotation;
    Vector3 SourcePosition;
    void Start()
    {
        //Example to get source by Handedness.
        GetSourceByHandedness = JMRInteractionManager.Instance.GetSourceByHandedness(Handedness.Right);

        //Example to get the current source
        GetCurrentSource = JMRInteractionManager.Instance.GetCurrentSource();

        //Example to get a list of all sources connected to the device
        GetSources = JMRInteractionManager.Instance.GetSources();
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentSource != null)
        {
            // Example to get button states from the source.
            J_CurrentSelectTryGetButton.text = "TryGetSelect() :" + CurrentSource.TryGetSelect().ToString();
            J_CurrentBackTryGetButton.text = "TryGetBack() :" + CurrentSource.TryGetBack().ToString();
            J_CurrentHomeTryGetButton.text = "TryGetHome() :" + CurrentSource.TryGetHome().ToString();
            J_CurrentFunctionTryGetButton.text = "TryGetFunctionButton() :" + CurrentSource.TryGetFunctionButton().ToString();

            // Example to get source rotation
            if (CurrentSource.TryGetRotation(out SourceRotation))
                J_CurrentSourceRotation.text = "TryGetRotation(out SourceRotation) :" + SourceRotation.ToString();

            //Example to get source position
            //NOTE: current Source doesn't provide positional data
            if (CurrentSource.TryGetPosition(out SourcePosition))
                J_CurrentSourcePosition.text = "TryGetPosition(out SourcePosition) :" + SourcePosition.ToString();

        }
        else
        {
            if (GetSourceByHandedness == null)
            {
                GetSourceByHandedness = JMRInteractionManager.Instance.GetSourceByHandedness(Handedness.Right);
            }
            else
            {
                CurrentSource = GetSourceByHandedness;
                Debug.Log("CurrentSource is updated With GetSourceByHandedness");
            }
            if (GetCurrentSource == null)
            {
                GetCurrentSource = JMRInteractionManager.Instance.GetCurrentSource();
            }
            else
            {
                CurrentSource = GetCurrentSource;
                Debug.Log("CurrentSource is updated With GetCurrentSource");
            }
            if (GetSources.Count < 0)
            {
                GetSources = JMRInteractionManager.Instance.GetSources();
            }
            else
            {
                CurrentSource = GetSources[0];
                Debug.Log("CurrentSource is updated With GetSources[0]");
            }
            // CurrentSource = JMRInteractionManager.Instance.GetSourceByHandedness(Handedness.Right);
        }
    }
}
