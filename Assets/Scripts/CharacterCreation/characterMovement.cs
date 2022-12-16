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

[RequireComponent (typeof (Animator))]
public class characterMovement : MonoBehaviour
{
	public Vector3 goalPosition = Vector3.zero;

	public float moveSpeed = 1f;

    public UnityEvent onReachGoalPos;
    private FMODUnity.StudioEventEmitter soundEmitter;
    private FMOD.Studio.EventInstance emitterEventInstance;
   // private FMOD.Studio.ParameterInstance floorTypeParam;


	private Animator characterAnimator;
	public bool facingRight = true;
    private ScriptedEventManager currentEventScript;
    private bool isMoving = false;

    private void Start()
    {
        characterAnimator = GetComponent<Animator>();
        if(goalPosition == Vector3.zero)
            goalPosition = transform.position;
    
        soundEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

        int footstepIndex = SoundEnvironment.Instance.GetEnvironmentSoundIndex();
        
        if(soundEmitter != null)
        {   
        	emitterEventInstance = soundEmitter.EventInstance;
            soundEmitter.SetParameter("FloorType", (float)footstepIndex);
            //emitterEventInstance.getParameter("Combat", out floorTypeParam);
            //floorTypeParam.setValue(footstepIndex);
        	emitterEventInstance.setParameterByName("FloorType", (float)footstepIndex);
            
            float newParamValue;
            emitterEventInstance.getParameterByName("FloorType", out newParamValue);
        }

        facingRight = (transform.localScale.x > 0);
    }

    private void Update()
    {
    	float xDifference = 0;
        if(this.transform.position != goalPosition)
        {
        	xDifference = goalPosition.x - transform.position.x; 
			    transform.position = Vector3.MoveTowards(transform.position, goalPosition, moveSpeed * Time.deltaTime);
        }
        else if(isMoving)
        {
            isMoving = false;
            onReachGoalPos.Invoke();
        }
        UpdateAnimator(xDifference);
    }

    public void SetGoalPosition(Vector3 newGoalPos)
    {
        isMoving = true;
    	goalPosition = newGoalPos;
    }

    public void SetXGoalPosition(Vector3 newGoalPos)
    {
        isMoving = true;
        goalPosition = new Vector3(newGoalPos.x, this.transform.position.y, this.transform.position.z);
    }

    private void UpdateAnimator(float xMovement)
    {
		if(facingRight && xMovement < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) *-1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
        else if (!facingRight && xMovement > 0){
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			facingRight = true;
		}
		characterAnimator.SetFloat("xSpeed", Mathf.Abs(xMovement));
	}

    public void FaceTowards(Transform targetTransform)
    {
        if(targetTransform.position.x > this.transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            facingRight = true;
        } else {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) *-1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
            Debug.Log("turning for target at: " + targetTransform.position.x + " as " + this.gameObject.name + " is at " + transform.position.x);
        }
    }

    public void PlayFootStepSound()
    {
        if(soundEmitter != null)
            soundEmitter.Play();
        //FMOD_StudioSystem.instance.PlayOneShot("event:/sound/character/footsteps", transform.position);
    }
}


/*[RequireComponent (typeof (Animator))]
public class characterMovement : MonoBehaviour
{
	public Vector3 goalPosition = Vector3.zero;

	public float moveSpeed = 1f;

    public UnityEvent onReachGoalPos;
    private FMODUnity.StudioEventEmitter soundEmitter;
    private FMOD.Studio.EventInstance footstepEvent;

	private Animator characterAnimator;
	public bool facingRight = true;
    private ScriptedEventManager currentEventScript;
    private bool isMoving = false;

    private void Start()
    {
        characterAnimator = GetComponent<Animator>();
        if(goalPosition == Vector3.zero)
            goalPosition = transform.position;
    
        soundEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        footstepEvent = FMODUnity.RuntimeManager.CreateInstance("event:/sound/character/footsteps");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent, this.transform, GetComponent<Rigidbody2D>());

        var attributes = FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position);
		footstepEvent.set3DAttributes(attributes);

        int footstepIndex = SoundEnvironment.Instance.GetEnvironmentSoundIndex();
        
        Debug.Log("Setting sound emitter parameter to: " + footstepIndex); 
        //footstepEvent.setParameterByName("FloorType", (float)footstepIndex);
    }

    private void Update()
    {
    	float xDifference = 0;
        if(this.transform.position != goalPosition)
        {
        	xDifference = goalPosition.x - transform.position.x; 
			transform.position = Vector3.MoveTowards(transform.position, goalPosition, moveSpeed * Time.deltaTime);
        }
        else if(isMoving)
        {
            isMoving = false;
            onReachGoalPos.Invoke();
        }
        UpdateAnimator(xDifference);
    }

    public void SetGoalPosition(Vector3 newGoalPos)
    {
        isMoving = true;
    	goalPosition = newGoalPos;
    }

    private void UpdateAnimator(float xMovement)
    {
		if(facingRight && xMovement < 0 || !facingRight && xMovement > 0){
			transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
			facingRight = !facingRight;
		}
		characterAnimator.SetFloat("xSpeed", Mathf.Abs(xMovement));
	}

    public void PlayFootStepSound()
    {
    	Debug.Log("Should play sound now");
    	//FMODUnity.RuntimeManager.PlayOneShotAttached(footstepEvent, this.gameObject);
        footstepEvent.start();
        //FMOD_StudioSystem.instance.PlayOneShot("event:/sound/character/footsteps", transform.position);
    }
}
*/