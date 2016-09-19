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

public class DaisyEndingDrag : DaisyState {
	
	private static DaisyEndingDrag instance;
	//public Vector3 HackPosition = Vector3.zero;
	private EndingDragMilo endingdragmilo;
	
	private DaisyEndingDrag()
	{
		daisystate = DAISYSTATE.ENDINGDRAG;
		Daisy = GameObject.Find("DaisyFollowMilo");
		if(Daisy==null)
			throw new UnityException();
	}
	
	public override void Set()
	{
		stateTransition = new Dictionary<DAISYEVENT,DaisyState>();
		stateTransition[DAISYEVENT.MILOMOVE] = DaisyFollowMilo.Instance;
	}
	
	public static DaisyEndingDrag Instance
	{
		get{
			if(instance==null){
				
				instance = new DaisyEndingDrag();
			}
			return instance;
		}
	}
	
	public override void Enter(DaisyStateMachine fsm)
	{
		endingdragmilo = Daisy.GetComponent<EndingDragMilo>();
		EfficientDebug.DebugLog("Entered EndingDrag");
	}
	
	public override void Execute(DaisyStateMachine fsm)
	{
		// static idle frame animation - Need to be done later
		endingdragmilo.isEnding = true;
			
	}
	
	public override void Exit(DaisyStateMachine fsm)
	{
		endingdragmilo.isEnding = false;
		EfficientDebug.DebugLog("Exited Ending Drag");
	}
}
