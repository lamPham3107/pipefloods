using UnityEngine;
using System.Collections;
using System;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class TableLevel : MonoBehaviour
{
		/// <summary>
		/// The selected level.
		/// </summary>
		public static TableLevel selectedLevel;

		/// <summary>
		/// Table Level ID.
		/// </summary>
		public int ID = -1;

		/// <summary>
		/// The stars number(Rating).
		/// </summary>
		public DataManager.LevelData.StarsNumber starsNumber = DataManager.LevelData.StarsNumber.ZERO;

		/// <summary>
		/// Whether the Level is locked or not.
		/// </summary>
		[HideInInspector]
		public bool isLocked;

		// Use this for initialization
		void Start ()
		{
				///Setting up the ID for Table Level
				if (ID == -1) {
						string [] tokens = gameObject.name.Split ('-');
						if (tokens != null) {
								ID = int.Parse (tokens [1]);
						}
				}

				///Setting up the Title for Table Level
				GameObject leveTitleGameObject = transform.Find ("LevelTitle").gameObject;//Find LevelTitle GameObject
				if (leveTitleGameObject != null) {
						TextMesh textMeshComponent = leveTitleGameObject.GetComponent<TextMesh> ();//Get LevelTitle Text Mesh Component
						if (textMeshComponent != null) {
								if (string.IsNullOrEmpty (textMeshComponent.text)) {
										textMeshComponent.text = ID.ToString ();//Set the Title as the ID
								}
						}
				}
		}

}