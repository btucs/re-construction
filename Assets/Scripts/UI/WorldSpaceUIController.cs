#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUIController : MonoBehaviour
{
	public Transform worldSpaceCanvas;
  	public GameObject objectMarkerPrefab;
  	public GameObject taskMarkerPrefab;
    public GameObject speechBubblePrefab;
    public Vector2 speechBubbleOffset = new Vector2(0.5f, 0.5f);

    private List<EnvironmentInteractionMarker> objectMarkers = new List<EnvironmentInteractionMarker>();
    private List<ObjectTaskEntry> taskMarkers = new List<ObjectTaskEntry>();
    private List<GameObject> speechBubbles = new List<GameObject>();

    public void SetSingleMarkerActive(ObjectTaskEntry _taskMarker)
    {
    	SetActiveStateOfAllMarkers(false);

    	_taskMarker.gameObject.SetActive(true);
    }

    public void SetSingleMarkerActive(EnvironmentInteractionMarker _envMarker)
    {
    	SetActiveStateOfAllMarkers(false);

    	_envMarker.gameObject.SetActive(true);
    }

    public void SetActiveStateOfAllMarkers(bool nowActive)
    {
    	foreach(EnvironmentInteractionMarker objectMarker in objectMarkers)
    	{
    		objectMarker.gameObject.SetActive(nowActive);
    	}
    	foreach(ObjectTaskEntry taskMarker in taskMarkers)
    	{
    		taskMarker.gameObject.SetActive(nowActive);
    	}
    }

    public bool GetActiveState()
    {
        return worldSpaceCanvas.gameObject.activeSelf;
    }

    public void SetWorldSpaceUIActive(bool toBeActive)
	{
		worldSpaceCanvas.gameObject.SetActive(toBeActive);
	}

    public void RefreshMarkers()
    {
    	foreach(EnvironmentInteractionMarker objectMarker in objectMarkers)
    	{
    		objectMarker.RefreshAppearance();
    	}
    	foreach(ObjectTaskEntry taskMarker in taskMarkers)
    	{
    		taskMarker.RefreshAppearance();
    	}
    }

    public NPCSpeechBubble CreateNPCSpeechBubble(NPCDialogController npcDialog)
    {
        GameObject bubbleObj = Instantiate(speechBubblePrefab, worldSpaceCanvas);
        NPCSpeechBubble bubbleScript = bubbleObj.GetComponent<NPCSpeechBubble>();

        bubbleScript.Setup(npcDialog, speechBubbleOffset);

        return bubbleScript;
    }

	public void CreateObjectInteractionMarker(GameWorldObject theObject)
	{
		GameObject markerObj = Instantiate(objectMarkerPrefab, worldSpaceCanvas);
		markerObj.GetComponent<EnvironmentInteractionMarker>().Setup(theObject);
	}

	public GameObject CreateTaskInteractionMarker(GameWorldObject theObject, ObjectTaskData theTask)
	{
		GameObject markerObj = Instantiate(taskMarkerPrefab, worldSpaceCanvas);
		RectTransform objBody = markerObj.GetComponent<RectTransform>();
		objBody.anchoredPosition = theObject.GetMarkerPosition();
		objBody.anchoredPosition += theTask.taskButtonPosition;
		return markerObj;
	}
}
