#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutputVariableAnalysisController : MonoBehaviour
{
  [Required]
  public GameObject MLEPanel;
  [Required]
  public MLESceneLoader MLESceneLoaderScript;

  [Required]
  public TextMeshProUGUI DescriptionText;

  [Required]
  public TextMeshProUGUI UnitText;

  [Required]
  public TEXDraw FormulaTEXDraw;

  [Required]
  public GameObject FormularInfoPanel;

  [Required]
  public GameObject VariableHolderPanel;

  public KonstruktorNavTitle breadcrum;

  [Required]
  public TMP_LinkHandler linkHandler;

  [Required]
  public GameObject analysisPanel;

  public GlossaryController glossary;

  private InventoryItem currentItemCopy;
  private VariableInfoSO variableInfo;

  public void SetDisplayContent(InventoryItem item) {

    if(variableInfo == null) {

      GameController controller = GameController.GetInstance();
      variableInfo = controller.gameAssets.variableInfo;
    }

    if(currentItemCopy != null) {

      Destroy(currentItemCopy.gameObject);
      currentItemCopy = null;
    }

    TaskOutputVariable outputVariable = (TaskOutputVariable)item.magnitude.Value;
    VariableInfoSO.VariableInfoEntry info = variableInfo.GetInfoFor(outputVariable.unit);

    MLEDataSO mleData = info.mle;
    if(mleData != null) {
      MLEPanel.SetActive(true);
      MLESceneLoaderScript.currentMLEData = mleData;
      MLESceneLoaderScript.taskRef = item.magnitude.taskData;
      MLESceneLoaderScript.objectRef = item.magnitude.taskObject;
    } else {

      MLEPanel.SetActive(false);
    }

    DescriptionText.text = info.tmpDescription;

    UnitText.text = "Lösungsweg:";
    //UnitText.text = outputVariable.textMeshProUnit;
    //Debug.Log("OutputVariableAnalysisController: " + outputVariable.unit);

    //FormulaTEXDraw.text = "Formula";
    FormulaTEXDraw.text = info.formula;

    currentItemCopy = Instantiate(item, VariableHolderPanel.transform);
    currentItemCopy.enableDrag = false;
    currentItemCopy.OnTapEvent.AddListener(HandleTap);

    RectTransform currentItemTransform = (RectTransform)currentItemCopy.transform;
    currentItemTransform.sizeDelta = new Vector2(80, 80);

    if(breadcrum != null) {
      breadcrum.SetPrevLayerText();
    }
  }

  private void Start() {
    linkHandler.RegisterPrefix("glossary", HandleGlossaryLink);
  }

  private void HandleGlossaryLink(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      string entryName = linkId.Split(':')[1].Replace('_', ' ');
      glossary.transform.parent.gameObject.SetActive(true);
      glossary.ShowSingleEntry(entryName);
    }
  }

  private void HandleTap(InventoryItem item) {

    gameObject.SetActive(false);
    analysisPanel.SetActive(false);
  }
}