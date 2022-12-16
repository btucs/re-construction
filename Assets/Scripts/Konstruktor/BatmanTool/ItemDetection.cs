#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.Events;

public class ItemDetection : MonoBehaviour
{
  public Transform analyzeArea;
  public AnalyzeTextureController texController;
  public KonstructorSetup konstruktorManager;
  public GameObject analyzeButtonPanel;

  public UnityEvent onAnalyzerStart = new UnityEvent();
  public UnityEvent onAnalyzerEnd = new UnityEvent();

  private Camera mainCam;
  private bool active = false;

  void Start() {
    ShowOrHideAnalyzeButton();
    Disable();
  }

  public void ShowOrHideAnalyzeButton() {

    bool shouldShow = GameController.GetInstance().gameState.onboardingData.konstruktorData.analyzeToolUnlocked;
    analyzeButtonPanel.SetActive(shouldShow);
  }

  public void ToggleActiveState() {

    if(active) {

      Disable();
    } else {

      CenterAndEnable();
    }
  }

  public void CenterAndEnable() {
    konstruktorManager.SetSpeechBubblesActive(false);
    Vector3 cameraPos = Camera.main.transform.position;
    analyzeArea.position = new Vector3(cameraPos.x, cameraPos.y, analyzeArea.position.z);
    analyzeArea.gameObject.SetActive(true);
    texController.ToggleDisplaceAnalyzeArea(false);
    active = true;

    onAnalyzerStart?.Invoke();
  }

  public void Disable() {
    konstruktorManager.SetSpeechBubblesActive(true);
    texController.ToggleDisplaceAnalyzeArea(true);
    analyzeArea.gameObject.SetActive(false);
    active = false;

    onAnalyzerEnd?.Invoke();
  }
}
