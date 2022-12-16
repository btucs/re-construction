using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogSoundResolver : MonoBehaviour
{
    public UnityEvent femaleDialogueStart = new UnityEvent();
    public UnityEvent femaleDialogueEnd = new UnityEvent();

    public UnityEvent maleDialogueStart = new UnityEvent();
    public UnityEvent maleDialogueEnd = new UnityEvent();

    private VoiceType type;

    public void SetVoiceType(VoiceType newType)
    {
    	type = newType;
    }


    public void StartDialogueSound()
    {
    	if(type == VoiceType.male)
    	{
    		if(maleDialogueStart != null)
    			maleDialogueStart.Invoke();
    	} else {
    		if(femaleDialogueStart != null)
    			femaleDialogueStart.Invoke();
    	}
    }

    public void EndDialogueSound()
    {
    	if(type == VoiceType.male)
    	{
    		if(maleDialogueEnd != null)
    			maleDialogueEnd.Invoke();
    	} else {
    		if(femaleDialogueEnd != null)
    			femaleDialogueEnd.Invoke();
    	}
    }
}
