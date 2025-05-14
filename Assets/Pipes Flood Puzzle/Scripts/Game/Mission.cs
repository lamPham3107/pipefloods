using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

using System.Collections.Generic;

#pragma warning disable 0168 // variable declared but not used.

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(LevelsManager))]

/// <summary>
/// Important Note : Keep the IDs of Missions in an incremental sequence such as : 1,2,3,4,5,6,7..etc
/// </summary>
public class Mission : MonoBehaviour
{
		/// <summary>
		/// Mission ID.
		/// </summary>
		public int ID = -1;

		/// <summary>
		/// The selected Mission.
		/// </summary>
		public static Mission selectedMission;

		/// <summary>
		/// Number of Rows.
		/// </summary>
		[HideInInspector]
		public int rowsNumber;

		/// <summary>
		/// Number of Columns.
		/// </summary>
		[HideInInspector]
		public int colsNumber;

		[HideInInspector]
		public bool isLocked;

		/// <summary>
		/// The LevelsManager Component.
		/// </summary>
		[HideInInspector]
		public LevelsManager levelsManagerComponent;

		void Awake ()
		{
				InitMission ();
		}

		/// <summary>
		/// Inits the mission.
		/// </summary>
		private void InitMission ()
		{
				//Add onClick event
				GetComponent<Button> ().onClick.AddListener (() => UIEvents.instance.MissionButtonEvent (GetComponent<Mission> ()));

				//Setting up the LevelsManager Component
				levelsManagerComponent = GetComponent<LevelsManager> ();
		
				Debug.Log ("Setting up Mission <b>" + gameObject.name + "</b> of ID " + ID);

				//Get the Number of Rows from LevelsManager Component
				rowsNumber = levelsManagerComponent.numberOfRows;
				//Get the Number of Columns from LevelsManager Component
				colsNumber = levelsManagerComponent.numberOfCols;
		}

		/// <summary>
		/// Print Invalid message name in the console.
		/// </summary>
		private void InvalidNameMessage ()
		{
				Debug.LogError ("Invalid Mission Name , Run the scene again...");
		}

		/// <summary>
		/// Set the stars score of the mission.
		/// </summary>
		public void SetStarsScore ()
		{
				Transform score = transform.Find ("Score");	
				//Set the mission score
				if (score != null) {
						Transform starsCount = score.Find ("StarsCount");
						if (starsCount != null) {
								starsCount.GetComponent<Text> ().text = GetStarsCount (this);
						}
				}
		}
	
		/// <summary>
		/// Get the score count.
		/// </summary>
		/// <returns>The score count.</returns>
		public static string GetStarsCount (Mission mission)
		{
				string result = "";
				try {
						int totalStarsCount = mission.levelsManagerComponent.levels.Count * 3;
						int currentStarsCount = 0;
						DataManager.MissionData missionData = DataManager.FindMissionDataById (mission.ID, DataManager.instance.filterdMissionsData);
			
						List<DataManager.LevelData> levelsData = missionData.levelsData;
						foreach (DataManager.LevelData lvl in levelsData) {
						if (lvl.starsNumber == DataManager.LevelData.StarsNumber.ONE) {
										currentStarsCount += 1;
						} else if (lvl.starsNumber == DataManager.LevelData.StarsNumber.TWO) {
										currentStarsCount += 2;
						} else if (lvl.starsNumber == DataManager.LevelData.StarsNumber.THREE) {
										currentStarsCount += 3;
								}
						}
						result = currentStarsCount + " / " + totalStarsCount;
				} catch (Exception ex) {
						//Catch the Exception
				}
				return result;
		}
}