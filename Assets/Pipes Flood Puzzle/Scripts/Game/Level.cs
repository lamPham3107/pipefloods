using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

/// <summary>
/// A level used with LevelsManager Component.
/// When you create a new level using Inspector ,you will create new instace of this class
/// </summary>
[System.Serializable]
public class Level
{
	/// <summary>
	/// The cells list.
	/// </summary>
	public List<Cell> cells = new List<Cell> ();

	/// <summary>
	/// The list of paths in the level.
	/// </summary>
	public List<Path> paths = new List<Path> ();

	/// <summary>
	/// Whether to show the grid on create the level.
	/// </summary>
	public bool showGridOnCreate = false;

	public Level(){
		paths.Add (new Path ());
	}

	[System.Serializable]
	public class Cell
	{
		/// <summary>
		/// Whether the cell is visible(used with inspector only).
		/// </summary>
		public bool showCell;

		/// <summary>
		/// The pipe identifier.
		/// </summary>
		public string pipeIdentifier;

		/// <summary>
		/// Whether to enable grid cell background or not.
		/// </summary>
		public bool enableBackground = true;
	}

	[System.Serializable]
	public class Path{

		/// <summary>
		/// Whether to show contents of the path(used for editor only).
		/// </summary>
		public bool showContents = true;

		/// <summary>
		/// The index of the source.
		/// </summary>
		public int sourceIndex;

		/// <summary>
		/// The index of the destination.
		/// </summary>
		public int destinationIndex;
	}
}