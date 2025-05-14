using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class PipesManagerWindow: EditorWindow
{
	private Vector2 scrollPos;
	private Vector2 scale;
	private Vector2 scrollView = new Vector2 (550, 430);
	private Vector2 pathSize = new Vector2 (60, 60);
	private static PipesManagerWindow window;
	private static bool showInstructions = true;
	private static PipesManager.Pipe newPipe;
	private static PipesManager.Pipe currentPipe;
	private static int selectedBar = 0;
	private static int selectedPipe;
	private static string[] pipesIdentifiers;
	private static string[] bars = new string[] {"Create New Pipe","Edit Pipes"};
	private static PipesManager pipesManager;

	public static void Init ()
	{
		pipesManager = GameObject.FindObjectOfType<PipesManager> ();
		InitPipesIdentifiers ();
		ResetPipeParameters ();

		if (newPipe == null) {
			CreateNewPipeInstance ();
		}
		window = (PipesManagerWindow)EditorWindow.GetWindow (typeof(PipesManagerWindow));
		float windowSize = Screen.currentResolution.height * 0.75f;
		window.position = new Rect (50, 100, windowSize , windowSize);
		window.maximized = false;
		window.titleContent.text = "Manage Pipes";
		window.Show ();
	}

	[MenuItem("Tools/Pipes Flood Puzzle/Pipes Manager #g",false,1)]
	static void ManagePipes ()
	{
		Init ();
	}
	
	[MenuItem("Tools/Pipes Flood Puzzle/Pipes Manager #g",true,1)]
	static bool ManagePipesValidate ()
	{
		return !Application.isPlaying && GameObject.FindObjectOfType<PipesManager> () != null;
	}
	
	void OnGUI ()
	{
		if (window == null || newPipe == null || pipesManager == null || Application.isPlaying) {
			return;
		}

		window.Repaint ();

		scrollView = new Vector2 (position.width, position.height);
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (scrollView.x), GUILayout.Height (scrollView.y));
	
		selectedBar = GUILayout.Toolbar (selectedBar, bars);
		
		switch (selectedBar) {
		case 0:
			CreateNewPipe ();
			break;
		case 1:
			EditPipes ();
			break;
		}

		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (position.width / 2 - 100);
		GUI.backgroundColor = Color.clear;
		if (GUILayout.Button ("Indie Studio", GUILayout.Width (200), GUILayout.Height (22))) {
			Application.OpenURL (Links.indieStudioStoreURL);
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndScrollView ();
		
		if (GUI.changed) {
			DirtyUtil.MarkSceneDirty ();
		}
	}

	private void CreateNewPipe ()
	{
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		showInstructions = EditorGUILayout.Foldout (showInstructions, "Instructions");
		EditorGUILayout.Separator ();
		if (showInstructions) {
			EditorGUILayout.HelpBox ("Insert unique identifier for Pipe (example: Horizontal-Pipe)", MessageType.Info);
			EditorGUILayout.HelpBox ("Define the paths and the sprites of the Pipe", MessageType.Info);
			EditorGUILayout.HelpBox ("[Important] Click On Apply button in the PipesManager GameObject.", MessageType.Info);
			EditorGUILayout.HelpBox ("Save your changes (ctrl/cmd+s).", MessageType.Info);
		}
		EditorGUILayout.Separator ();

		newPipe.identifier = EditorGUILayout.TextField ("Pipe Identifier", newPipe.identifier);
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();

		RenderPipePaths (newPipe);

		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (35);
		if (GUILayout.Button ("Create", GUILayout.Width (120), GUILayout.Height (25))) {
			if (string.IsNullOrEmpty (newPipe.identifier)) {
				EditorUtility.DisplayDialog ("Wraning", "Please enter the identifier of the pipe", "ok");
			}else if(PipeIdentifierExists(newPipe.identifier)){
				EditorUtility.DisplayDialog ("Wraning", "This Identifier already used , select another one", "ok");
			}else if(newPipe.sprite == null) {
				EditorUtility.DisplayDialog ("Wraning", "Please select the sprite of the pipe", "ok");
			}  else {
				bool isOk = EditorUtility.DisplayDialog ("Generate New Pipe", "Are you sure to generate new pipe ?", "ok", "cancel");
				if (isOk) {
					pipesManager.pipes.Add (newPipe);
					InitPipesIdentifiers();
					CreateNewPipeInstance ();
					EditorUtility.DisplayDialog ("Done", "New Pipe has been generated sucessfully", "ok");
				}
			}
		}

		EditorGUILayout.EndHorizontal ();
	}

	private void EditPipes ()
	{
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		showInstructions = EditorGUILayout.Foldout (showInstructions, "Instructions");
		EditorGUILayout.Separator ();
		if (showInstructions) {
			EditorGUILayout.HelpBox ("Select the Pipe you want to edit", MessageType.Info);
			EditorGUILayout.HelpBox ("Edit the selected pipe", MessageType.Info);
			EditorGUILayout.HelpBox ("[Important] Click On Apply button in the PipesManager GameObject.", MessageType.Info);
			EditorGUILayout.HelpBox ("Save your changes (ctrl/cmd+s).", MessageType.Info);
		}
		EditorGUILayout.Separator ();

		if (pipesManager.pipes.Count == 0) {
			return;
		}

		selectedPipe = EditorGUILayout.Popup ("Selected Pipe", selectedPipe, pipesIdentifiers); 
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();

		if (selectedPipe >= 0 && selectedPipe < pipesManager.pipes.Count) {
			currentPipe = pipesManager.pipes [selectedPipe];
		} else {
			currentPipe = null;
		}

		if (currentPipe != null) {
			RenderPipePaths (currentPipe);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (35);
			GUI.backgroundColor = Colors.redColor;

			if (GUILayout.Button ("Remove Pipe", GUILayout.Width (120), GUILayout.Height (25))) {
					bool isOk = EditorUtility.DisplayDialog ("Confirm Remove Pipe", "All references on this Pipe in LevelsManager will be removed , are you sure ?", "yes", "no");
					if (isOk) {
						pipesManager.pipes.RemoveAt (selectedPipe);
						ResetPipeParameters();
						InitPipesIdentifiers();
					}
			}
			GUI.backgroundColor = Colors.whiteColor;
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
		}
	}

	private void RenderPipePaths (PipesManager.Pipe pipe)
	{
		if (pipe == null) {
			return;
		}

		pipe.sprite = EditorGUILayout.ObjectField ("Normal Sprite",pipe.sprite, typeof(Sprite), true) as Sprite;
		pipe.connectSprite = EditorGUILayout.ObjectField ("Connect Sprite",pipe.connectSprite, typeof(Sprite), true) as Sprite;
		EditorGUILayout.Separator ();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (pathSize.x + 7);
		ShowPathBackgroundColor (pipe.topPath.enabled);
		if (GUILayout.Button (CommonUtil.TrueFalseToOnOff (pipe.topPath.enabled), GUILayout.Width (pathSize.x), GUILayout.Height (pathSize.y))) {
			//Top Path
			pipe.topPath.enabled = !pipe.topPath.enabled;
		}
		GUI.backgroundColor = Colors.whiteColor;         
		
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		ShowPathBackgroundColor (pipe.leftPath.enabled);
		if (GUILayout.Button (CommonUtil.TrueFalseToOnOff (pipe.leftPath.enabled), GUILayout.Width (pathSize.x), GUILayout.Height (pathSize.y))) {
			//Left Path
			pipe.leftPath.enabled = !pipe.leftPath.enabled;
		}

		GUI.backgroundColor = Colors.transparent;
		if (GUILayout.Button (pipe.sprite !=null ? pipe.sprite.texture:null, GUILayout.Width (pathSize.x), GUILayout.Height (pathSize.y))) {
			//Do nothing
		}
		GUI.backgroundColor = Colors.whiteColor;

		ShowPathBackgroundColor (pipe.rightPath.enabled);
		
		if (GUILayout.Button (CommonUtil.TrueFalseToOnOff (pipe.rightPath.enabled), GUILayout.Width (pathSize.x), GUILayout.Height (pathSize.y))) {
			//Right Path
			pipe.rightPath.enabled = !pipe.rightPath.enabled;
		}
		GUI.backgroundColor = Colors.whiteColor;
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (pathSize.x + 7);
		ShowPathBackgroundColor (pipe.bottomPath.enabled);
		if (GUILayout.Button (CommonUtil.TrueFalseToOnOff (pipe.bottomPath.enabled), GUILayout.Width (pathSize.x), GUILayout.Height (pathSize.y))) {
			//Bottom Path
			pipe.bottomPath.enabled = !pipe.bottomPath.enabled;
		}
		GUI.backgroundColor = Colors.whiteColor;
		EditorGUILayout.EndHorizontal ();
	}

	private static void CreateNewPipeInstance ()
	{
		newPipe = new PipesManager.Pipe ();
	}

	private static void InitPipesIdentifiers ()
	{
		pipesIdentifiers = new string[pipesManager.pipes.Count];
		for (int i = 0; i < pipesManager.pipes.Count; i++) {
			pipesIdentifiers [i] = pipesManager.pipes [i].identifier;
		}
	}

	private bool PipeIdentifierExists(string id){
		foreach (PipesManager.Pipe pipe in pipesManager.pipes) {
			if(pipe.identifier == id){
				return true;
			}
		}

		return false;
	}

	private static void ResetPipeParameters ()
	{
		selectedPipe = 0;
	}

	private void ShowPathBackgroundColor (bool value)
	{
		if (value) {
			GUI.backgroundColor = Colors.greenColor;         
		} else {
			GUI.backgroundColor = Colors.redColor;         
		}
	}

	void OnInspectorUpdate ()
	{
		Repaint ();
	}
}