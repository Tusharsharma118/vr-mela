using System;
using System.Collections.Generic;
using JMRSDK;
using JMRSDK.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class CameraExample : MonoBehaviour
{
    public Text LogText;

    // private JMRCameraAPI cameraAPI;
    private string mediacapturepath;

    private string cameraStatus = "Unavailable";

    // public Material renderMat;
    // public Material renderMat2;
    private List<FrameSize> previewResolutionsList = new List<FrameSize>();
    private List<FrameSize> captureResolutionsList = new List<FrameSize>();
    private FrameSize CurrentRes;
    private Texture2D camTexture;
    private Texture2D camTexture2;

    private bool previewStarted;

    public JMR2DDropDown previewResolutions, captureResolutions;

    public JMRUIButton startPreviewButton, stopPreviewButton;
    public JMRUIButton captureImageButton, captureImageNameButton;
    public JMRUIButton startRecordButton, startRecordNameButton;
    public JMRUIButton pauseRecordButton, resumeRecordButton, stopRecordButton;

    private void Start()
    {
        pauseRecordButton.gameObject.SetActive(false);
        resumeRecordButton.gameObject.SetActive(false);
        stopRecordButton.gameObject.SetActive(false);
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
        }
        else
        {
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        JMRCameraManager.OnCameraConnect += OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect += OnCameraDisconnect;
        JMRCameraManager.OnImageCapture += OnImageCapture;
        JMRCameraManager.OnVideoRecord += OnVideoRecord;
    }

    private void OnDisable()
    {
        JMRCameraManager.OnCameraConnect -= OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect -= OnCameraDisconnect;
        JMRCameraManager.OnImageCapture -= OnImageCapture;
        JMRCameraManager.OnVideoRecord -= OnVideoRecord;
    }

    private void OnVideoRecord(String obj, JMRCameraManager.VideoRecordState state)
    {
        Debug.Log($"OnVideoRecord({state}, {obj}");
        switch (state)
        {
            case JMRCameraManager.VideoRecordState.Started:
                cameraStatus = "Record : VIDEO_RECORD_STATE_STARTED";
                break;
            case JMRCameraManager.VideoRecordState.Paused:
                cameraStatus = "Record : VIDEO_RECORD_STATE_PAUSED";
                break;
            case JMRCameraManager.VideoRecordState.Resumed:
                cameraStatus = "Record : VIDEO_RECORD_STATE_RESUMED";
                break;
            case JMRCameraManager.VideoRecordState.Stopped:
                cameraStatus = "Record : VIDEO_RECORD_STATE_STOPPED";
                break;
            case JMRCameraManager.VideoRecordState.Completed:
                cameraStatus = "Record : VIDEO_RECORD_STATE_COMPLETED";
                break;
            default:
                cameraStatus = "Record : UNKNOWN STATE";
                break;
        }

        mediacapturepath = obj;
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
            cameraStatus = "Connect";

            previewResolutionsList.Clear();
            previewResolutionsList = JMRCameraManager.Instance.GetPreviewResolutions();
            if (previewResolutionsList != null)
            {
                for (int i = 0; i < previewResolutionsList.Count; i++)
                {
                    previewResolutions.AddNewOption(i, previewResolutionsList[i].frameSizeText);
                }
            }

            captureResolutionsList.Clear();
            captureResolutionsList = JMRCameraManager.Instance.GetCaptureResolutions();
            if (captureResolutionsList != null)
            {
                for (int i = 0; i < captureResolutionsList.Count; i++)
                {
                    captureResolutions.AddNewOption(i, captureResolutionsList[i].frameSizeText);
                }
            }

            CurrentRes = JMRCameraManager.Instance.GetCurrentCaptureResolution();
            camConnect = true;
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(false);
            isCamAvailable = true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
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
            JMRCameraManager.Instance.SetPreviewResolution(previewResolutionsList[res]);
    }

    public void SetCaptureRes(int res)
    {
        if (captureResolutionsList != null && res < captureResolutionsList.Count)
            JMRCameraManager.Instance.SetCaptureResolution(captureResolutionsList[res]);
    }

    private void LateUpdate()
    {
        LogText.text = "Camera API \n "
                       + "isCamera Available:\t" + cameraStatus + "\n"
                       + "isRecording:\t" + JMRCameraManager.Instance.IsRecording + "\n"
                       + "RecordingState:\t" + JMRCameraManager.Instance.GetRecordingState() + "\n"
                       + "Prev Res:\t" + JMRCameraManager.Instance.GetCurrentPreviewResolution().frameSizeText + "\n"
                       + "Capture Res:\t" + JMRCameraManager.Instance.GetCurrentCaptureResolution().frameSizeText + "\n"
            ;
    }
}