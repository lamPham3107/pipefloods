using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[Serializable]
public class AdPackage
{
	public bool isEnabled = true;
	public List<AdEvent> adEvents = new List<AdEvent> ();
	public Package package;

	[Serializable]
	public class AdEvent
	{
		public Event evt;
		public Type type = Type.BANNER;
		#if GOOGLE_MOBILE_ADS
		public GoogleMobileAds.Api.AdPosition adPostion = GoogleMobileAds.Api.AdPosition.Bottom;
		#endif
		public bool isEnabled = false;

		public enum Event
		{
			ON_SHOW_WIN_DIALOG,
			ON_SHOW_PAUSE_DIALOG,
			ON_SHOW_TIMEOUT_DIALOG,
			ON_SHOW_RESET_GAME_DIALOG,
			ON_SHOW_LOCKED_DIALOG,
			ON_SHOW_NEED_TIME_DIALOG,
			ON_SHOW_EXIT_DIALOG,
			ON_INCREASE_TIME,
			ON_LOAD_MAIN_SCENE,
			ON_LOAD_OPTIONS_SCENE,
			ON_LOAD_HTP_SCENE,
			ON_LOAD_MISSIONS_SCENE,
			ON_LOAD_LEVELS_SCENE,
			ON_LOAD_GAME_SCENE,
			ON_LOAD_ABOUT_SCENE,
		}

		public enum Type
		{
			BANNER,
			INTERSTITIAL,
			RewardBasedVideo,
		}
	}

	public enum Package
	{
		ADMOB,
		CHARTBOOST,
		UNITY
	}

	/// <summary>
	/// Build the ad events.
	/// </summary>
	public void BuildAdEvents ()
	{
		Array eventsEnum = Enum.GetValues (typeof(AdEvent.Event));

		if (NeedsToResetAdEventsList (eventsEnum, adEvents)) {
			adEvents.Clear ();
		}

		foreach (AdEvent.Event e in eventsEnum) {
			if (!InAdEventsList (adEvents, e)) {
				adEvents.Add (new AdEvent (){ evt = e });
			}
		}
	}

	/// <summary>
	/// Whether the given event in the adEvents list.
	/// </summary>
	/// <returns><c>true</c>, if evt was found, <c>false</c> otherwise.</returns>
	/// <param name="adEvents">Ad events.</param>
	/// <param name="evt">Evt.</param>
	public bool InAdEventsList (List<AdEvent> adEvents, AdEvent.Event evt)
	{
		if (adEvents == null) {
			return false;
		}

		foreach (AdEvent adEvent in adEvents) {
			if (adEvent.evt == evt) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Whether to reset ad events list or not.
	/// </summary>
	/// <returns><c>true</c>, if reset ad events list was needed, <c>false</c> otherwise.</returns>
	/// <param name="eventsEnum">Events enum.</param>
	/// <param name="adEvents">Ad events.</param>
	public bool NeedsToResetAdEventsList (Array eventsEnum, List<AdEvent> adEvents)
	{
		if (eventsEnum.Length != adEvents.Count) {
			return true;
		}

		return false;
	}
		
}

