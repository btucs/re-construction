#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "KonstruktorLayer")]
public class KonstruktorLayerCommand: ScriptableObject {

  public string[] layerNames = new string[0];
  protected GameObject[] layers;

  public void SetLayers(GameObject[] layers) {

    this.layers = FilterByName(layers, layerNames);
  }

  public void Execute() {

    toggleActive(true);
  }

  public void Undo() {

    toggleActive(false);
  }

  private void toggleActive(bool isActive) {

    for (int i = 0; i < layers.Length; i++) {

      layers[i].SetActive(isActive);
    }
  }

  private GameObject[] FilterByName(GameObject[] layers, string[] layerNames) {

    GameObject[] filtered = layers.Aggregate(
      new List<GameObject>(),
      (List<GameObject> aggregate, GameObject current) => {

        if (layerNames.Contains(current.name)) {

          aggregate.Add(current);
        }

        return aggregate;
      }
     ).ToArray();

    return filtered;
  }
}
