#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct KonstructorModuleAsset {

  public string name;
  public KonstructorModuleSO module;
}

public class ModuleSORepository : MonoBehaviour {

  public List<KonstructorModuleAsset> moduleSORepository = new List<KonstructorModuleAsset>();

  public KonstructorModuleSO GetAsset(string name) {

    return moduleSORepository.Find((KonstructorModuleAsset asset) => asset.name == name).module;
  }

  // return all modules accept placeholder
  public List<KonstructorModuleAsset> GetSelectableModules() {

    return moduleSORepository.Where((KonstructorModuleAsset asset) => asset.name != "Placeholder").ToList();
  }
}
