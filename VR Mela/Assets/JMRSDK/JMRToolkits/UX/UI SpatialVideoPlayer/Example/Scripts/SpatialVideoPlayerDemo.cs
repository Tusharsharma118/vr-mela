using JMRSDK.Toolkit.UI;
using MediaServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SpatialVideoPlayerDemo : MonoBehaviour
{
    public JMRSpatialVideoPlayer videoPlayer;
    public GameObject videoPlayer_MaxPlayer;

    public List<VideoClip> clips = new List<VideoClip>();

    private int currentVideoIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.OnVideoEnded += VideoEnded;
        }

        LoadNextVideoAndPlay();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.OnVideoEnded -= VideoEnded;
        }
    }

    private void LoadNextVideoAndPlay()
    {
        if (videoPlayer != null)
        {
            currentVideoIndex++;

            if(currentVideoIndex >= clips.Count)
            {
                currentVideoIndex = 0;
            }

            videoPlayer.SetAndPlayMediaItem(clips[currentVideoIndex]);
        }
    }

    private void VideoEnded(VideoPlayer obj)
    {
        LoadNextVideoAndPlay();
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
