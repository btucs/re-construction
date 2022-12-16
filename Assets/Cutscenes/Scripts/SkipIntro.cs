#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using FMODUnity;

public class SkipIntro : MonoBehaviour
{
  [Required]
  public Button skipButton;
  [Required]
  public Text skipButtonText;
  [Required]
  public PlayableDirector director;
  [Required]
  public StudioEventEmitter fmodEmitter;

  [Required]
  public string skipText;
  [Required]
  public float skipTo;
  [Required]
  public string loadSceneName;

  public void Skip() {

    director.time = skipTo;
    fmodEmitter.EventInstance.setTimelinePosition((int) skipTo * 1000);
  }

  public void UpdateSkipButton() {

    skipButtonText.text = skipText;
    skipButton.onClick.RemoveListener(Skip);
    skipButton.onClick.AddListener(LoadScene);
  }

  public void LoadScene() {

    SceneManager.LoadScene(loadSceneName);
  }

  private void Start() {

    skipButton.onClick.AddListener(Skip);
  }
}
