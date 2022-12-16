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
using FullSerializer;

[System.Serializable]
[fsObject(Converter = typeof(AreaDataSOConverter))]
[CreateAssetMenu(menuName="WorldMap/AreaData")]
public class AreaDataSO : ScriptableObject, UIDSearchableInterface 
{
	[SerializeField, ReadOnly]
	private string uid;

	public string areaName;
	public string sceneName;
	[MultiLineProperty(4)]
	public string areaDescription;
	public Sprite areaPreviewIMG;
	public Sprite areaIconIMG;
	public string introSceneName;
  /// <summary>
  /// MapPosition when used in AreaData, offset when used in SubAreaData
  /// </summary>
  public Vector2Int mapPositionOrOffset;

	public string UID {
		get {
			return uid;
		}
	}  

	private void OnValidate()
	{
	#if UNITY_EDITOR
		if(uid == "" || uid == null)
		{
			uid = Guid.NewGuid().ToString();
			UnityEditor.EditorUtility.SetDirty(this);
		}
	#endif
	}
}