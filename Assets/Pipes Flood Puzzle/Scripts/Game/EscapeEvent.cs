using UnityEngine;
using System.Collections;
using UnityEngine.Events;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

/// Escape or Back event
public class EscapeEvent : MonoBehaviour
{
	/// <summary>
	/// On escape/back event
	/// </summary>
	public UnityEvent escapeEvent;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			OnEscapeClick ();
		}
	}

	/// <summary>
	/// On Escape click event.
	/// </summary>
	public void OnEscapeClick ()
	{
		bool visibleDialogFound = HideVisibleDialogs ();
		if (visibleDialogFound) {
			return;
		}

		escapeEvent.Invoke ();
	}

	/// <summary>
	/// Hide the visible dialogs.
	/// </summary>
	/// <returns><c>true</c>, if visible dialogs was visible, <c>false</c> otherwise.</returns>
	private bool HideVisibleDialogs ()
	{
		bool visibleDialogFound = false;
	
		Dialog[] dialogs = GameObject.FindObjectsOfType<Dialog> ();
		if (dialogs != null) {
			foreach (Dialog d in dialogs) {
				if (d.visible) {
					if (d.name == "PauseDialog") {
						GameManager.instance.Resume ();
					} else {
						d.Hide (true);
					}
					visibleDialogFound = true;
				}
			}
		}
		return visibleDialogFound;
	}
}