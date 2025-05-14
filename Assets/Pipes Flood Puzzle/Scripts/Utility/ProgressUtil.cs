using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class ProgressUtil
{
		//Display Cancable Progress Bar
		public static bool DisplayCancableProgressBar(float progress,string title,string info){
			#if UNITY_EDITOR
				return  EditorUtility.DisplayCancelableProgressBar (title, info, progress);
			#else
				return false;
			#endif
		}

		//Display Progress Bar
		public static void DisplayProgressBar (float progress, string title,string info)
		{
			#if UNITY_EDITOR
				EditorUtility.DisplayProgressBar (title, info, progress);
			#endif

		}

		//Hide Progress Bar
		public static void HideProgressBar(){
			#if UNITY_EDITOR
				EditorUtility.ClearProgressBar ();
			#endif
		}
}