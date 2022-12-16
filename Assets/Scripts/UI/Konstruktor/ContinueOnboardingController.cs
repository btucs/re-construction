#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueOnboardingController : MonoBehaviour
{
  public DropArea checkOutputArea;
  private KonstruktorOnboardingController tutorialController;
  private bool hasAssignedSearchedVar = false;

  public GameObject continueOnboardingButton;

  public GameObject selectionUI;
  private bool selectionUIwasActive = false;
  private bool hasSelectedModule = false;

  public Transform InventoryTransform;
  public Transform SearchedAreaTransform;
  private bool hasFoundGivenVar = false;
  private bool hasFoundSearchedVar = false;
  private bool hasContinuedAssignmentTutorial = false;

  //private bool hasContinuedEventByCommand = false;

  private bool continueEventOnNextObstacle = false;

  void Start()
  {
    if(tutorialController == null)
    {
    	tutorialController = GetComponent<KonstruktorOnboardingController>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if(tutorialController.activeTutorial != null) {
      // ------------ first onboarding part
      if(tutorialController.activeTutorial == tutorialController.givenSearchedTutorial) {

        //has the player identified all given and searched items?
        if(InventoryTransform != null) {
          if(hasFoundGivenVar == false && InventoryTransform.childCount > 1) {
            hasFoundGivenVar = true;
          }
        }
        if(SearchedAreaTransform != null) {
          if(hasFoundSearchedVar == false && SearchedAreaTransform.childCount > 0) {
            if(SearchedAreaTransform.GetChild(0).GetComponent<InventoryItem>() != null) {
              hasFoundSearchedVar = true;
              Debug.Log("SearchedVar is there.");

            }
          }
        }

        if(!hasContinuedAssignmentTutorial &&
          tutorialController.givenSearchedTutorial.currentTutorialPart > 4 &&
          hasFoundSearchedVar && hasFoundGivenVar) {
          continueEventOnNextObstacle = true;
          hasContinuedAssignmentTutorial = true;
        }
      }

      // ------------ second Onboarding Part
      // has the player dragged the searched var into the output module?
      if(checkOutputArea != null) {
        if(hasAssignedSearchedVar == false && checkOutputArea.droppedItem != null) {
          hasAssignedSearchedVar = true;
          continueEventOnNextObstacle = true;
        }
      }

      if(selectionUI != null) {
        if(selectionUIwasActive == false && selectionUI.activeSelf) {
          selectionUIwasActive = true;
          //Debug.Log("SelectionUI was active");
        }
      }

      if(!continueEventOnNextObstacle && hasSelectedModule == false && selectionUIwasActive && !selectionUI.activeSelf) {
        //Debug.Log("Has given Continue Command");
        hasSelectedModule = true;
        continueEventOnNextObstacle = true;
      }

      if(continueEventOnNextObstacle) {
        int tutorialIndex = tutorialController.activeTutorial.currentTutorialPart;
        if(tutorialIndex != -1 && tutorialController.activeTutorial.tutorialSteps[tutorialIndex].continueOnButton == false) {
          tutorialController.ContinueActiveEvent();
          continueEventOnNextObstacle = false;
        }
      }
    }
  }

  public void Reset()
  {
    hasAssignedSearchedVar = false;
    selectionUIwasActive = false;
    hasSelectedModule = false;
    selectionUIwasActive = false;
    //hasContinuedEventByCommand = false;
		continueEventOnNextObstacle = false;
  }
}
