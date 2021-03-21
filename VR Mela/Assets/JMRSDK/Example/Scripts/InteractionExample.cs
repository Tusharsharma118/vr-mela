using System.Collections.Generic;
using JMRSDK.InputModule;
using UnityEngine;
using UnityEngine.UI;

public class InteractionExample : MonoBehaviour, ITouchHandler, ISelectHandler ,ISwipeHandler
{
    public Transform controllersUI;
    private List<ControllerDevice> Controllers = new List<ControllerDevice>();
    private Dictionary<int, int> batteryLevels = new Dictionary<int, int>();
    private Dictionary<int, string> touchStatus = new Dictionary<int, string>();
    private Dictionary<int, string> swipeStatus = new Dictionary<int, string>();
    private Dictionary<int, string> actionStatus = new Dictionary<int, string>();

    private void Awake()
    {
        JMRInputManager.Instance.AddGlobalListener(gameObject);
    }

    private void OnEnable()
    {
        JMRInteractionManager.OnBatteryUpdate += OnBatteryUpdate;
        JMRInteractionManager.OnConnected += OnConnect;
        JMRInteractionManager.OnDisconnected += OnDisconnect;
        JMRInteractionManager.OnStartScanning += OnStartScan;
    }

    private void OnDisable()
    {
        JMRInteractionManager.OnBatteryUpdate -= OnBatteryUpdate;
        JMRInteractionManager.OnConnected -= OnConnect;
        JMRInteractionManager.OnDisconnected -= OnDisconnect;
        JMRInteractionManager.OnStartScanning -= OnStartScan;
    }

    private void OnStartScan(JMRInteractionManager.InteractionDeviceType devType, int index)
    {
        Log($"OnStartScan({devType}, {index})");
        string deviceType = devType.ToString();
    }

    private void OnDisconnect(JMRInteractionManager.InteractionDeviceType devType, int index, string val)
    {
        Log($"OnDisconnect({devType}, {index}, {val})");
        Controllers = JMRInteractionManager.Instance.ControllerDevices;
    }

    private void OnConnect(JMRInteractionManager.InteractionDeviceType devType, int index, string val)
    {
        Log($"OnConnect({devType}, {index}, {val})");
        Controllers = JMRInteractionManager.Instance.ControllerDevices;
        if (!batteryLevels.ContainsKey(index)) batteryLevels.Add(index, -1);
        if (!touchStatus.ContainsKey(index)) touchStatus.Add(index, "None");
        if (!swipeStatus.ContainsKey(index)) swipeStatus.Add(index, "None");
        if (!actionStatus.ContainsKey(index)) actionStatus.Add(index, "None");
    }

    private void OnBatteryUpdate(JMRInteractionManager.InteractionDeviceType deviceType, int index, int obj)
    {
        Log($"onBatteryUpdate({deviceType}, {index}, {obj})");
        batteryLevels[index] = obj;
    }

    private void LateUpdate()
    {
        foreach (Transform t in controllersUI)
        {
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < Controllers.Count; i++)
        {
            ControllerDevice device = Controllers[i];
            Text text = GetTextElement(i);
            text.text = $"Controller {device.index}\n" +
                        $"Battery :\t {batteryLevels[device.index]}%\n" +
                        $"Touch {touchStatus[device.index]}\n" +
                        $"Swipe {swipeStatus[device.index]}\n" +
                        $"Action {actionStatus[device.index]}";
        }
    }

    private Text GetTextElement(int i)
    {
        Text text;
        if (i < controllersUI.childCount)
        {
            text = controllersUI.GetChild(i).GetComponent<Text>();
        }
        else
        {
            text = Instantiate(controllersUI.GetChild(0), controllersUI).GetComponent<Text>();
            text.gameObject.name = "Text_" + i;
            text.transform.SetParent(controllersUI.transform);
        }

        text.gameObject.SetActive(true);
        return text;
    }

    private void Log(string text)
    {
        Debug.Log("Interaction Example >> " + text);
    }
    public void OnTouchStart(TouchEventData eventData, Vector2 TouchData) => touchStatus[(int) eventData.SourceId] = $"Start : {TouchData}";
    public void OnTouchStop(TouchEventData eventData, Vector2 TouchData) => touchStatus[(int) eventData.SourceId] = $"Stop : {TouchData}";
    public void OnTouchUpdated(TouchEventData eventData, Vector2 TouchData) => touchStatus[(int) eventData.SourceId] = $"Updated : {TouchData}";
    public void OnSwipeLeft(SwipeEventData eventData, float value) => swipeStatus[(int) eventData.SourceId] = $"Left : {value}";
    public void OnSwipeRight(SwipeEventData eventData, float value) => swipeStatus[(int) eventData.SourceId] = $"Right : {value}";
    public void OnSwipeUp(SwipeEventData eventData, float value) => swipeStatus[(int) eventData.SourceId] = $"Up : {value}";
    public void OnSwipeDown(SwipeEventData eventData, float value) => swipeStatus[(int) eventData.SourceId] = $"Down : {value}";
    public void OnSwipeStarted(SwipeEventData eventData) => swipeStatus[(int) eventData.SourceId] = "Started";
    public void OnSwipeUpdated(SwipeEventData eventData, Vector2 swipeData) { } //=> swipeStatus[(int) eventData.SourceId] = "Updated";
    public void OnSwipeCompleted(SwipeEventData eventData) => swipeStatus[(int) eventData.SourceId] = "Completed";
    public void OnSwipeCanceled(SwipeEventData eventData) => swipeStatus[(int) eventData.SourceId] = "Canceled";
    public void OnSelectDown(SelectEventData eventData) => actionStatus[(int) eventData.SourceId] = "OnSelectDown";
    public void OnSelectUp(SelectEventData eventData) => actionStatus[(int) eventData.SourceId] = "OnSelectUp";
}