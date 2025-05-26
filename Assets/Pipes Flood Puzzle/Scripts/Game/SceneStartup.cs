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
        if (SceneManager.GetActiveScene().name == "Main")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Options")
        {
            AdsManager.ins.ShowMrec(true);
        }
        else if (SceneManager.GetActiveScene().name == "HowToPlay")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Missions")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Levels")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
        }
        else if (SceneManager.GetActiveScene().name == "About")
        {
        }
    }

	void OnDestroy ()
	{
        if (SceneManager.GetActiveScene().name == "Main")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Options")
        {
            AdsManager.ins.HideMrec(true);
        }
        else if (SceneManager.GetActiveScene().name == "HowToPlay")
        {
            AdsManager.ins.ShowInterstitial();
        }
        else if (SceneManager.GetActiveScene().name == "Missions")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Levels")
        {
        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
        }
        else if (SceneManager.GetActiveScene().name == "About")
        {
        }
    }
}
