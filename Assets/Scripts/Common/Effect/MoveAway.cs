using UnityEngine;
using System.Collections;

public class MoveAway : MonoBehaviour {

	public bool RunOsStart = false;
	public float Speed = 0.2f;
	public float TimeToLive = 1;

	private Vector3 move;
	private bool isRun;
	private float rotateAngle;
	
	void Start () {
		if(RunOsStart) {
			Run();
		}
	}

	public void Run() {
		isRun = true;
		rotateAngle = Random.Range(1, 5);
		float angle =  2f * Mathf.PI * Random.Range(1, 361)/360;
		move = new Vector3(Mathf.Cos(angle) * Speed, Mathf.Sin(angle) * Speed, 0);
		Destroy(gameObject, TimeToLive);
	}
	
	// Update is called once per frame
	void Update () {
		if(isRun) {
			gameObject.transform.Rotate(new Vector3(0f, 0f, rotateAngle));
			gameObject.transform.position += move;
		}
	}
}