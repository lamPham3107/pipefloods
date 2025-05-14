using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class WinDialog : MonoBehaviour
{
	/// <summary>
	/// Star sound effect.
	/// </summary>
	public AudioClip starSoundEffect;

	/// <summary>
	/// Win dialog animator.
	/// </summary>
	public Animator WinDialogAnimator;

	/// <summary>
	/// First star fading animator.
	/// </summary>
	public Animator firstStarFading;

	/// <summary>
	/// Second star fading animator.
	/// </summary>
	public Animator secondStarFading;

	/// <summary>
	/// Third star fading animator.
	/// </summary>
	public Animator thirdStarFading;

	/// <summary>
	/// The level title text.
	/// </summary>
	public Text levelTitle;
		
	/// <summary>
	/// The level score text.
	/// </summary>
	public Text levelScore;

	/// <summary>
	/// The level best score text.
	/// </summary>
	public Text levelBestScore;

	/// <summary>
	/// The time text reference.
	/// </summary>
	public Text timeText;

	/// <summary>
	/// The visible flag.
	/// </summary>
	[HideInInspector]
	public bool visible;

	/// <summary>
	/// The effects audio source.
	/// </summary>
	private AudioSource effectsAudioSource;

	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static WinDialog instance;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		}
	}

	// Use this for initialization
	void Start ()
	{
		///Setting up the references
		visible = false;

		if (WinDialogAnimator == null) {
			WinDialogAnimator = GetComponent<Animator> ();
		}

		if (firstStarFading == null) {
			firstStarFading = transform.Find ("Stars").Find ("FirstStarFading").GetComponent<Animator> ();
		}

		if (secondStarFading == null) {
			secondStarFading = transform.Find ("Stars").Find ("SecondStarFading").GetComponent<Animator> ();
		}

		if (thirdStarFading == null) {
			thirdStarFading = transform.Find ("Stars").Find ("ThirdStarFading").GetComponent<Animator> ();
		}

		if (effectsAudioSource == null) {
			effectsAudioSource = GameObject.Find ("AudioSources").GetComponents<AudioSource> () [1];
		}
				
		if (levelTitle == null) {
			levelTitle = transform.Find ("Level").GetComponent<Text> ();
		}

		if (levelScore == null) {
			levelScore = transform.Find ("Score").Find ("Value").GetComponent<Text> ();
		}

		if (levelBestScore == null) {
			levelBestScore = transform.Find ("BestScore").Find ("Value").GetComponent<Text> ();
		}

		if (timeText == null) {
			timeText = transform.Find ("Time").Find ("Value").GetComponent<Text> ();
		}
	}

	/// <summary>
	/// When the GameObject becomes visible
	/// </summary>
	void OnEnable ()
	{
		//Hide the Win Dialog
		Hide ();
	}

	/// <summary>
	/// Animate the score coroutine.
	/// </summary>
	/// <returns>The score coroutine.</returns>
	/// <param name="timeInSecond">Time in second.</param>
	private IEnumerator AnimateScoreCoroutine(int timeInSecond){

		yield return new WaitForSeconds(0.5f);

		float delay = 0.03f;
		int scoreCount = 0;

		//every 2(score period) seconds add 4(points factor) point(s) below
		for (int i = 0; i < Mathf.CeilToInt(timeInSecond * 1.0f/GameManager.instance.scorePeriod); i++) {
			yield return new WaitForSeconds (delay);
			scoreCount += GameManager.instance.pointsFactor;
			SetScore (scoreCount);
			SetTime (timeInSecond - (i+1) * GameManager.instance.scorePeriod);
			AudioClips.instance.PlayCounterSFX ();
		}

		SetTime (0);
	}

	/// <summary>
	/// Show the Win Dialog.
	/// </summary>
	public void Show ()
	{
		if (WinDialogAnimator == null) {
			return;
		}

		StartCoroutine ("AnimateScoreCoroutine",Timer_origin.instance.timeInSeconds);
		BlackArea.Show ();
		WinDialogAnimator.SetTrigger ("Running");
		//AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_WIN_DIALOG);
		visible = true;
	}

	/// <summary>
	/// Hide the Win Dialog.
	/// </summary>
	public void Hide ()
	{
		BlackArea.Hide ();
		StopAllCoroutines ();

		if(WinDialogAnimator.isInitialized)
		WinDialogAnimator.SetBool ("Running", false);

		if(firstStarFading.isInitialized)
		firstStarFading.SetBool ("Running", false);

		if(secondStarFading.isInitialized)
		secondStarFading.SetBool ("Running", false);

		if(thirdStarFading.isInitialized)
		thirdStarFading.SetBool ("Running", false);

		visible = false;
		//AdsManager.instance.HideAdvertisment ();
		GameObject.FindObjectOfType<SceneStartup> ().ShowAd ();
	}

	/// <summary>
	/// Fade stars Coroutine.
	/// </summary>
	/// <returns>The stars.</returns>
	public IEnumerator FadeStars ()
	{
		//set up stars number
		DataManager.LevelData.StarsNumber starsNumber = CalculateStarsNumber();

		float delayBetweenStars = 0.5f;
		if (starsNumber == DataManager.LevelData.StarsNumber.ONE) {//Fade with One Star
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			firstStarFading.SetTrigger ("Running");
		} else if (starsNumber == DataManager.LevelData.StarsNumber.TWO) {//Fade with Two Stars
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			firstStarFading.SetTrigger ("Running");
			yield return new WaitForSeconds (delayBetweenStars);
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			secondStarFading.SetTrigger ("Running");
		} else if (starsNumber == DataManager.LevelData.StarsNumber.THREE) {//Fade with Three Stars
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			firstStarFading.SetTrigger ("Running");
			yield return new WaitForSeconds (delayBetweenStars);
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			secondStarFading.SetTrigger ("Running");
			yield return new WaitForSeconds (delayBetweenStars);
			if (effectsAudioSource != null)
				CommonUtil.PlayOneShotClipAt (starSoundEffect, Vector3.zero, effectsAudioSource.volume);
			thirdStarFading.SetTrigger ("Running");
		}
		yield return 0;
	}

	/// <summary>
	/// Set the level title.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetLevelTitle (string value)
	{
		if (string.IsNullOrEmpty (value) || levelTitle == null) {
			return;
		}
		levelTitle.text = value;
	}

	/// <summary>
	/// Set the score.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetScore (float value)
	{
		if (levelScore == null) {
			return;
		}

		if (value == Mathf.Infinity) {
			levelScore.text = "Score : -";
		} else {
			levelScore.text = "Score : " + value;
		}
	}

	/// <summary>
	/// Set the best score.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetBestScore (float value)
	{
		if (levelBestScore == null) {
			return;
		}

		if (value == Mathf.NegativeInfinity) {
			levelBestScore.text = "Best Score : -";
		} else {
			levelBestScore.text = "Best Score : " + value;
		}
	}

	/// <summary>
	/// Sets the time value.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetTime (float value)
	{
		if (levelScore == null) {
			return;
		}

		if (value == Mathf.Infinity) {
			timeText.text = "Time : -";
		} else {
			timeText.text = "Time : " + value;
		}
	}

	/// <summary>
	/// calculates the stars number using progress script.
	/// </summary>
	/// <returns>The stars number.</returns>
	public DataManager.LevelData.StarsNumber CalculateStarsNumber(){
		
		if (Progress.instance.starsCount == 1) {
			return DataManager.LevelData.StarsNumber.ONE;
		} else if (Progress.instance.starsCount == 2) {
			return DataManager.LevelData.StarsNumber.TWO;
		}else if (Progress.instance.starsCount == 3) {
			return DataManager.LevelData.StarsNumber.THREE;
		}

		return DataManager.LevelData.StarsNumber.ZERO;
	}
}