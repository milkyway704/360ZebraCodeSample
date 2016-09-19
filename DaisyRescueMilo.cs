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

public class DaisyRescueMilo : DaisyState {

	private static DaisyRescueMilo instance;
	//public Vector3 HackPosition = Vector3.zero;
	private RescueMilo rescuemilo;

	
	private DaisyRescueMilo()
	{
		daisystate = DAISYSTATE.RESCUEMILO;
		Daisy = GameObject.Find("DaisyFollowMilo");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		stateTransition[DAISYEVENT.MILOMOVE] = DaisyFollowMilo.Instance;
	}
	
	public static DaisyRescueMilo Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyRescueMilo();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		rescuemilo = Daisy.GetComponent<RescueMilo>();
		EfficientDebug.DebugLog("Entered Rescue");
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{
		// static idle frame animation - Need to be done later
		rescuemilo.needRescue = true;
			
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{
		rescuemilo.needRescue = false;
		GameObject.FindGameObjectWithTag("Player").GetComponent<MiloWaterEffect>().EndDrown();
		EfficientDebug.DebugLog("Exited Rescue");
	}
}
