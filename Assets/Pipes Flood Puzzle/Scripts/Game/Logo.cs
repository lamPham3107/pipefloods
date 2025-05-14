using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Logo : MonoBehaviour {

	public float sleepTime = 5;

	// Use this for initialization
	void Start () {
		Invoke ("LoadMainScene", sleepTime);
	}

	private void LoadMainScene(){
		StartCoroutine(SceneLoader.LoadSceneAsync("Main"));
	}
	
}
