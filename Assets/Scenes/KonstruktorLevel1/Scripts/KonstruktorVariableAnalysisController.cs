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

public class KonstruktorVariableAnalysisController : MonoBehaviour {

  [Required]
  public OutputVariableAnalysisController outputAnalysisController;

  [Required]
  public InputVariableAnalysisController inputAnalysisController;

  public void SetDisplayContent(InventoryItem item) {

    TaskVariable taskVariable = item.magnitude.Value;

    if(taskVariable is TaskInputVariable) {

      inputAnalysisController.gameObject.SetActive(true);
      inputAnalysisController.SetDisplayContent(item);

      return;
    }

    if(taskVariable is TaskOutputVariable) {

      outputAnalysisController.gameObject.SetActive(true);
      outputAnalysisController.SetDisplayContent(item);
    }
  }
}
