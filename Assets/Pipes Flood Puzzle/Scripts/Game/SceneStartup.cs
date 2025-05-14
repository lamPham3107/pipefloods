using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class SceneStartup : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		ShowAd ();
	}

	public void ShowAd ()
	{
		//if (SceneManager.GetActiveScene ().name == "Main") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_MAIN_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "Options") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_OPTIONS_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "HowToPlay") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_HTP_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "Missions") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_MISSIONS_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "Levels") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_LEVELS_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "Game") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_GAME_SCENE);
		//} else if (SceneManager.GetActiveScene ().name == "About") {
		//	AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_ABOUT_SCENE);
		//}
	}

	void OnDestroy ()
	{
		//AdsManager.instance.HideAdvertisment ();
	}
}
