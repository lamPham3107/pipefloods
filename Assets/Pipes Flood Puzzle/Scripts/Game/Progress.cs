using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Progress : MonoBehaviour
{
	/// <summary>
	/// The star off sprite.
	/// </summary>
	public Sprite starOff;
		
	/// <summary>
	/// The star on sprite.
	/// </summary>
	public Sprite starOn;

	/// <summary>
	/// The level stars.
	/// </summary>
	public Image[] levelStars;

	/// <summary>
	/// The fill image reference.
	/// </summary>
	public Image fill;

	/// <summary>
	/// The finish call back holder.
	/// </summary>
	public Transform finishCallBackHolder;

	/// <summary>
	/// The finish call back.
	/// </summary>
	public string finishCallBack;

	/// <summary>
	/// Droped star flags.
	/// </summary>
	private bool [] dropedStar;

	/// <summary>
	/// The stars counter.
	/// </summary>
	[HideInInspector]
	public int starsCount;

	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static Progress instance;

	void Awake(){
		if (instance == null){
			instance = this;
		}
		SetUIThreeStars ();
	}

	// Use this for initialization
	void Start ()
	{
		dropedStar = new bool[3];
	}

	/// <summary>
	/// Set the value of the progress.
	/// </summary>
	/// <param name="currentTime">Current time.</param>
	public void SetProgress (float currentTime)
	{

        fill.fillAmount = 1 - (currentTime / (Mission.selectedMission.levelsManagerComponent.starsTimePeriod * 4.0f * 2));

		if (currentTime > 0 && currentTime <= Mission.selectedMission.levelsManagerComponent.starsTimePeriod * 4.0f) {
			SetUIThreeStars ();
			starsCount = 3;
		} else if (currentTime > Mission.selectedMission.levelsManagerComponent.starsTimePeriod * 4.0f && currentTime <= (6.0f * Mission.selectedMission.levelsManagerComponent.starsTimePeriod)) {
			SetUITwoStars ();
			starsCount = 2;
			DropFirstStar ();
		} else if (currentTime > (2.0f * Mission.selectedMission.levelsManagerComponent.starsTimePeriod) && currentTime <= (8.0f * Mission.selectedMission.levelsManagerComponent.starsTimePeriod)) {	
			SetUIOneStar ();
			starsCount = 1;
			DropSecondStar ();
		} else {
			if (GameManager.instance.isRunning)
			{
				//0 Stars
				SetUIZeroStars();
				starsCount = 0;
				DropThirdStar();

				Timer_origin.instance.Pause();
				GameManager.instance.isRunning = false;
				//Invoke("FinishCallBack", 2);
				FinishCallBack();
			}
		}
	}

	/// <summary>
	/// Set zero stars in the UI.
	/// </summary>
	private void SetUIZeroStars(){
		if (levelStars [0] != null) {
			levelStars [0].sprite = starOff;
		}
		if (levelStars [1] != null) {
			levelStars [1].sprite = starOff;
		}
		if (levelStars [2] != null) {
			levelStars [2].sprite = starOff;
		}
	}

	/// <summary>
	/// Set one star in the UI.
	/// </summary>
	private void SetUIOneStar(){
		//1 Star
		if (levelStars [0] != null) {
			levelStars [0].sprite = starOff;
		}
		if (levelStars [1] != null) {
			levelStars [1].sprite = starOff;
		}
		if (levelStars [2] != null) {
			levelStars [2].sprite = starOn;
		}
	}

	/// <summary>
	/// Set two stars in the UI.
	/// </summary>
	private void SetUITwoStars(){
		//2 Stars
		if (levelStars [0] != null) {
			levelStars [0].sprite = starOff;
		}
		if (levelStars [1] != null) {
			levelStars [1].sprite = starOn;
		}
		if (levelStars [2] != null) {
			levelStars [2].sprite = starOn;
		}
	}

	/// <summary>
	/// Set three stars in the UI.
	/// </summary>
	private void SetUIThreeStars(){
		//3 Stars
		if (levelStars [0] != null) {
			levelStars [0].sprite = starOn;
		}
		if (levelStars [1] != null) {
			levelStars [1].sprite = starOn;
		}
		if (levelStars [2] != null) {
			levelStars [2].sprite = starOn;
		}
	}

	/// <summary>
	/// Finish(TimeOut) call back.
	/// </summary>
	private void FinishCallBack(){
		if (finishCallBackHolder != null && !string.IsNullOrEmpty (finishCallBack)) {
			//On time out call back
			finishCallBackHolder.gameObject.SendMessage (finishCallBack);
		}
	}

	/// <summary>
	/// Drop the first star.
	/// </summary>
	private void DropFirstStar(){
		if (dropedStar [0]) {
			return;
		}

		dropedStar [0] = true;
		DropStarEffect (levelStars[0].transform);
	}

	/// <summary>
	/// Drop the second star.
	/// </summary>
	private void DropSecondStar(){
		if (dropedStar [1]) {
			return;
		}

		dropedStar [1] = true;
		DropStarEffect (levelStars[1].transform);
	}

	/// <summary>
	/// Drop the third star.
	/// </summary>
	private void DropThirdStar(){
		if (dropedStar [2]) {
			return;
		}

		dropedStar [2] = true;
		DropStarEffect (levelStars[2].transform);
	}

	/// <summary>
	/// Drop star effect.
	/// </summary>
	/// <param name="star">Star transform reference.</param>
	private void DropStarEffect(Transform star){
		if (star == null) {
			return;
		}

		GameObject tempInstance = Instantiate (star.gameObject, star.position, Quaternion.identity) as GameObject;
		tempInstance.GetComponent<Image> ().sprite = starOn;
		tempInstance.transform.SetParent (star.parent);
		tempInstance.transform.localScale = star.localScale;

		Rigidbody2D tempInstanceRB = tempInstance.GetComponent<Rigidbody2D> ();
		tempInstanceRB.isKinematic = false;
		tempInstanceRB.AddForce (new Vector2(0.1f,1) * 400);
		tempInstanceRB.AddTorque (360);

		AudioClips.instance.PlayDropStarSFX ();

		Destroy (tempInstance, 5);
	}
}
