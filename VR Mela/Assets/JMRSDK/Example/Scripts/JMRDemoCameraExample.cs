using JMRSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JMRDemoCameraExample : MonoBehaviour
{
    public Text LogText;

    public RectTransform PreviewImageRect;

    // private JMRCameraAPI cameraAPI;
    private string mediacapturepath;

    private string cameraStatus = "Unavailable";

    // public Material renderMat;
    // public Material renderMat2;
    private List<FrameSize> previewResolutionsList = null;
    private List<FrameSize> captureResolutionsList = null;
    private FrameSize CurrentRes;
    private Texture2D camTexture;
    private Texture2D camTexture2;

    private bool previewStarted;

    public Button startPreviewButton, stopPreviewButton;
    public Button captureImageButton, captureImageNameButton;
    public Button startRecordButton, startRecordNameButton;
    public Button pauseRecordButton, resumeRecordButton, stopRecordButton;
    public GameObject resolutionControlParent;
    public Dropdown PrevResDropdown;
    public Dropdown CaptureResDropdown;

    private void Start()
    {
        pauseRecordButton.gameObject.SetActive(false);
        resumeRecordButton.gameObject.SetActive(false);
        stopRecordButton.gameObject.SetActive(false);
        //resolutionControlParent.SetActive(false);
        InvokeRepeating("DisplayParameters", 1f, 2f);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            previewStarted = false;
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
            startPreviewButton.gameObject.SetActive(true);
            stopPreviewButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (previewStarted)
        {
            stopPreviewButton.gameObject.SetActive(true);
            captureImageButton.gameObject.SetActive(true);
            captureImageNameButton.gameObject.SetActive(true);
            startRecordButton.gameObject.SetActive(true);
            startRecordNameButton.gameObject.SetActive(true);
            //resolutionControlParent.SetActive(true);
        }
        else
        {
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
            //resolutionControlParent.SetActive(false);
        }
    }

    private void OnEnable()
    {
        JMRCameraManager.OnCameraConnect += OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect += OnCameraDisconnect;
        JMRCameraManager.OnImageCapture += OnImageCapture;
        JMRCameraManager.OnCameraError += OnError;
        JMRCameraManager.OnVideoRecord += OnVideoRecord;

        PrevResDropdown.onValueChanged.AddListener(ResDropDownChange);
        CaptureResDropdown.onValueChanged.AddListener(CaptureResDropDownChange);
    }

    string cameraError = "";
    private void OnError(string obj)
    {
        cameraError = obj;
    }

    private void OnDisable()
    {
        JMRCameraManager.OnCameraConnect -= OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect -= OnCameraDisconnect;
        JMRCameraManager.OnImageCapture -= OnImageCapture;
        JMRCameraManager.OnCameraError -= OnError;
        JMRCameraManager.OnVideoRecord -= OnVideoRecord;

        PrevResDropdown.onValueChanged.RemoveListener(ResDropDownChange);
        CaptureResDropdown.onValueChanged.RemoveListener(CaptureResDropDownChange);
    }

    string videoRecordStatus = "";
    private void OnVideoRecord(String obj, JMRCameraManager.VideoRecordState state)
    {
        Debug.Log($"OnVideoRecord({state}, {obj}");
        switch (state)
        {
            case JMRCameraManager.VideoRecordState.Started:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_STARTED";
                break;
            case JMRCameraManager.VideoRecordState.Paused:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_PAUSED";
                break;
            case JMRCameraManager.VideoRecordState.Resumed:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_RESUMED";
                break;
            case JMRCameraManager.VideoRecordState.Stopped:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_STOPPED";
                break;
            case JMRCameraManager.VideoRecordState.Completed:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_COMPLETED";
                break;
            default:
                videoRecordStatus = "Record : UNKNOWN STATE";
                break;
        }

        mediacapturepath = obj;
    }

    public void ResDropDownChange(int resolutionindex)
    {
        foreach (var val in previewResolutionsList)
        {
            if (val.frameSizeText == PrevResDropdown.options[resolutionindex].text)
            {
                
                if (!Application.isEditor)
                    JMRCameraManager.Instance.SetPreviewResolution(val);
            }
        }
    }
    public void CaptureResDropDownChange(int resolutionindex)
    {
        foreach (var val in captureResolutionsList)
        {
            if (val.frameSizeText == CaptureResDropdown.options[resolutionindex].text)
            {
                if (!Application.isEditor)
                    JMRCameraManager.Instance.SetCaptureResolution(val);
            }
        }
    }

    private void OnImageCapture(string obj)
    {
        mediacapturepath = obj;
    }

    private void OnCameraDisconnect()
    {
        cameraStatus = "Disconnect";
        isCamAvailable = false;
    }

    private bool camConnect;
    private bool isCamAvailable = false;

    private void OnCameraConnect()
    {
        try
        {
            Debug.LogError("++++++++++++++++++++++++Camera Connected");
            cameraStatus = "Connect";

            CurrentRes = JMRCameraManager.Instance.GetCurrentCaptureResolution();
            camConnect = true;
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(false);
            isCamAvailable = true;

            UpdateDrodownLists();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void UpdateDrodownLists()
    {
        PrevResDropdown.ClearOptions();
        previewResolutionsList = JMRCameraManager.Instance.GetPreviewResolutions();

        if (previewResolutionsList != null)
        {
            //Debug.LogError("+++++++++++++++++++++++Preview Resolution count: " + previewResolutionsList.Count);

            FrameSize cr = JMRCameraManager.Instance.GetCurrentPreviewResolution();

            for (int i = 0; i < previewResolutionsList.Count; i++)
            {
                //Debug.LogError("+++++++++++++++++++++++Preview Resolution Data " + i + " " + previewResolutionsList[i].frameSizeText + "=" + previewResolutionsList[i].Width + "x" + previewResolutionsList[i].Height + "@" + previewResolutionsList[i].frameSizeText);
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = previewResolutionsList[i].frameSizeText;
                PrevResDropdown.options.Add(option);

                if (cr.Width == previewResolutionsList[i].Width && cr.Height == previewResolutionsList[i].Height)
                {
                    PrevResDropdown.value = i;
                    PrevResDropdown.captionText.text = option.text;
                    PreviewImageRect.sizeDelta = new Vector2(cr.Width, cr.Height);
                }
            }
        }
        else
        {
            Debug.LogError("Preview Resolution List NULL");
        }

        CaptureResDropdown.ClearOptions();
        captureResolutionsList = JMRCameraManager.Instance.GetCaptureResolutions();

        if (captureResolutionsList != null)
        {
            //Debug.LogError("+++++++++++++++++++++++Capture Resolution count: " + captureResolutionsList.Count);

            FrameSize cr = JMRCameraManager.Instance.GetCurrentCaptureResolution();

            for (int i = 0; i < captureResolutionsList.Count; i++)
            {
                //Debug.LogError("+++++++++++++++++++++++Capture Resolution Data " + i + " " + captureResolutionsList[i].frameSizeText + "=" + captureResolutionsList[i].Width + "x" + captureResolutionsList[i].Height + "@" + captureResolutionsList[i].frameSizeText);
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = captureResolutionsList[i].frameSizeText;
                CaptureResDropdown.options.Add(option);

                if (cr.Width == captureResolutionsList[i].Width && cr.Height == captureResolutionsList[i].Height)
                {
                    CaptureResDropdown.value = i;
                    CaptureResDropdown.captionText.text = option.text;
                }
            }
        }
        else
        {
            Debug.LogError("Capture Resolution List NULL");
        }
    }

    public void CaptureImage()
    {
        JMRCameraManager.Instance.CaptureImage();
    }

    public void CaptureImage(string name)
    {
        JMRCameraManager.Instance.CaptureImage(Application.persistentDataPath + "/" + name);
    }

    public void StartPreview()
    {
        JMRCameraManager.Instance.StartPreview();
        startPreviewButton.gameObject.SetActive(false);
        stopPreviewButton.gameObject.SetActive(true);
        previewStarted = true;
    }

    public void StopPreview()
    {
        JMRCameraManager.Instance.StopPreview();
        startPreviewButton.gameObject.SetActive(true);
        stopPreviewButton.gameObject.SetActive(false);
        previewStarted = false;
    }

    public void StartRecording()
    {
        if (JMRCameraManager.Instance.StartRecording())
        {
            pauseRecordButton.gameObject.SetActive(true);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(true);
        }
    }

    public void StartRecording(string name)
    {
        if (JMRCameraManager.Instance.StartRecording(Application.persistentDataPath + "/" + name))
        {
            pauseRecordButton.gameObject.SetActive(true);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(true);
        }
    }

    public void StopRecording()
    {
        JMRCameraManager.Instance.StopRecording();
        pauseRecordButton.gameObject.SetActive(false);
        resumeRecordButton.gameObject.SetActive(false);
        stopRecordButton.gameObject.SetActive(false);
    }

    public void PauseRecording()
    {
        JMRCameraManager.Instance.PauseRecording();
        pauseRecordButton.gameObject.SetActive(false);
        resumeRecordButton.gameObject.SetActive(true);
    }

    public void ResumeRecording()
    {
        JMRCameraManager.Instance.ResumeRecording();
        pauseRecordButton.gameObject.SetActive(true);
        resumeRecordButton.gameObject.SetActive(false);
    }

    public void SetPreviewRes(int res)
    {
        if (previewResolutionsList != null && res < previewResolutionsList.Count) 
        {
            JMRCameraManager.Instance.SetPreviewResolution(previewResolutionsList[res]);
        }
    }

    public void SetCaptureRes(int res)
    {
        if (captureResolutionsList != null && res < captureResolutionsList.Count)
        {
            JMRCameraManager.Instance.SetCaptureResolution(captureResolutionsList[res]);
        }
    }
    private double latency = -1, maxLatency = -1, prevLatency = -1;

    private float latencyDisplayDelay = 0.5f;

    private void CalculateMaxLatency(float latency)
    {
        this.latency = latency;
        maxLatency = this.latency > maxLatency ? this.latency : maxLatency;
    }

    private void LateUpdate()
    {
        if (!Application.isEditor)
        {
            if (previewResolutionsList == null || captureResolutionsList == null)
            {
                UpdateDrodownLists();
            }
        }
        else
        {
            LogText.text = "Camera not available";
        }
    }

    public void DisplayParameters()
    {
        if (!Application.isEditor)
        {
            CalculateMaxLatency(JMRCameraManager.Instance.GetPreviewLatency());
            
            LogText.text = "Camera API \n "
                                + "isCamera Available:\t" + (JMRCameraManager.Instance.IsAvailable ? "Connect" : "Disconnect") + "\n"

                                + "isRecording:\t" + JMRCameraManager.Instance.IsRecording + "\n"

                                + "RecordingState:\t" + JMRCameraManager.Instance.GetRecordingState() + "\n"

                                + "Video Record Status:\t" + videoRecordStatus + "\n"

                                + "Preview Latency:\t" + (previewStarted ? (int)latency + " ms" : "Preview Not Started") + "\n"

                                + "Preview Max Latency:\t" + (previewStarted ? (int)maxLatency + " ms" : "Preview Not Started") + "\n"

                                + "Prev Res:\t" + JMRCameraManager.Instance.GetCurrentPreviewResolution().frameSizeText + "\n"

                                + "Capture Res:\t" + JMRCameraManager.Instance.GetCurrentCaptureResolution().frameSizeText + "\n"

                                + "Error Text:\t" + cameraError + "\n"

                    ;
        }
        else
        {
            LogText.text = "Camera not available";
        }
    }
}
