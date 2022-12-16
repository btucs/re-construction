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

public class CustomAnimationController : MonoBehaviour
{
    public List<AnimationState> states;
    private List<AnimationState> activeStates = new List<AnimationState>();

    private void Start()
    {
    	foreach(AnimationState state in states)
    	{
   			if(state.isDefaultState)
    		{
    			state.SetEndStateInstant();
    		}
   		}
    }

    private void Update()
    {
      for(int i = activeStates.Count - 1; i >= 0; i--)
      {
        if(activeStates[i].GetActiveState())
        {
          activeStates[i].AddFrameTime(Time.deltaTime);
          UpdateAnimationElements(activeStates[i]);
        } else {
          activeStates.RemoveAt(i);
        }
      }
    }

    private bool UpdateAnimationElements(AnimationState stateToUpdate)
    {
    	bool stillAnimating = false;
      float timeSinceStart = stateToUpdate.GetTimeSinceStart();
    	float normalizedTime = timeSinceStart / stateToUpdate.animDuration;
    	foreach(ObjectActiveChange objToDisable in stateToUpdate.setActiveObjects)
    	{
   			if(objToDisable.changeObj.activeSelf != objToDisable.toBeActive)
    		{
    			stillAnimating = true;
    			if(normalizedTime > objToDisable.normalizedTimestamp) 
    				objToDisable.changeObj.SetActive(objToDisable.toBeActive);
    		}
   		}

      foreach(SpriteChange spriteChange in stateToUpdate.spriteChanges)
      {
        //Debug.Log(spriteChange.changeImg.sprite == spriteChange.newSprite);
        if(spriteChange.changeImg.sprite != spriteChange.newSprite)
        {
          stillAnimating = true;
          if(normalizedTime > spriteChange.normalizedTimestamp)
          {
            spriteChange.changeImg.sprite = spriteChange.newSprite;
          } 
        }
      }

    	foreach(ImageColorChange colorChange in stateToUpdate.colorChanges)
    	{
	    	if(colorChange.alphaOnly && colorChange.colorizedImage.color.a != colorChange.targetColor.a)
	    	{
	    		Color newColor = colorChange.colorizedImage.color;
	    		newColor.a = Mathf.Lerp(colorChange.GetStartColor().a, colorChange.targetColor.a, timeSinceStart / stateToUpdate.animDuration);
	    		colorChange.colorizedImage.color = newColor;
	    	}
	   		else if(!colorChange.alphaOnly && colorChange.colorizedImage.color != colorChange.targetColor)
	    	{
	    		colorChange.colorizedImage.color = Color.Lerp(colorChange.GetStartColor(), colorChange.targetColor, timeSinceStart / stateToUpdate.animDuration);
	    		stillAnimating = true;
	    	}
   		}

   		foreach(ScaleChange scaleChange in stateToUpdate.scaleChanges)
    	{
   			if(scaleChange.changeTransform.localScale != scaleChange.targetScale)
    		{
    			scaleChange.changeTransform.localScale = Vector3.Lerp(scaleChange.GetStartScale(), scaleChange.targetScale, timeSinceStart / stateToUpdate.animDuration);
    			stillAnimating = true;
    		}
   		}

   		foreach(CanvasGroupChange groupChange in stateToUpdate.groupChanges)
   		{
   			if(groupChange.canvasGroup.alpha != groupChange.targetAlpha)
   			{
	    		groupChange.canvasGroup.alpha = Mathf.Lerp(groupChange.GetStartAlpha(), groupChange.targetAlpha, timeSinceStart / stateToUpdate.animDuration);
	    		stillAnimating = true;   				
   			}
   		}
   		return stillAnimating;
    }

    public void AddActiveAnimationState(string stateName)
    {
    	foreach(AnimationState currentState in states)
    	{
    		if(currentState.name == stateName)
    		{
          //Debug.Log("Added active animationstate " + stateName);
    			activeStates.Add(currentState);
    			currentState.StartAnimation();

          foreach(string cancelName in currentState.animationOverrides)
          {
            CancelAnimationByName(cancelName);
          }
        }
    	}
    }

    private void CancelAnimationByName(string toCancel)
    {
      for(int i = activeStates.Count - 1; i >= 0; i--)
      {
        if(activeStates[i].name == toCancel)
        {
          activeStates.RemoveAt(i);
        }
      }
    }
}

[System.Serializable]
public class AnimationState
{
	public string name;
	public float animDuration = 1f;
	public bool isDefaultState = false;
 	public List<ObjectActiveChange> setActiveObjects = new List<ObjectActiveChange>();
 	public List<ImageColorChange> colorChanges = new List<ImageColorChange>();
 	public List<ScaleChange> scaleChanges = new List<ScaleChange>();
 	public List<CanvasGroupChange> groupChanges = new List<CanvasGroupChange>();
  public List<SpriteChange> spriteChanges = new List<SpriteChange>();
  public List<string> animationOverrides = new List<string>();
  private bool isAnimating = false;
  private float timeSinceStart = 0f;

  public bool GetActiveState()
  {
    if(!isAnimating || timeSinceStart > animDuration)
      return false;

    return true;
  }

  public void StartAnimation()
  {
    isAnimating = true;
    timeSinceStart = 0f;
    SetStartValues();
  }

  public void AddFrameTime(float timePassed)
  {
    timeSinceStart += timePassed;
  }

  public float GetTimeSinceStart()
  {
    return timeSinceStart;
  }

 	private void SetStartValues()
  {
 		foreach(ImageColorChange colorChange in colorChanges)
    	{
    		colorChange.SetStartColor(colorChange.colorizedImage.color);
   		}

   		foreach(ScaleChange scaleChange in scaleChanges)
    	{
   			scaleChange.SetStartScale(scaleChange.changeTransform.localScale);
   		}

   		foreach(CanvasGroupChange groupChange in groupChanges)
   		{
   			groupChange.SetStartAlpha(groupChange.canvasGroup.alpha);
   		}
 	}

 	public void SetEndStateInstant()
 	{
 		foreach(ObjectActiveChange objToChange in setActiveObjects)
    	{
   			objToChange.changeObj.SetActive(objToChange.toBeActive);
   		}

    	foreach(ImageColorChange colorChange in colorChanges)
    	{
	    	if(colorChange.alphaOnly)
	    	{
	    		Color newColor = colorChange.colorizedImage.color;
	    		newColor.a = colorChange.targetColor.a;
	    		colorChange.colorizedImage.color = newColor;
	    	} else {
	    		colorChange.colorizedImage.color = colorChange.targetColor;
	    	}

   		}

   		foreach(ScaleChange scaleChange in scaleChanges)
    	{
   			scaleChange.changeTransform.localScale = scaleChange.targetScale;
   		}

   		foreach(CanvasGroupChange groupChange in groupChanges)
   		{
   			groupChange.canvasGroup.alpha = groupChange.targetAlpha;
   		}

      foreach(SpriteChange spriteChange in spriteChanges)
      {
        spriteChange.changeImg.sprite = spriteChange.newSprite;
      }
 	}
}

[System.Serializable]
public class ObjectActiveChange
{
	public GameObject changeObj;
	public bool toBeActive = true;
	[Range(0f, 1f)]
	public float normalizedTimestamp = 0f; 

}

[System.Serializable]
public class ImageColorChange
{
	public Image colorizedImage;
	public bool alphaOnly = false;
	public Color targetColor = Color.white;

	private Color startColor;
	public void SetStartColor(Color col)
	{
		startColor = col;
	}

	public Color GetStartColor()
	{
		return startColor;
	}

}

[System.Serializable]
public class ScaleChange
{
	public Transform changeTransform;
	public Vector3 targetScale = new Vector3(1f,1f,1f);

	private Vector3 startScale;
	public void SetStartScale(Vector3 scale)
	{
		startScale = scale;
	}

	public Vector3 GetStartScale()
	{
		return startScale;
	}
}

[System.Serializable]
public class CanvasGroupChange
{
	public CanvasGroup canvasGroup;
	[Range(0, 1)]
	public float targetAlpha = 1f;
	private float startAlpha;

	public void SetStartAlpha(float alphaVal)
	{
		startAlpha = alphaVal;
	}

	public float GetStartAlpha()
	{
		return startAlpha;
	}
}

[System.Serializable]
public class SpriteChange
{
  public Image changeImg;
  public Sprite newSprite;
  [Range(0f, 1f)]
  public float normalizedTimestamp = 0f; 
}