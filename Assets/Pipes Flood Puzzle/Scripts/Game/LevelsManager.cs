using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

[DisallowMultipleComponent]
public class LevelsManager : MonoBehaviour
{
	public Sprite defaultSprite;
	public Sprite firstGridCellBackground;
	public Sprite secondGridCellBackground;
	public readonly static int rowsLimit = 15;
	public readonly static int colsLimit = 15;
	public int numberOfCols = 6;
	public int numberOfRows = 8;
	/// <summary>
	/// The stars time period in seconds.
	///Example if value equals 20 then 0 -> 20 seconds = 3 stars , 20 -> 40 = 2 stars , 40 -> 80 = 1 stars ,otherwise zero 
	/// </summary>
	public int starsTimePeriod = 20;
	public List<Level> levels = new List<Level> ();
	[HideInInspector]
	public int previousNumberOfRows = -1;
	[HideInInspector]
	public int previousNumberOfCols = -1;
	[HideInInspector]
	public int selectedLevel;

}