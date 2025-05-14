using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class AudioClips : MonoBehaviour
{
	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static AudioClips instance;

	//AudioClips references
	public AudioClip backgroundMusic;
	public AudioClip buttonClickSFX;
	public AudioClip dropStarSFX;
	public AudioClip increaseSFX;
	public AudioClip lockedSFX;
	public AudioClip completedSFX;
	public AudioClip rotateSFX;
	public AudioClip counterSFX;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	void Start ()
	{
		//Play the background music clip on start
		PlayBackgroundMusic ();
	}

	public void PlayBackgroundMusic ()
	{
		AudioSources.instance.MusicAudioSource ().clip = backgroundMusic;
		AudioSources.instance.MusicAudioSource ().Play ();
	}

	public void PlayIncreaseSFX ()
	{
		AudioSources.instance.PlaySFXClip (increaseSFX, false);
	}

	public void PlayDropStarSFX ()
	{
		AudioSources.instance.PlaySFXClip (dropStarSFX, false);
	}

	public void PlayLockedSFX ()
	{
		AudioSources.instance.PlaySFXClip (lockedSFX, false);
	}

	public void PlayCompletedSFX ()
	{
		AudioSources.instance.PlaySFXClip (completedSFX, false);
	}
		
	public void PlayRotateSFX ()
	{
		AudioSources.instance.PlaySFXClip (rotateSFX, false);
	}

	public void PlayCounterSFX(){
		AudioSources.instance.PlaySFXClip (counterSFX, false);
	}

	public void PlayButtonClickSFX ()
	{
		AudioSources.instance.PlaySFXClip (buttonClickSFX, false);
	}
}
