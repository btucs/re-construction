#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5 || UNITY_5_4_OR_NEWER
#define UNITY_FEATURE_UGUI
#endif

using UnityEngine;
#if UNITY_FEATURE_UGUI
using UnityEngine.UI;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using TMPro;

//-----------------------------------------------------------------------------
// Copyright 2015-2018 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

/// <summary>
/// A demo of a simple video player using uGUI for display
/// Uses two MediaPlayer components, with one displaying the current video
/// while the other loads the next video.  MediaPlayers are then swapped
/// once the video is loaded and has a frame available for display.
/// This gives a more seamless display than simply using a single MediaPlayer
/// as its texture will be destroyed when it loads a new video
/// </summary>
public class CustomVCR : MonoBehaviour
{
  public MediaPlayer _mediaPlayer;
  public MediaPlayer _mediaPlayerB;
  public MediaPlayer _mediaPlayerC;
  public DisplayUGUI _mediaDisplay;
  public RectTransform _bufferedSliderRect;

  public MediaPlayerEvent Events = new MediaPlayerEvent();

  public Slider _videoSeekSlider;
  private float _setVideoSeekSliderValue;
  private bool _wasPlayingOnScrub;

  public bool swapToPrevious = false;

  public Slider _audioVolumeSlider;
  private float _setAudioVolumeSliderValue;

  public Toggle _AutoStartToggle;
  public Toggle _MuteToggle;

  //public MediaPlayer.FileLocation _location = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
  public string _folder = "AVProVideoDemos/";
  public string[] _videoFiles = { "BigBuckBunny_720p30.mp4", "SampleSphere.mp4" };
  public ToggleUIImages togglePlayPauseButton;

  public Text remainingMinutes;
  public Text remainingSeconds;

  private int _VideoIndex = 0;
  private Image _bufferedSliderImage;

  [HideInInspector]
  public bool videoIsPlaying = false;

  [HideInInspector]
  public bool errorVideo = false;

  [HideInInspector]
  public bool startVideo = false;

  public TMP_Dropdown playbackSpeedDropdown;
  private float playbackSpeed = 1f;

  private MediaPlayer _loadingPlayer;
  private MediaPlayer previousPlayer;
  public MediaPlayer PlayingPlayer {
    get {
      if(LoadingPlayer == _mediaPlayer && previousPlayer == _mediaPlayerC
        || LoadingPlayer == _mediaPlayerC && previousPlayer == _mediaPlayer) {
        return _mediaPlayerB;
      } else if(LoadingPlayer == _mediaPlayer && previousPlayer == _mediaPlayerB
          || LoadingPlayer == _mediaPlayerB && previousPlayer == _mediaPlayer) {
        return _mediaPlayerC;
      }
      return _mediaPlayer;
    }
  }

  public MediaPlayer LoadingPlayer {
    get {
      return _loadingPlayer;
    }
  }


  private void SwapPlayers() {
    // Pause the previously playing video
    PlayingPlayer.Control.Pause();

    if(swapToPrevious == false) {
      // Swap the videos
      if(LoadingPlayer == _mediaPlayer) {
        _loadingPlayer = _mediaPlayerB;
      } else {
        _loadingPlayer = _mediaPlayer;
      }
    } else {
      previousPlayer = PlayingPlayer;
      swapToPrevious = false;
    }

    // Change the displaying video
    _mediaDisplay.CurrentMediaPlayer = PlayingPlayer;
    PlayingPlayer.PlaybackRate = playbackSpeed;

    //OnPlayPauseButton();
  }

  public void OnOpenVideoFile() {
    LoadingPlayer.OpenMedia(new MediaPath(System.IO.Path.Combine(_folder, _videoFiles[_VideoIndex]), MediaPathType.AbsolutePathOrURL), autoPlay: false);
    int prevVidIndex = _VideoIndex - 1;
    if(prevVidIndex < 0) {
      prevVidIndex = _videoFiles.Length + prevVidIndex;
    }
    previousPlayer.OpenMedia(new MediaPath(System.IO.Path.Combine(_folder, _videoFiles[prevVidIndex]), MediaPathType.AbsolutePathOrURL), autoPlay: false);

    _VideoIndex = (_VideoIndex + 1) % (_videoFiles.Length);

    if(string.IsNullOrEmpty(LoadingPlayer.MediaPath.Path)) {
      LoadingPlayer.CloseMedia();
      _VideoIndex = 0;
    } else {
      LoadingPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, LoadingPlayer.MediaPath.Path, false);
      //				SetButtonEnabled( "PlayButton", !_mediaPlayer.m_AutoStart );
      //				SetButtonEnabled( "PauseButton", _mediaPlayer.m_AutoStart );
    }

    if(string.IsNullOrEmpty(previousPlayer.MediaPath.Path)) {
      previousPlayer.CloseMedia();
    } else {
      previousPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, previousPlayer.MediaPath.Path, false);
    }

    if(_bufferedSliderRect != null) {
      _bufferedSliderImage = _bufferedSliderRect.GetComponent<Image>();
    }
  }

  public void OnOpenPreviousVideoFile() {
    swapToPrevious = true;
    OnOpenVideoFile();
  }

  public void OnAutoStartChange() {
    if(PlayingPlayer && PlayingPlayer.AutoStart != _AutoStartToggle.isOn) {
      PlayingPlayer.AutoStart = _AutoStartToggle.isOn;
    }
    if(LoadingPlayer && LoadingPlayer.AutoStart != _AutoStartToggle.isOn) {
      LoadingPlayer.AutoStart = _AutoStartToggle.isOn;
    }
  }

  public void SetLoadingPlayerAutoStartTrue() {
    LoadingPlayer.AutoStart = true;
    PlayingPlayer.AutoStart = true;
  }

  public void OnMuteChange() {
    if(PlayingPlayer) {
      PlayingPlayer.Control.MuteAudio(_MuteToggle.isOn);
    }
    if(LoadingPlayer) {
      LoadingPlayer.Control.MuteAudio(_MuteToggle.isOn);
    }
  }

  public void OnPlayButton() {
    if(PlayingPlayer) {
      PlayingPlayer.Control.Play();
      //				SetButtonEnabled( "PlayButton", false );
      //				SetButtonEnabled( "PauseButton", true );
    }
  }


  public void OnPlayPauseButton() {
    if(PlayingPlayer) {
      if (videoIsPlaying) {
        videoIsPlaying = false;
        PlayingPlayer.Control.Pause();
      } else {
        videoIsPlaying = true;
        PlayingPlayer.Control.Play();
        startVideo = false;
      }
      //				SetButtonEnabled( "PlayButton", false );
      //				SetButtonEnabled( "PauseButton", true );
      togglePlayPauseButton.ChangeUiImage(!videoIsPlaying);
    }
  }

  public void OnPauseButton() {
    if(PlayingPlayer) {
      PlayingPlayer.Control.Pause();
      //				SetButtonEnabled( "PauseButton", false );
      //				SetButtonEnabled( "PlayButton", true );
    }
  }

  public void OnVideoSeekSlider() {
    if(PlayingPlayer && _videoSeekSlider && _videoSeekSlider.value != _setVideoSeekSliderValue) {
      PlayingPlayer.Control.Seek(_videoSeekSlider.value * PlayingPlayer.Info.GetDuration());
    }
  }

  public void OnVideoSliderDown() {
    if(PlayingPlayer) {
      _wasPlayingOnScrub = PlayingPlayer.Control.IsPlaying();
      if(_wasPlayingOnScrub) {
        PlayingPlayer.Control.Pause();
        //					SetButtonEnabled( "PauseButton", false );
        //					SetButtonEnabled( "PlayButton", true );
      }
      OnVideoSeekSlider();
    }
  }
  public void OnVideoSliderUp() {
    if(PlayingPlayer && _wasPlayingOnScrub) {
      PlayingPlayer.Control.Play();
      _wasPlayingOnScrub = false;

      //				SetButtonEnabled( "PlayButton", false );
      //				SetButtonEnabled( "PauseButton", true );
    }
  }

  public void OnAudioVolumeSlider() {
    if(PlayingPlayer && _audioVolumeSlider && _audioVolumeSlider.value != _setAudioVolumeSliderValue) {
      PlayingPlayer.Control.SetVolume(_audioVolumeSlider.value);
    }
    if(LoadingPlayer && _audioVolumeSlider && _audioVolumeSlider.value != _setAudioVolumeSliderValue) {
      LoadingPlayer.Control.SetVolume(_audioVolumeSlider.value);
    }
  }
  //		public void OnMuteAudioButton()
  //		{
  //			if( _mediaPlayer )
  //			{
  //				_mediaPlayer.Control.MuteAudio( true );
  //				SetButtonEnabled( "MuteButton", false );
  //				SetButtonEnabled( "UnmuteButton", true );
  //			}
  //		}
  //		public void OnUnmuteAudioButton()
  //		{
  //			if( _mediaPlayer )
  //			{
  //				_mediaPlayer.Control.MuteAudio( false );
  //				SetButtonEnabled( "UnmuteButton", false );
  //				SetButtonEnabled( "MuteButton", true );
  //			}
  //		}

  public void OnRewindButton() {
    if(PlayingPlayer) {
      PlayingPlayer.Control.Rewind();
    }
  }

  private void Awake() {
    _loadingPlayer = _mediaPlayerB;
    previousPlayer = _mediaPlayerC;
  }

  void Start() {

    if(playbackSpeedDropdown != null) {

      playbackSpeedDropdown.onValueChanged.AddListener(OnPlaybackSpeedEvent);
    }

    if(PlayingPlayer) {
      PlayingPlayer.Events.AddListener(OnVideoEvent);

      if(LoadingPlayer) {
        LoadingPlayer.Events.AddListener(OnVideoEvent);
      }

      if(_audioVolumeSlider) {
        // Volume
        if(PlayingPlayer.Control != null) {
          float volume = PlayingPlayer.Control.GetVolume();
          _setAudioVolumeSliderValue = volume;
          _audioVolumeSlider.value = volume;
        }
      }

      // Auto start toggle
      //_AutoStartToggle.isOn = PlayingPlayer.m_AutoStart;

      if(PlayingPlayer.AutoOpen) {
        //					RemoveOpenVideoButton();

        //					SetButtonEnabled( "PlayButton", !_mediaPlayer.m_AutoStart );
        //					SetButtonEnabled( "PauseButton", _mediaPlayer.m_AutoStart );
      } else {
        //					SetButtonEnabled( "PlayButton", false );
        //					SetButtonEnabled( "PauseButton", false );
      }

      //				SetButtonEnabled( "MuteButton", !_mediaPlayer.m_Muted );
      //				SetButtonEnabled( "UnmuteButton", _mediaPlayer.m_Muted );

      OnOpenVideoFile();
    }
  }

  private void OnDestroy() {
    if(LoadingPlayer) {
      LoadingPlayer.Events.RemoveListener(OnVideoEvent);
    }
    if(PlayingPlayer) {
      PlayingPlayer.Events.RemoveListener(OnVideoEvent);
    }

    if(playbackSpeedDropdown != null) {

      playbackSpeedDropdown.onValueChanged.RemoveListener(OnPlaybackSpeedEvent);
    }
  }

  void Update()
  {
    if(PlayingPlayer && PlayingPlayer.Info != null && PlayingPlayer.Info.GetDuration() > 0f)
    {
      float time = (float) PlayingPlayer.Control.GetCurrentTime();
      float duration = (float) PlayingPlayer.Info.GetDuration();
      float d = Mathf.Clamp(time / duration, 0.0f, 1.0f);

      _setVideoSeekSliderValue = d;
      _videoSeekSlider.value = d;

      if(_bufferedSliderRect != null)
      {
        if(PlayingPlayer.Control.IsBuffering())
        {
          double t1 = 0f;
          double t2 = 0f;
          if(PlayingPlayer.Control.GetBufferedTimes().Count > 0)
          {
            TimeRange range = PlayingPlayer.Control.GetBufferedTimes()[0];
            t1 = range.StartTime;
            t1 /= PlayingPlayer.Info.GetDuration();
            t2 = range.EndTime;
            t2 /= PlayingPlayer.Info.GetDuration();
          }

          Vector2 anchorMin = Vector2.zero;
          Vector2 anchorMax = Vector2.one;

          if(_bufferedSliderImage != null && _bufferedSliderImage.type == Image.Type.Filled)
          {
            _bufferedSliderImage.fillAmount = d;
          }
          else
          {
            anchorMin[0] = (float) t1;
            anchorMax[0] = (float) t2;
          }

          _bufferedSliderRect.anchorMin = anchorMin;
          _bufferedSliderRect.anchorMax = anchorMax;
        }
      }
      
      if (videoIsPlaying)
      {
        SetRemainingTimeUI();
      }
    }
  }

  // Callback function to handle events
  public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode) {

    switch(et) {
      case MediaPlayerEvent.EventType.ReadyToPlay:
        SwapPlayers();
        startVideo = true;
        break;
      case MediaPlayerEvent.EventType.Stalled:
        //MediaPlayerIsReady();
        break;
      case MediaPlayerEvent.EventType.Started:
        break;
      case MediaPlayerEvent.EventType.FirstFrameReady:
        break;
      case MediaPlayerEvent.EventType.FinishedPlaying:
        HasFinishedPlaying();
        break;
      case MediaPlayerEvent.EventType.Error:
        errorVideo = true;
        break;
    }

    // forward Event to other handlers
    Events.Invoke(mp, et, errorCode);
  }

  void HasFinishedPlaying() {
    videoIsPlaying = false;
    togglePlayPauseButton.ChangeUiImage();
  }

  void MediaPlayerIsReady() {
    //OnPlayPauseButton();
  }

  //		private void SetButtonEnabled( string objectName, bool bEnabled )
  //		{
  //			Button button = GameObject.Find( objectName ).GetComponent<Button>();
  //			if( button )
  //			{
  //				button.enabled = bEnabled;
  //				button.GetComponentInChildren<CanvasRenderer>().SetAlpha( bEnabled ? 1.0f : 0.4f );
  //				button.GetComponentInChildren<Text>().color = Color.clear;
  //			}
  //		}

  //		private void RemoveOpenVideoButton()
  //		{
  //			Button openVideoButton = GameObject.Find( "OpenVideoButton" ).GetComponent<Button>();
  //			if( openVideoButton )
  //			{
  //				openVideoButton.enabled = false;
  //				openVideoButton.GetComponentInChildren<CanvasRenderer>().SetAlpha( 0.0f );
  //				openVideoButton.GetComponentInChildren<Text>().color = Color.clear;
  //			}
  //
  //			if( _AutoStartToggle )
  //			{
  //				_AutoStartToggle.enabled = false;
  //				_AutoStartToggle.isOn = false;
  //				_AutoStartToggle.GetComponentInChildren<CanvasRenderer>().SetAlpha( 0.0f );
  //				_AutoStartToggle.GetComponentInChildren<Text>().color = Color.clear;
  //				_AutoStartToggle.GetComponentInChildren<Image>().enabled = false;
  //				_AutoStartToggle.GetComponentInChildren<Image>().color = Color.clear;
  //			}
  //		}

  void SetRemainingTimeUI()
  {
    double currentTimeS = PlayingPlayer.Control.GetCurrentTime();
    double durationS = PlayingPlayer.Info.GetDuration();
    double remainingTimeS = (durationS - currentTimeS);

    string minutes = Mathf.Floor((int)remainingTimeS / 60).ToString("00");
    string seconds = ((int)remainingTimeS % 60).ToString("00");

    remainingMinutes.text = minutes;
    remainingSeconds.text = seconds;
  }

  private void OnPlaybackSpeedEvent(int index) {

    switch(index) {

      case 0:
        playbackSpeed = 1f;
        break;

      case 1:
        playbackSpeed = 1.5f;
        break;

      case 2:
        playbackSpeed = 2f;
        break;

      default:
        playbackSpeed = 1f;
        break;
    }

    PlayingPlayer.PlaybackRate = playbackSpeed;
  }
}
#endif