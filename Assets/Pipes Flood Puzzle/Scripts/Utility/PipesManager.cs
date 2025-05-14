using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class PipesManager : MonoBehaviour {

	/// <summary>
	/// The public PipesManager instance.
	/// </summary>
	public static PipesManager instance;

	/// <summary>
	/// The pipes list.
	/// </summary>
	public List<Pipe> pipes = new List<Pipe> ();

	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	[Serializable]
	public class Pipe
	{
		/// <summary>
		/// The default identifier.
		/// </summary>
		public static string defaultIdentifier = "None";

		/// <summary>
		/// The identifier of the pipe.
		/// </summary>
		public string identifier;
		
		/// <summary>
		/// The default/normal sprite of the pipe.
		/// </summary>
		public Sprite sprite;

		/// <summary>
		/// The connect sprite of the pipe.
		/// </summary>
		public Sprite connectSprite;

		/// <summary>
		/// The left path.
		/// </summary>
		public Path leftPath;
		
		/// <summary>
		/// The top path.
		/// </summary>
		public Path topPath;
		
		/// <summary>
		/// The right path.
		/// </summary>
		public Path rightPath;
		
		/// <summary>
		/// The bottom path.
		/// </summary>
		public Path bottomPath;
		
		public Pipe ()
		{
			//init the paths of the pipe
			this.leftPath = new Path ();
			this.topPath = new Path ();
			this.rightPath = new Path ();
			this.bottomPath = new Path ();
		}

		[Serializable]
		public class Path
		{
			public bool enabled;
		}
	}
}
