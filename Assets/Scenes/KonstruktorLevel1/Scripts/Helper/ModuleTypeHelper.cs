#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleSolutionEnum {

  Calculate,
  Draw,
  Place,
}

public static class ModuleTypeHelper {

  public static Dictionary<KonstruktorModuleType, ModuleSolutionEnum> mappings = new Dictionary<KonstruktorModuleType, ModuleSolutionEnum>() {
    { KonstruktorModuleType.ForceGraphical, ModuleSolutionEnum.Draw },
    { KonstruktorModuleType.Vector, ModuleSolutionEnum.Draw },
    { KonstruktorModuleType.Equilibrium, ModuleSolutionEnum.Draw },
    { KonstruktorModuleType.LineVolatility, ModuleSolutionEnum.Draw },
    { KonstruktorModuleType.Interaction, ModuleSolutionEnum.Draw },

    { KonstruktorModuleType.ReplacementModel, ModuleSolutionEnum.Place },
  };

  public static ModuleSolutionEnum GetSolutionType(KonstructorModuleSO module) {

    if(module.formula.Length > 0) {

      return ModuleSolutionEnum.Calculate;
    }

    mappings.TryGetValue(module.moduleType, out ModuleSolutionEnum solutionType);

    return solutionType;
  }
}
