using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Pipe : MonoBehaviour
{
	/// <summary>
	/// The left path.
	/// </summary>
	[HideInInspector]
	public PipesManager.Pipe.Path leftPath;
	
	/// <summary>
	/// The top path.
	/// </summary>
	[HideInInspector]
	public PipesManager.Pipe.Path topPath;
	
	/// <summary>
	/// The right path.
	/// </summary>
	[HideInInspector]
	public PipesManager.Pipe.Path rightPath;
	
	/// <summary>
	/// The bottom path.
	/// </summary>
	[HideInInspector]
	public PipesManager.Pipe.Path bottomPath;

	/// <summary>
	/// The normal sprite of the pipe.
	/// </summary>
	[HideInInspector]
	public Sprite normalSprite;

	/// <summary>
	/// The connected sprite of the pipe.
	/// </summary>
	[HideInInspector]
	public Sprite connectSprite;

	/// <summary>
	/// The target rotation.
	/// </summary>
	private Quaternion targetRotation;

	/// <summary>
	/// The target scale.
	/// </summary>
	private Vector3 clickScale = new Vector3(1.5f,1.5f,1);

	/// <summary>
	/// The lerp speed.
	/// </summary>
	private float lerpSpeed = 10f;

	/// <summary>
	/// The temp left enabled flag.
	/// </summary>
	private static bool tempLeftEnabled;

	/// <summary>
	/// The temp top enabled flag.
	/// </summary>
	private static bool tempTopEnabled;

	/// <summary>
	/// The temp right enabled flag.
	/// </summary>
	private static bool tempRightEnabled;

	/// <summary>
	/// The temp bottom enabled flag.
	/// </summary>
	private static bool tempBottomEnabled;

	/// <summary>
	/// Temporary vector.
	/// </summary>
	private Vector3 tempVector;

	/// <summary>
	/// The initial scale of the pipe.
	/// </summary>
	private Vector3 initialScale;

	/// <summary>
	/// Whether to lock the sprite of pipe or not.
	/// </summary>
	[HideInInspector]
	public bool lockSprite;

	void Start(){
		//set up the initial scale of the pipe
		initialScale = transform.localScale;
	}

	/// <summary>
	/// Init the attributes of the pipe.
	/// </summary>
	public void Init ()
	{
		targetRotation = Quaternion.Euler (Vector3.zero);
		leftPath = new PipesManager.Pipe.Path ();
		topPath = new PipesManager.Pipe.Path ();
		rightPath = new PipesManager.Pipe.Path ();
		bottomPath = new PipesManager.Pipe.Path ();
	}

	void Update(){

		if (!Mathf.Approximately (transform.localScale.magnitude, initialScale.magnitude)) {
			tempVector = transform.localScale;
			tempVector.x = Mathf.Lerp (tempVector.x, 1, Time.deltaTime * lerpSpeed);
			tempVector.y = Mathf.Lerp (tempVector.y, 1, Time.deltaTime * lerpSpeed);
			transform.localScale = tempVector;
		}

		if (!Mathf.Approximately (transform.eulerAngles.z, targetRotation.eulerAngles.z)) {
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation,  Time.deltaTime * lerpSpeed);
		}
	}

	/// <summary>
	/// Rotate the paths of the pipe.
	/// </summary>
	private void RotatePaths(){

		 tempLeftEnabled = leftPath.enabled;
		 tempTopEnabled = topPath.enabled;
		 tempRightEnabled = rightPath.enabled;
		 tempBottomEnabled = bottomPath.enabled;
		
		leftPath.enabled = tempBottomEnabled;
		topPath.enabled = tempLeftEnabled;
		rightPath.enabled = tempTopEnabled;
		bottomPath.enabled = tempRightEnabled;
	}

	/// <summary>
	/// Get the clock wise rotation.
	/// </summary>
	/// <returns>The clock wise rotation.</returns>
	private Quaternion GetClockWiseRotation(){
		return Quaternion.Euler (new Vector3 (0, 0, transform.eulerAngles.z - 90));
	}

	/// <summary>
	/// Immediate rotation without delay.
	/// </summary>
	public void ImmediateRotation(){

		int rand  = Random.Range (1, 3);

		for (int i = 0; i < rand ; i++) {
			RotatePaths ();
			transform.rotation = targetRotation = GetClockWiseRotation();
		}
	}

	/// <summary>
	/// Delayed rotation.
	/// </summary>
	public void DelayedRotation(){
		SetClickScale ();
		RotatePaths ();
		transform.rotation = targetRotation;
		targetRotation = GetClockWiseRotation ();
	}

	/// <summary>
	/// Set the click scale for the pipe.
	/// </summary>
	public void SetClickScale(){
		transform.localScale = clickScale;
	}

	/// <summary>
	/// Set the connect sprite for the pipe.
	/// </summary>
	public void SetConnectSprite(){
		if (lockSprite)
			return;
		GetComponent<Image> ().sprite = connectSprite;
	}

	/// <summary>
	/// Set the normal sprite for the pipe.
	/// </summary>
	public void SetNormalSprite(){
		if (lockSprite)
			return;
		GetComponent<Image>().sprite= normalSprite;
	}
}
