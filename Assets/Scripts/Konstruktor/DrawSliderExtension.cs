#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawSliderExtension : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    //public delegate void SliderInteractionEvent();
    //public static event SliderInteractionEvent OnPointerUpEvent;
    public UnityEvent onPointerUpEvent = new UnityEvent();

/*    private void OnEnable()
    {
    	if(onPointerUpEvent == null)
    	{
    	 onPointerUpEvent = new UnityEvent()
    	}
    }*/

	public void OnPointerDown(PointerEventData eventData)
	{
		//OnPointerDown is also required to receive OnPointerUp callbacks
	} 

	public void OnPointerUp(PointerEventData eventData)
	{
		onPointerUpEvent.Invoke();
	}

}
