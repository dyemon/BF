using UnityEngine;
using System.Collections;

public class MoveAway : MonoBehaviour {

	public bool runOsStart = false;
	public float speed = 1f;
	
	private Vector3 move;
	private bool isRun;
	private float rotateAngle;
	
	void Start () {
		if(runOsStart) {
			Run();
		}
	}

	public void Run() {
		isRun = true;
		rotateAngle = Random.Range(1, 5);
		float angle =  2f * Mathf.PI / Random.Range(1, 361);
		move = new Vector3(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(isRun) {
			gameObject.transform.Rotate(new Vector3(0f, 0f, rotateAngle));
			gameObject.transform.position += move;
		}
	}
}