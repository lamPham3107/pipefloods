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

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
	/// <summary>
	/// The grid cell prefab.
	/// </summary>
	public GameObject gridCellPrefab;

	/// <summary>
	/// The cell content prefab.
	/// </summary>
	public GameObject cellContentPrefab;

	/// <summary>
	/// The grid.
	/// </summary>
	public GameObject grid;

	/// <summary>
	/// The wheel prefab.
	/// </summary>
	public GameObject wheelPrefab;

	/// <summary>
	/// The grid cells rect transform.
	/// </summary>
	public RectTransform gridCellsRectTransform;

	/// <summary>
	/// The grid cells transform.
	/// </summary>
	public Transform gridCellsTransform;

	/// <summary>
	/// The grid container reference.
	/// </summary>
	public RectTransform gridContainer;

	/// <summary>
	/// The level text.
	/// </summary>
	public Text levelText;

	/// <summary>
	/// The moves text.
	/// </summary>
	public Text movesText;

	/// <summary>
	/// The increase time dialog reference.
	/// </summary>
	public Dialog increaseTimeDialog;

	/// <summary>
	/// The no connection dialog reference.
	/// </summary>
	public Dialog noConnectionDialog;

	/// <summary>
	/// The grid cells in the grid.
	/// </summary>
	[HideInInspector]
	public GridCell[] gridCells;

	/// <summary>
	/// The number of rows of the grid.
	/// </summary>
	[HideInInspector]
	public int numberOfRows;

	/// <summary>
	/// The number of columns of the grid.
	/// </summary>
	[HideInInspector]
	public int numberOfColumns;

	/// <summary>
	/// The rotate hint animator.
	/// </summary>
	public Animator rotateHint;

	/// <summary>
	/// The completed animator.
	/// </summary>
	public Animator completedTextAnimator;

	/// <summary>
	/// The current level.
	/// </summary>
	[HideInInspector]
	public Level currentLevel;

	/// <summary>
	/// The current title of the current level.
	/// </summary>
	private string currentLevelTitle;

	/// <summary>
	/// The current level score.
	/// </summary>
	private int currentLevelScore;

	/// <summary>
	/// The number of moves.
	/// </summary>
	private int moves;

	/// <summary>
	/// The points factor(used to calculate the score).
	/// </summary>
	[HideInInspector]
	public int pointsFactor = 4;

	/// <summary>
	/// The score period(used to calculate the score).
	/// </summary>
	[HideInInspector]
	public int scorePeriod = 2;

	/// <summary>
	/// The size of the grid.
	/// </summary>
	private Vector2 gridSize;

	/// <summary>
	/// The current mission data.
	/// </summary>
	private DataManager.MissionData currentMissionData;

	/// <summary>
	/// The current level data.
	/// </summary>
	private DataManager.LevelData currentLevelData;

	/// <summary>
	/// Whether the GameLoop is running or not.
	/// </summary>
	[HideInInspector]
	public bool isRunning;

	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static GameManager instance;

	public static bool isShowingRateDialog = false;
	public static int levelShowRate = 5;
    void Awake(){
		if (instance == null) {
			instance = this;
		}
	}

	void Start ()
	{
		//Setting up the references

		if (grid == null) {
			grid = GameObject.Find ("Grid");
		}

		if (gridCellsTransform == null) {
			gridCellsTransform = grid.transform.Find ("GridCells").transform;
		}

		if (gridCellsRectTransform == null) {
			gridCellsRectTransform = gridCellsTransform.GetComponent<RectTransform> ();
		}



        try {
			//Setting up Attributes
			numberOfRows = Mission.selectedMission.rowsNumber;
			numberOfColumns = Mission.selectedMission.colsNumber;
			grid.name = numberOfRows + "x" + numberOfColumns + "-Grid";
			//get current Mission data
			currentMissionData = DataManager.FindMissionDataById (Mission.selectedMission.ID, DataManager.instance.filterdMissionsData);
		} catch (Exception ex) {
			Debug.Log (ex.Message);
		}

        //Create New level (the selected level)
        CreateNewLevel();

        //Check whether the current level is completed or not
        CheckLevelComplete ();
	}

	/// <summary>
	/// When the GameObject becomes invisible.
	/// </summary>
	void OnDisable ()
	{
		//stop the timer
		Timer_origin.instance.Stop ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameLoop ();
	}

	/// <summary>
	/// The Game's loop.
	/// </summary>
	private void GameLoop ()
	{
		if (!isRunning) {
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
			RayCast (Input.mousePosition);
		} 
	}

	/// <summary>
	/// Raycast the click (touch) on the screen.
	/// </summary>
	/// <param name="clickPosition">The position of the click (touch).</param>
	/// <param name="clickType">The type of the click(touch).</param>
	private void RayCast (Vector3 clickPosition)
	{
		Vector3 tempClickPosition = Camera.main.ScreenToWorldPoint (clickPosition);
		RaycastHit2D tempRayCastHit2D = Physics2D.Raycast (tempClickPosition, Vector2.zero);
		Collider2D tempCollider2D = tempRayCastHit2D.collider;
			
		if (tempCollider2D != null) {
			//When a ray hit a grid cell
			if (tempCollider2D.tag == "GridCell") {
				OnPipeClick (tempCollider2D.GetComponent<GridCell> ().pipe);
			}
		}
	}

	/// <summary>
	/// On pipe click.
	/// </summary>
	/// <param name="pipe">Pipe reference.</param>
	private void OnPipeClick(Pipe pipe){
		
		if (pipe == null) {
			return;
		}

		AudioClips.instance.PlayRotateSFX();

		IncreaseMoves ();
		pipe.DelayedRotation();
		CheckLevelComplete();
	}

	/// <summary>
	/// Create a new level.
	/// </summary>
	private void CreateNewLevel ()
	{
		try {
			currentLevelScore = 0;
			currentLevelTitle = "Level " + TableLevel.selectedLevel.ID;
			SetLevelTitleText();
			currentLevel = Mission.selectedMission.levelsManagerComponent.levels [TableLevel.selectedLevel.ID - 1];
			currentLevelData = currentMissionData.FindLevelDataById (TableLevel.selectedLevel.ID);
            ActiveRateDialog();
            if (!isShowingRateDialog)
            {
                BuildTheGrid();
            }
            else
            {
                RateDialog.instance.onCloseRateDialog = () =>
                {
                    BuildTheGrid();
                };
            }

            //AdsManager.instance.HideAdvertisment ();
        } catch (Exception ex) {
			Debug.LogWarning ("Make sure you have selected a level, and there are no empty references in GameManager component");
		}
	}

	/// <summary>
	/// Build the grid.
	/// </summary>
	private void BuildTheGrid ()
	{

        Debug.Log("Setting up the Grid " + numberOfRows + "x" + numberOfColumns);
        //Calculate the cell size 
        float cellSize = Math.Min (gridCellsRectTransform.rect.width * 1.0f / numberOfColumns, gridCellsRectTransform.rect.height * 1.0f / numberOfRows);
			
		//Calculate the horizontal margin of the grid
		Vector3 tempPos = gridCellsRectTransform.anchoredPosition;
		float xMargin =  gridCellsRectTransform.rect.width - numberOfColumns * cellSize;
		tempPos.x += xMargin / 2.0f;

		//Calculate the vertical margin of the grid
		float yMargin =  gridCellsRectTransform.rect.height - numberOfRows * cellSize;
		tempPos.y -= yMargin / 2.0f;
		gridCellsRectTransform.anchoredPosition = tempPos;

		//Set the grid cell size (width,height)
		Vector2 gridCellSize = new Vector2 (cellSize, cellSize);

		//Set the grid cell local position
		Vector3 gridCellPosition = Vector3.zero;

		gridCells = new GridCell[numberOfRows * numberOfColumns];
		Vector2 cellContentsSize;
		GameObject gridCell, gridCellCover;
		GridCell gridCellComponent;
		RectTransform gridCellRectTransform, cellContentRectTransform,starRectTransform;
		int gridCellIndex;
		float x, y;//grid cell x,y offset

		for (int i = 0; i < numberOfRows; i++) {
			for (int j = 0; j < numberOfColumns; j++) {
				
				//Calculate grid cell index
				gridCellIndex = i * numberOfColumns + j;
				
				//Create new grid cell
				gridCell = Instantiate (gridCellPrefab) as GameObject;

				//Get the Rect Transfrom
				gridCellRectTransform = gridCell.GetComponent<RectTransform> ();

				if(currentLevel.cells[gridCellIndex].enableBackground){
					//Set the background of the grid cell
					SetGridCellBackground (gridCell, i, j);
				}else{
					gridCell.GetComponent<Image> ().enabled = false;
				}

				//Name the new grid cell
				gridCell.name = "GridCell-" + gridCellIndex;
				//Set the new grid cell parent
				gridCell.transform.SetParent (gridCellsTransform);
				//Set the scale of the new grid cell to one
				gridCell.transform.localScale = Vector3.one;
				//Set the position of the grid cell
				gridCell.transform.localPosition = gridCellPosition;
				//Set the collider size for the new grid cell
				gridCell.GetComponent<BoxCollider2D> ().size = gridCellSize;

				//Move and size the new grid cell
				x = -gridCellsRectTransform.rect.width / 2 + gridCellSize.x * j;
				y = gridCellsRectTransform.rect.height / 2 - gridCellSize.y * (i + 1);
				gridCellRectTransform.offsetMin = new Vector2 (x, y);
				
				x = gridCellRectTransform.offsetMin.x + gridCellSize.x;
				y = gridCellRectTransform.offsetMin.y + gridCellSize.y;
				gridCellRectTransform.offsetMax = new Vector2 (x, y);

				//Get the GridCell component
				gridCellComponent = gridCell.GetComponent<GridCell> ();

				//Set the grid cell index
				gridCellComponent.index = gridCellIndex;
				gridCells [gridCellIndex] = gridCellComponent;

				PipesManager.Pipe pipe = GridUtil.GetPipeByIdentifier (currentLevel.cells [gridCellIndex].pipeIdentifier);

				if (pipe != null) {
					//Create new pipe for the grid cell
					GameObject cellContent = Instantiate (cellContentPrefab) as GameObject;
					cellContent.transform.SetParent (gridCellRectTransform);
					cellContent.name = "Pipe";

					//Move and size the cell content/pipe
					cellContent.transform.localPosition = gridCellPosition;
				
					cellContent.transform.localScale = Vector3.one;
					cellContentRectTransform = cellContent.GetComponent<RectTransform> ();

					cellContentsSize.x = gridCellRectTransform.rect.width;
					cellContentsSize.y = gridCellRectTransform.rect.height;
					
					cellContentRectTransform.offsetMin = Vector2.zero;
					cellContentRectTransform.offsetMax = Vector3.zero;

					gridCellComponent.pipe = cellContent.GetComponent<Pipe> ();
					gridCellComponent.pipe.Init ();

					gridCellComponent.pipe.leftPath.enabled = pipe.leftPath.enabled;
					gridCellComponent.pipe.topPath.enabled = pipe.topPath.enabled;
					gridCellComponent.pipe.rightPath.enabled = pipe.rightPath.enabled;
					gridCellComponent.pipe.bottomPath.enabled = pipe.bottomPath.enabled;
				
					gridCellComponent.pipe.normalSprite = pipe.sprite;
					gridCellComponent.pipe.connectSprite = pipe.connectSprite;

					if (pipe.sprite != null) {
						cellContent.GetComponent<Image> ().sprite = pipe.sprite;
					} else {
						cellContent.GetComponent<Image> ().enabled = false;
					}

					//Rotate the pipe immediately on build
					//gridCellComponent.pipe.ImmediateRotation();
				}
			}
		}

		foreach (Level.Path levelPath in currentLevel.paths) {
			//Create wheels for source and destination pipes
			CreateWheel (levelPath.sourceIndex,true,true);
			CreateWheel (levelPath.destinationIndex,false,false);

			//Set the size and the position for rotate hint
			rotateHint.GetComponent<RectTransform> ().sizeDelta = gridCellSize;
			rotateHint.transform.position = gridCells[levelPath.sourceIndex].transform.position;

			//Toggle the rotate hit
			ToggleRotateHint ();

			//Hide the rotate hint after 2 seconds
			Invoke ("ToggleRotateHint", 2);
			//break;
		}
	}

	/// <summary>
	/// Create new wheel above the pipe.
	/// </summary>
	/// <param name="index">Index of the cell.</param>
	/// <param name="setConnected">If set to <c>true</c> set connected sprite.</param>
	/// <param name="lockSprite">If set to <c>true</c> lock the sprite.</param>
	private void CreateWheel(int index,bool setConnected,bool lockSprite){
		gridCells [index].DisableCollider ();
		if(setConnected)
			gridCells [index].pipe.SetConnectSprite ();

		if(lockSprite)
		gridCells [index].pipe.lockSprite = true;
		
		GameObject wheel = Instantiate (wheelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		wheel.transform.SetParent (gridCells [index].transform);
		wheel.transform.localScale = Vector3.one;

		RectTransform wheelRectT = wheel.GetComponent<RectTransform> ();
		wheelRectT.sizeDelta = gridCells [index ].GetComponent<RectTransform> ().sizeDelta;
		wheelRectT.offsetMax = Vector2.zero;
		wheelRectT.offsetMin = Vector2.zero;
		wheelRectT.anchoredPosition3D = Vector3.zero;
	}

	/// <summary>
	/// Set the background image for the given grid cell .
	/// </summary>
	/// <param name="gridCell">Grid cell.</param>
	/// <param name="i">The row's index.</param>
	/// <param name="j">The column's index</param>
	private void SetGridCellBackground (GameObject gridCell, int i, int j)
	{
		if (gridCell == null) {
			return;
		}

		if (i % 2 == 0) {
			if (j % 2 == 0) {
				gridCell.GetComponent<Image> ().sprite = Mission.selectedMission.levelsManagerComponent.firstGridCellBackground;
			} else {
				gridCell.GetComponent<Image> ().sprite = Mission.selectedMission.levelsManagerComponent.secondGridCellBackground;
			}
		} else {
			if (j % 2 == 0) {
				gridCell.GetComponent<Image> ().sprite = Mission.selectedMission.levelsManagerComponent.secondGridCellBackground;
			} else {
				gridCell.GetComponent<Image> ().sprite = Mission.selectedMission.levelsManagerComponent.firstGridCellBackground;
			}
		}
	}

	/// <summary>
	/// Go to the next level.
	/// </summary>
	public void NextLevel ()
	{
		if (TableLevel.selectedLevel.ID >= 1 && TableLevel.selectedLevel.ID < LevelsTable.levels.Count) {
			//Get the next level and check if it's locked , then do not load the next level
			DataManager.MissionData currentMissionData = DataManager.FindMissionDataById (Mission.selectedMission.ID, DataManager.instance.filterdMissionsData);//Get the current mission
			if (TableLevel.selectedLevel.ID + 1 <= currentMissionData.levelsData.Count) {
				DataManager.LevelData nextLevelData = currentMissionData.FindLevelDataById (TableLevel.selectedLevel.ID + 1);///Get the next level
				if (nextLevelData.isLocked) {
					//Play locked sound effect
					AudioClips.instance.PlayLockedSFX();
					//Skip the next
					return;
				}
			}
			TableLevel.selectedLevel = LevelsTable.levels [TableLevel.selectedLevel.ID];//Set the selected level
            UIEvents.instance.LoadGameScene();

		} else {
			//Play lock sound effectd
			AudioClips.instance.PlayLockedSFX();
		}
	}

	//// <summary>
	/// Back to the previous level.
	/// </summary>
	public void PreviousLevel ()
	{
		if (TableLevel.selectedLevel.ID > 1 && TableLevel.selectedLevel.ID <= LevelsTable.levels.Count) {
			TableLevel.selectedLevel = LevelsTable.levels [TableLevel.selectedLevel.ID - 2];
			UIEvents.instance.LoadGameScene();
		} else {
			//Play lock sound effectd
			AudioClips.instance.PlayLockedSFX();
		}
	}

	/// <summary>
	/// Check Wheter the level is completed or not.
	/// </summary>
	private void CheckLevelComplete ()
	{
		bool completed = true;

		for (int i = 0; i < gridCells.Length; i++) {
			if (gridCells [i].pipe != null)
				gridCells [i].pipe.SetNormalSprite ();
		}

		if (currentLevel.paths.Count == 0) {
			completed = false;
			Debug.LogError ("You need to define at least one path in level" + TableLevel.selectedLevel.ID);
		}

		foreach(Level.Path levelPath in currentLevel.paths){

			List<int> path = GridUtil.GetGridPath (GridUtil.FindPath (gridCells,levelPath.sourceIndex, levelPath.destinationIndex, Mission.selectedMission.rowsNumber, Mission.selectedMission.colsNumber,true), levelPath.sourceIndex, levelPath.destinationIndex);

			if (path.Count == 0) {
				completed = false;
				continue;
			}
		}

		if (completed) {
			
			isRunning = false;

			SaveDataOnLevelComplete ();

			//Play completed text aniamtor
			completedTextAnimator.SetTrigger ("Toggle");

			//pause the timer
			Timer_origin.instance.Pause ();

			//Play level completed sound effect
			AudioClips.instance.PlayCompletedSFX ();

			//Show black area
			BlackArea.Show ();

			//Setup the attributes of the win dialog
			WinDialog.instance.SetLevelTitle (currentLevelTitle);
			WinDialog.instance.SetTime (Timer_origin.instance.timeInSeconds);
			WinDialog.instance.SetBestScore (currentLevelData.bestScore);


            //Show win dialog
            WinDialog.instance.Invoke("Show", 2);
            Debug.Log("Level completed");



        }
	}

	/// <summary>
	/// Save the data on level complete.
	/// </summary>
	private void SaveDataOnLevelComplete(){
		try {
			if (currentLevelData.ID == currentMissionData.levelsData.Count) {
				if (currentMissionData.ID + 1 <= DataManager.instance.filterdMissionsData.Count) {
					//Unlock the next mission
					DataManager.MissionData nextMissionData = DataManager.FindMissionDataById (currentMissionData.ID + 1, DataManager.instance.filterdMissionsData);
					nextMissionData.isLocked = false;
				}
			}

			if (currentLevelData.ID == currentMissionData.levelsData.Count) {
				if (currentMissionData.ID + 1 <= DataManager.instance.filterdMissionsData.Count) {
					//Unlock the next mission
					DataManager.MissionData nextMissionData = DataManager.FindMissionDataById (currentMissionData.ID + 1, DataManager.instance.filterdMissionsData);
					nextMissionData.isLocked = false;
				}
			}

			//Calcualte current score
			currentLevelScore = Mathf.CeilToInt(Timer_origin.instance.timeInSeconds * 1.0f/scorePeriod) * pointsFactor;

			//Set the stars of the level
			currentLevelData.starsNumber = WinDialog.instance.CalculateStarsNumber();

			//Set the best score of the level
			if (currentLevelData.bestScore < currentLevelScore) {
				currentLevelData.bestScore = currentLevelScore;
			}
		
			TableLevel.selectedLevel.starsNumber = currentLevelData.starsNumber;
			if (currentLevelData.ID + 1 <= currentMissionData.levelsData.Count) {
				//Unlock the next level
				DataManager.LevelData nextLevelData = currentMissionData.FindLevelDataById (TableLevel.selectedLevel.ID + 1);
				nextLevelData.isLocked = false;
			}

			//Save current Mission & Level data only using Player Prefs
			if (DataManager.instance.serilizationMethod == DataManager.SerilizationMethod.PLAYER_PREFS)
				DataManager.saveAllPlayerPref = false;

			DataManager.instance.SaveMissions (DataManager.instance.filterdMissionsData);
		} catch (Exception ex) {
			Debug.LogError (ex.Message);
		}
	}


	/// <summary>
	/// Pause the game.
	/// </summary>
	public void Pause(){
		if (!isRunning) {
			return;
		}

		//AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_PAUSE_DIALOG);
		Timer_origin.instance.Pause ();
		isRunning = false;
		GameObject.Find ("PauseDialog").GetComponent<Dialog> ().Show (true);
	}

	/// <summary>
	/// Pause the gamse.
	/// </summary>
	public void Resume(){
		Timer_origin.instance.Resume ();
		GameObject.Find ("PauseDialog").GetComponent<Dialog> ().Hide (true);
		isRunning = true;
	}

	/// <summary>
	/// Set the level title text.
	/// </summary>
	private void SetLevelTitleText(){
		if(levelText == null)
			return;
		
		levelText.text = currentLevelTitle;
	}

	/// <summary>
	/// Toggle the rotate hint.
	/// </summary>
	private void ToggleRotateHint(){
		rotateHint.SetTrigger ("Toggle");
	}

	/// <summary>
	/// Increase number of moves.
	/// </summary>
	public void IncreaseMoves(){
		moves++;
		movesText.text = moves.ToString ();
	}

	/// <summary>
	/// On Time out.
	/// </summary>
	public void TimeOut(){
		//show time out dialog
		//AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_TIMEOUT_DIALOG);
		AdsManager.ins.ShowInterstitial();
        GameObject.Find ("TimeOutDialog").GetComponent<Dialog> ().Show (false);
		//Debug.Log ("TimeOut");
	}
    public void ActiveRateDialog()
    {
        if (TableLevel.selectedLevel.ID == levelShowRate)
        {
            isShowingRateDialog = true;
        }
        else
        {
			isShowingRateDialog = false;
            Debug.Log("Level not reached");
        }


        if (RateDialog.instance != null)
        {
            Debug.Log("Showing rate dialog");
            RateDialog.instance.ShowRateDialog();
        }
    }
}