using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK.Intent;

public class IntentSenderExample : MonoBehaviour
{
    public InputField PackageName;
    public InputField ClassName;
    public Toggle IsDataIntent;

    public InputField StringKey;
    public InputField IntKey;
    public InputField LongKey;
    public InputField DoubleKey;
    public InputField BoolKey;

    public InputField StringData;
    public InputField IntData;
    public InputField LongData;
    public InputField DoubleData;
    public Toggle BoolData;
    public InputField DeepLink;

    public Button LaunchIntentButton;

    Dictionary<string, object> KeyValuePair = new Dictionary<string, object>();

    void Start()
    {
        LaunchIntentButton.onClick.AddListener(LaunchIntent);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void LaunchIntent()
    {
        if(IsDataIntent)
        {
            if(CheckKeyValueNotEmpty(StringKey,StringData))
            {
                KeyValuePair.Add(StringKey.text, StringData.text);
            }
            if (CheckKeyValueNotEmpty(IntKey, IntData))
            {
                KeyValuePair.Add(IntKey.text, int.Parse(IntData.text));
            }
            if (CheckKeyValueNotEmpty(LongKey, LongData))
            {
                KeyValuePair.Add(LongKey.text, long.Parse(LongData.text));
            }
            if (CheckKeyValueNotEmpty(DoubleKey, DoubleData))
            {
                KeyValuePair.Add(DoubleKey.text, double.Parse(DoubleData.text));
            }
            if (!string.IsNullOrEmpty(BoolKey.text))
            {
                KeyValuePair.Add(BoolKey.text, BoolData.isOn);
            }
            if(!string.IsNullOrEmpty(PackageName.text) && !string.IsNullOrEmpty(ClassName.text))
            {
                Intent intent = IntentManager.Instance.GetIntentWithData(KeyValuePair, PackageName.text, ClassName.text);
                IntentManager.Instance.LaunchIntent(intent);
            }
        }
        //else
        //{
        //    if(!string.IsNullOrEmpty(DeepLink.text))
        //    {
        //        Intent intent = IntentManager.Instance.GetIntentWithDeepLink(DeepLink.text, "android.intent.action.VIEW");
        //        IntentManager.Instance.LaunchIntent(intent);
        //    }
        //}
    }

    private bool CheckKeyValueNotEmpty(InputField key, InputField value)
    {
        if (string.IsNullOrEmpty(key.text) || (string.IsNullOrEmpty(value.text)))
            return false;
        return true;
    }
}
