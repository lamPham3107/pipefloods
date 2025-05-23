using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

[DisallowMultipleComponent]
public class LevelsTable : MonoBehaviour
{
		/// <summary>
		/// Whether to create groups pointers or not.
		/// </summary>
		public bool createGroupsPointers = true;

		/// <summary>
		/// The levels list.
		/// </summary>
		public static List<TableLevel> levels;

		/// <summary>
		/// The groups parent.
		/// </summary>
		public Transform groupsParent;

		/// <summary>
		/// The pointers parent.
		/// </summary>
		public Transform pointersParent;

		/// <summary>
		/// The level bright.
		/// </summary>
		public Transform levelBright;

		/// <summary>
		/// The star on sprite.
		/// </summary>
		public Sprite starOn;

		/// <summary>
		/// The star off sprite.
		/// </summary>
		public Sprite starOff;

		/// <summary>
		/// The level prefab.
		/// </summary>
		public GameObject levelPrefab;
	
		/// <summary>
		/// The levels group prefab.
		/// </summary>
		public GameObject levelsGroupPrefab;

		/// <summary>
		/// The pointer prefab.
		/// </summary>
		public GameObject pointerPrefab;

		/// <summary>
		/// temporary transform.
		/// </summary>
		private Transform tempTransform;

		/// <summary>
		/// temporary mission data.
		/// </summary>
		private DataManager.MissionData tempMissionData;

		/// <summary>
		/// The Number of levels per group.
		/// </summary>
		[Range (1, 100)]
		public int levelsPerGroup = 12;

		/// <summary>
		/// Number of columns per group.
		/// </summary>
		[Range (1, 10)]
		public int columnsPerGroup = 3;

		/// <summary>
		/// The last level that user reached.
		/// </summary>
		private Transform lastLevel;

		void Start ()
		{	
				StartCoroutine (Init ());
		}

		void Update ()
		{
				if (lastLevel != null && levelBright!=null) {
						//Set the bright postion to the last level postion
						if (!Mathf.Approximately (lastLevel.position.magnitude, levelBright.position.magnitude)) {
								levelBright.position = lastLevel.position;
						}
				}
		}

		/// <summary>
		/// Set the stars count.
		/// </summary>
		private void SetStarsScore ()
		{
				GameObject score = GameObject.Find ("Score");	
				//Set the mission score
				if (score != null) {
						Transform starsCount = score.transform.Find ("StarsCount");
						if (starsCount != null) {
								starsCount.GetComponent<Text> ().text = Mission.GetStarsCount (Mission.selectedMission);
						}
				}
		}

		private IEnumerator Init ()
		{
				//define the levels list
				levels = new List<TableLevel> ();

				//Set Stars Score
				SetStarsScore ();

				//Show Loading Panel
				ShowLoadingPanel ();

				//Create new levels
				yield return StartCoroutine (CreateLevels ());

				ScrollSlider.instance.Init ();

				//Hide Loading Panel
				HideLoadingPanel ();
		}

		/// <summary>
		/// Creates the levels in Groups.
		/// </summary>
		private IEnumerator CreateLevels ()
		{
				yield return 0;
						
				//Clear current levels list
				levels.Clear ();

				//Get LevelsManager Component from the wanted (selected) Mission
				LevelsManager levelsManagerComponent = Mission.selectedMission.levelsManagerComponent;

				TableLevel tableLevelComponent = null;
				GameObject tableLevelGameObject = null;
		 
				//The ID of the level
				int ID = 0;
			
				GameObject levelsGroup = null;
			
				//Group index
				int groupIndex = 0;

				//Create Levels inside groups
				for (int i = 0; i < levelsManagerComponent.levels.Count; i++) {

						if (i % levelsPerGroup == 0) {
								groupIndex = (i / levelsPerGroup);
								levelsGroup = Group.CreateGroup (levelsGroupPrefab, groupsParent, groupIndex, columnsPerGroup);
								if (createGroupsPointers) {
										Pointer.CreatePointer (groupIndex, levelsGroup, pointerPrefab, pointersParent);
								}
						}

						//Create Level
						ID = (i + 1);//the id of the level
						tableLevelGameObject = Instantiate (levelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						tableLevelGameObject.transform.SetParent (levelsGroup.transform);//setting up the level's parent
						tableLevelComponent = tableLevelGameObject.GetComponent<TableLevel> ();//get TableLevel Component
						tableLevelComponent.ID = ID;//setting up level ID
						tableLevelGameObject.name = "Level-" + ID;//level name
						tableLevelGameObject.transform.localScale = Vector3.one;

						SettingUpLevel (tableLevelComponent,groupIndex);//setting up the level contents (stars number ,islocked,...)
						levels.Add (tableLevelComponent);//add table level component to the list
				}



				if (levelsManagerComponent.levels.Count == 0) {
						Debug.Log ("There are no Levels in this Mission");
				} else {
						Debug.Log ("New levels have been created");
				}
		}


		/// <summary>
		/// Settings up the level contents in the table.
		/// </summary>
		/// <param name="tableLevel">Table level.</param>
		/// <param name="ID">ID of the level.</param>
		private void SettingUpLevel (TableLevel tableLevel,int groupIndex)
		{
				if (tableLevel == null) {
						return;
				}

				//Get Mission Data of the current Mission
				tempMissionData = DataManager.FindMissionDataById (Mission.selectedMission.ID, DataManager.instance.filterdMissionsData);
				if (tempMissionData == null) {
						Debug.Log ("Null MissionData");
						return;
				}

				//Find the Level Data of the given tableLevel
				DataManager.LevelData levelData = tempMissionData.FindLevelDataById (tableLevel.ID);

				//Find the next Level Data of the given tableLevel
				DataManager.LevelData nextLevelData = tempMissionData.FindLevelDataById (tableLevel.ID + 1);

				levelData = tempMissionData.FindLevelDataById (tableLevel.ID);
				if (levelData == null) {
						Debug.Log ("Null LevelData");
						return;
				}

				//Set isLocked flag
				tableLevel.isLocked = levelData.isLocked;

				//Set level click event
				tableLevel.GetComponent<Button> ().onClick.AddListener (() => UIEvents.instance.LevelButtonEvent (tableLevel));

				//If the level is locked then , skip the next
				if (levelData.isLocked) {
						return;
				}
             
				//Setting up the level's bright and the selected group
				if (nextLevelData != null) {
						if ((nextLevelData.isLocked && !levelData.isLocked)) {
								SetLastLevel (tableLevel.transform);
								SetSelectedGroup (groupIndex);
						}
				} else if ((!levelData.isLocked && tableLevel.ID == Mission.selectedMission.levelsManagerComponent.levels.Count)) {
						//if (levelData.starsNumber == TableLevel.StarsNumber.ZERO) {
								SetLastLevel (tableLevel.transform);
						//}
						SetSelectedGroup (groupIndex);
				}

				//Make the button interactable
				//tableLevel.GetComponent<Button> ().interactable = true;

				//Show the stars of the level
				tableLevel.transform.Find ("Stars").gameObject.SetActive (true);

				//Hide the lock
				tableLevel.transform.Find ("Lock").gameObject.SetActive (false);

				//Show the title of the level
				tableLevel.transform.Find ("LevelTitle").gameObject.SetActive (true);

				//Setting up the level title
				tableLevel.transform.Find ("LevelTitle").GetComponent<Text> ().text = tableLevel.ID.ToString ();

				//Get stars Number from current Level Data
				tableLevel.starsNumber = levelData.starsNumber;
				tempTransform = tableLevel.transform.Find ("Stars");

				//Apply the current Stars Rating 
				if (levelData.starsNumber == DataManager.LevelData.StarsNumber.ONE) {//One Star
						tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
						tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOff;
						tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
				} else if (levelData.starsNumber == DataManager.LevelData.StarsNumber.TWO) {//Two Stars
						tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
						tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOn;
						tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
				} else if (levelData.starsNumber == DataManager.LevelData.StarsNumber.THREE) {//Three Stars
						tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
						tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOn;
						tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOn;
				} else {//Zero Stars
						tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOff;
						tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOff;
						tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
				}
		}

		/// <summary>
		/// Set the last level.
		/// </summary>
		/// <param name="lastLevel">Last level.</param>
		private void SetLastLevel(Transform lastLevel){
				this.lastLevel = lastLevel;
				if (levelBright != null) {
					levelBright.position = lastLevel.position;
					levelBright.GetComponent<Image> ().enabled = true;
					levelBright.GetComponent<Animator> ().SetTrigger ("isRunning");
				}
		}

		/// <summary>
		/// Set the selected group.
		/// </summary>
		/// <param name="groupIndex">Group index.</param>
		private void SetSelectedGroup(int groupIndex){
				//Setup the last selected group index
				ScrollSlider.instance.currentGroupIndex = groupIndex;
		}

		/// <summary>
		/// Show the loading panel.
		/// </summary>
		public void ShowLoadingPanel ()
		{
				if (Mission.selectedMission.levelsManagerComponent.levels.Count > 200) {
						GameObject loading = GameObject.Find ("Loading");
						if (loading != null) {
								loading.transform.GetChild (0).gameObject.SetActive (true);
						}
				}
		}

		/// <summary>
		/// Hide the loading panel.
		/// </summary>
		public void HideLoadingPanel ()
		{
				if (Mission.selectedMission.levelsManagerComponent.levels.Count > 200) {
						GameObject loading = GameObject.Find ("Loading");
						if (loading != null) {
								loading.transform.GetChild (0).gameObject.SetActive (false);
						}
				}
		}
}