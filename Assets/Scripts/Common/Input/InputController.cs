﻿using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	public class Touch {
		public float deltaTime;
		public TouchPhase phase;
		public Vector3 position;
		public Vector2 deltaPosition;

		public void SetTouch( UnityEngine.Touch touch) {
			this.deltaTime = touch.deltaTime;
			this.deltaPosition = touch.deltaPosition;
			this.phase = touch.phase;
			this.position = touch.position;
		}

	
		public override string ToString() {
			return "Phase: " + phase + " Position: " + position;
		}
	}

	private static bool useTouch;
	private static bool mousePress = false;
	private static Vector3? lastMousePosition;
	private static float? lastTime;

	private static bool mouseButtonDown = false;
	private static bool mouseButtonUp = false;

	public void Awake () {
		Debug.Log(Application.platform);
		useTouch = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
	//	useTouch = false;
	}
	
	public void Update() {
		if(useTouch) {
			return;
		}

		mouseButtonDown = false;
		mouseButtonUp = false;

		if(Input.GetMouseButtonDown(0)) {
			mousePress = true;
			mouseButtonDown = true;
		}
		if(Input.GetMouseButtonUp(0)) {
			mousePress = false;
			mouseButtonUp = true;
		}

	}

	public void LateUpdate() {
		if(useTouch) {
			return;
		}

		if(mousePress) {
			lastTime = Time.time;
			lastMousePosition = Input.mousePosition;
		} else {
			lastTime = null;
			lastMousePosition = null;
		}

	
	}

	public static bool IsTouch() {
		return (useTouch) ? Input.touchCount > 0 : mousePress;
	}

	/*
	public static Vector3[] GetCurrentTouchPoints(bool convertToScreen) {
		Vector3[] res;
		if(useTouch) {
			res = new Vector3[Input.touchCount];
			for(int i = 0; i < Input.touchCount; i++) {
				if(convertToScreen) {
					res[i] = Camera.main.ScreenPointToRay(Input.touches[i].position).origin;
				} else {
					res[i] = Input.touches[i].position;
				}
			}
			return res;
		} else {
			if(!mousePress) {
				return new Vector3[0];
			} 

			return (convertToScreen)? new Vector3[] {Camera.main.ScreenPointToRay(Input.mousePosition).origin} : new Vector3[] {Input.mousePosition};				
		}
	}

	public static Vector2[] GetTouchDeltaPositions() {
		if(useTouch) {
			Vector2[] res = new Vector2[Input.touchCount];
			for(int i = 0; i < Input.touchCount; i++) {
				res[i] = Input.touches[i].deltaPosition;
			}
			return res;
		} else {
			if(!mousePress || lastMousePosition == null) {
				return new Vector2[0];
			}
			Vector3 delta = Input.mousePosition - lastMousePosition.Value;

			return  new Vector2[] { new Vector2(delta.x, delta.y)};
		}
	}

	public static float[] GetTouchDeltaTimes() {
		if(useTouch) {
			float[] res = new float[Input.touchCount];
			for(int i = 0; i < Input.touchCount; i++) {
				res[i] = Input.touches[i].deltaTime;
			}
			return res;
		} else {
			if(!mousePress || lastTime == null) {
				return new float[0];
			}

			return new float[] {Time.time - lastTime.Value};
		}
	}

	public static TouchPhase[] getTouchPhases() {
		if(useTouch) {
			TouchPhase[] res = new TouchPhase[Input.touchCount];
			for(int i = 0; i < Input.touchCount; i++) {
				res[i] = Input.touches[i].phase;
			}
			return res;
		} else {
			if(Input.GetMouseButtonDown(0)) {
				return new TouchPhase[] {TouchPhase.Began};
			}
			if(Input.GetMouseButtonUp(0)) {
				return new TouchPhase[] {TouchPhase.Ended};
			}
			if(mousePress) {
				return new TouchPhase[] {TouchPhase.Moved};
			}

			return new TouchPhase[0];
		}
	}
*/

	public static InputController.Touch[] getTouches() {
		if(useTouch) {
			InputController.Touch[] res = new InputController.Touch[Input.touchCount];
			for(int i = 0; i < Input.touchCount; i++) {
				res[i] = new InputController.Touch();
				res[i].SetTouch(Input.touches[i]);
			}
			return res;
		} else {
			return (mousePress)? new InputController.Touch[] { FillMouseTouch() } : new InputController.Touch[0];
		}
	}

	private static InputController.Touch FillMouseTouch() {
		InputController.Touch touch = new InputController.Touch();

		touch.deltaTime = (lastTime == null)? 0 : Time.time - lastTime.Value;
		touch.position = Input.mousePosition;

		if(lastMousePosition == null) {
			touch.deltaPosition = Vector2.zero;
		} else {
			Vector3 delta = Input.mousePosition - lastMousePosition.Value;
			touch.deltaPosition = new Vector2(delta.x, delta.y);
		}
//		Debug.Log(mouseButtonUp);

		if(Input.GetMouseButtonUp(0) || mouseButtonUp) {
			touch.phase = TouchPhase.Ended;
		} else if(Input.GetMouseButtonDown(0) || mouseButtonDown) {
			touch.phase = TouchPhase.Began;
		}
		else if(mousePress) {
			touch.phase = TouchPhase.Moved;
		}

		return touch;
	}

	public static Ray TouchToRay(InputController.Touch touch, Camera camera = null) {
		if(camera == null) {
			camera = Camera.main;
		}

		return camera.ScreenPointToRay(Preconditions.NotNull(touch.position)); 
	}

	public static int GetFingerId() {
		if(useTouch) {
			if(Input.touchCount > 0) {
				return Input.GetTouch(0).fingerId;
			}
		}
		return -1;
	}
}
