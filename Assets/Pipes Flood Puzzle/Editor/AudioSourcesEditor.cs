using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[CustomEditor (typeof(AudioSources))]
public class AudioSourcesEditor: Editor
{
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Separator ();
		EditorGUILayout.HelpBox ("The First AudioSource component used for the Music.", MessageType.Info);
		EditorGUILayout.HelpBox ("The second AudioSource component used for the Sound Effects.", MessageType.Info);
		EditorGUILayout.HelpBox ("Click on Apply button that located on the top to save your changes", MessageType.Info);
		EditorGUILayout.Separator ();
	}
}

