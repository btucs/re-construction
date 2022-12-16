#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateLoader : MonoBehaviour
{
	public MenuUIController myUIController;
	public TaskListManager taskListScript;
	public RoomState gameWorldController;
	public SystemNotificationController notificationScript;

    public void OpenPreviousUI(MLEDataTransporter _mleReturnData)
    {
    	if(_mleReturnData.typeOfAccess == OriginType.taskPreview && _mleReturnData.openPrevUI == true)
    	{
    		Debug.Log("Opening UI of Last Object");
    		/*foreach (GameWorldObject interactableObj in gameWorldController.objectScripts)
    		{
    			if(interactableObj.objectData == _mleReturnData.originObject)
	    		{
	    			interactableObj.OpenObjectMenu();
	    			Camera myCam = Camera.main;
	    			myCam.transform.position = new Vector3(
	    				interactableObj.transform.position.x + interactableObj.objectData.cameraOffset.x,
	    				interactableObj.transform.position.y + interactableObj.objectData.cameraOffset.y,
	    				myCam.transform.position.z);
	    			foreach(ObjectTaskEntry taskScript in taskListScript.taskListEntries)
	    			{
	    				if(taskScript.taskData == _mleReturnData.originSceneTask)
	    				{
	    					taskScript.ActivateTaskSelection();
	    					taskListScript.OpenMLEPreview();
	    					//taskScript.transform.GetComponent<SingleMenuButton>().SetThisButtonActive();
	    				}
	    			}
	    			break;
    			}
    		}*/
    	}
    }
}
