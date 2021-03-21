using JMRSDK;
using JMRSDK.Toolkit.UI;
using TMPro;
using UnityEngine;

public class VoiceExample : MonoBehaviour
{
    public TMP_Text LogText;

    public JMRUIButton startButton, stopButton;
    
    private string spEvents;
    private string spPartialResult;
    private string spResult;
    private string Hotkeyword;
    private string initResult;

    private void OnEnable()
    {
        JMRVoiceManager.OnSpeechEvent += SpeechEvent;
        JMRVoiceManager.OnSpeechPartialResults += SpeechPartialResult;
        JMRVoiceManager.OnSpeechResults += SpeechResult;
        JMRVoiceManager.OnHotwordDetected += HotkeywordDetected;
    }

    private void HotkeywordDetected(int arg1, float arg2)
    {
        Hotkeyword = "HotKeyword " + arg1 + " " + arg2;
    }

    private void SpeechResult(string obj, long ts)
    {
        spResult = obj;
    }

    private void SpeechPartialResult(string obj, long ts)
    {
        spPartialResult = obj;
    }

    private void SpeechEvent(JMRVoiceManager.SpeechEvent speechEvent, long ts)
    {
        spEvents = speechEvent.ToString();
    }

    public void StartListening()
    {
        JMRVoiceManager.Instance.StartListening();
    }

    public void StopListening()
    {
        JMRVoiceManager.Instance.StopListening();
    }

    private void LateUpdate()
    {
        if (JMRVoiceManager.Instance.IsSpeechRecognizerAvailable)
        {
            startButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(false);
        }
        
        if (!Application.isEditor)
        {
            LogText.text = "Voice API \n "
                           + "Voice init:\t" + JMRVoiceManager.Instance.IsAPIReady() + "\n"
                           + "Voice services available:\t" + JMRVoiceManager.Instance.IsSpeechRecognizerAvailable + "\n"
                           + "Voice Events:\t" + spEvents + "\n"
                           + "Voice Partial Result:\t" + spPartialResult + "\n"
                           + "Voice Result:\t" + spResult + "\n"
                           + "Voice HotKeyword:\t" + Hotkeyword + "\n"
                ;
        }
    }
}