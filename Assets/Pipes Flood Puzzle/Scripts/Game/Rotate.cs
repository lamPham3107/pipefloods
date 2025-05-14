using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class Rotate : MonoBehaviour {

	public float speed = 10;
	public bool Enabled;
	private Vector3 direction = new Vector3 (0, 0, 1);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Enabled) {
			transform.Rotate (direction * speed * Time.smoothDeltaTime);
		}
	}
}
