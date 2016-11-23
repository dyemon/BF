﻿using UnityEngine;
using System.Collections;

public class AMove {

	private Vector3 startPos;
	private Vector3 movePos;
	private float time;
	private float speed;
	private bool ui;

	private float startTime;
	private float startDurationTime;

	public AMove(Vector3 startPos, Vector3 movePos, float speed, bool ui) {
		this.startPos = startPos;
		this.movePos = movePos;
		this.speed = speed;
		this.ui = ui;
		time = CalcTime(startPos, movePos, speed);
	}

	public AMove(Vector3 startPos, Vector3 movePos, bool ui) {
		this.startPos = startPos;
		this.movePos = movePos;
		this.ui = ui;
	}

	public static float CalcTime(Vector3 startPos, Vector3 movePos, float speed) {
		return Vector3.Distance(startPos, movePos) / speed;
	}

	public void SetTime(float time) {
		this.time = time;
	}
	public float GetTime() {
		return time;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
		time = CalcTime(startPos, movePos, speed);		
	}
	public float GetSpeed() {
		return speed;
	}

	public void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}

	public bool Move(GameObject gameObject) {
		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

		if(ui) {
			RectTransform rt = gameObject.GetComponent<RectTransform>();
			rt.localPosition = Vector3.Lerp(startPos, movePos, t);
		} else {
			gameObject.transform.position = Vector3.Lerp(startPos, movePos, t);
		}

		return (time > 0);
	}
}