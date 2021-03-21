// Copyright (c) 2020 JioGlass. All Rights Reserved.

using MediaServices;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(MediaPlayer))]
    public class JMRNativeVideoPlayer : MonoBehaviour, IMediaEventListner, IAudioEventListner
    {
        private enum PanelType { NONE, VOLUME, VIDEO_QUALITY_SETTING, AUDIO_SUBTITLE }

        public List<string> videoURL = new List<string>();
        public bool PlayOnAwake = true;
        public bool Loop = false;

        #region Media Player Variables
        private MediaPlayer mediaPlayer;
        private MediaRenderer mediaRendrer;
        private MediaTrackSelector mediaTrackSelector;
        private MediaLoadController mediaLoadController;
        private AudioManager audioManager;
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
        [SerializeField] 
        private Material VideoPlayerMaterial;

        [Header("Max Player")]
        [SerializeField]
        private GameObject MaxPlayer;
        [SerializeField]
        private GameObject MaxPlayerControls;
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
        [Tooltip("In milli seconds")]
        private int seekValue = 10000;
        [SerializeField]
        [Tooltip("In seconds")]
        private float autoHideTimeThreshold = 3f;

        [Header("Min Player")]
        [SerializeField]
        private GameObject MinPlayer;
        [SerializeField]
        private GameObject MinPlayerControls;
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
        private GameObject QualityOptionParent;
        [SerializeField]
        private GameObject AudioOptionPrefab;
        [SerializeField]
        private GameObject AudioOptionParent;
        [SerializeField]
        private GameObject SubtitleOptionPrefab;
        [SerializeField]
        private GameObject SubtitleOptionParent;
        #endregion

        #region Private Variables
        private PanelType j_PanelType;
        private bool j_IsFirstTimePlay;
        private long j_TotalDuration;
        private MediaDetails j_CurrentMediaDetails;
        private int j_CurrentQualityIndex;
        private int j_CurrentAudioIndex;
        private int j_CurrentSubtitleIndex;
        private bool j_IsPointerOverControl = false;
        private bool j_IsControlsActive = false;
        private float j_Timer = 0f;
        #endregion

        #region Mono
        private void Awake()
        {
            mediaPlayer = GetComponent<MediaPlayer>();
        }

        private void Start()
        {
            InitMediaPlayer();

            if (videoURL.Count > 0)
            {
                LoadVideoFromURL();
            }

            if (PlayOnAwake)
            {
                PlayPause();
            }
        }

        private void OnDestroy()
        {
            UnregisterListner();
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
        }
        #endregion

        #region Private Methods
        private void InitMediaPlayer()
        {
            // Setup Media Player object.
            mediaRendrer = new MediaRenderer();
            mediaTrackSelector = new MediaTrackSelector();
            mediaLoadController = new MediaLoadController();
            mediaPlayer.Init(mediaRendrer, mediaTrackSelector, mediaLoadController);
            if (VideoPlayerMaterial != null)
            {
                mediaPlayer.PrepareVideoPlayerSurface(VideoPlayerMaterial);
            }
            else
            {
                JMRLogHandler.LogError("JMRNativeVideoPlayer: Video player material not available.");
            }
            mediaPlayer.setLogEnabled(true);

            // Setup Audio Manager object.
            audioManager = new AudioManager(mediaPlayer);
            audioManager?.AppResume();

            RegisterListner();

            if (Loop)
            {
                mediaPlayer.SetRepeatMode(2);
            }

            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            ProgressSlider.value = 0;
            BufferSlider.fillAmount = 0;

            // At start hide panels
            ToggleControlPanels(PanelType.NONE, false);

            // At start show all control buttons
            ToggleControlButtons(true);

            // By default Min Player is disabled
            MinPlayer.SetActive(false);
        }

        private void RegisterListner()
        {
            mediaPlayer?.AddMediaPlayerListner(this);
            audioManager?.AddAudioEventListener(this);
        }

        private void UnregisterListner()
        {
            mediaPlayer?.RemoveMediaPlayerListener();
            audioManager?.RemoveAudioEventListener();
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

        private void LoadVideoFromURL()
        {
            MediaItem videoMediaItem;

            for (int i = 0; i < videoURL.Count; i++)
            {
                videoMediaItem = new MediaItem(
                    videoURL[i],
                    false,
                    null,
                    "Example " + i,
                    null,
                    null,
                    null
                );
                mediaPlayer.AddItem(videoMediaItem);
            }
            
        }
        #endregion

        #region Public Methods
        public bool AddItem(MediaItem mediaItem)
        {
            return mediaPlayer.AddItem(mediaItem);
        }

        public bool AddItems(ArrayList mediaItems)
        {
            return mediaPlayer.AddItems(mediaItems);
        }

        public bool RemoveItem(MediaItem mediaItem)
        {
            return mediaPlayer.RemoveItem(mediaItem);
        }

        public bool RemoveAllItems()
        {
            return mediaPlayer.RemoveAllItems();
        }

        public void MoveItem(int newIndex, int oldIndex)
        {
            mediaPlayer.MoveItem(newIndex, oldIndex);
        }

        public ArrayList GetAllItems()
        {
            return mediaPlayer.GetAllItems();
        }

        public int GetCurrentIndex()
        {
            return mediaPlayer.GetCurrentIndex();
        }

        public int GetPlaylistSize()
        {
            return mediaPlayer.Size();
        }

        public bool IsExistInCurrentList(MediaItem mediaItem)
        {
            return mediaPlayer.IsExistInCurrentList(mediaItem);
        }

        public void Play(MediaItem mediaItem)
        {
            mediaPlayer.Play(mediaItem);
        }

        public void Play(int index)
        {
            if (mediaPlayer.Size() > index)
                mediaPlayer.Play(index);
        }

        public bool TogglePlayPause()
        {
            bool isMediaPlaying = mediaPlayer.TogglePlayPause();

            if (!isMediaPlaying)
            {
                PlayPauseButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PlaySprite);
                PlayPauseMinButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PlaySprite);
            }
            else
            {
                PlayPauseButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
                PlayPauseMinButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
            }

            return isMediaPlaying;
        }

        public bool IsPlaying()
        {
            return mediaPlayer.IsPlaying();
        }

        public void PlayPause()
        {
            if (mediaPlayer.Size() > 0)
            {
                if (!j_IsFirstTimePlay)
                {
                    j_IsFirstTimePlay = true;
                    Play(0);
                    PlayPauseButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
                    PlayPauseMinButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(PauseSprite);
                }
                else
                {
                    TogglePlayPause();
                }
            }
            else
            {
                Debug.LogError("JMRNativeVideoPlayer : No media available to play. Add media item to media player object before playing.");
            }

            ToggleControlPanels(PanelType.NONE, false);
        }

        public void Forward()
        {
            mediaPlayer.Forward(seekValue);
        }

        public void Rewind()
        {
            mediaPlayer.Rewind(seekValue);
        }

        public void Next()
        {
            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            SubtitleText.text = "";

            mediaPlayer.Next();
        }

        public void Previous()
        {
            j_CurrentQualityIndex = 0;
            j_CurrentAudioIndex = 0;
            j_CurrentSubtitleIndex = 0;

            SubtitleText.text = "";

            mediaPlayer.Previous();
        }

        public bool HasNext()
        {
            return mediaPlayer.HasNext();
        }

        public bool HasPrevious()
        {
            return mediaPlayer.HasPrevious();
        }

        public bool Stop()
        {
            return mediaPlayer.Stop();
        }

        public void Retry()
        {
            mediaPlayer.Retry();
        }

        /// <summary>
        /// NONE = 0 
        /// ONE = 1 
        /// ALL = 2 
        /// </summary>
        /// <param name="repeatMode"></param>
        public void SetRepeatMode(int repeatMode)
        {
            mediaPlayer.SetRepeatMode(repeatMode);
        }

        public int GetRepeatMode()
        {
            return mediaPlayer.GetRepeatMode();
        }

        public bool GetShuffleModeEnabled()
        {
            return mediaPlayer.ShuffleModeEnabled();
        }

        public void SetShuffleModeEnabled(bool mode)
        {
            mediaPlayer.SetShuffleModeEnabled(mode);
        }

        public void OnVolumeSliderValueChange(float value)
        {
            audioManager.changeVolume((int)value);
        }

        public void OnProgressSliderValueChange(float value)
        {
            long newCurrentPosition = (long)(value * j_TotalDuration);
            mediaPlayer.SeekBarPosition(newCurrentPosition);
        }

        public void OnProgressSliderValueChangeDone()
        {
            //long newCurrentPosition = (long)(ProgressSlider.SliderValueUI * j_TotalDuration);
            //mediaPlayer.SeekBarPosition(newCurrentPosition);
        }

        public void OnQualitySelected(int index)
        {
            j_CurrentQualityIndex = index;
            MediaItemQuality quality = (MediaItemQuality)j_CurrentMediaDetails.mediaItemQuality[j_CurrentQualityIndex];
            mediaTrackSelector.ChangeVideoQuality((int)quality.width, (int)quality.height, (int)quality.bitRate_bps);
        }

        public void OnAudioSelected(int index)
        {
            j_CurrentAudioIndex = index;
            mediaTrackSelector.ChangeAudioLanguage((TrackSelectorModel)j_CurrentMediaDetails.audioTrackList[j_CurrentAudioIndex]);
        }

        public void OnSubtitleSelected(int index)
        {
            j_CurrentSubtitleIndex = index;
            mediaTrackSelector.ChangeSubtitleLanguage((TrackSelectorModel)j_CurrentMediaDetails.subtitleLanguageList[j_CurrentSubtitleIndex]);
        }

        public void OnVolumeButtonClick()
        {
            // Mute or unmute audio
            //audioManager.ToggleMuteUnMute();

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
            if (IsPlaying())
            {
                TogglePlayPause();
            }

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

        #region Media Event Listner
        void IMediaEventListner.onCurrentSelectedMediaItem(MediaItem mediaItem)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onIsPlayingChanged(bool isPlaying)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onMediaItemQualityChange(MediaItemQuality mediaItemQuality)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onPlayerError(string errorMessage, int errorType, int httpErrorCode, string extraInfo)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onPositionDiscontinuity(int reason)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onQueuePositionChanged(int previousIndex, int newIndex)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onRepeatModeChanged(int repeatMode)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onShuffleModeChanged(bool isShuffleEnable)
        {
            //throw new System.NotImplementedException();
        }

        void IMediaEventListner.onStartContent(long ContentDuration, string formattedContentDuration)
        {
            //throw new System.NotImplementedException();

            MediaItem currentMediaItem = (MediaItem) mediaPlayer.GetAllItems()[mediaPlayer.GetCurrentIndex()];
            //if (TitleText != null)
            //    TitleText.text = currentMediaItem?._title;

            j_TotalDuration = ContentDuration;
            TotalDurationText.text = Utility.GetFormattedDuration(j_TotalDuration);
            CurrentProgressText.text = formattedContentDuration;

            SubtitleText.text = "";
        }

        void IMediaEventListner.onUpdateContent(string formattedCurrentPosition, long currentPosition, string formattedBufferedPosition, long bufferedPosition)
        {
            //throw new System.NotImplementedException();

            CurrentProgressText.text = formattedCurrentPosition;
            //ProgressSlider.value = (float) ((float) currentPosition / (float) j_TotalDuration);
            ProgressSlider.SetValueWithoutNotify((float)((float)currentPosition / (float)j_TotalDuration));
            BufferSlider.fillAmount = (float) ((float) bufferedPosition / (float) j_TotalDuration);
        }

        void IMediaEventListner.onUpdateSubtitle(string subtitleText)
        {
            //throw new System.NotImplementedException();
            
            SubtitleText.text = subtitleText;
        }

        void IMediaEventListner.onVideoDetails(MediaDetails mediaDetails)
        {
            //throw new System.NotImplementedException();

            ClearChildren(QualityOptionParent);
            ClearChildren(AudioOptionParent);
            ClearChildren(SubtitleOptionParent);

            j_CurrentMediaDetails = mediaDetails;

            if (j_CurrentMediaDetails.mediaItemQuality?.Count > 0)
            {
                foreach (MediaItemQuality quality in j_CurrentMediaDetails?.mediaItemQuality)
                {
                    GameObject qualityOptionObject = Instantiate(QualityOptionPrefab, QualityOptionParent.transform);
                    //qualityOptionObject.GetComponent<JMRToolkitTextAccessor>().SetText(quality?.width + "X" + quality?.height);
                    qualityOptionObject.GetComponent<JMRThemeConfigHelper>().SetText(quality?.width + "X" + quality?.height);
                }
            }
            StartCoroutine(ReactivateGameObject(QualityOptionParent));

            if (j_CurrentMediaDetails.audioTrackList?.Count > 0)
            {
                foreach (TrackSelectorModel audio in j_CurrentMediaDetails?.audioTrackList)
                {
                    GameObject audioOptionObject = Instantiate(AudioOptionPrefab, AudioOptionParent.transform);
                    //audioOptionObject.GetComponent<JMRToolkitTextAccessor>().SetText(audio?.lang);
                    audioOptionObject.GetComponent<JMRThemeConfigHelper>().SetText(audio?.lang);
                }
            }
            StartCoroutine(ReactivateGameObject(AudioOptionParent));

            if (j_CurrentMediaDetails.subtitleLanguageList?.Count > 0)
            {
                foreach (TrackSelectorModel sub in j_CurrentMediaDetails?.subtitleLanguageList)
                {
                    GameObject subtitleOptionObject = Instantiate(SubtitleOptionPrefab, SubtitleOptionParent.transform);
                    //subtitleOptionObject.GetComponent<JMRToolkitTextAccessor>().SetText(sub?.lang);
                    subtitleOptionObject.GetComponent<JMRThemeConfigHelper>().SetText(sub?.lang);
                }
            }
            StartCoroutine(ReactivateGameObject(SubtitleOptionParent));
        }
        #endregion

        #region Audio Event Listner
        public void onInitialize(int minMusicStreamVolume, int maxMusicStreamVolume)
        {
            //throw new System.NotImplementedException();

            //VolumeSlider.MinValue = minMusicStreamVolume;
            //VolumeSlider.MaxValue = maxMusicStreamVolume;
            //VolumeSlider.SliderValueUI = audioManager.GetAudioCurrentVolume();

            VolumeSlider.minValue = minMusicStreamVolume;
            VolumeSlider.maxValue = maxMusicStreamVolume;
            VolumeSlider.value = audioManager.GetAudioCurrentVolume();
        }

        public void onVolumeChange(int currentVolume)
        {
            //throw new System.NotImplementedException();

            //VolumeSlider.SliderValueUI = currentVolume;
            VolumeSlider.value = currentVolume;
        }

        public void onVolumeMuteUnMute(bool isMute)
        {
            //throw new System.NotImplementedException();

            if (isMute)
            {
                MuteUnmuteButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(MuteSprite);
            }
            else
            {
                MuteUnmuteButton.GetComponent<JMRThemeConfigHelper>().SetSpriteIcon(UnmuteSprite);
            }
        }
        #endregion

        public void SetLogEnabled(bool val)
        {
            mediaPlayer.setLogEnabled(val);
        }

        public void SetBackgroundPlay(bool isEnabled)
        {
            mediaPlayer.SetBackgroundPlay(isEnabled);
        }
    }
}