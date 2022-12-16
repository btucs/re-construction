#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PermanentItem : ScriptableObject, UIDSearchableInterface
{	
	[SerializeField, ReadOnly]
	private string uid;
	
	public string itemName = "Item";
	public int price = 100;
	[PreviewField(100, ObjectFieldAlignment.Right)]
	[HideLabel]
	public Sprite image;

	public bool canBeBought = false;
	public ProgressRequirement requirement;


	public string UID {
    	get {
      		return uid;
    	}
  	}

	private void OnValidate() {
#if UNITY_EDITOR
    	if(uid == "" || uid == null) {
    		uid = Guid.NewGuid().ToString();
    		UnityEditor.EditorUtility.SetDirty(this);
    	}
#endif
	}

	public bool Equals(PermanentItem compareItem)
	{
		return (uid == compareItem.UID);
	}
}
