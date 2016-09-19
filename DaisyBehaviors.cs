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

public class DaisyBehaviors: MonoBehaviour
{
	public bool enableControl = true;
	public INPUT_SCHEMA m_ControlSchema;
	
	private GameObject leader; //object to be followed
	GameObject me;
	float startTime;
	float timeDelay = 0.10f;
	bool sameSpot;
	bool moving;
	public int mouseclick = 0;
	public static int cubeflag = 0;
	public static Vector3 mouseposition;
	private GameObject milo;
	private MiloStateMachine fsm;
	public static int eventEnter = 0;
	
	// Variables for Scout Range
	public float ScoutRange;
	private CameraController CamController;
	private MiloControls2 miloControl;
	[HideInInspector]
	public Transform MiloPosition;
	public bool catchup;
	private float distMiloDaisy;
	private List<Vector3> pathpoints = new List<Vector3>();
	private Vector3[] pathArray;
	private bool isOverlord;
	[HideInInspector]
	public GameObject bubbleFront;
	[HideInInspector]
	public GameObject bubbleBack;
	public bool camfocusmilo;
	
	//public GameObject[] POI;
	public List<GameObject> POI = new List<GameObject>();
	public float dialogueTimer;
	private float temptimer;
	private int i;
	
	// Variables for Daisy's State Machine
	private DaisyStateMachine daisyfsm;
	[HideInInspector]
	public bool canmovedaisy;
	
	// Ric's add on
	public float m_CollisionDetectAdjustment;
	public Cursor m_CursorSystem;
	public float m_DaisyMoveTime;
	
	[HideInInspector]
	public tk2dAnimatedSprite anim;
	
	[HideInInspector]
	private GravityBody gravityBody;
	[HideInInspector]
	public Quaternion desiredRotation;
	[HideInInspector]
	public Quaternion steeringRotation;
	[HideInInspector]
	public Vector3 steeringAngles;
	[HideInInspector]
	public Vector3 rotationAngles;
	[HideInInspector]
	public bool savedfromdaisy;
	
	// Use this for initialization - Added by Chengyin
	void Start ()
	{
		me = this.gameObject;
		m_CursorSystem = GameObject.Find("CursorSystem").GetComponent<Cursor>();
		//m_CursorSystem.setControlSchema(m_ControlSchema);
		if(leader == null)
		{
			leader = GameObject.Find("Milo2.0");
		}
		
		transform.position = leader.transform.position;
		sameSpot = true;
		moving = false;
		mouseclick = 0;
		cubeflag = 0;
		mouseposition = Vector3.zero;

		
		milo = GameObject.Find("Milo2.0");
		fsm = milo.GetComponent<MiloStateMachine>();
		daisyfsm = GetComponent<DaisyStateMachine>();
		miloControl = GameObject.Find("Milo2.0").GetComponent("MiloControls2")as MiloControls2;
		CamController = GameObject.Find("Main Camera").GetComponent("CameraController")as CameraController;
		anim = GetComponent<tk2dAnimatedSprite> ();
		
		ScoutRange = 5000000;
		//UnchainedRange = 20000;
		catchup = true;
		isOverlord = false;
		dialogueTimer = 2.0f;
		temptimer = 0;
		i = 0;
		camfocusmilo = false;
		canmovedaisy = true;
		//daisyfsm.ProcessEvent(DAISYEVENT.MILOMOVE);
		
		/* Milo rotation parameters*/
		gravityBody = leader.GetComponent<GravityBody>();
		steeringRotation = Quaternion.identity;
		steeringAngles = new Vector3(0,0,0);
		savedfromdaisy = false;
	}
	
	// @Brooke for daisy speech bubble triggering
	// so hack do not keep for later, only works for tutorial
	void OnTriggerEnter(Collider trigger)
	{
		if(trigger.gameObject.name.Contains("POI_"))
		{
			if (!POI.Contains(trigger.gameObject))
				POI.Add(trigger.gameObject);
		}else{
			EfficientDebug.DebugLog("collide" + trigger.gameObject.name);
			mouseclick = 0;
			mouseposition = this.transform.position + (this.transform.position - trigger.transform.position).normalized * m_CollisionDetectAdjustment;
			this.transform.position += (this.transform.position - trigger.transform.position).normalized * m_CollisionDetectAdjustment;
			//this.transform.position = milo.transform.position;
		}

	}
	
	// Update is called once per frame
	
	void Update ()
	{
		this.GetComponentInChildren<SpeechBubbleTrigger>().Triggered();
		/*Rotate with Milo*/
		rotationAngles = gravityBody.finalRotation.eulerAngles;
		//making finalrotation consistant
		rotationAngles.x = 0;
		rotationAngles.y = 0;
		steeringRotation.eulerAngles = steeringAngles;
		gravityBody.finalRotation.eulerAngles = rotationAngles;
		desiredRotation = gravityBody.finalRotation * steeringRotation;
		transform.rotation = desiredRotation;
		
		CheckDaisyMove();
		
		if(SameDirection(me.transform.right, milo.transform.right) && isOverlord == false) {
			TurnAround();
		}	
		
		MiloPosition = GameObject.Find("Milo2.0").transform;
		// Respawn Drag
		if (leader == milo && (
			fsm.curState == PLAYERSTATETYPE.DEATH 
			|| fsm.curState == PLAYERSTATETYPE.RESPAWNING)) {
			//EfficientDebug.DebugLog("here");
			daisyfsm.ProcessEvent(DAISYEVENT.MILONEEDRESPAWN);
			//return;	//use RespawnMilo or EndingDragMilo instead
		}
		// Ending Drag
		else if (leader == milo && (fsm.curState == PLAYERSTATETYPE.ENDING)) {
			daisyfsm.ProcessEvent(DAISYEVENT.MILOEND);
		}
		// Rescue Drag in water
		else if (leader == milo && (fsm.curState == PLAYERSTATETYPE.DROWN)) {
			daisyfsm.ProcessEvent(DAISYEVENT.MILODROWN);
		}
		// Idle
		else if (leader == milo && (fsm.curState == PLAYERSTATETYPE.IDLE)) {
			daisyfsm.ProcessEvent(DAISYEVENT.MILOIDLE);
		}
		// Follow
		else {
			daisyfsm.ProcessEvent(DAISYEVENT.MILOMOVE);
		}
		//me.transform.LookAt(Camera.main.transform.position);
		
		
		// Put triggered POI into array and Generate Speechbubble if Daisy get back to Milo
		if(isOverlord == false && POI.Count != 0 && (me.transform.position - MiloPosition.position).sqrMagnitude <= 2000) {
			bubbleBack.transform.position = new Vector3(this.transform.position.x + 60.0f, this.transform.position.y + 60.0f, this.transform.position.z + 1.0f);
			bubbleFront.transform.position = new Vector3(this.transform.position.x + 60.0f, this.transform.position.y + 60.0f, this.transform.position.z + 1.5f);
			GameObject[] poi = POI.ToArray();
			//EfficientDebug.DebugLog(poi.length);
			if(temptimer >= dialogueTimer){
				i++;
				temptimer = 0;
				if(i == POI.Count) {
					POI.Clear();
					i = 0;
				}
			}
			else {
				me.GetComponentInChildren<TextMesh>().text = poi[i].name.ToString();
				/*if(gameObject.GetComponent<TempForegroundBubble>().renderer.enabled)
				{
					gameObject.GetComponent<TempSpeechBubble>().Stop();
				}
				gameObject.GetComponent<TempSpeechBubble>().isLooping = poi[i].GetComponent<SpeechBubbleTrigger>().isLooping;
				gameObject.GetComponent<TempSpeechBubble>().Play(poi[i].gameObject);*/
				temptimer += Time.deltaTime * 5;
			}			
		}
		
		if (enableControl&&(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || 
			Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space) 
			|| Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)
			|| (m_ControlSchema == INPUT_SCHEMA.XPAD && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)))
			&& canmovedaisy == true && savedfromdaisy == false) {
			mouseclick = 0;
			catchup = true;
			isOverlord = false;
			
			if(camfocusmilo == false) {
				CamController.player = milo.transform;
				CamController.currTarget = milo.transform;
			}
			daisyfsm.ProcessEvent(DAISYEVENT.MILOMOVE);
			
			Destroy (me.gameObject.GetComponent<FixedJoint>());
			miloControl.enableControl = true;
			
		}
		
		if (enableControl&&(Input.GetMouseButton (0) || (m_ControlSchema == INPUT_SCHEMA.XPAD && Input.GetAxis("XButton_R") == 1))
			&& canmovedaisy == true && savedfromdaisy == false) {

			mouseclick = 1;
			if (m_ControlSchema == INPUT_SCHEMA.XPAD)
				mouseposition = Camera.main.ScreenToWorldPoint (new Vector3(m_CursorSystem.getXpadPosition().x, 
																			m_CursorSystem.getXpadPosition().y, 
																			m_CursorSystem.getXpadPosition().z));	
			else
				mouseposition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mouseposition.z = 10;
			// Need to fix: or it will ruin the plug
			if(!this.GetComponent("FixedJoint"))
				if(SameDirection((mouseposition - me.transform.position), me.transform.right)) {
					TurnAround();
				}
			
			if((mouseposition - MiloPosition.position).sqrMagnitude >= ScoutRange) {
				mouseclick = 0;
				EfficientDebug.DebugLog("Daisy Can't Go Too Far!");
			}
			isOverlord = true;
			
		}
		
		if (mouseclick == 1) {
			daisyfsm.ProcessEvent(DAISYEVENT.PLAYERCONTROL);
			if(camfocusmilo == false) {
				CamController.currTarget = this.gameObject.transform;
				CamController.player = this.gameObject.transform;
			}
			miloControl.enableControl = false;

			catchup = false;
			
			iTween.MoveUpdate (me, iTween.Hash ("position", mouseposition, "time", m_DaisyMoveTime));
			//Synchronize the Speech Bubble
			//iTween.MoveUpdate (bubbleBack, new Vector3(mouseposition.x + 60.0f, mouseposition.y + 60.0f, mouseposition.z +1.0f), 4.0f);
			//iTween.MoveUpdate (bubbleFront, new Vector3(mouseposition.x + 60.0f, mouseposition.y + 60.0f, mouseposition.z +1.5f), 4.0f);
			
			if (cubeflag == 1) {
				//	print ("event triggered");
				mouseclick = 0;
			}
			
			if (Mathf.Sqrt ((mouseposition.x - this.transform.position.x) * (mouseposition.x - this.transform.position.x) +
			(mouseposition.y - this.transform.position.y) * (mouseposition.y - this.transform.position.y)) < 5) {
				mouseclick = 0;
				//	print ("Reached");
			}
			
			if(gameObject.GetComponent<DaisyStateMachine>().daisyAudioWoosh)
			{
				gameObject.GetComponent<DaisyStateMachine>().daisyAudioWoosh.GetComponent<AudioRandomPlay>().EnablePlay();
			}

		} else {
			if (sameSpot) {//if we're in the same spot, check to see if we've moved from that spot
				Vector3 posi = leader.transform.position;
				float rot = leader.transform.eulerAngles.z;
			
				posi.x += (leader.transform.localScale.x * Mathf.Cos (rot * Mathf.PI / 180.0f) - leader.transform.localScale.y * Mathf.Sin (rot * Mathf.PI / 180.0f));
				posi.y += (leader.transform.localScale.y * Mathf.Cos (rot * Mathf.PI / 180.0f) + leader.transform.localScale.x * Mathf.Sin (rot * Mathf.PI / 180.0f));
				posi.z = 10;
			
				if (Vector3.Distance (me.transform.position, posi) > 40) {
					startTime = Time.time;
					//iTween.Stop(me,true);
					sameSpot = false;
					////EfficientDebug.DebugLog("moving "+Time.time);
				}
			} else if (!moving) {//give a short delay before deciding to follow
				if (Time.time - startTime > timeDelay) {
					moving = true;
				}
			} else { //play catch-up
				if(catchup == true) {
					Vector3 posi = leader.transform.position;
					float rot = leader.transform.eulerAngles.z;
					
					posi.x += (leader.transform.localScale.x * Mathf.Cos (rot * Mathf.PI / 180.0f) - leader.transform.localScale.y * Mathf.Sin (rot * Mathf.PI / 180.0f));
					posi.y += (leader.transform.localScale.y * Mathf.Cos (rot * Mathf.PI / 180.0f) + leader.transform.localScale.x * Mathf.Sin (rot * Mathf.PI / 180.0f));
					posi.z = 10;
			
					iTween.MoveUpdate (me, iTween.Hash ("position", posi, "time", 2));
					iTween.RotateUpdate (me, leader.transform.eulerAngles, 4);

					if (Vector3.Distance (me.transform.position, posi) < 40) { //once there, reset the whole scenario
						moving = false;
						sameSpot = true;
						//iTween.Stop(me,true);
						////EfficientDebug.DebugLog("stopping "+Time.time);
					}
				}
			}
		}
		
	}
	
	void FixedUpdate(){
		DaisyCollisionDetect();
	}
	
	// function to do collision detection
	private void DaisyCollisionDetect(){
		//EfficientDebug.DebugLog("Daisy CD!");
		Debug.DrawLine(this.transform.position, this.transform.position + this.transform.rotation * Vector3.up * 20);
		Debug.DrawLine(this.transform.position, this.transform.position + this.transform.rotation * Vector3.down * 20);
		Debug.DrawLine(this.transform.position, this.transform.position + this.transform.rotation * Vector3.left * 20);
		Debug.DrawLine(this.transform.position, this.transform.position + this.transform.rotation * Vector3.right * 20);
	}
	
	private bool SameDirection(Vector3 vec1, Vector3 vec2)
	{
		if(Vector3.Dot(vec1,vec2)>0)
			return true;
		else
			return false;
	}
		
	private void TurnAround()
	{
		steeringAngles.y = 180 - steeringAngles.y;
		steeringRotation.eulerAngles = steeringAngles;
		desiredRotation = gravityBody.finalRotation * steeringRotation;
		transform.rotation = desiredRotation;
	}
	
	public void CheckDaisyMove()
	{
		if(fsm.curState == PLAYERSTATETYPE.JUMP || fsm.curState == PLAYERSTATETYPE.INAIR
			|| fsm.curState == PLAYERSTATETYPE.NOGRAVITYFIELD || fsm.curState == PLAYERSTATETYPE.ENDING
			|| fsm.curState == PLAYERSTATETYPE.RESPAWNING || fsm.curState == PLAYERSTATETYPE.DEATH 
			|| fsm.curState == PLAYERSTATETYPE.DROWN) {
			canmovedaisy = false;
		}
		else {
			canmovedaisy = true;
		}
	}
	
	public void DisableSaveState() {
		savedfromdaisy = false;
	}
	
	public void SetInputSchema(INPUT_SCHEMA inSchema)
	{
		m_ControlSchema = inSchema;
	}
}
