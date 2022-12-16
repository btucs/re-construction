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
using Sirenix.OdinInspector;

public class MLEProgressBarController : MonoBehaviour {

  [Required, ChildGameObjectsOnly]
  public Text headline;
  [Required, ChildGameObjectsOnly]
  public Text mleName;

  // public MLEProgressBarMilestone mleMilestone;

  [Required, ChildGameObjectsOnly]
  public Button previousStep;

  [Required, ChildGameObjectsOnly]
  public Button nextStep;

  [Required, ChildGameObjectsOnly]
  public Text currentScore;

  [Required, ChildGameObjectsOnly]
  public Transform ProgressBarContainer;

  [Required, ChildGameObjectsOnly]
  public RectTransform positionMarker;

  [Required, AssetsOnly]
  public GameObject ProgressBarElementPrefab;

  private List<MLEProgressBarMilestone> milestones = new List<MLEProgressBarMilestone>();
  
  public void Initialize(string mleName, List<MLEProgressBarMilestone> milestones) {
    this.mleName.text = mleName;
    this.milestones = milestones;
    CreateProgressBar(milestones);
    UpdateButtonStates();
  }

  public int CurrentStep { get; private set; } = 0;
  public int TotalSteps { get; private set; } = 0;
  public MLEProgressBarMilestone CurrentMilestone {
    get {
      return milestones[CurrentStep];
    }
  }

  private void CreateProgressBar(List<MLEProgressBarMilestone> milestones) {
    
    TotalSteps = milestones.Count - 1;
    //placing previously created Gameobjects along Progress Bar
    if(TotalSteps > 1) {
      for(int i = 0; i < milestones.Count; i++) {
        float xPos = (float)i * 1f / ((float)milestones.Count - 1f);
        milestones[i].GetComponent<RectTransform>().anchorMin = new Vector2(xPos, 0.5f);
        milestones[i].GetComponent<RectTransform>().anchorMax = new Vector2(xPos, 0.5f);

        milestones[i].UpdateGraphics();
      }
    }
  }

  private void UpdateButtonStates() {
    previousStep.interactable = true;
    nextStep.interactable = true;

    if(CurrentStep == 0) {

      previousStep.interactable = false;
    }
  }

  public void SetProgressBarActive(bool activeState)
  {
    this.gameObject.SetActive(activeState);
  }

  public void UpdateProgressBar(int progressBarPosition) {

    float xPos = (float)progressBarPosition / ((float)TotalSteps);
    positionMarker.anchorMin = new Vector2(xPos, 0.5f);
    positionMarker.anchorMax = new Vector2(xPos, 0.5f);
  }

  public void NextStep() {

    if(CurrentStep < TotalSteps) {

      CurrentStep++;
      UpdateProgressBar(CurrentStep);
    }

    UpdateButtonStates();
  }

  public void SkipCurrentStep() {

    if(CurrentMilestone.state == MilestoneState.upcoming) {

      CurrentMilestone.state = MilestoneState.skipped;
      CurrentMilestone.UpdateGraphics();
    }

    NextStep();
  }

  public void MarkSuccess() {

    CurrentMilestone.state = MilestoneState.success;
    CurrentMilestone.UpdateGraphics();
  }

  public void MarkFailure() {

    CurrentMilestone.state = MilestoneState.failure;
    CurrentMilestone.UpdateGraphics();
  }

  public void SetScore(string score) {

    currentScore.text = score;
  }
}
