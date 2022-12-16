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
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
	public CanvasGroup fadeGrp;
	public float fadeSpeed = 2f;

    private bool visible = false;
    private bool animating = false;
    private string sceneToLoad;

	void Update()
	{
		if(animating)
		{
			if(visible)
			{
				if(fadeGrp.alpha > 0f)
				{
					fadeGrp.alpha = Mathf.Clamp(fadeGrp.alpha - fadeSpeed * Time.deltaTime, 0f, 1f);
				}
				else
				{
					animating = false;
					visible = false;
					GameObject.Destroy(this.gameObject);
				}
			} 
			else 
			{
				if(fadeGrp.alpha < 1f)
				{
					fadeGrp.alpha = Mathf.Clamp(fadeGrp.alpha + fadeSpeed * Time.deltaTime, 0f, 1f);
				}
				else
				{
					visible = true;
					LoadScene(sceneToLoad);
				}
			}
		}
	}

	public void LoadSceneWithAnimation(string sceneName)
	{
		sceneToLoad = sceneName;
		animating = true;
		DontDestroyOnLoad(this.gameObject);
	}

	private void LoadScene(string name)
	{
	    if(name == null || name == "") {
	      SceneManager.LoadScene("_Start");
	    } else {
	      SceneManager.LoadScene(name);
	    }
  	}
}
