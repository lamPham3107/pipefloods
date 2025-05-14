using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com
 
public class GridUtil
{
	public static Hashtable FindPath (GridCell[] gridCells, int fromIndex, int toIndex, int numberOfRows, int numberOfColumns, bool fillPath)
	{
		Hashtable path = new Hashtable ();
		int gridSize = numberOfRows * numberOfColumns;

		//if index out of grid bounds
		if (!(fromIndex >= 0 && fromIndex < gridSize) || !(toIndex >= 0 && toIndex < gridSize)) {
			return path;
		}

		int index = fromIndex;
		Pipe pipeComponent;

		try {
			
			Queue<int> queue = new Queue<int> ();
			queue.Enqueue (index);
			
			int i;
			int j;
			while (queue.Count != 0) {
				index = queue.Dequeue ();

				if (gridCells [index].pipe == null)
					continue;
				
				if (!fillPath)
				if (index == toIndex) {
					break;
				} 
				
				i = index / numberOfColumns;
				j = index - (i * numberOfColumns);

				pipeComponent = GetGridPipe (gridCells, numberOfColumns, i, j + 1);
			
				if (gridCells [index].pipe.rightPath.enabled && (pipeComponent != null ? pipeComponent.leftPath.enabled : false)) {
					Enqueue (i, j + 1, numberOfRows, numberOfColumns, index, queue, path, fillPath, pipeComponent);//Right
				}

				pipeComponent = GetGridPipe (gridCells, numberOfColumns, i, j - 1);
				if (gridCells [index].pipe.leftPath.enabled && (pipeComponent != null ? pipeComponent.rightPath.enabled : false)) {
					Enqueue (i, j - 1, numberOfRows, numberOfColumns, index, queue, path, fillPath, pipeComponent);//Left
				}

				pipeComponent = GetGridPipe (gridCells, numberOfColumns, i - 1, j);
				if (gridCells [index].pipe.topPath.enabled && (pipeComponent != null ? pipeComponent.bottomPath.enabled : false)) {
					Enqueue (i - 1, j, numberOfRows, numberOfColumns, index, queue, path, fillPath, pipeComponent);//Top
				}

				pipeComponent = GetGridPipe (gridCells, numberOfColumns, i + 1, j);
				if (gridCells [index].pipe.bottomPath.enabled && (pipeComponent != null ? pipeComponent.topPath.enabled : false)) {
					Enqueue (i + 1, j, numberOfRows, numberOfColumns, index, queue, path, fillPath, pipeComponent);//Bottom
				}
			}
			
		} catch (System.Exception ex) {
			Debug.LogException (ex);
		}
		
		return path;
	}

	public static void Enqueue (int i, int j, int numberOfRows, int numberOfColumns, int index, Queue<int> queue, Hashtable path, bool fillPath, Pipe pipe)
	{
		if ((i >= 0 && i < numberOfRows) && (j >= 0 && j < numberOfColumns)) {
			int currentIndex = (int)(i * numberOfColumns + j);
	
			if (fillPath)
				pipe.SetConnectSprite ();

			if (!InHashPath (path, currentIndex)) {
				path.Add (currentIndex, index);
				queue.Enqueue (currentIndex);
			}
		}
	}

	public static List<int> GetGridPath (Hashtable hashPath, int fromIndex, int toIndex)
	{
		List<int> path = new List<int> ();
		if (hashPath.Count == 0) {
			return path;
		}
		
		int currentKey = toIndex;
		path.Add (currentKey);
		while (currentKey != fromIndex) {
			if (hashPath.ContainsKey (currentKey)) {
				currentKey = (int)hashPath [currentKey];
				path.Insert (0, currentKey);
			} else {
				path.Clear ();
				return path;
			}
		}
		return path;
	}

	public static bool InHashPath (Hashtable path, int index)
	{
		if (path.ContainsKey (index)) {
			return true;
		}
		return false;
	}

	public static Sprite GetPipeSprite (string pipeIdentifier, PipesManager pipesManager)
	{
		if (pipesManager == null) {
			return null;
		}
		
		foreach (PipesManager.Pipe pipe in pipesManager.pipes) {
			if (pipe.identifier == pipeIdentifier) {
				return pipe.sprite;
			}
		}
		return null;
	}

	public static PipesManager.Pipe GetPipeByIdentifier (string pipeIdentifier)
	{
		if (PipesManager.instance == null) {
			return null;
		}
		
		foreach (PipesManager.Pipe pipe in PipesManager.instance.pipes) {
			if (pipe.identifier == pipeIdentifier) {
				return pipe;
			}
		}
		return null;
	}

	public static Pipe GetGridPipe (GridCell[] gridCells, int numberOfColumns, int i, int j)
	{

		int index = i * numberOfColumns + j;
		if (index >= 0 && index < gridCells.Length) {
			return gridCells [index].pipe;
		}
		return null;
	}
}
