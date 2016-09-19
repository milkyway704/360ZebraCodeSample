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

public class DaisyUnchained : DaisyState {

	private static DaisyUnchained instance;
	//public Vector3 HackPosition = Vector3.zero;
	
	private DaisyUnchained()
	{
		daisystate = DAISYSTATE.UNCHAINED;
		Daisy = GameObject.Find("DaisyFollowMilo");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		//stateTransition[DAISYEVENT.PLAYERNOCONTROL] = DaisyFollowMilo.Instance;
		stateTransition[DAISYEVENT.MILOMOVE] = DaisyFollowMilo.Instance;
		stateTransition[DAISYEVENT.MILONEEDRESPAWN] = DaisyRespawnMilo.Instance;
	}
	
	public static DaisyUnchained Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyUnchained();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		if (fsm.GetComponent<DaisyBehaviors>().anim != null)
			fsm.GetComponent<DaisyBehaviors>().anim.Play("Run");
		EfficientDebug.DebugLog("Entered Unchained");
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{
		// static idle frame animation - Need to be done later
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{

		EfficientDebug.DebugLog("Exited Unchained");
	}
}
