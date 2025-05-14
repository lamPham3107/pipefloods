using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[CustomEditor(typeof(LevelsManager))]
public class LevelsManagerEditor : Editor
{
	private static bool showInstructions = true;
	private static float horizontalSpace = 15;
	private static string[] bars = new string[] {"Level Cells"};
	private static int selectedBar = 0;
	private int selectedCell;
	private string[] levels;
	private int selectedPipe;
	public static PipesManager pipesManager;
	private int previousLevel = -1;

	public override void OnInspectorGUI ()
	{
		if (Application.isPlaying) {
			return;
		}

		LevelsManager attrib = (LevelsManager)target;//get the target

		EditorGUILayout.Separator ();
		#if !(UNITY_5 || UNITY_2017 || UNITY_2018_0 || UNITY_2018_1 || UNITY_2018_2)
			//Unity 2018.3 or higher
			EditorGUILayout.BeginHorizontal();
			GUI.backgroundColor = Colors.cyanColor;
			EditorGUILayout.Separator();
			if(PrefabUtility.GetPrefabParent(attrib.gameObject)!=null)
			if (GUILayout.Button("Apply", GUILayout.Width(70), GUILayout.Height(30), GUILayout.ExpandWidth(false)))
			{
				PrefabUtility.ApplyPrefabInstance(attrib.gameObject, InteractionMode.AutomatedAction);
			}
			GUI.backgroundColor = Colors.whiteColor;
			EditorGUILayout.EndHorizontal();
		#endif
		EditorGUILayout.Separator ();
		
		showInstructions = EditorGUILayout.Foldout (showInstructions, "Instructions");
		EditorGUILayout.Separator ();
		if (showInstructions) {
			EditorGUILayout.HelpBox ("* Select number of Rows and Columns to create a Grid of Size equals [Rows x Columns].", MessageType.None);
			EditorGUILayout.HelpBox ("* Click on 'Create New Level' to create a new Level for the Mission", MessageType.None);
			EditorGUILayout.HelpBox ("* Click on 'Remove Levels' to remove all Levels in the Mission", MessageType.None);
			EditorGUILayout.HelpBox ("* Click on 'The Grid' to view the grid of the Level", MessageType.None);
			EditorGUILayout.HelpBox ("* Click on 'The Grid' to set/edit the pipes of the level", MessageType.None);
			EditorGUILayout.HelpBox ("* Click on 'Remove Level' to remove the Level from the Mission", MessageType.None);
			EditorGUILayout.HelpBox ("Save your changes (ctrl/cmd+s).", MessageType.Info);
			EditorGUILayout.Separator ();
		}
		EditorGUILayout.Separator ();
		attrib.numberOfRows = EditorGUILayout.IntSlider ("Number of Rows", attrib.numberOfRows, 2, LevelsManager.rowsLimit);
		EditorGUILayout.Separator ();
		attrib.numberOfCols = EditorGUILayout.IntSlider ("Number of Columns", attrib.numberOfCols, 2, LevelsManager.colsLimit);

		attrib.starsTimePeriod = EditorGUILayout.IntSlider ("Stars Period(s)",attrib.starsTimePeriod,0,500); 
		EditorGUILayout.Separator ();

		attrib.firstGridCellBackground = EditorGUILayout.ObjectField ("First GridCell BG", attrib.firstGridCellBackground, typeof(Sprite), true) as Sprite;
		EditorGUILayout.Separator ();

		attrib.secondGridCellBackground = EditorGUILayout.ObjectField ("Second GridCell BG", attrib.secondGridCellBackground, typeof(Sprite), true) as Sprite;
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();

		if (attrib.previousNumberOfRows == -1) {
			attrib.previousNumberOfRows = attrib.numberOfRows;
		}

		if (attrib.previousNumberOfCols == -1) {
			attrib.previousNumberOfCols = attrib.numberOfCols;
		}

		if (attrib.previousNumberOfCols != attrib.numberOfCols || attrib.previousNumberOfRows != attrib.numberOfRows) {
	
			if (attrib.levels.Count != 0) {
				bool isOk = EditorUtility.DisplayDialog ("Confirm Message", "Changing grid size leads to reset all levels", "ok", "cancel");
				if (isOk) {
					LevelsManagerUtil.RemoveLevels (attrib);
					CloseGridWindow ();
				} else {
					attrib.numberOfRows = attrib.previousNumberOfRows;
					attrib.numberOfCols = attrib.previousNumberOfCols;
				}
			} else {
				LevelsManagerUtil.RemoveLevels (attrib);
			}
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Review Pipes Flood", GUILayout.Width (305), GUILayout.Height (25))) {
			Application.OpenURL ("https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:9268");
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

		GUI.backgroundColor = Colors.greenColor;         
		if (GUILayout.Button ("Create New Level", GUILayout.Width (150), GUILayout.Height (25))) {
			Level lvl = LevelsManagerUtil.CreateNewLevel (attrib, true);
			if (lvl != null) {
				lvl.showGridOnCreate = true;
			}
		}
		GUI.backgroundColor = Colors.whiteColor;         

		GUI.backgroundColor = Colors.redColor;         
		if (GUILayout.Button ("Remove Levels", GUILayout.Width (150), GUILayout.Height (25))) {
			if (attrib.levels.Count != 0) {
				bool isOk = EditorUtility.DisplayDialog ("Removing Levels", "Are you sure you want to remove all levels ?", "yes", "cancel");
				if (isOk) {
					LevelsManagerUtil.RemoveLevels (attrib);
					CloseGridWindow ();
				}
			}
		}
		GUI.backgroundColor = Colors.whiteColor;         

		GUILayout.EndHorizontal ();

		EditorGUILayout.Separator ();
		GUILayout.Box ("Levels Section", GUILayout.ExpandWidth (true), GUILayout.Height (23));
		EditorGUILayout.Separator ();

		EditorGUILayout.HelpBox ("Move between levels using Selected Level slider", MessageType.Info);

		EditorGUILayout.LabelField ("Number of Levels : " + attrib.levels.Count);
		if (attrib.levels.Count == 0) {
			return;
		} 

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<", GUILayout.Width (18), GUILayout.Height (15))) {
			if (attrib.selectedLevel - 1 >= 0)
				attrib.selectedLevel -= 1;
		}

		attrib.selectedLevel = attrib.selectedLevel + 1;
		attrib.selectedLevel = EditorGUILayout.IntSlider ("Selected Level", attrib.selectedLevel, 1, attrib.levels.Count); 
		attrib.selectedLevel = attrib.selectedLevel - 1;

		if (GUILayout.Button (">", GUILayout.Width (18), GUILayout.Height (15))) {
			if (attrib.selectedLevel + 1 < attrib.levels.Count)
				attrib.selectedLevel += 1;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Separator ();

		GUILayout.Box ("Level " + (attrib.selectedLevel + 1), GUILayout.ExpandWidth (true), GUILayout.Height (23));

		if (attrib.levels [attrib.selectedLevel].showGridOnCreate || attrib.selectedLevel != previousLevel) {
			previousLevel = attrib.selectedLevel;
			GridWindowEditor.Init (attrib.levels [attrib.selectedLevel], "Level " + (attrib.selectedLevel + 1), attrib);
			attrib.levels [attrib.selectedLevel].showGridOnCreate = false;
		}

		EditorGUILayout.Separator ();

		GUI.backgroundColor = Colors.cyanColor;         
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("The Grid", GUILayout.Width (110), GUILayout.Height (23))) {
			GridWindowEditor.Init (attrib.levels [attrib.selectedLevel], "Level " + (attrib.selectedLevel + 1), attrib);
		}

		GUI.backgroundColor = Colors.redColor;         
		if (GUILayout.Button ("Remove Level " + (attrib.selectedLevel + 1), GUILayout.Width (110), GUILayout.Height (23))) {
			bool isOk = EditorUtility.DisplayDialog ("Removing Level", "Are you sure you want to remove level " + (attrib.selectedLevel + 1) + " ?", "yes", "cancel");
			if (isOk) {
				LevelsManagerUtil.RemoveLevel (attrib.selectedLevel, attrib);
				if (attrib.levels.Count == 0) {
					CloseGridWindow ();
				}
				return;
			}
		}
		GUI.backgroundColor = Colors.whiteColor;         

		GUILayout.EndHorizontal ();
		EditorGUILayout.Separator ();

		selectedBar = GUILayout.Toolbar (selectedBar, bars);

		switch (selectedBar) {
		case 0:
			ShowLevelCells (attrib.levels [attrib.selectedLevel], attrib.selectedLevel, attrib);
			break;
		}

		if (GUI.changed) {
			DirtyUtil.MarkSceneDirty ();
		}
	}

	private void ShowLevelCells (Level level, int levelIndex, LevelsManager attrib)
	{
		if (level.cells.Count == 0) {
			return;
		} 

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<", GUILayout.Width (18), GUILayout.Height (15))) {
			if (selectedCell - 1 >= 0)
				selectedCell -= 1;
		}

		selectedCell = EditorGUILayout.IntSlider ("Selected Cell", selectedCell, 0, level.cells.Count - 1); 

		if (GUILayout.Button (">", GUILayout.Width (18), GUILayout.Height (15))) {
			if (selectedCell + 1 < level.cells.Count)
				selectedCell += 1;
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Separator ();

		if (pipesManager == null) {
			pipesManager = GameObject.FindObjectOfType<PipesManager> ();
		}

		if (pipesManager != null) {
			List<PipesManager.Pipe> pipes = pipesManager.pipes;
			string [] pipesIdentifiers = new string[pipes.Count + 1];
			pipesIdentifiers [0] = PipesManager.Pipe.defaultIdentifier;
			for (int i = 1; i < pipesIdentifiers.Length; i++) {
				pipesIdentifiers [i] = pipes [i - 1].identifier;
			}

			level.cells [selectedCell].enableBackground = EditorGUILayout.Toggle ("Enable Background", level.cells [selectedCell].enableBackground);		
			selectedPipe = GetPipeIndentifierIndex (level.cells [selectedCell].pipeIdentifier, pipesIdentifiers);
			selectedPipe = EditorGUILayout.Popup ("Pipe", selectedPipe, pipesIdentifiers); 
			level.cells [selectedCell].pipeIdentifier = pipesIdentifiers [selectedPipe];

		}
		EditorGUILayout.Separator ();
	}

	private int GetPipeIndentifierIndex (string id, string[] pipesIdentifiers)
	{
		for (int i = 0; i < pipesIdentifiers.Length; i++) {
			if (pipesIdentifiers [i].Equals (id)) {
				return i;
			}
		}

		return 0;
	}

	public static void CloseGridWindow ()
	{
		if (GridWindowEditor.window != null) {
			GridWindowEditor.window.Close ();
		}
	}
}