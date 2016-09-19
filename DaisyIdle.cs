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

public class DaisyIdle : DaisyState {
	
	private static DaisyIdle instance;
	//public Vector3 HackPosition = Vector3.zero;
	
	private DaisyIdle()
	{
		daisystate = DAISYSTATE.IDLE;
		Daisy = GameObject.Find("DaisyFollowMilo");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		stateTransition[DAISYEVENT.PLAYERCONTROL] = DaisyUnchained.Instance;
		stateTransition[DAISYEVENT.MILOMOVE] = DaisyFollowMilo.Instance;
		
		stateTransition[DAISYEVENT.MILODROWN] = DaisyRescueMilo.Instance;
		stateTransition[DAISYEVENT.MILONEEDRESPAWN] = DaisyRespawnMilo.Instance;
		stateTransition[DAISYEVENT.MILOEND] = DaisyEndingDrag.Instance;
		
	}
	
	public static DaisyIdle Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyIdle();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		if (fsm.GetComponent<DaisyBehaviors>().anim != null)
		{
			fsm.GetComponent<DaisyBehaviors>().anim.Play("Idle");
			EfficientDebug.DebugLog("Entered Idle");
		}
		
		if(fsm.daisyAudioIdle)
		{
			fsm.daisyAudioIdle.GetComponent<AudioRandomPlay>().EnablePlay();
		}
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{
		// static idle frame animation - Need to be done later

			
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{

		EfficientDebug.DebugLog("Exited Idle");
		if(fsm.daisyAudioIdle)
		{
			fsm.daisyAudioIdle.GetComponent<AudioRandomPlay>().DisablePlay();
		}
	}
}
