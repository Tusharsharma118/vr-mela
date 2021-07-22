using UnityEngine;
using JMRSDK;
using TMPro;
using JMRSDK.Dialogue;

public class JMRDemoDeviceExample : MonoBehaviour
{
    public TMP_Text LogText;

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            JMRRigManager.OnDeviceError += OnDeviceError;
        }
    }
    

    private void OnDisable()
    {
        if (!Application.isEditor)
        {
            JMRRigManager.OnDeviceError -= OnDeviceError;
        }
    }

    private void OnDeviceError(string errorMessage)
    {
        JMRSDKDialogueManager.Instance.Show("Device Error", errorMessage, "OK", OnErrorButtonClicked);
    }

    private void Start()
    {
        Invoke("UpdateData", 1f);
    }
    private void UpdateData()
    {

        if (!Application.isEditor)
        {
            LogText.text = "Device API \n" +
                           "Device ManufacturerName :\t" + (string.IsNullOrEmpty(JMRRigManager.Instance.GetManufacturerName()) ? "Device Not Found!" : JMRRigManager.Instance.GetManufacturerName()) + "\n" +
                           "Device Protocal version :\t" + JMRRigManager.Instance.GetProtocolVersion() + "\n" +
                           "HMD Device version :\t" + (string.IsNullOrEmpty(JMRRigManager.Instance.GetHmdDeviceVersion()) ? "Device Not Found!" : JMRRigManager.Instance.GetHmdDeviceVersion()) + "\n";
        }
        else
        {
            LogText.text = "Device API \n Device ManufacturerName:\t" + " JioGlass" + " Device code name:\t" + "Emulator";
        }

    }
    public void UpdateDeviceDetails()
    {
        Debug.Log("ButtonClicked");
        UpdateData();
    }

    public void OnErrorButtonClicked()
    {
        JMRSDKDialogueManager.Instance.Hide();
    }

}
