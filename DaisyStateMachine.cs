/*
 * Author - Chengyin, Ric
 * Description - 
 * 
 * Person to Contact - Chengyin
 * 
 * */

using UnityEngine;
using System.Collections;

public class DaisyStateMachine : MonoBehaviour {
	
	private DaisyState currentState;
	private DaisyState previousState;
	private DaisyState globalState;
	
	[HideInInspector]
	public DAISYSTATE curState;
	
	[HideInInspector]
	public DaisyBehaviors d_Controler;

	public GameObject daisy;
	
	//For Audio
	public GameObject daisyAudioIdle;
	public GameObject daisyAudioWoosh;
	public GameObject daisyAudioRespawn;
	public GameObject daisyAudioGrunt;
	
	// Use this for initialization
	void Start () {
		//initialize state
		DaisyState s = DaisyIdle.Instance;
		s = DaisyFollowMilo.Instance;
		s = DaisyRespawnMilo.Instance;
		s = DaisyRescueMilo.Instance;
		s = DaisyUnchained.Instance;
		s = DaisyEndingDrag.Instance;
		
		s = DaisyIdle.Instance;
		s.Set();
		s = DaisyFollowMilo.Instance;
		s.Set();
		s = DaisyRespawnMilo.Instance;
		s.Set();
		s = DaisyRescueMilo.Instance;
		s.Set();
		s = DaisyUnchained.Instance;
		s.Set();
		s = DaisyEndingDrag.Instance;
		s.Set();
		
		SetCurrentState(DaisyIdle.Instance);
		
		daisy = gameObject;
		d_Controler = gameObject.GetComponent<DaisyBehaviors>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//if there is a current state, perform
		if(currentState!=null)
			currentState.Execute(this);
		
		//if there is a global state, perform
		if(globalState!=null)
			globalState.Execute(this);
		
		if(currentState==null)
		{
			
		}
	}
	
	
	public void SetCurrentState(DaisyState state)
	{
		currentState = state;
	}
	
	public void SetPreviousState(DaisyState state)
	{
		previousState = state;
	}
	
	public void SetGlobalState(DaisyState state)
	{
		globalState = state;
	}
	
	//change state to newState
	public bool ChangeState(DaisyState newState)
	{
		//sanity check
		if(newState==null)
			return false;
		
		//keep previous state
		previousState = currentState;
		
		//perform Exit actions of previous state
		if (previousState == null)
		{
			Debug.LogWarning("Previous state is null when entering new state " + newState.ToString());
		}
		else
		{
			previousState.Exit(this);
		}
		
		currentState = newState;
		
		//perform Enter behaviours of current state
		currentState.Enter(this);
		
		curState = currentState.daisystate;
		
		return true;
	}
	
	//revert to the previous state
	public void RevertState()
	{
		ChangeState(previousState);
	}
	
	public bool ProcessEvent(DAISYEVENT eventOcc)
	{

		if(currentState==null){
			//EfficientDebug.DebugLog("Fuddu");
		}
		currentState.processEvent(this,eventOcc);
		return true;
	}
}
