using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class Destroy : MonoBehaviour
{
		/// <summary>
		/// Destroy time.
		/// </summary>
		public float time;

		// Use this for initialization
		void Start ()
		{
				///Destry the current gameobject
				Destroy (gameObject, time);
		}
}
