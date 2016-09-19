/*
 * Author - Chengyin
 * Description - Daisy following milo in platform world. 
 *  
 * Person to Contact - Chengyin
 * 
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DaisyState {
		
	protected Dictionary<DAISYEVENT, DaisyState> stateTransition;
	public DAISYSTATE daisystate;
	public GameObject Daisy;
	public GameObject Milo;
	public abstract void Enter(DaisyStateMachine fsm);
	public abstract void Execute(DaisyStateMachine fsm);
	public abstract void Exit(DaisyStateMachine fsm);
	public abstract void Set();
	
	public void processEvent (DaisyStateMachine fsm, DAISYEVENT eventOcc)
	{
		if(stateTransition.ContainsKey(eventOcc))
		{
			fsm.ChangeState(stateTransition[eventOcc]);
		}
	}
	
}
