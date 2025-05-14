using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class GridWindowEditor : EditorWindow
{
	private Vector2 mainScrollPos;
	private static Level level;
	private static LevelsManager levelsManager;
	private static int gridCellndex;
	private Vector2 offset = new Vector2 (0, 0);
	private Vector2 toolScale = new Vector2 (45, 45);
	private Vector2 gridScale;
	public static GridWindowEditor window;
	private static string levelTitle;
	private static int selectedHelpPath, previousPath;
	private Texture2D toolTexture;
	private static int minScale = 25, maxScale = 45;
	private static int zoom = maxScale;
	private static int gridSize;
	private PipesManager.Pipe selectedPipe;
	private Tool selectedTool = Tool.None;

	public static void Init (Level lvl, string lvlTitle, LevelsManager attrib)
	{
		levelTitle = lvlTitle;
		levelsManager = attrib;
		level = lvl;
		window = (GridWindowEditor)EditorWindow.GetWindow (typeof(GridWindowEditor));
		float windowSize = Screen.height * 0.85f;
		window.position = new Rect (50, 100, windowSize, windowSize);
		window.titleContent.text = levelTitle;
		gridSize = levelsManager.numberOfRows * levelsManager.numberOfCols;
	}

	void OnGUI ()
	{
		if (window == null || level == null || levelsManager == null || Application.isPlaying) {
			return;
		}

		if (level.cells.Count == 0) {
			return;
		}

		window.Repaint ();

		mainScrollPos = EditorGUILayout.BeginScrollView (mainScrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height));

		EditorGUILayout.Separator ();
		GUILayout.Box ("Tools Section", GUILayout.ExpandWidth (true), GUILayout.Height (23));
		GUILayout.BeginHorizontal ();
		GUI.backgroundColor = Color.green;         

		int cols = 8;
		int rows = Mathf.CeilToInt (LevelsManagerEditor.pipesManager.pipes.Count / (cols * 1.0f));


		if (GUILayout.Button ("Add New Path", GUILayout.Width (100), GUILayout.Height (20))) {
			if(level.paths.Count < Mathf.Max(cols,rows)){
				level.paths.Add (new Level.Path ());
			}
		}

		if (level.paths.Count != 0) {
			GUI.backgroundColor = Colors.redColor;         
			if (GUILayout.Button ("Remove Last Path", GUILayout.Width (115), GUILayout.Height (20))) {
				if (level.paths.Count != 0) {
					level.paths.RemoveAt (level.paths.Count - 1);
				}
			}
			GUI.backgroundColor = Colors.whiteColor;
		}

		GUI.backgroundColor = Colors.whiteColor;

		GUILayout.EndHorizontal ();
	

		EditorGUILayout.HelpBox ("Select a tool from the tools list below and then click on the cell(s) in the grid.", MessageType.Info);

		Vector2 dragPostion = Vector2.zero;
		dragPostion.x = GUIUtility.GUIToScreenPoint (Event.current.mousePosition).x - window.position.x;
		dragPostion.y = GUIUtility.GUIToScreenPoint (Event.current.mousePosition).y - window.position.y;
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Reset", GUILayout.Width (toolScale.x), GUILayout.Height (toolScale.y))) {
			selectedTool = Tool.ResetPipe;
			selectedPipe = null;
		}
		
		if (GUILayout.Button ("TBG", GUILayout.Width (toolScale.x), GUILayout.Height (toolScale.y))) {
			selectedTool = Tool.ToggleBackground;
			selectedPipe = null;
		}

		if (rows == 0) {
			EditorGUILayout.EndHorizontal ();
		}

		PipesManager.Pipe tempPipe;
		int index;
		for (int i = 0; i < rows; i++) {
			if (i != 0) {
				EditorGUILayout.BeginHorizontal ();
			}
			for (int j = 0; j < cols; j++) {
				index = i * cols + j;
				if (!(index >= 0 && index < LevelsManagerEditor.pipesManager.pipes.Count)) {
					continue;
				}
				tempPipe = LevelsManagerEditor.pipesManager.pipes [index];
				if (GUILayout.Button (tempPipe.sprite!=null?tempPipe.sprite.texture:null, GUILayout.Width (toolScale.x), GUILayout.Height (toolScale.y))) {
					selectedTool = Tool.SetPipe;
					selectedPipe = tempPipe;
				}
			}
			EditorGUILayout.EndHorizontal ();
		}

		GUILayout.Label ("Selected Tool : " + selectedTool.ToString ());

		GUILayout.Box ("Level Grid "+levelsManager.numberOfRows + "x" + levelsManager.numberOfCols, GUILayout.ExpandWidth (true), GUILayout.Height (23));
		EditorGUILayout.Separator ();

		for (int i = 0; i <  level.paths.Count; i++) {
			level.paths [i].showContents = EditorGUILayout.Foldout (level.paths [i].showContents, "Path");//"Path[" + i + "]"

			if (level.paths [i].showContents) {
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			//GUILayout.Space(15);
			GUILayout.BeginVertical ();
			GUI.backgroundColor = Colors.greenColor;
			level.paths [i].sourceIndex = EditorGUILayout.IntSlider ("Source", level.paths [i].sourceIndex, 0, gridSize - 1); 
			EditorGUILayout.Separator ();
			GUI.backgroundColor = Colors.whiteColor;

			GUI.backgroundColor = Colors.greenColor;
			level.paths [i].destinationIndex = EditorGUILayout.IntSlider ("Destination", level.paths [i].destinationIndex, 0, gridSize - 1);
			EditorGUILayout.Separator ();
			GUI.backgroundColor = Colors.whiteColor;
			GUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			}
		}

		zoom = EditorGUILayout.IntSlider ("Grid Zoom", zoom, minScale, maxScale);
		gridScale.x = gridScale.y = zoom;
	
		GridGUILayout ();

		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (position.width / 2 - 100);
		GUI.backgroundColor = Color.clear;
		if (GUILayout.Button ("Indie Studio", GUILayout.Width (200), GUILayout.Height (22))) {
			Application.OpenURL (Links.indieStudioStoreURL);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndScrollView ();

		toolTexture = selectedPipe != null ? (selectedPipe.sprite != null ? selectedPipe.sprite.texture : null) : null;

		if (toolTexture != null) {
			GUI.DrawTexture (new Rect (dragPostion.x, dragPostion.y, 50, 50), toolTexture);
		}

		if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
			selectedTool = Tool.None;
			selectedPipe = null;
		}

		if (GUI.changed) {
			DirtyUtil.MarkSceneDirty ();
		}
	}

	private void GridGUILayout ()
	{
		EditorGUILayout.Separator ();
		GUILayout.Space (5);
		for (int i = 0; i < levelsManager.numberOfRows; i++) {

			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);

			for (int j = 0; j < levelsManager.numberOfCols; j++) {
				gridCellndex = i * levelsManager.numberOfCols + j;

				toolTexture = null;
				if (level.cells [gridCellndex].pipeIdentifier != PipesManager.Pipe.defaultIdentifier) {
					Sprite tempSprite = GridUtil.GetPipeSprite (level.cells [gridCellndex].pipeIdentifier, LevelsManagerEditor.pipesManager);
					if (tempSprite != null) {
						toolTexture = tempSprite.texture;
					}
				}

				if (!level.cells [gridCellndex].enableBackground) {
					GUI.backgroundColor = Colors.transparent;
				}

				for (int k = 0; k <  level.paths.Count; k++) {
					if (gridCellndex == level.paths [k].sourceIndex) {
						GUI.backgroundColor = Colors.paleGreen;
						if (level.cells [gridCellndex].enableBackground) {
							GUI.backgroundColor = Colors.greenColor;
						}
					}else if(gridCellndex == level.paths[k].destinationIndex){
						GUI.backgroundColor = Colors.paleGreen;
						if (level.cells [gridCellndex].enableBackground) {
							GUI.backgroundColor = Colors.greenColor;
						}
					}
				}

				if (GUILayout.Button (toolTexture, GUILayout.Width (gridScale.x), GUILayout.Height (gridScale.y))) {
					if (selectedTool == Tool.SetPipe) {
						if (selectedPipe != null) {
							level.cells [gridCellndex].pipeIdentifier = selectedPipe.identifier;
						}
					} else if (selectedTool == Tool.ResetPipe) {
						selectedPipe = null;
						level.cells [gridCellndex].pipeIdentifier = PipesManager.Pipe.defaultIdentifier;
					} else if (selectedTool == Tool.ToggleBackground) {
						selectedPipe = null;
						level.cells [gridCellndex].enableBackground = !level.cells [gridCellndex].enableBackground;
					}
					//selectedTool = Tool.None;
				}
				GUI.backgroundColor = Color.white;

				GUILayout.Space (offset.x);
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (offset.y);

			GUI.contentColor = Color.white;
			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			for (int j = 0; j < levelsManager.numberOfCols; j++) {
				gridCellndex = i * levelsManager.numberOfCols + j;

				GUILayout.TextField (gridCellndex + "", GUILayout.Width (gridScale.x), GUILayout.Height (20));
				GUILayout.Space (offset.x);
			}
			GUILayout.EndHorizontal ();
		}
		EditorGUILayout.Separator ();
	}

	public enum Tool
	{
		None,
		SetPipe,
		ResetPipe,
		ToggleBackground
	}
}