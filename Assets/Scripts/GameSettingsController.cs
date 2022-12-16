#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSettingsController : MonoBehaviour
{
  public GameObject audioSettingsContent;
  public GameObject graphicSettingsContent;
  public GameObject generalSettingsContent;
  public Slider uiSoundSlider;
  public Slider environmentSoundSlider;
  public Slider musicSoundSlider;
  public Toggle soundToggle;
  public Button submitButton;


  private GameController controller;
  private SettingPreferences currentSettings;
  private bool settingsChanged = false;
  private UnityAction confirmReset;

  private FMOD.Studio.Bus Master;
  private float masterVolume = 1f;

  private FMOD.Studio.VCA musicVCA;
  private FMOD.Studio.VCA effectsVCA;

  private void Awake()
  {
    Master = FMODUnity.RuntimeManager.GetBus("bus:/");
    musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
    effectsVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Sound");
  }

  private void Start()
  {
    InitializeSettings();
  }

  public void InitializeSettings()
  {
    controller = GameController.GetInstance();
    if (currentSettings == null)
      currentSettings = new SettingPreferences(controller.gameState.settings);
    UpdateSliderValues();
    Debug.Log("music sound level is " + currentSettings.audioSettings.musicVolume);
  }

  public void DisplayAudioSettings()
  {
    submitButton.gameObject.SetActive(true);
    audioSettingsContent.SetActive(true);
    graphicSettingsContent.SetActive(false);
    generalSettingsContent.SetActive(false);
  }

  public void DisplayGraphicSettings()
  {
    submitButton.gameObject.SetActive(true);
    audioSettingsContent.SetActive(false);
    graphicSettingsContent.SetActive(true);
    generalSettingsContent.SetActive(false);
  }

  public void DisplayGeneralSettings()
  {
    submitButton.gameObject.SetActive(false);
    audioSettingsContent.SetActive(false);
    graphicSettingsContent.SetActive(false);
    generalSettingsContent.SetActive(true);
  }

  public void SetGraphicsQuality(int index)
  {
    if (index != currentSettings.graphicsQualityIndex)
    {
      QualitySettings.SetQualityLevel(index);
      currentSettings.graphicsQualityIndex = index;
      settingsChanged = true;
    }
    UpdateSubmitButton();
  }

  public void RequestGameReset()
  {
    string confirmText = "Bist du sicher, dass du den Spielstand zurücksetzen willst?";
    confirmReset = null;
    confirmReset += ResetSaveGame;
    PopUpManager.Instance.DisplayConfirmPopUp(confirmText, confirmReset);
  }

  public void ResetSaveGame()
  {
    controller.gameState.Reset();
    controller.SaveGame();
    SceneManager.LoadScene("Game Opening");
  }

  public void LoadMultiplayerScene()
  {
    SceneManager.LoadScene("StartMultiplayer");
  }

  public void LoadWelcomeScene()
  {
    controller.SaveGame();
    SceneManager.LoadScene("WelcomeScreen");
  }

  public void SubmitSettings()
  {
    settingsChanged = false;
    controller.gameState.settings = currentSettings;
    controller.SaveGame();
    UpdateSubmitButton();
  }

  public void ToggleSound(bool toBeActive)
  {
    currentSettings.audioSettings.soundEnabled = toBeActive;
    settingsChanged = true;
    UpdateSubmitButton();
    SetMasterVolume(toBeActive);
  }

  public void SetUIVolume(float volume)
  {
    if (currentSettings.audioSettings.UISoundVolume != volume)
    {
      currentSettings.audioSettings.UISoundVolume = volume;
      settingsChanged = true;
    }
    UpdateSubmitButton();
  }

  public void SetEnvironmentVolume(float volume)
  {
    if (currentSettings.audioSettings.environmentVolume != volume)
    {
      currentSettings.audioSettings.environmentVolume = volume;
      SetEffectsSoundLevel(volume);
      settingsChanged = true;
    }
    UpdateSubmitButton();
  }

  public void SetMusicVolume(float volume)
  {
    if (currentSettings.audioSettings.musicVolume != volume)
    {
      currentSettings.audioSettings.musicVolume = volume;
      settingsChanged = true;
      SetMusicSoundLevel(volume);
    }
    UpdateSubmitButton();
  }

  public void ExitGame()
  {
    Debug.Log("App should now exit.");
    controller.SaveGame();
    Application.Quit();
  }

  private void UpdateSubmitButton()
  {
    submitButton.interactable = settingsChanged;
  }

  private void UpdateSliderValues()
  {
    uiSoundSlider.value = currentSettings.audioSettings.UISoundVolume;
    environmentSoundSlider.value = currentSettings.audioSettings.environmentVolume;
    musicSoundSlider.value = currentSettings.audioSettings.musicVolume;
    soundToggle.isOn = currentSettings.audioSettings.soundEnabled;

    SetMasterVolume(currentSettings.audioSettings.soundEnabled);
    SetMusicSoundLevel(currentSettings.audioSettings.musicVolume);
    SetEffectsSoundLevel(currentSettings.audioSettings.environmentVolume);
  }

  private void SetMasterVolume(bool soundIsOn)
  {
    masterVolume = soundIsOn ? 1f : 0f;
    Master.setVolume(masterVolume);
  }

  private void SetMusicSoundLevel(float newSoundLevel)
  {
    musicVCA.setVolume(newSoundLevel);
  }

  private void SetEffectsSoundLevel(float newSoundLevel)
  {
    effectsVCA.setVolume(newSoundLevel);
  }

}
