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

public class DaisyRespawnMilo : DaisyState {

	private static DaisyRespawnMilo instance;
	//public Vector3 HackPosition = Vector3.zero;
	private RespawnMilo respawnmilo;
	
	private DaisyRespawnMilo()
	{
		daisystate = DAISYSTATE.RESPAWNMILO;
		Daisy = GameObject.Find("DaisyFollowMilo");
		Milo = GameObject.Find("Milo2.0");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		stateTransition[DAISYEVENT.MILOMOVE] = DaisyFollowMilo.Instance;
		stateTransition[DAISYEVENT.MILOIDLE] = DaisyIdle.Instance;
	}
	
	public static DaisyRespawnMilo Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyRespawnMilo();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		respawnmilo = Daisy.GetComponent<RespawnMilo>();
		Daisy.GetComponent<DaisyBehaviors>().canmovedaisy = false;
		Daisy.GetComponent<DaisyBehaviors>().savedfromdaisy = true;
		if(fsm.daisyAudioRespawn)
		{
			fsm.daisyAudioRespawn.GetComponent<AudioRandomPlay>().EnablePlay();
		}
		EfficientDebug.DebugLog("Entered RESPAWN");
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{
		// static idle frame animation - Need to be done later
		respawnmilo.needRespawn = true;
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{
		respawnmilo.needRespawn = false;
		Daisy.GetComponent<RespawnMilo>().dragSpeed = 0;

		Daisy.GetComponent<RespawnMilo>().cleanup();
		Daisy.GetComponent<DaisyBehaviors>().Invoke("DisableSaveState", 2.0f);
		EfficientDebug.DebugLog("Daisy finished respawning");
		EfficientDebug.DebugLog("Exited RESPAWN");
		
		// give Milo a short involunable time
		Milo.GetComponent<MiloStateMachine>().SetMiloInvulnerable();
	}
}
