using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

/// <summary>
/// Data manager.
/// (Manages saving(writing)/loading(reading) the data of the game)
/// </summary>
[DisallowMultipleComponent]
public class DataManager : MonoBehaviour
{
	/// <summary>
	/// The serilization method for reading and writing.
	/// </summary>
	public SerilizationMethod serilizationMethod = SerilizationMethod.PLAYER_PREFS;

	/// <summary>
	/// The scene missions data.
	/// (will be loaded from the Missions scene)
	/// </summary>
	private List<MissionData> sceneMissionsData;

	/// <summary>
	/// The file missions data.
	/// (will be loaded from the file)
	/// </summary>
	private List<MissionData> fileMissionsData;

	/// <summary>
	/// The filterd missions data.
	/// (The final missions data after filtering)
	/// </summary>
	public List<MissionData> filterdMissionsData;

	/// <summary>
	/// The name of the file without the extension.
	/// </summary>
	public string fileName = "pipesflood";

	/// <summary>
	/// Whether the Missions data is empty or null.
	/// </summary>
	private bool isNullOrEmpty;

	/// <summary>
	/// Whether it's need to save new data.
	/// </summary>
	private bool needsToSaveNewData;

	/// <summary>
	/// This Gameobject defined as a Singleton.
	/// </summary>
	public static DataManager instance;

	/// <summary>
	/// These fields used with PlayerPrefs
	/// </summary>
	private string missionPrefix = "Mission";
	private string levelPrefix = "Level";
	private string seperator = "_";
	public static bool saveAllPlayerPref;
	
	void Awake ()
	{
			if (instance == null) {
					saveAllPlayerPref = false;
					instance = this;
					DontDestroyOnLoad (gameObject);
			} else {
					Destroy (gameObject);
			}
	
			#if UNITY_IPHONE
				//Enable the MONO_REFLECTION_SERIALIZER(For IOS Platform Only)
				System.Environment.SetEnvironmentVariable ("MONO_REFLECTION_SERIALIZER", "yes");
			#endif
	}

	/// <summary>
	/// Inits the game data.
	/// </summary>
	public void InitGameData (Transform callBackHolder,string callBack)
	{
			//Reset flags
			isNullOrEmpty = false;
			needsToSaveNewData = false;
	
			//Load Missions data from the Missions Scene
			sceneMissionsData = LoadMissionsDataFromScene ();
			if (sceneMissionsData == null) {
					return;
			}
	
			if (sceneMissionsData.Count == 0) {
					return;
			}
	
			//Load Missions data from the xml/binary files or player prefs
			fileMissionsData = LoadMissions ();
	
			if (fileMissionsData == null) {
					isNullOrEmpty = true;
			} else {
					if (fileMissionsData.Count == 0) {
							isNullOrEmpty = true;
					}
			}
	
			//If the Missions data in the file or player prefs is null or empty,then save the scene Missions data to the file or player prefs
			if (isNullOrEmpty) {
					if(serilizationMethod == SerilizationMethod.PLAYER_PREFS)
						saveAllPlayerPref = true;
					SaveMissions (sceneMissionsData);
					filterdMissionsData = sceneMissionsData;
			} else {
					//Otherwise get the filtered Missions Data
					filterdMissionsData = GetFilterdMissionsData ();
		
					//If it's need to save a new Missions data to the file or player prefs ,then save it
					if (needsToSaveNewData) {
							if(serilizationMethod == SerilizationMethod.PLAYER_PREFS)
								saveAllPlayerPref = true;
							SaveMissions (filterdMissionsData);
					}
			}

			if (callBackHolder != null && !string.IsNullOrEmpty (callBack)) {
					callBackHolder.SendMessage (callBack);
			}
	}

	//MissionData is class used for persistent loading and saving
	[System.Serializable]
	public class MissionData
	{
			public int ID;//The ID of the Mission
			public bool isLocked = true;//Whether the mission is locked or not
			public List<LevelData> levelsData = new List<LevelData> ();//The levels of the mission
	
			/// <summary>
			/// Find the level data by ID.
			/// </summary>
			/// <returns>The level data.</returns>
			/// <param name="ID">the ID of the level.</param>
			public LevelData FindLevelDataById (int ID)
			{
					foreach (LevelData levelData in levelsData) {
							if (levelData.ID == ID) {
									return levelData;
							}
					}
					return null;
			}
	}

	//LevelData is class used for persistent loading and saving
	[System.Serializable]
	public class LevelData
	{
			public int ID;//The id of the level
			public bool isLocked = true;//Whether the level is locked or not
			public StarsNumber starsNumber = StarsNumber.ZERO;//Number of stars (stars rating)
			public int bestScore = 0;//Best score

			public enum StarsNumber
			{
				ZERO,
				ONE,
				TWO,
				THREE
			}
	}

	/// <summary>
	/// Reset the game data.
	/// </summary>
	public void ResetGameData ()
	{
			if (serilizationMethod == SerilizationMethod.PLAYER_PREFS) {
				ResetGameUsingPlayerPrefs ();
			} else {
				ResetGameUsingSerilization ();
			}

			Debug.Log ("Game Data has been reset successfully");
	}

	/// <summary>
	/// Reset the game data using player prefs.
	/// </summary>
	private void ResetGameUsingPlayerPrefs(){
		PlayerPrefs.DeleteAll ();
	}

	/// <summary>
	/// Reset the game data using serilization.
	/// </summary>
	private void ResetGameUsingSerilization(){

		try {
			fileMissionsData = LoadMissions ();

			if (fileMissionsData == null) {
				return;
			}

			//Reset the levels of each mission
			foreach (MissionData missionData in fileMissionsData) {
				if (missionData == null) {
					continue;
				}

				if (missionData.ID == 1) {
					missionData.isLocked = false;
				} else {
					missionData.isLocked = true;
				}

				foreach (LevelData levelData in missionData.levelsData) {
					if (levelData == null) {
						continue;
					}

					//UnLock the level of ID equals 1(first level) ,otherwise lock the others
					if (levelData.ID == 1) {
						levelData.isLocked = false;
					} else {
						levelData.isLocked = true;
					}

					//Set stars number to zero
					levelData.starsNumber = LevelData.StarsNumber.ZERO;
					//Set best score to 0
					levelData.bestScore = 0;
				}
			}

			SaveMissions (fileMissionsData);
		} catch (Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Load the missions data from the scene.
	/// </summary>
	/// <returns>The missions data from the scene.</returns>
	private List<MissionData> LoadMissionsDataFromScene ()
	{
			Debug.Log ("Loading Missions Data from Scene");
	
			GameObject[] missions = CommonUtil.FindGameObjectsOfTag ("Mission");//Get missions sorted by name
	
			if (missions == null) {
					Debug.Log ("No Mission with 'Mission' tag found");
					return null;
			}
	
			Mission tempMission = null;
			LevelsManager tempLevelManager = null;
	
			List<MissionData> tempMissionsData = new List<MissionData> ();
			MissionData tempMissionData = null;
			for (int i = 0; i < missions.Length; i++) {
					tempMission = missions [i].GetComponent<Mission> ();
					tempLevelManager = missions [i].GetComponent<LevelsManager> ();
					tempMissionData = new MissionData ();
					if (i == 0) {
							tempMissionData.isLocked = false;
					}
					tempMissionData.ID = tempMission.ID;
					tempMissionData.levelsData = GetLevelData (tempLevelManager.levels);
		
					tempMissionsData.Add (tempMissionData);
			}
	
			return tempMissionsData;
	}

	/// <summary>
	/// Get the level data.
	/// </summary>
	/// <returns>The new levels data.</returns>
	/// <param name="levels">The current levels data.</param>
	private List<LevelData> GetLevelData (List<Level> levels)
	{
			if (levels == null) {
					return null;
			}
	
			LevelData tempLevelData = null;
			List<LevelData> tempLevelsData = new List<LevelData> ();
			int ID = 1;
			for (int i = 0; i < levels.Count; i++) {
					tempLevelData = new LevelData ();
					tempLevelData.ID = ID;
					ID++;
					if (i == 0) {
							//First level must be unlocked
							tempLevelData.isLocked = false;
					}
					tempLevelsData.Add (tempLevelData);
			}
	
			return tempLevelsData;
	}

	/// <summary>
	/// Get the filterd missions data.
	/// (Compare the Missions data in the file with the Missions data in the scene)
	/// Scenario :
	/// -you may have a set of missions saved in the file
	/// -if you add/delete a mission using inspector
	/// -then we need to determine and save the new data
	/// </summary>
	/// <returns>The filterd missions data.</returns>
	private List<MissionData> GetFilterdMissionsData ()
	{
			if (fileMissionsData == null || sceneMissionsData == null) {
					return null;
			}
	
			MissionData tempMissionData = null;
			List<MissionData> tempFilteredMissionsData = new List<MissionData> ();
	
			foreach (MissionData missionData in sceneMissionsData) {
		
					//Get the mission data from the file
					tempMissionData = FindMissionDataById (missionData.ID, fileMissionsData);
					if (tempMissionData != null) {
							//If the number of levels in the scene mission Equals to the number of levels in the file mission
							if (missionData.levelsData.Count == tempMissionData.levelsData.Count) {
									//Add tempMissionData(file mission data) to the filtered list
									tempFilteredMissionsData.Add (tempMissionData);
							} else {//Otherwise,its need to save new data
									//TODO:levels data will be lost,when a level is added or removed
									needsToSaveNewData = true;
									//Add the  missionData(scene mission data) to the filtered list 
									tempFilteredMissionsData.Add (missionData);
							}
					} else {//Otherwise,its need to save new data
							needsToSaveNewData = true;
							//Add the missionData(scene mission data) to the filtered list 
							tempFilteredMissionsData.Add (missionData);
					}
			}
			return tempFilteredMissionsData;
	}

	/// <summary>
	/// Save the missions.
	/// </summary>
	/// <param name="missionsData">Missions data.</param>
	public void SaveMissions (List<MissionData> missionsData)
	{
			#if UNITY_ANDROID || UNITY_IPHONE
			if (serilizationMethod == SerilizationMethod.BINARY) {
				FileManager.SaveDataToBinaryFile (missionsData,GetPipeFloodFilePath());
			} else if (serilizationMethod == SerilizationMethod.XML) {
				FileManager.SaveDataToXMLFile (missionsData,GetPipeFloodFilePath());
			} else if (serilizationMethod == SerilizationMethod.PLAYER_PREFS){
				SaveDataToPlayerPrefs(missionsData);
			}
			#elif (UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
			if (serilizationMethod == SerilizationMethod.PLAYER_PREFS)
			{
				SaveDataToPlayerPrefs(missionsData);
			}
			#else
			if (serilizationMethod == SerilizationMethod.BINARY) {
					FileManager.SaveDataToBinaryFile (missionsData, GetPipeFloodFilePath ());
			} else if (serilizationMethod == SerilizationMethod.XML) {
					FileManager.SaveDataToXMLFile (missionsData, GetPipeFloodFilePath ());
			} else if (serilizationMethod == SerilizationMethod.PLAYER_PREFS) {
					SaveDataToPlayerPrefs (missionsData);
			}
			#endif
	}

	/// <summary>
	/// Load the missions.
	/// </summary>
	/// <returns>The missions list.</returns>
	public List<MissionData> LoadMissions ()
	{
			#if UNITY_ANDROID || UNITY_IPHONE
			if (serilizationMethod == SerilizationMethod.BINARY) {
				return	FileManager.LoadDataFromBinaryFile<List<MissionData>> (GetPipeFloodFilePath());
			} else if (serilizationMethod == SerilizationMethod.XML) {
				return	FileManager.LoadDataFromXMLFile<List<MissionData>> (GetPipeFloodFilePath());
			}else if(serilizationMethod == SerilizationMethod.PLAYER_PREFS){
				return LoadDataFromPlayerPrefs();
			}
			#elif (UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
			if (serilizationMethod == SerilizationMethod.PLAYER_PREFS)
			{
				return LoadDataFromPlayerPrefs();
			}
			#else
			if (serilizationMethod == SerilizationMethod.BINARY) {
					return FileManager.LoadDataFromBinaryFile<List<MissionData>> (GetPipeFloodFilePath ());
			} else if (serilizationMethod == SerilizationMethod.XML) {
					return FileManager.LoadDataFromXMLFile<List<MissionData>> (GetPipeFloodFilePath ());
			} else if (serilizationMethod == SerilizationMethod.PLAYER_PREFS) {
					return LoadDataFromPlayerPrefs ();
			}
			#endif
	
			return null;
	}

	/// <summary>
	/// Save the mission locked data  using Player Prefs(locked or not).
	/// </summary>
	/// <param name="missionData">Mission data.</param>
	private void SavePPMissionLockedData(MissionData missionData){
		PlayerPrefs.SetInt (missionPrefix + missionData.ID + seperator + "isLocked", CommonUtil.TrueFalseBoolToZeroOne (missionData.isLocked));
	}

	
	/// <summary>
	/// Save the level locked data using Player Prefs(locked or not).
	/// </summary>
	/// <param name="missionData">Mission data.</param>
	/// <param name="levelData">Level data.</param>
	private void SavePPLevelLockedData(MissionData missionData,LevelData levelData){
		PlayerPrefs.SetInt (missionPrefix + missionData.ID + seperator + levelPrefix + levelData.ID + seperator + "isLocked", CommonUtil.TrueFalseBoolToZeroOne (levelData.isLocked));
	}

	/// <summary>
	/// Save the data to player prefs.
	/// </summary>
	/// <param name="missionsData">Missions data.</param>
	public void SaveDataToPlayerPrefs (List<MissionData> missionsData)
	{
			if (missionsData == null) {
					return;
			}

			Debug.Log ("Saving Data to PlayerPrefs");
	
			foreach (MissionData missionData in missionsData) {
					if (!saveAllPlayerPref) {
						 if (missionData.ID != Mission.selectedMission.ID) {
								if (missionData.ID == Mission.selectedMission.ID + 1) {
									SavePPMissionLockedData (missionData);
								}
								continue;
						  }
					}

					//Store current mission data
					SavePPMissionLockedData (missionData);
			
					foreach (LevelData levelData in missionData.levelsData) {
							if (!saveAllPlayerPref) {
									if (levelData.ID != TableLevel.selectedLevel.ID) {
											if (levelData.ID == TableLevel.selectedLevel.ID + 1) {
												SavePPLevelLockedData (missionData,levelData);
											}
											continue;
									}
							}

							//Store current level data
							SavePPLevelLockedData (missionData,levelData);
							PlayerPrefs.SetInt (missionPrefix + missionData.ID + seperator + levelPrefix + levelData.ID + seperator + "starsNumber", CommonUtil.LevelStarsNumberEnumToIntNumber (levelData.starsNumber));
                            PlayerPrefs.SetInt(missionPrefix + missionData.ID + seperator + levelPrefix + levelData.ID + seperator + "bestScore", levelData.bestScore);
					}
			}
	
			PlayerPrefs.Save ();
	}

	/// <summary>
	/// Load the data from player prefs.
	/// </summary>
	/// <returns>The data from player prefs.</returns>
	public List<MissionData> LoadDataFromPlayerPrefs ()
	{
			Debug.Log ("Loading Data from PlayerPrefs");
	
			foreach (MissionData sceneMissionData in sceneMissionsData) {
					//Load current mission data
					string key = "";
		
					//isLocked key for the current mission
					key = missionPrefix + sceneMissionData.ID + seperator + "isLocked";
					if (PlayerPrefs.HasKey (key)) {
							sceneMissionData.isLocked = CommonUtil.ZeroOneToTrueFalseBool (PlayerPrefs.GetInt (key));
					}
		
					foreach (LevelData levelData in sceneMissionData.levelsData) {
							//Load current level data
			
							//isLocked key for the current level
							key = missionPrefix + sceneMissionData.ID + seperator + levelPrefix + levelData.ID + seperator + "isLocked";
							if (PlayerPrefs.HasKey (key)) {
									levelData.isLocked = CommonUtil.ZeroOneToTrueFalseBool (PlayerPrefs.GetInt (key));
							}
			
							//starsNumber key for the current level
							key = missionPrefix + sceneMissionData.ID + seperator + levelPrefix + levelData.ID + seperator + "starsNumber";
							if (PlayerPrefs.HasKey (key)) {
									levelData.starsNumber = CommonUtil.IntNumberToLevelStarsNumberEnum (PlayerPrefs.GetInt (key));
							}

                            //bestScore key for the current level
                            key = missionPrefix + sceneMissionData.ID + seperator + levelPrefix + levelData.ID + seperator + "bestScore";
                            if (PlayerPrefs.HasKey(key))
                            {
                                levelData.bestScore = PlayerPrefs.GetInt(key);
                            }
					}
			}
	
			return sceneMissionsData;
	}

	/// <summary>
	/// Finds the mission data by id.
	/// </summary>
	/// <returns>The mission data by ID.</returns>
	/// <param name="ID">the ID of the mission.</param>
	/// <param name="missionsData">Missions data list to search in.</param>
	public static MissionData FindMissionDataById (int ID, List<MissionData> missionsData)
	{
			if (missionsData == null) {
					return null;
			}
	
			foreach (MissionData missionData in missionsData) {
					if (missionData.ID == ID) {
							return missionData;
					}
			}
	
			return null;
	}


	/// <summary>
	/// Settings up the path of the file ,relative to the current platform.
	/// </summary>
	private string GetPipeFloodFilePath ()
	{
			string fileExtension = "";
			string filePath = "";
	
			#if UNITY_ANDROID
			//Get Android Path
			filePath = FileManager.GetAndroidFileFolder();
			if (serilizationMethod == SerilizationMethod.BINARY) {
				fileExtension = ".bin";
			} else if (serilizationMethod == SerilizationMethod.XML) {
				fileExtension = ".xml";
			}	
			#elif UNITY_IPHONE
			//Get iPhone Documents Path
			filePath = FileManager.GetIPhoneFileFolder();
			if (serilizationMethod == SerilizationMethod.BINARY) {
				fileExtension = ".bin";
			} else if (serilizationMethod == SerilizationMethod.XML) {
				fileExtension = ".xml";
			}
			#elif !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL)
			//Others
			filePath = FileManager.GetOthersFileFolder ();
			if (serilizationMethod == SerilizationMethod.BINARY) {
					fileExtension = ".bin";
			} else if (serilizationMethod == SerilizationMethod.XML) {
					fileExtension = ".xml";
			}
			#endif
			filePath += "/" + fileName + fileExtension;
	
			return filePath;
	}

	public enum SerilizationMethod
	{
		#if (UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL)
           PLAYER_PREFS
		#else
        	PLAYER_PREFS,
			BINARY,
			XML
		#endif
    };
}