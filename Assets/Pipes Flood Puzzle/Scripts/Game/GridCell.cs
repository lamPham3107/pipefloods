using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class GridCell : MonoBehaviour
{      
		/// <summary>
		/// The pipe of the grid cell.
		/// </summary>
		//[HideInInspector]
		public Pipe pipe;

		/// <summary>
		/// The index of the GridCell in the Grid.
		/// </summary>
		public int index;

		/// <summary>
		/// Disable the collider of the cell.
		/// </summary>
		public void DisableCollider(){
			GetComponent<Collider2D> ().enabled = false;
		}
}