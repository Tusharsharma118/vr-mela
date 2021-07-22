using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JMRPackageSetup
{
#if UNITY_EDITOR
    //[MenuItem("JioMixedReality/IntentAppConfig/Setup For Sender")]
    static void SetUpLootMogul()
    {
        PlayerSettings.applicationIdentifier = "com.Tesseract.IntentSender";
        PlayerSettings.productName = "IntentSender";
        Debug.Log("Package setup => " + PlayerSettings.applicationIdentifier);
    }

    //[MenuItem("JioMixedReality/IntentAppConfig/Setup For Receiver")]
    static void SetUpLootMantra()
    {
        PlayerSettings.applicationIdentifier = "com.Tesseract.IntentReceiver";
        PlayerSettings.productName = "IntentReceiver";
        Debug.Log("Package setup => " + PlayerSettings.applicationIdentifier);
    }
#endif
}