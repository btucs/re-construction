#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawHandlerFactory
{
  public static AbstractDrawHandler CreateAndAddDrawHandler(TaskTypeEnum taskTypeEnum, KonstruktorDrawController drawController) {

    switch(taskTypeEnum) {

      case TaskTypeEnum.DrawVector: return CreateAndAddVectorHandler(drawController);
      case TaskTypeEnum.DrawForce: return CreateAndAddForceHandler(drawController);
      case TaskTypeEnum.DrawEquilibrium: return CreateAndAddEquilibriumHandler(drawController);
      case TaskTypeEnum.DrawLineVolatility: return CreateAndAddLineVolatilityHandler(drawController);
      case TaskTypeEnum.DrawInteraction: return CreateAndAddDrawInteractionHandler(drawController);
      default: throw new Exception("Task type not handled or not a draw type");
    }
  }

  public static AbstractDrawHandler CreateAndAddDrawHandler(KonstruktorModuleType moduleTypeEnum, KonstruktorDrawController drawController) {

    switch(moduleTypeEnum) {

      case KonstruktorModuleType.Vector: return CreateAndAddVectorHandler(drawController);
      case KonstruktorModuleType.ForceGraphical: return CreateAndAddForceHandler(drawController);
      case KonstruktorModuleType.Equilibrium:  return CreateAndAddEquilibriumHandler(drawController);
      case KonstruktorModuleType.LineVolatility: return CreateAndAddLineVolatilityHandler(drawController);
      case KonstruktorModuleType.Interaction: return CreateAndAddDrawInteractionHandler(drawController);
      default: throw new Exception("Module type not handled or not a draw type");
    }
  }

  public static DrawVectorHandler CreateAndAddVectorHandler(KonstruktorDrawController drawController) {

    DrawVectorHandler handler = drawController.gameObject.AddComponent<DrawVectorHandler>();
    AddGeneralData(ref handler, drawController);

    handler.pointTemplate = drawController.pointTemplate;

    return handler;
  }

  public static DrawForceHandler CreateAndAddForceHandler(KonstruktorDrawController drawController) {

    DrawForceHandler handler = drawController.gameObject.AddComponent<DrawForceHandler>();
    AddGeneralData(ref handler, drawController);

    TaskDataSO.CoordinateSystemData coordinateSystemData = drawController.coordinateSystemData;

    handler.scale = drawController.scale;
    handler.scaleAmount = drawController.scaleAmount;
    handler.angleTemplate = drawController.angleTemplate;
    handler.angleStep = drawController.angleStep;

    handler.amountStep = coordinateSystemData.intermediateSteps;
    handler.amountStepSize = coordinateSystemData.unitSize;

    return handler;
  }

  public static DrawEquilibriumHandler CreateAndAddEquilibriumHandler(KonstruktorDrawController drawController) {

    DrawEquilibriumHandler handler = drawController.gameObject.AddComponent<DrawEquilibriumHandler>();
    AddGeneralData(ref handler, drawController);

    TaskDataSO.CoordinateSystemData coordinateSystemData = drawController.coordinateSystemData;
    handler.scale = drawController.scale;
    handler.scaleAmount = drawController.scaleAmount;
    handler.angleTemplate = drawController.angleTemplate;
    handler.angleStep = drawController.angleStep;
    handler.vectorTemplate = drawController.vectorTemplate;

    handler.editAngleButton = drawController.editAngleButton;
    handler.editPointButton = drawController.editPointButton;

    handler.amountStep = coordinateSystemData.intermediateSteps;
    handler.amountStepSize = coordinateSystemData.unitSize;

    handler.spriteTransparencyController = drawController.spriteTransparencyController;

    return handler;
  }

  public static DrawLineVolatilityHandler CreateAndAddLineVolatilityHandler(KonstruktorDrawController drawController) {

    DrawLineVolatilityHandler handler = drawController.gameObject.AddComponent<DrawLineVolatilityHandler>();
    AddGeneralData(ref handler, drawController);

    TaskDataSO.CoordinateSystemData coordinateSystemData = drawController.coordinateSystemData;
    handler.scale = drawController.scale;
    handler.scaleAmount = drawController.scaleAmount;
    handler.vectorTemplate = drawController.vectorTemplate;

    handler.editPointButton = drawController.editPointButton;

    handler.amountStep = coordinateSystemData.intermediateSteps;
    handler.amountStepSize = coordinateSystemData.unitSize;

    handler.continueButton = drawController.continueButton;

    return handler;
  }

  public static DrawInteractionHandler CreateAndAddDrawInteractionHandler(KonstruktorDrawController drawController) {

    DrawInteractionHandler handler = drawController.gameObject.AddComponent<DrawInteractionHandler>();
    AddGeneralData(ref handler, drawController);


    TaskDataSO.CoordinateSystemData coordinateSystemData = drawController.coordinateSystemData;
    handler.scale = drawController.scale;
    handler.scaleAmount = drawController.scaleAmount;
    handler.angleTemplate = drawController.angleTemplate;
    handler.angleStep = drawController.angleStep;
    handler.vectorTemplate = drawController.vectorTemplate;

    handler.editAngleButton = drawController.editAngleButton;
    handler.editPointButton = drawController.editPointButton;

    handler.amountStep = coordinateSystemData.intermediateSteps;
    handler.amountStepSize = coordinateSystemData.unitSize;

    return handler;
  }

  public static void AddGeneralData<T>(ref T handler, KonstruktorDrawController drawController) where T : AbstractDrawHandler {

    handler.leftSidebarContent = drawController.leftSidebarContent;
    handler.rightSidebarContent = drawController.rightSidebarContent;
    handler.drawnItemsContainer = drawController.drawnItemsContainer;

    handler.xMarker = drawController.xMarker;
    handler.yMarker = drawController.yMarker;

    handler.inputOutputPlaceholderTemplate = drawController.inputOutputPlaceholderTemplate;

    handler.drawLine = drawController.drawLine;
    handler.drawButton = drawController.drawButton;
    handler.confirmPointPositionButton = drawController.confirmPointPositionButton;
    handler.coordinateSystemData = drawController.coordinateSystemData;

    handler.inputMenuController = drawController.inputMenuController;
    handler.currentTask = drawController.currentTask;
    handler.currentTaskObject = drawController.currentTaskObject;
  }
}
