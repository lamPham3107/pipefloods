using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class Timer_origin : MonoBehaviour
{
	/// <summary>
	/// Text Component
	/// </summary>
	public Text uiText;

	/// <summary>
	/// The time increment counter text.
	/// </summary>
	public Text timeIncCountText;

	/// <summary>
	/// The time in seconds.
	/// </summary>
	public int timeInSeconds;

	/// <summary>
	/// Whether the timer is paused or not.
	/// </summary>
	private bool isPaused;

	/// <summary>
	/// The time counter.
	/// </summary>
	private float timeCounter;

	/// <summary>
	/// The total time.
	/// </summary>
	private int totalTime;

	/// <summary>
	/// The sleep time.
	/// </summary>
	private float sleepTime;

	/// <summary>
	/// The time increment counter.
	/// </summary>
	private int incCount;

	/// <summary>
	/// The initial color of the time.
	/// </summary>
	private Color timeInitialColor;

	/// <summary>
	/// The time increment counter str key
	/// </summary>
	private readonly string incCountKey = "TimeIncrement";

	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static Timer_origin instance;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		}

		if (uiText == null) {
			uiText = GetComponent<Text> ();
		}

		if (PlayerPrefs.HasKey (incCountKey)) {
			incCount = PlayerPrefs.GetInt (incCountKey);
			SetIncCounterTextValue ();
		} else {
			ResetIncCounter ();
		}

		timeInitialColor = uiText.color;

		Pause ();
	}

	/// <summary>
	/// Run the timer.
	/// </summary>
	public void Run(){
		isPaused = false;
		timeCounter = 0;
		sleepTime = 0.01f;
		totalTime =  (Mission.selectedMission.levelsManagerComponent.starsTimePeriod * 4 ) * 2;
        timeInSeconds = totalTime;
		CancelInvoke("Wait");
		InvokeRepeating ("Wait", 0, sleepTime);
	}

	/// <summary>
	/// Stop the Timer.
	/// </summary>
	public void Stop ()
	{
		CancelInvoke ();
	}

	/// <summary>
	/// Reset the timer.
	/// </summary>
	public void Reset ()
	{
		Stop ();
		Run ();
	}

	/// <summary>
	/// Pause the Timer.
	/// </summary>
	public void Pause ()
	{
		isPaused = true;
	}

	/// <summary>
	/// Resume the Timer.
	/// </summary>
	public void Resume ()
	{
		isPaused = false;
	}

	/// <summary>
	/// Wait.
	/// </summary>
	private void Wait ()
	{
		if (!isPaused) {
			timeCounter += sleepTime;
		}
		timeInSeconds = totalTime -  (int)timeCounter;
		ApplyTime ();
		Progress.instance.SetProgress (timeCounter);
	}

	/// <summary>
	/// Applies the time into TextMesh Component.
	/// </summary>
	private void ApplyTime ()
	{
		if (uiText == null) {
			return;
		}

		//calculate the minutes
		int mins = (int)(timeInSeconds / 60);

		//calculat the remaining seconds
		int seconds = (int)(timeInSeconds % 60);

		if (timeInSeconds < 11) {
			uiText.color = Colors.timeOutColor;
		} else {
			uiText.color = timeInitialColor;
		}

		uiText.text = GetNumberWithZeroFormat (mins) + ":" + GetNumberWithZeroFormat (seconds);
	}

	/// <summary>
	/// Get the number with zero format.
	/// </summary>
	/// <returns>The number with zero format.</returns>
	/// <param name="number">Ineger Number.</param>
	public static string GetNumberWithZeroFormat (int number)
	{
		string strNumber = "";
		if (number < 10) {
			strNumber += "0";
		}
		strNumber += number;
		
		return strNumber;
	}

	/// <summary>
	/// Increases the time.
	/// </summary>
	/// <param name="value">Increase Value.</param>
	public void IncreaseTime ()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			Pause ();
			GameManager.instance.isRunning = false;
			GameManager.instance.noConnectionDialog.Show (false);
			return;//skip if there is no connection
		}

		if (incCount == 0) {
			Pause ();
			GameManager.instance.isRunning = false;
			AudioClips.instance.PlayLockedSFX ();
			//Show Increase Time dialog
			GameManager.instance.increaseTimeDialog.Show (false);
            //AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_NEED_TIME_DIALOG);

            return;
		}

		if (timeCounter < 1) {
			AudioClips.instance.PlayLockedSFX ();
			return;
		}

		incCount--;
		timeCounter -= Mission.selectedMission.levelsManagerComponent.starsTimePeriod * 0.85f;
		timeCounter = Mathf.Clamp (timeCounter,0, Mathf.Infinity);
		SetIncCounterTextValue ();
		AudioClips.instance.PlayIncreaseSFX ();
		PlayerPrefs.SetInt (incCountKey, incCount);
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// Reset the increment counter.
	/// </summary>
	public void ResetIncCounter(){
		incCount = 3;
		SetIncCounterTextValue ();
	}

	/// <summary>
	/// Sets the increment counter text value.
	/// </summary>
	public void SetIncCounterTextValue(){
		timeIncCountText.text = incCount.ToString ();
	}

	/// <summary>
	/// Whether the timer is paused.
	/// </summary>
	/// <returns><c>true</c> if the timer is paused; otherwise, <c>false</c>.</returns>
	public bool IsPaused(){
		return isPaused;
	}
}