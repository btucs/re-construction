#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

public class SolutionModuleEntryController : MonoBehaviour
{
  public Text moduleName;
  public Image moduleIcon;

  public Sprite drawModuleSprite;

  private KonstructorModuleSO connectedModule;
  private ConstructorSolutionController solutionController;


  public void SetData(KonstructorModuleSO module, ConstructorSolutionController controller) {
    connectedModule = module;
    solutionController = controller;
  }

  public void UpdateButtonContent() {
    if(connectedModule.formula.Length == 0) {
      moduleIcon.sprite = drawModuleSprite;
    }
    moduleName.text = connectedModule.title;
  }

  public void OpenPreview() {
    solutionController.OpenModulePreview(connectedModule);
  }
}
