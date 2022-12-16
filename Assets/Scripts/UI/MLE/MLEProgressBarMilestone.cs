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

public enum MilestoneType{
	video, interactive
};

public enum MilestoneState 
{
	upcoming, success, failure, skipped, current
};

public class MLEProgressBarMilestone : MonoBehaviour
{
  public MilestoneType type;
  public Sprite videoIcon;
  public Sprite interactiveIcon;
  public Sprite correctAnswerIcon;
  public Sprite wrongAnswerIcon;
  public Text titleText;
  private Image backgroundImg;
  public Image IconImage;
  [HideInInspector]
  public MilestoneState state = MilestoneState.upcoming;
  [HideInInspector]
  public string titleString = "Video";

  public Color32 upComingColor;
  public Color32 skippedColor;
  public Color32 successColor;
  public Color32 failureColor;

  public void UpdateGraphics() {
    UpdateBackgroundColor();
    UpdateIconImage();
  }

  private void UpdateBackgroundColor() {
    if(backgroundImg == null) {

      backgroundImg = GetComponent<Image>();
    }

    switch(state) {

      case MilestoneState.upcoming:
        backgroundImg.color = upComingColor;
        break;

      case MilestoneState.skipped:
        backgroundImg.color = skippedColor;
        break;

      case MilestoneState.success:
        backgroundImg.color = successColor;
        break;

      case MilestoneState.failure:
        backgroundImg.color = failureColor;
        break;
    }
  }

  private void UpdateIconImage() {

    if(type == MilestoneType.video) {
      IconImage.sprite = videoIcon;
      titleText.text = titleString;
      return;
    }

    if(type == MilestoneType.interactive) {
      titleText.text = "Quiz";
      switch(state) {

        case MilestoneState.upcoming:
        case MilestoneState.skipped:
          IconImage.sprite = interactiveIcon;
          break;

        case MilestoneState.success:
          IconImage.sprite = correctAnswerIcon;
          break;

        case MilestoneState.failure:
          IconImage.sprite = wrongAnswerIcon;
          break;
      }
    }
  }
}