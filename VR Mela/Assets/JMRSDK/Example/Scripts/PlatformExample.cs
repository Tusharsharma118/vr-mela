using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;
using UnityEngine.UI;
using System;
using TMPro;

public class PlatformExample : MonoBehaviour
{
    public TMP_Text LogText;

    private void Start()
    {
        Invoke("UpdateData", 1f);
        // UpdateData();
    }
    private void UpdateData()
    {

        if (!Application.isEditor)
        {
            LogText.text = "Platform API \n" +
                           "Platform ManufacturerName:\t" + JMRPlatformManager.Instance.GetManufacturerName() + "\n" +
                           "Platform code name:\t" + JMRPlatformManager.Instance.GetCodeName();
            //+ "\n"
            //+ "Platform Security patch:\t" + platformAPI.getSecurityPatchLevel() + "\n"
            //+ "Platform build Number:\t" + platformAPI.getBuildNumber() + "\n"
            //+ "Platform version:\t" + platformAPI.getVersion() + "\n"
            //+ "Platform Level:\t" + platformAPI.getLevel() + "\n"
            //+ "Platform Ver Name:\t" + platformAPI.getVersionName() + "\n"
            //+ "Platform Wifi Mac:\t" + platformAPI.getWifiMac() + "\n"
            //+ "Platform sdk version:\t" + platformAPI.getSDKVersion() + "\n"
            //+ "Platform Sdk Level:\t" + platformAPI.getSDKLevel() + "\n"
            //+ "Platform sdk Name:\t" + platformAPI.getSDKVersionName() + "\n"
            //+ "Platform service ver:\t" + platformAPI.getServiceVersion() + "\n"
            //+ "Platform isExtendedDisplaySupported:\t" + platformAPI.isExtendedDisplaySupported() + "\n"
            //;
        }
        else
        {
            LogText.text = "Platform API \n Platform ManufacturerName:\t" +" JioGlass" + " Platform code name:\t" + "Emulator";
        }

    }
    public void UpdatePlatformDetails()
    {
        Debug.Log("ButtonClicked");
        UpdateData();
    }



}
