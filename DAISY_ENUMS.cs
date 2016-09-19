/*
 * Author - Chengyin
 * Description - Daisy following milo in platform world. 
 *  
 * Person to Contact - Chengyin
 * 
 * */

using UnityEngine;
using System.Collections;

public enum DAISYSTATE{
	IDLE,
	UNCHAINED,
	FOLLOWMILO,
	RESPAWNMILO,
	ENDINGDRAG,
	RESCUEMILO
}

public enum DAISYEVENT{
	MILOIDLE,
	PLAYERCONTROL,
	PLAYERNOCONTROL,
	MILOMOVE,
	MILONEEDRESPAWN,
	MILOEND,
	MILODROWN,
}

