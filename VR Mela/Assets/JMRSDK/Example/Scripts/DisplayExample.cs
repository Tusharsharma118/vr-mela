using JMRSDK;
using TMPro;
using UnityEngine;

public class DisplayExample : MonoBehaviour
{
    public TMP_Text LogText;

    private void LateUpdate()
    {
        if (JMRDisplayManager.Instance.IsConnected())
        {
            LogText.text = "Display API \n "
                           + "Display \t" + JMRDisplayManager.Instance.GetDisplayMode() + "\n"
                           + "Display Brightness:\t" + JMRDisplayManager.Instance.GetDisplayBrightness() + "\n"
                           + "Display \t" + JMRDisplayManager.Instance.GetDisplayPowerState() + "\n"
                           + "Display \t" + JMRDisplayManager.Instance.GetDisplayPowerControlMode() + "\n"
                           + "Device \t" + JMRDisplayManager.Instance.GetDisplayBrightnessMode() + "\n"
                ;
        }
    }
}