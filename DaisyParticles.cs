/*
 * Author - Chengyin
 * Description - Daisy following milo in platform world. 
 *  
 * Person to Contact - Chengyin
 * 
 * */
 
using UnityEngine;
using System.Collections;

public class DaisyParticles : MonoBehaviour {
	
	private Transform daisyTransform;
	private DaisyStateMachine daisyState;
	private tk2dAnimatedSprite daisyAnim;
	
	public GameObject[] particleObjects; 			/* particleObj[0] should always be Idle, 1 should be Run */
	private GameObject currentParticleObject;
	
	public float particleZValue = 0.1f;
	
	private Vector3[] originalLocalPosArray;
	private Vector3 originalLocalPosition;
	
	
	void Awake() {
		currentParticleObject = particleObjects[0];
		originalLocalPosition = currentParticleObject.transform.localPosition;
		
		// Disable all particles except 0 in case we have need for more particle effects later
		for(int i = 1; i < particleObjects.Length; i++) {
			particleObjects[i].active = false;
		}
	}
	
	
	void Start () {
		daisyAnim = GetComponent<tk2dAnimatedSprite>();
	}
	
	
	void Update () {
		BringParticlesForward();
		
		DetermineAnimationState();
	}
	
	
	// IsFlipped(): Check if Daisy has been flipped face to the left (if upright and if transform.rotation.eulerAngles y is 180), and return true if y is 180
	private bool IsFlipped()
	{
		if(Mathf.Floor(transform.rotation.eulerAngles.y) == 180) {
			return true;
		}
		
		else {
			return false;
		}
	}

    // BringPartclesForward(): Bring current particle object up so that it's in front of Daisy (so it has higher z value)
	private void BringParticlesForward() {
		
		/* Use originalLocalPosition to prevent drifting of original position. Otherwise particles
		get weird because of movement (seems to be charge jumps that do it, but unsure if that's the only cause) */
		currentParticleObject.transform.localPosition = originalLocalPosition;
		
		if(IsFlipped ()) {
			currentParticleObject.transform.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y, -particleZValue);
		}
		else {
			currentParticleObject.transform.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y, particleZValue);
			
		}
	}
	
	
	/* DetermineAnimationState(): Checks what animation is playing and use the proper particle object. May need to modify
	 * if particle effects themselves must have their own state or must react to Daisy's actual state */
	private void DetermineAnimationState() {
		if(daisyAnim.CurrentClip != null) {
			if(daisyAnim.CurrentClip.name == "Idle") {
				
				if(currentParticleObject.name != "DaisyParticlesIdle") {
					//particleObjects[1].active = false;			/* I feel like this is an awful way to do it. Oops. */
					//particleObjects[0].active = true;
					//currentParticleObject = particleObjects[0];
					currentParticleObject.active = false;
					currentParticleObject = particleObjects[0];
					currentParticleObject.active = true;
					
				}
			}
			
			else if(daisyAnim.CurrentClip.name == "Run") {
				
				if(currentParticleObject.name != "DaisyParticlesRun") {
					currentParticleObject.active = false;
					currentParticleObject = particleObjects[1];
					currentParticleObject.active = true;
					
				}
			}
		}
	}
    
}
