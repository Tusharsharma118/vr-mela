using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.Intent;
using UnityEngine.UI;

public class IntentRecieverExample : MonoBehaviour
{
    public InputField StringKey;
    public InputField IntKey;
    public InputField LongKey;
    public InputField DoubleKey;
    public InputField BoolKey;

    public Text StringValue;
    public Text IntValue;
    public Text LongValue;
    public Text DoubleValue;
    public Text BoolValue;
    //public Text DeeplinkValue;
    public Text Error;

    public Button ReadDataButton;
    //public Button ReadDeepLinkButton;

    //private string[] keys = new string[] { "key1,key2" };
    //public string deeplinkURL;
   
    private void Start()
    {
        IntentManager.Instance.onError += OnError;
        ReadDataButton.onClick.AddListener(ReadIntentData);
        //ReadDeepLinkButton.onClick.AddListener(ReadDeepLinkData); 
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    if(!pause)
    //    {
    //        //ReadDeepLinkData();
    //    }
    //}

    void ReadIntentData()
    {
        if(!string.IsNullOrEmpty(StringKey.text))
        {
            StringValue.text = IntentManager.Instance.GetIntentStringData(StringKey.text);
        }
        if(!string.IsNullOrEmpty(IntKey.text))
        {
            IntValue.text = IntentManager.Instance.GetIntentIntData(IntKey.text, -1).ToString();
        }
        if (!string.IsNullOrEmpty(LongKey.text))
        {
            LongValue.text = IntentManager.Instance.GetIntentLongData(LongKey.text, -1).ToString();
        }
        if (!string.IsNullOrEmpty(DoubleKey.text))
        {
            DoubleValue.text = IntentManager.Instance.GetIntentDoubleData(DoubleKey.text, -1).ToString();
        }
        if (!string.IsNullOrEmpty(BoolKey.text))
        {
            BoolValue.text = IntentManager.Instance.GetIntentBoolData(BoolKey.text, false).ToString();
        }
    }

    void OnError(int code)
    {
        if (code == IntentManager.KEY_NOT_PRESENT)
            Error.text = "KEY_NOT_PRESENT";
        else if (code == IntentManager.INTENT_NOT_RECEIVED)
            Error.text = "INTENT_NOT_RECEIVED";
    }

    //void ReadDeepLinkData()
    //{
    //    Debug.Log("ReadDeepLinkData");
    //    Dictionary<string, string> queryData = IntentManager.Instance.GetIntentQueryData(keys);
    //    Debug.Log("KEY COUNTS : " + queryData.Count);
    //    string data = "";
    //    foreach(var kvp in queryData)
    //    {
    //        data += kvp.Key + " => " + queryData[kvp.Key]+"\n";
    //        Debug.Log(kvp.Key + " => " + queryData[kvp.Key]);
    //    }
    //    DeeplinkValue.text = data;
    //}
}
