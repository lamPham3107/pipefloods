using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Options : MonoBehaviour
{
	/// <summary>
	/// The music slider reference.
	/// </summary>
	public Slider musicSlider;
			
	/// <summary>
	/// The sfx slider reference.
	/// </summary>
	public Slider sfxSlider;

	// Use this for initialization
	void Start ()
	{
		SetMusicValue (AudioSources.instance.MusicAudioSource ().volume);
		SetSFXValue (AudioSources.instance.SFXAudioSource ().volume);
	}

	/// <summary>
	/// On music slider change event.
	/// </summary>
	public void OnMusicSliderChange ()
	{
		SetMusicValue (musicSlider.value);
	}

	/// <summary>
	/// On sfx slider change event.
	/// </summary>
	public void OnSFXSliderChange ()
	{
		SetSFXValue (sfxSlider.value);
	}

	/// <summary>
	/// Set the music value.
	/// </summary>
	/// <param name="value">Value.</param>
	private void SetMusicValue (float value)
	{
		AudioSources.instance.MusicAudioSource ().volume = value;
		musicSlider.value = value;
		PlayerPrefs.SetFloat (AudioSources.musicAudioSourceVolumeKey, value);
	}

	/// <summary>
	/// Set the SFX value.
	/// </summary>
	/// <param name="value">Value.</param>
	private void SetSFXValue (float value)
	{
		AudioSources.instance.SFXAudioSource ().volume = value;
		sfxSlider.value = value;
		PlayerPrefs.SetFloat (AudioSources.sfxAudioSourceVolumeKey, value);
	}
}
