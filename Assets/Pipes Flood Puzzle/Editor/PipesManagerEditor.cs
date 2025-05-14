using UnityEngine;
using System.Collections;
using UnityEditor;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[CustomEditor(typeof(PipesManager))]
public class PipesManagerEditor : Editor
{
	private bool windowVisible;

	public override void OnInspectorGUI ()
	{
		PipesManager attrib = (PipesManager)target;//get the target

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
		
		EditorGUILayout.HelpBox ("Click on 'Manage Pipes' to manage the pipes of the game", MessageType.Info);

		GUI.backgroundColor = Colors.greenColor;
		if (GUILayout.Button ("Manage Pipes", GUILayout.Width (150), GUILayout.Height (22))) {
			PipesManagerWindow.Init();
		}
		EditorGUILayout.Separator ();

		GUI.backgroundColor = Colors.whiteColor;
		if (!windowVisible) {
			windowVisible = true;
			PipesManagerWindow.Init();
		}
		if (GUI.changed) {
			DirtyUtil.MarkSceneDirty ();
		}
	}
}
