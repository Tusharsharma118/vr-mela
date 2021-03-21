using JMRSDK;
using UnityEngine;
using UnityEngine.UI;

public class TrackingExample : MonoBehaviour
{
    public Text LogText;
    public Transform cubeParent;

    private void OnEnable()
    {
        JMRTrackerManager.OnHeadPosition += OnHeadPosition;
        JMRTrackerManager.OnHeadRotation += OnHeadRotation;
    }

    private void OnDisable()
    {
        JMRTrackerManager.OnHeadPosition -= OnHeadPosition;
        JMRTrackerManager.OnHeadRotation -= OnHeadRotation;
    }

    private void OnHeadPosition(Vector3 obj)
    {
        cubeParent.localPosition = obj;
    }

    private void OnHeadRotation(Quaternion obj)
    {
        cubeParent.localRotation = obj;
    }

    private void LateUpdate()
    {
        LogText.text = $"Tracking API\n" +
                       $"Position : {cubeParent.localPosition}\n" +
                       $"Rotation : {cubeParent.localRotation}";
    }
}