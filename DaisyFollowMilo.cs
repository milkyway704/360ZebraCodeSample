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

public class DaisyFollowMilo : DaisyState {

	private static DaisyFollowMilo instance;
	//public Vector3 HackPosition = Vector3.zero;
	private RescueMilo rescuemilo;
	private RespawnMilo respawnmilo;
	
	private DaisyFollowMilo()
	{
		daisystate = DAISYSTATE.FOLLOWMILO;
		Daisy = GameObject.Find("DaisyFollowMilo");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		stateTransition[DAISYEVENT.MILODROWN] = DaisyRescueMilo.Instance;
		stateTransition[DAISYEVENT.MILONEEDRESPAWN] = DaisyRespawnMilo.Instance;
		stateTransition[DAISYEVENT.MILOEND] = DaisyEndingDrag.Instance;
		stateTransition[DAISYEVENT.PLAYERCONTROL] = DaisyUnchained.Instance;
		stateTransition[DAISYEVENT.MILOIDLE] = DaisyIdle.Instance;
	}
	
	public static DaisyFollowMilo Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyFollowMilo();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		if (fsm.GetComponent<DaisyBehaviors>().anim != null)
			fsm.GetComponent<DaisyBehaviors>().anim.Play("Run");
		if(fsm.daisyAudioIdle)
		{
			fsm.daisyAudioIdle.GetComponent<AudioRandomPlay>().play = true;
		}
		EfficientDebug.DebugLog("Entered Follow");
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{

		//Daisy.GetComponent<DaisyBehaviors>().catchup = true;
		//Daisy.GetComponent<DaisyBehaviors>().isOverlord = false;
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{
		if(fsm.daisyAudioIdle)
		{
			fsm.daisyAudioIdle.GetComponent<AudioRandomPlay>().play = false;
		}
		EfficientDebug.DebugLog("Exited Follow");
	}
}
