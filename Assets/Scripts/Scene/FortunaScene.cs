using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortunaScene : WindowScene {
	public const string SceneName = "Fortuna";

	public GameObject Ruletka;

	private bool rotateRuletka;
	private int angle;

	void Start () {
		
	}
	
	public void OnRuletkaStart() {
		Vector3 rotate = Vector3.zero;
		rotate.z = Random.Range(1800, 1800 + 360);

		AnimatedObject ao = Ruletka.GetComponent<AnimatedObject>();
		ao.AddRotate(null, rotate, 5, new ABase.BezierPoints(0.5f, 0.9f, 0.8f, 1f))
			.OnStop(() => {
				OnRuletkaStop();
			})
			.Build().Run();	
	}

	void Update() {
	/*	if(!rotateRuletka) {
			return;
		}

		int speed = 1;
		int rotateAngle = Time.deltaTime * speed;
		angle -= rotateAngle;
		*/
	}

	void OnRuletkaStop() {
		int deg = 36;
		float angle = Ruletka.transform.rotation.eulerAngles.z + deg/2;
	//	Debug.Log(angle);
		if(angle >= 360) {
			angle = angle - 360;
		}

		int index = (int)Mathf.Floor(angle / deg);
		Debug.Log(angle + " " + index);
	}
}
