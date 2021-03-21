// Copyright (c) 2020 JioGlass. All Rights Reserved.

using MediaServices;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(VideoPlayer))]
    public class JMRSpatialVideoPlayer : MonoBehaviour
    {
        private enum PanelType { NONE, VOLUME, VIDEO_QUALITY_SETTING, AUDIO_SUBTITLE }

        #region Unity Video Player Variables
        private VideoPlayer videoPlayer;
        public VideoPlayer VideoPlayer => videoPlayer;
        #endregion

        #region Public Fields
        #endregion

        #region Serialize Fields
        [SerializeField]
        private Sprite PlaySprite;
        [SerializeField]
        private Sprite PauseSprite;
        [SerializeField]
        private Sprite UnmuteSprite;
        [SerializeField]
        private Sprite MuteSprite;

        [Header("Max Player")]
        [SerializeField]
        private GameObject MaxPlayer;
        [SerializeField]
        private GameObject MaxPlayerControls;
        [SerializeField]
        private RawImage maxPlayerRawImage;
        //[SerializeField]
        //private TMP_Text TitleText;
        [SerializeField]
        private TMP_Text SubtitleText;
        [SerializeField]
        private JMRUIButton PlayPauseButton;
        [SerializeField]
        private JMRUIButton MuteUnmuteButton;
        [SerializeField]
        //private JMRUISlider VolumeSlider;
        private Slider VolumeSlider;
        [SerializeField]
        //private JMRUISlider ProgressSlider;
        private Slider ProgressSlider;
        [SerializeField] 
        private Image BufferSlider;
        [SerializeField] 
        private TMP_Text CurrentProgressText;
        [SerializeField] 
        private TMP_Text TotalDurationText;
        [SerializeField]
        [Tooltip("In seconds")]
        private int seekValue = 10;
        [SerializeField]
        [Tooltip("In seconds")]
        private float autoHideTimeThreshold = 3f;

        [Header("Min Player")]
        [SerializeField]
        private GameObject MinPlayer;
        [SerializeField]
        private GameObject MinPlayerControls;
        [SerializeField]
        private RawImage minPlayerRawImage;
        [SerializeField]
        private JMRUIButton PlayPauseMinButton;

        [Header("Control Panels")]
        [SerializeField]
        private GameObject VolumeBarPanel;
        [SerializeField]
        private GameObject VideoQualitySettingPanel;
        [SerializeField]
        private GameObject AudioSubtitlePanel;
        [SerializeField]
        private GameObject QualityOptionPrefab;
        [SerializeField]
        private GameObject AudioOptionPrefab;
        [SerializeField]
        private GameObject SubtitleOptionPrefab;
        [SerializeField] 
        private GameObject QualityOptionParent;
        [SerializeField] 
        private GameObject AudioOptionParent;
        [SerializeField] 
        private GameObject SubtitleOptionParent;
        #endregion

        #region Private Variables
        private PanelType j_PanelType;
        private bool j_IsFirstTimePlay;
        private long j_TotalDuration;
        private int j_CurrentQualityIndex;
        private int j_CurrentAudioIndex;
        private int j_CurrentSubtitleIndex;
        private bool j_IsPointerOverControl = false;
        private bool j_IsControlsActive = false;
        private float j_Timer = 0f;
        private TimeSpan currentTimeSpan;
        private TimeSpan totalTimeSpan;
        private string currentTimeFormatted;
        private string totalTimeFormatted;
        private RenderTexture videoRenderTexture;
        private float seekWaitTimer = 0;
        #endregion

        #region Actions
        public Action<VideoPlayer> OnVideoEnded;
        public Action<VideoPlayer> OnVideoPrepareComplete;
        public Action<VideoPlayer, string> OnVideoError;
        #endregion

        #region Mono
        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        private void Start()
        {
            videoPlayer.loopPointReached += VideoLoopPointReached;
            videoPlayer.prepareCompleted += VideoPrepareCompleted;
            videoPlayer.errorReceived += VideoErrorReceived;
            videoPlayer.seekCompleted += VideoSeekCompleted;

            InitMediaPlayer();
        }

        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= VideoLoopPointReached;
            videoPlayer.prepareCompleted -= VideoPrepareCompleted;
            videoPlayer.errorReceived -= VideoErrorReceived;
            videoPlayer.seekCompleted -= VideoSeekCompleted;

            ReleaseRenderTexture();
        }

        private void Update()
        {
            if (j_IsControlsActive && !j_IsPointerOverControl)    // If pointer is not over any control button/UI
            {
                if (j_Timer > autoHideTimeThreshold)
                {
                    j_Timer = 0f;
                    
                    ToggleControlButtons(false);
                    ToggleControlPanels(PanelType.NONE, false);
                }
                else
                {
                    j_Timer += Time.deltaTime;
                }
            }

            if (seekWaitTimer > 0) 
            {
                seekWaitTimer -= Time.deltaTime;
            }

            if (ProgressSlider != null && seekWaitTimer <=0)
            {
                //ProgressSlider.SliderValueUI = (float) videoPlayer.frame / (float) videoPlayer.frameCount;
                //ProgressSlider.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                ProgressSlider.SetValueWithoutNotify((float)videoPlayer.frame / (float)videoPlayer.frameCount);
            }

            UpdateTimerDisplay();
        }
        #endregion

        #region Private Methods
        private void InitMediaPlayer()
        {
            videoPlayer.playOnAwake = false;

            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            //ProgressSlider.SliderValueUI = 0;
            ProgressSlider.value = 0;
            BufferSlider.fillAmount = 0;

            videoPlayer.SetDirectAudioVolume((ushort)0, 0.5f);
            //VolumeSlider.SliderValueUI = 0.5f;
            VolumeSlider.value = 0.5f;
            VolumeMuteUnMute(false);    // For showing Unmute button

            // At start hide panels
            ToggleControlPanels(PanelType.NONE, false);

            // At start show all control buttons
            ToggleControlButtons(true);

            // By default Min Player is disabled
            MinPlayer.SetActive(false);
        }

        private void SetVideoSize(float width, float height)
        {
            float videoAspectRatio = width / height;
            maxPlayerRawImage.GetComponent<AspectRatioFitter>().aspectRatio = videoAspectRatio;
            minPlayerRawImage.GetComponent<AspectRatioFitter>().aspectRatio = videoAspectRatio;

            // Release any existing render texture before creating new one
            ReleaseRenderTexture();

            videoRenderTexture = new RenderTexture((int)width, (int)height, 24, RenderTextureFormat.ARGB32);
            videoRenderTexture.antiAliasing = 2;
            videoRenderTexture.Create();

            videoPlayer.targetTexture = videoRenderTexture;

            maxPlayerRawImage.texture = videoRenderTexture;
            minPlayerRawImage.texture = videoRenderTexture;
        }

        private void ReleaseRenderTexture()
        {
            // Realese render textures
            if (videoRenderTexture != null)
            {
                videoRenderTexture.Release();
            }
            if (videoPlayer.targetTexture != null)
            {
                videoPlayer.targetTexture.Release();
            }
            if (maxPlayerRawImage.texture != null)
            {
                maxPlayerRawImage.texture = null;
            }
            if (minPlayerRawImage.texture != null)
            {
                minPlayerRawImage.texture = null;
            }
        }

        private void UpdateTimerDisplay()
        {
            currentTimeSpan = TimeSpan.FromSeconds(videoPlayer.time);
            totalTimeSpan = TimeSpan.FromSeconds(videoPlayer.length);

            currentTimeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", currentTimeSpan.Hours, currentTimeSpan.Minutes, currentTimeSpan.Seconds);
            totalTimeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", totalTimeSpan.Hours, totalTimeSpan.Minutes, totalTimeSpan.Seconds);

            CurrentProgressText.text = currentTimeFormatted;
            TotalDurationText.text = totalTimeFormatted;
        }

        private void ClearChildren(GameObject parentObject)
        {
            foreach (Transform child in parentObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private IEnumerator ReactivateGameObject(GameObject mGameObject)
        {
            mGameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            mGameObject.SetActive(true);
        }

        private void ToggleControlPanels(PanelType _panelType, bool activeStatus)
        {
            // Hide all panels
            HideAllControlPanels();

            // Enable panel as per control button click
            switch(_panelType)
            {
                case PanelType.VOLUME:
                    VolumeBarPanel.SetActive(activeStatus);
                    break;

                case PanelType.VIDEO_QUALITY_SETTING:
                    VideoQualitySettingPanel.SetActive(activeStatus);
                    break;

                case PanelType.AUDIO_SUBTITLE:
                    AudioSubtitlePanel.SetActive(activeStatus);
                    break;

                case PanelType.NONE:
                    break;

                default:
                    break;
            }
        }

        private void HideAllControlPanels()
        {
            VolumeBarPanel.SetActive(false);
            VideoQualitySettingPanel.SetActive(false);
            AudioSubtitlePanel.SetActive(false);
        }

        private void ToggleControlButtons(bool isActive)
        {
            if (isActive)
            {
                // Show control buttons
                j_IsControlsActive = true;

                MaxPlayerControls.SetActive(true);
                MinPlayerControls.SetActive(true);
            }
            else
            {
                // Hide control buttons
                j_IsControlsActive = false;

                MaxPlayerControls.SetActive(false);
                MinPlayerControls.SetActive(false);
            }
        }

        private void VolumeMuteUnMute(bool isMute)
        {
            if (isMute)
            {
                MuteUnmuteButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(MuteSprite);
            }
            else
            {
                MuteUnmuteButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(UnmuteSprite);
            }
        }

        private void VideoLoopPointReached(VideoPlayer source)
        {
            TogglePlayPauseButtonIcon(true);

            if (OnVideoEnded != null)
            {
                OnVideoEnded.Invoke(source);
            }
        }

        private void VideoPrepareCompleted(VideoPlayer source)
        {
            ClearChildren(AudioOptionParent);

            if (source.source == VideoSource.VideoClip)
            {
                SetVideoSize((int)source.clip.width, (int)source.clip.height);
            }
            else
            {
                SetVideoSize((int)source.width, (int)source.height);
            }

            //videoPlayer.controlledAudioTrackCount = videoPlayer.clip.audioTrackCount;

            //if (videoPlayer.controlledAudioTrackCount > 0)
            //{
            //    for (int i = 0; i < videoPlayer.controlledAudioTrackCount; i++)
            //    {
            //        string lang = videoPlayer.clip.GetAudioLanguage((ushort)i);
            //        if (!string.IsNullOrEmpty(lang))
            //        {
            //            GameObject audioOptionObject = Instantiate(AudioOptionPrefab, AudioOptionParent.transform);
            //            audioOptionObject.GetComponent<JMRThemeConfigHelper>().SetText(lang);
            //        }
            //    }
            //}
            //StartCoroutine(ReactivateGameObject(AudioOptionParent));

            if (OnVideoPrepareComplete != null)
            {
                OnVideoPrepareComplete.Invoke(source);
            }
        }

        private void VideoErrorReceived(VideoPlayer source, string message)
        {
            if (OnVideoError != null)
            {
                OnVideoError.Invoke(source, message);
            }
        }

        private void VideoSeekCompleted(VideoPlayer source)
        {
            seekWaitTimer = 0.75f;
        }
        #endregion

        #region Public Methods
        public void SeekStart()
        {
            seekWaitTimer = 0.75f;
        }

        public void PlayPause()
        {
            if (videoPlayer.source == VideoSource.VideoClip && videoPlayer.clip == null)    // If video clip null
            {
                Debug.LogError("No video clip available to play");
                return;
            }
            else if (videoPlayer.source == VideoSource.Url && string.IsNullOrEmpty(videoPlayer.url))    // If video URL null
            {
                Debug.LogError("No video url available to play");
                return;
            }

            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                TogglePlayPauseButtonIcon(true);
            }
            else
            {
                if (videoPlayer.isPrepared)
                {
                    videoPlayer.Prepare();
                }
                videoPlayer.Play();
                TogglePlayPauseButtonIcon(false);
            }

            ToggleControlPanels(PanelType.NONE, false);
        }

        public bool IsPlaying()
        {
            return videoPlayer.isPlaying;
        }

        public void Pause()
        {
            videoPlayer.Pause();
            TogglePlayPauseButtonIcon(true);
        }

        public void Stop()
        {
            videoPlayer.Stop();
            TogglePlayPauseButtonIcon(true);
        }

        /// <summary>
        /// Set true for showing play button and false for showing pause button
        /// </summary>
        /// <param name="showPlayIcon"></param>
        public void TogglePlayPauseButtonIcon(bool showPlayIcon)
        {
            if (showPlayIcon)
            {
                PlayPauseButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PlaySprite);
                PlayPauseMinButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PlaySprite);
            }
            else
            {
                PlayPauseButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
                PlayPauseMinButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
            }
        }

        public void Forward()
        {
            double updatedTime = videoPlayer.time += seekValue;
            if (updatedTime <= videoPlayer.length)
            {
                videoPlayer.time = updatedTime;
            }
        }

        public void Rewind()
        {
            double updatedTime = videoPlayer.time -= seekValue;
            if (updatedTime >= 0 )
            {
                videoPlayer.time = updatedTime;
            }
        }

        public void SetMediaItem(VideoClip videoClip)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
        }

        public void SetMediaItem(string videoUrl)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoUrl;
        }

        public void SetAndPlayMediaItem(VideoClip videoClip)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            videoPlayer.Play();
            TogglePlayPauseButtonIcon(false);
        }

        public void SetAndPlayMediaItem(string videoUrl)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoUrl;
            videoPlayer.Play();
            TogglePlayPauseButtonIcon(false);
        }

        public void Next()
        {
            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            SubtitleText.text = "";
        }

        public void Previous()
        {
            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            SubtitleText.text = "";
        }

        public void OnVolumeSliderValueChange(float value)
        {
            if (videoPlayer.canSetDirectAudioVolume)
            {
                videoPlayer.SetDirectAudioVolume((ushort)j_CurrentAudioIndex, value);

                if (value <= 0)
                {
                    VolumeMuteUnMute(true);     // For showing Mute button
                }
                else
                {
                    VolumeMuteUnMute(false);    // For showing Unmute button
                }
            }
        }

        public void OnProgressSliderValueChange(float value)
        {
            float frame = value* videoPlayer.frameCount;
            videoPlayer.frame = (long)frame;
        }

        public void OnProgressSliderValueChangeDone()
        {
            //float frame = ProgressSlider.SliderValueUI * videoPlayer.frameCount;
            //videoPlayer.frame = (long)frame;
        }

        public void OnQualitySelected(int index)
        {
            j_CurrentQualityIndex = index;
        }

        public void OnAudioSelected(int index)
        {
            j_CurrentAudioIndex = index;

            //videoPlayer.controlledAudioTrackCount = videoPlayer.clip.audioTrackCount;

            //if (videoPlayer.controlledAudioTrackCount > 0)
            //{
            //    for (int i = 0; i < videoPlayer.controlledAudioTrackCount; i++)
            //    {
            //        if (i != index)
            //        {
            //            videoPlayer.SetDirectAudioVolume((ushort)index, 0);     // Set other audiotracks volume to 0
            //        }
            //        else
            //        {
            //            videoPlayer.SetDirectAudioVolume((ushort)index, 0.5f);
            //        }
            //    }
            //}
        }

        public void OnSubtitleSelected(int index)
        {
            j_CurrentSubtitleIndex = index;
        }

        public void OnVolumeButtonClick()
        {
            ToggleControlPanels(PanelType.VOLUME, !VolumeBarPanel.activeSelf);
        }

        public void OnVideoQualitySetttingButtonClick()
        {
            ToggleControlPanels(PanelType.VIDEO_QUALITY_SETTING, !VideoQualitySettingPanel.activeSelf);
        }

        public void OnAudiosSubtitlesButtonClick()
        {
            ToggleControlPanels(PanelType.AUDIO_SUBTITLE, !AudioSubtitlePanel.activeSelf);
        }

        public void OnMinimiseButtonClick()
        {
            MaxPlayer.SetActive(false);
            MinPlayer.SetActive(true);
        }

        public void OnMaximiseButtonClick()
        {
            MaxPlayer.SetActive(true);
            MinPlayer.SetActive(false);
        }

        public void OnCloseClick()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }

            TogglePlayPauseButtonIcon(true);

            MaxPlayer.SetActive(false);
            MinPlayer.SetActive(false);
        }

        public void PointerOverControl(bool state)
        {
            j_IsPointerOverControl = state;

            if (state && !j_IsControlsActive)
            {
                ToggleControlButtons(true);
            }
        }
        #endregion
    }
}