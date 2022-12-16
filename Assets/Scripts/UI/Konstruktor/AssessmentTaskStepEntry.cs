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

public class AssessmentTaskStepEntry : MonoBehaviour {
	bool isExpanded = false;

  [Required]
  public Image dropDownButtonIcon;
  public TaskStateSpriteSwapper stepStateImgController;
  [Required]
  public RectTransform dropDownArea;
	public float animationTime = 0.1f;
  public float yOffset = 5;

  [Required]
  public Text StepNameText;
  [Required]
  public Text PointsText;
  [Required]
  public Text DetailsText;

  [Required]
  public string stepName;
  [Required]
  public int currentPoints;
  [Required]
  public int maxPoints;
  
  private string details;

  private RectTransform ownTransform;
	private Vector2 startSize;
	private Vector2 currentSize;
	private Vector2 targetSize;
	private float timeSinceAnimStart = 0f;

  private int currentIndex = 0;

  private void Start() {
    ownTransform = GetComponent<RectTransform>();
    startSize = ownTransform.sizeDelta;
    targetSize = ownTransform.sizeDelta;
    currentSize = ownTransform.sizeDelta;
    //ownTransform.sizeDelta = startSize;

    currentIndex = transform.GetSiblingIndex();
    UpdateTextFields();
  }

    // Update is called once per frame
  private void Update() {
    if(targetSize != ownTransform.sizeDelta)
    {
      timeSinceAnimStart += Time.deltaTime;
        	
      ownTransform.sizeDelta = Vector2.Lerp(currentSize, targetSize, (timeSinceAnimStart/animationTime));
    }
  }

  public void SetFeedback(string feedback) {

    details = feedback;
  }

  public void UpdateTextFields() {

    StepNameText.text = stepName;
    PointsText.text = currentPoints + " / " + maxPoints + " EP";
    DetailsText.text = details;
  }

  public void UpdateStateIcon()
  {
    stepStateImgController.UpdateIcon(maxPoints, currentPoints);
  }

  public void ToggleDropDownMenu()
  {
    if(!isExpanded)
    {
    	targetSize = new Vector2 (startSize.x, startSize.y + dropDownArea.sizeDelta.y + yOffset);
    	isExpanded = true;
    	currentSize = ownTransform.sizeDelta;
    	dropDownButtonIcon.GetComponent<RectTransform>().Rotate(new Vector3(0,0,180));
    	timeSinceAnimStart = 0f;
    } 
    else
    {
    	targetSize = new Vector2 (startSize.x, startSize.y);
    	isExpanded = false;
    	currentSize = ownTransform.sizeDelta;
    	dropDownButtonIcon.GetComponent<RectTransform>().Rotate(new Vector3(0,0,180));
    	timeSinceAnimStart = 0f;
    }
  }
}
