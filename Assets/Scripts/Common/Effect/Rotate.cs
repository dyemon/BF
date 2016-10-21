using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 RotateAngles = new Vector3(0, 0, -10);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(RotateAngles);
	}
}
