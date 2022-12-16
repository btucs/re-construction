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

public static class ClassificationHandlerFactory {

  public static AbstractClassificationHandler CreateAndAddClassificationHandler(KonstruktorModuleType moduleTypeEnum, KonstruktorClassificationController controller) {

    switch(moduleTypeEnum) {
      case KonstruktorModuleType.ReplacementModel: return CreateAndAddReplacementModelHandler(controller);
      case KonstruktorModuleType.FreeCut: 
      default: throw new Exception("Module type not handled or not a classification type");
    }
  }

  public static DrawReplacementModelHandler CreateAndAddReplacementModelHandler(KonstruktorClassificationController controller) {

    DrawReplacementModelHandler handler = controller.gameObject.AddComponent<DrawReplacementModelHandler>();
    AddGeneralData(ref handler, controller);

    handler.replacementModelController = controller.replacementModelController;
    handler.dropAreaController = controller.replacementModelDropAreaTemplate;

    return handler;
  }

  /*public static DrawFreeCutHandler CreateAndAddFreeCutHandler(Konstruktorcontroller controller) {

    DrawFreeCutHandler handler = controller.gameObject.AddComponent<DrawFreeCutHandler>();
    AddGeneralData(ref handler, controller);

    return handler;
  }*/

  public static void AddGeneralData<T>(ref T handler, KonstruktorClassificationController controller) where T : AbstractClassificationHandler {

    handler.leftSidebarContent = controller.leftSidebarContent;
    handler.rightSidebarContent = controller.rightSidebarContent;
    handler.placedItemsContainer = controller.placedItemsContainer;
    handler.inputOutputPlaceholderTemplate = controller.inputOutputPlaceholderTemplate;
    handler.continueButton = controller.continueButton;
    handler.inputMenuController = controller.inputMenuController;
  }
}
