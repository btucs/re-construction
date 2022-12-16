#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KOnboardingCustomController : MonoBehaviour
{
  private bool searchedVarFound = false;
  public bool firstPartActive = false;

  private bool secondPartActive = false;
  private bool firstPartFinished = false;
  private bool lastPartActive = false;
  private bool NPCHasArrived = false;
  private bool firstFrameUpdate = false;

  public GameObject GivenBar;
  public GameObject SolutionButton;

  public GameObject dialogWindow;
  public GameObject continueDialogButton;
  public Transform givenTransform;
  public OutputMenuController outputMenuController;
  public InputMenuController inputMenuController;
  public Button gotoConverterButton;
  public TaskDialogHandler dialogController;
  public DragOutOnboardingController dragAnimationController;

  private OutputMenuItemController outputMenuItemController;
  private GameObject NPCObject;
  private characterMovement NPCMover;
  private Vector3 originPos;
  private SpeechBubblueOnboardingScript bubbleRef;
  private SpeechBubblueOnboardingScript leftBubbleRef;


  private void Start() {

    //outputMenuItemController = outputMenuController.GetOutputItemController(0);
    //outputMenuItemController.gameObject.SetActive(false);
  }

  void Update() {
    if(!firstFrameUpdate) {
      firstFrameUpdate = true;
      NPCObject = GameObject.Find("NPC - Marlen");
      Vector3 goalPos = new Vector3(Camera.main.transform.position.x + 11f, 0, 0);
      NPCMover = NPCObject.GetComponent<characterMovement>();
      NPCMover.SetGoalPosition(goalPos);
      originPos = NPCObject.transform.position;
      NPCObject.transform.position = goalPos;

      GivenBar.SetActive(false);
      SolutionButton.SetActive(false);
    }

    if(firstPartActive && !searchedVarFound) {
      Debug.Log("Checking for Given Var");
      if(inputMenuController.GetNumberOfInputItems() > 0) {
        if(inputMenuController.GetInputItemControllerAt(0).droppedItem != null) {
          inputMenuController.GetInputItemControllerAt(0).SetShowHighlight(false);
          searchedVarFound = true;
          firstPartActive = false;
          firstPartFinished = true;
          //outputMenuItemController.ShowHighlight.Value = false;
          continueDialogButton.SetActive(true);
          //ActivateSecondTutorialPart();
          dialogController.closeButton.onClick.AddListener(ActivateSecondTutorialPart);
        }

      }

    }

    if(secondPartActive) {
      if(!NPCHasArrived) {
        //Debug.Log("goalpos is " + NPCMover.goalPosition + " and current pos is " + NPCObject.transform.position);
        if(NPCMover.goalPosition == NPCObject.transform.position) {
          NPCHasArrived = true;
          bubbleRef.gameObject.SetActive(true);
          lastPartActive = true;
        }
      }
    }

    if(lastPartActive) {
      InputMenuItemController[] inputMenuItems = inputMenuController.GetInputItemControllers();
      bool allSlotsFilled = inputMenuItems.All((InputMenuItemController inputMenuItem) => inputMenuItem.droppedItem != null);

      if(allSlotsFilled == true) {
        lastPartActive = false;
        SolutionButton.SetActive(true);
        bubbleRef.gameObject.SetActive(false);
      }
    }
  }


  public void EnableSearchedOrGivenBar() {
    if(firstPartFinished == false) {
      ActivateFirstTutorialPart();
    } else {
      GivenBar.SetActive(true);
      secondPartActive = false;
      lastPartActive = true;
    }
  }

  public void ActivateFirstTutorialPart() {
    if(!searchedVarFound) {
      Debug.Log("First tutorial part began.");
      //outputMenuItemController.gameObject.SetActive(true);
      //outputMenuItemController.ShowHighlight.Value = true;
      continueDialogButton.SetActive(false);
      GivenBar.SetActive(true);
      //SolutionButton.SetActive(false);
      firstPartActive = true;
      //UpdateInputHighlights();
      dragAnimationController.StartAnimationProcess();
    }
  }

  public void UpdateInputHighlights() {
    foreach(Transform child in inputMenuController.transform) {
      InputMenuItemController menuItemRef = child.GetComponent<InputMenuItemController>();
      if(child.gameObject.activeSelf == true && menuItemRef.droppedItem == null) {
        menuItemRef.SetShowHighlight(true);
      } else {
        menuItemRef.SetShowHighlight(false);
      }
    }
  }

  public void ActivateSecondTutorialPart() {
    if(firstPartFinished && !secondPartActive && !NPCHasArrived) {
      Debug.Log("Second tutorial part began.");
      secondPartActive = true;
      dialogWindow.SetActive(false);
      originPos.x = originPos.x - 0.4f;
      leftBubbleRef.gameObject.SetActive(false);
      NPCObject.GetComponent<characterMovement>().SetGoalPosition(originPos);
      //NPCHasArrived = false;
    }
  }

  public void FinalizeOnboarding() {

    GameController controller = GameController.GetInstance();
    KonstruktorSceneData data = controller.gameState.konstruktorSceneData;
    FinishedTaskData finishedTaskData = new FinishedTaskData(data.taskData, data.taskObject, 1);

    controller.gameState.taskHistoryData.taskHistoryData.Add(finishedTaskData);
    controller.SaveGame();
  }

  public void DisableGotoConverter(InventoryItem droppedItem) {

    droppedItem.OnTapEvent.AddListener((InventoryItem item) => gotoConverterButton.interactable = false);
  }

  public void SetSpeechBubbleRef(SpeechBubblueOnboardingScript _ref) {
    bubbleRef = _ref;
  }

  public void SetLeftSpeechBubbleRef(SpeechBubblueOnboardingScript _ref) {
    leftBubbleRef = _ref;
  }
}
