using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com
 
public class LevelsManagerUtil
{
		public static Level CreateNewLevel (LevelsManager attrib, bool showProgress)
		{
				if (attrib == null) {
						return null;
				}
		
				Level lvl = new Level ();
				for (int i = 0; i < attrib.numberOfRows * attrib.numberOfCols; i++) {
					lvl.cells.Add(new Level.Cell());
				}

				attrib.levels.Add (lvl);

				return lvl;
		}
	
		public static void RemoveLevels (LevelsManager attrib)
		{
				if (attrib == null) {
						return;
				}
				attrib.levels.Clear ();
				attrib.previousNumberOfRows = attrib.numberOfRows;
				attrib.previousNumberOfCols = attrib.numberOfCols;
		}
	
		public static void RemoveLevel (int index, LevelsManager attrib)
		{
				if (!(index >= 0 && index < attrib.levels.Count)) {
						return;
				}

				attrib.levels.RemoveAt (index);
		}
}