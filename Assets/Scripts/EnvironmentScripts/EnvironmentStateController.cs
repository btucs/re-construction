#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentStateController : MonoBehaviour
{
	public List<EnvironmentChange> positiveChanges = new List<EnvironmentChange>();
	public List<EnvironmentChange> negativeChanges = new List<EnvironmentChange>();
	public GameObject volumeController;
	private GameController gameController;

	private void Start()
	{
		gameController = GameController.GetInstance();
		UpdateEnvironment();
		UnityEngine.Rendering.VolumeProfile volumeProfile = volumeController.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
		if(!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

		// You can leave this variable out of your function, so you can reuse it throughout your class.
		UnityEngine.Rendering.Universal.ColorAdjustments colorAdjust;
		if(!volumeProfile.TryGet(out colorAdjust)) throw new System.NullReferenceException(nameof(colorAdjust));

		foreach(EnvironmentChange poschange in positiveChanges)
		{
			poschange.SetColorAdjustmentReference(colorAdjust);
		}
	}

	private void UpdateEnvironment()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		int state = gameController.gameState.gameworldData.mapData.GetAreaState(sceneName);

		//Der Zustand der Tiles auf der Weltkarte bestimmt den Status des jeweiligen Gebiets.
		//Ist der Zustand besonders gut oder schlecht werden Teile der festgelegten Änderungen vorgenommen 
		for(int i = 0; i < (int)(Mathf.Abs(state)/5); i++)
		{
			if(state > 0 && positiveChanges.Count > i)
			{
				positiveChanges.ElementAt(i).Activate();
			} else if(state < 0 && negativeChanges.Count > i)
			{
				negativeChanges.ElementAt(i).Activate();
			}
		}
	}

}

[System.Serializable]
public class EnvironmentChange
{
	public SpriteRenderer graphic;
	public Sprite alternativeImg;
	public bool toBeActive = true;
	public int PPsaturation = 0;
	public Color spriteTint = Color.white;
	private UnityEngine.Rendering.Universal.ColorAdjustments adjustmentScript;

	public void Activate()
	{
		if(graphic != null)
		{
			if(alternativeImg != null)
			{
				graphic.sprite = alternativeImg;
			}
			graphic.gameObject.SetActive(toBeActive);
			if(spriteTint != Color.white)
			{
				graphic.color = spriteTint;
			}
		}
		if(adjustmentScript != null)
		{
			
		}
	}

	public void SetColorAdjustmentReference(UnityEngine.Rendering.Universal.ColorAdjustments newRef)
	{
		adjustmentScript = newRef;
	}
}