using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Hint : MonoBehaviour {

	/// <summary>
	/// The hint animator reference.
	/// </summary>
	private Animator animator;

	// Use this for initialization
	void Start () {

		animator = GetComponent<Animator> ();

		//Show the hint in the first level only
		if (TableLevel.selectedLevel.ID == 1) {
			animator.enabled = true;
			Invoke ("HideHint", 3);
		} else {
			//Set GameManager,Timer interactable
			GameManager.instance.isRunning = true;
			Timer_origin.instance.Run ();
		}
	}

	/// <summary>
	/// Hide the hint.
	/// </summary>
	public void HideHint(){
		
		//Hide the hint
		animator.SetTrigger ("Off");

		//Set GameManager,Timer interactable
		GameManager.instance.isRunning = true;
		Timer_origin.instance.Run ();
	}
}
