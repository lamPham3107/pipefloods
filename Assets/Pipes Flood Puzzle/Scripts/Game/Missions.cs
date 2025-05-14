using UnityEngine;
using UnityEngine.UI;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class Missions : MonoBehaviour
{

	/// <summary>
	/// Whether to enable the lock feature for or not.
	/// </summary>
	public bool enableLockFeature = true;

	/// <summary>
	/// The last selected group.
	/// </summary>
	private static int lastSelectedGroup;

	// Use this for initialization
	void Awake ()
	{
		DataManager.instance.InitGameData (transform, "OnInitGameDataFinish");
		SetUpMissionsFeatures ();
	}

	private void OnInitGameDataFinish ()
	{

	}

	/// <summary>
	/// Set up missions features.
	/// </summary>
	private void SetUpMissionsFeatures ()
	{
		Mission[] missions = GameObject.FindObjectsOfType<Mission> ();

		Mission mission = null;
		for (int i = 0; i < DataManager.instance.filterdMissionsData.Count; i++) {
			mission = FindMissionById (DataManager.instance.filterdMissionsData [i].ID, missions);
			if (mission == null) {
				continue;//skip next
			}

			if (enableLockFeature) {//Enable the lock feature
				mission.isLocked = DataManager.instance.filterdMissionsData [i].isLocked;
				if (DataManager.instance.filterdMissionsData [i].isLocked) {
					mission.GetComponent<Button> ().interactable = false;
					mission.transform.Find ("Lock").gameObject.GetComponent<Image> ().enabled = true;
					//mission.transform.Find ("Star").gameObject.GetComponent<Image> ().enabled = false;
					//mission.transform.Find ("Score").gameObject.SetActive (false);
				} else {
					mission.transform.Find ("Lock").gameObject.GetComponent<Image> ().enabled = false;
				}

			} else {//Disable the lock feature
				mission.isLocked = false;
				Transform lockGameObject = mission.transform.Find ("Lock");
				if (lockGameObject != null) {
					Image lockImage = lockGameObject.GetComponent<Image> ();
					lockImage.enabled = false;
				}
			}
		}
	}

	/// <summary>
	/// Find the mission component by ID.
	/// </summary>
	/// <returns>The mission component by id.</returns>
	/// <param name="ID">ID of mission.</param>
	/// <param name="missions">Missions Components.</param>
	public Mission FindMissionById (int ID, Mission[] missions)
	{
		if (missions == null) {
			return null;
		}

		foreach (Mission mission in missions) {
			if (mission.ID == ID) {
				return mission;
			}
		}
		return null;
	}
}
