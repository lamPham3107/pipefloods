using UnityEditor;
using UnityEngine;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[CustomEditor (typeof(DataManager))]
public class DataManagerEditor : Editor
{
		public override void OnInspectorGUI ()
		{
				if (Application.isPlaying) {
						return;
				}
				DataManager attrib = (DataManager)target;//get the target
				
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
		
				EditorGUILayout.HelpBox ("Select Reading And Writing method using the Serilization Method", MessageType.Info);
				EditorGUILayout.Separator ();
				if (attrib.serilizationMethod != DataManager.SerilizationMethod.PLAYER_PREFS) {
						attrib.fileName = EditorGUILayout.TextField ("File Name", attrib.fileName);
				}
				attrib.serilizationMethod = (DataManager.SerilizationMethod)EditorGUILayout.EnumPopup ("Serilization Method", attrib.serilizationMethod);
		
				EditorGUILayout.Separator ();
				if (attrib.serilizationMethod != DataManager.SerilizationMethod.PLAYER_PREFS) {
						EditorGUILayout.BeginHorizontal ();

						if (GUILayout.Button ("Explore File Folder", GUILayout.Width (120), GUILayout.Height (25))) {
								string path = FileManager.GetCurrentPlatformFileFolder ();
								if (path != null) {
										EditorUtility.RevealInFinder (path);
								}
						}

						GUI.backgroundColor = Colors.redColor;
						if (GUILayout.Button ("Clean File Folder", GUILayout.Width (120), GUILayout.Height (25))) {
								bool result = EditorUtility.DisplayDialog ("Confirm Clean", "Are you sure you want to clean the folder ?", "Yes", "No");

								if (result) {
										string path = FileManager.GetCurrentPlatformFileFolder ();
										if (path != null) {
												FileManager.CleanFolderFiles (path);
										}
								}
						}
						GUI.backgroundColor = Colors.whiteColor;
						EditorGUILayout.EndHorizontal ();
				}

				if (GUI.changed) {
					DirtyUtil.MarkSceneDirty ();
				}
		}
}