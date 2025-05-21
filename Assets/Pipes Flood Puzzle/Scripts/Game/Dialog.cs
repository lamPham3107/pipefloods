using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Dialog : MonoBehaviour
{
	/// <summary>
	/// The animator of the dialog.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// The visible flag.
	/// </summary>
	[HideInInspector]
	public bool visible;

	void Start ()
	{
		visible = false;

		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
	}

	/// <summary>
	/// Show the dialog.
	/// </summary>
	public void Show (bool playClickSFX)
	{
		if (playClickSFX)
			AudioClips.instance.PlayButtonClickSFX ();

		BlackArea.Show ();
		animator.SetBool ("Off", false);
		animator.SetTrigger ("On");
		visible = true;
	}

	/// <summary>
	/// Hide the dialog.
	/// </summary>
	public void Hide (bool playClickSFX)
	{
		if (playClickSFX)
			AudioClips.instance.PlayButtonClickSFX ();
		
		BlackArea.Hide ();
		animator.SetBool ("On", false);
		animator.SetTrigger ("Off");
		visible = false;
		//AdsManager.instance.HideAdvertisment ();
		GameObject.FindObjectOfType<SceneStartup> ().ShowAd ();
	}

}