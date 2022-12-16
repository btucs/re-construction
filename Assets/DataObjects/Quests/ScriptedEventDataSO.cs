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
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName="GameProgress/Event")]
public class ScriptedEventDataSO : ScriptableObject, UIDSearchableInterface 
{
	[SerializeField, ReadOnly, LabelWidth(150)]
	private string uid;
    public string eventSceneName = "enter scene name";
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

}