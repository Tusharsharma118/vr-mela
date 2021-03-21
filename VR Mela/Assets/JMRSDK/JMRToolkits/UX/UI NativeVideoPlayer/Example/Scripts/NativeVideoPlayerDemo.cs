using JMRSDK.Toolkit.UI;
using MediaServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeVideoPlayerDemo : MonoBehaviour
{
    public JMRNativeVideoPlayer videoPlayer;
    public GameObject videoPlayer_MaxPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Next()
    {
        videoPlayer.Next();
    }

    public void Previous()
    {
        videoPlayer.Previous();
    }

    public void OpenMaxPlayer()
    {
        if (videoPlayer_MaxPlayer)
            videoPlayer_MaxPlayer.SetActive(true);
    }
}
