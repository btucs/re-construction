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
using UnityEngine.EventSystems;

public class NPCImageButton : MonoBehaviour {

  [Required]
  public TaskNPC taskNPC;
  [Required]
  public Image npcImage;

  public Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();

  private void Start() {

    Button button = GetComponent<Button>();
    button.onClick.AddListener(onClick.Invoke);
    npcImage.sprite = taskNPC.npc.thumbnail;
  }
}
