using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanCamera : MonoBehaviour {
	public Vector2 Speed = Vector2.zero;
	public Vector2 ShowdownSpeed = Vector2.zero;
	public float AnimateTime = 1f;

	public bool FixX = false;
	public bool FixY = false;
	public float MinX = 0;
	public float MinY = 0;
	public float MaxX = 0;
	public float MaxY = 0;

	private bool animate = false;
	private Vector2 touchDeltaPosition;

	Camera curCamera;

	void Start() {
		curCamera = GetCamera();
	}

	Camera GetCamera() {
		if(curCamera == null) {
			curCamera = GetComponent<Camera>();
		}
		return curCamera;
	}

	void Update() {
		if(animate) {
	//		return;
		}

		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0 && touches[0].phase == TouchPhase.Moved) {
			if(EventSystem.current.IsPointerOverGameObject(InputController.GetFingerId())) {
				return;
			}
			touchDeltaPosition = touches[0].deltaPosition;
			float height = GetCamera().orthographicSize;
			float width = height * GetCamera().aspect;

			if(FixX || 2 * width > (MaxX - MinX)) {
				touchDeltaPosition.x = 0;
			}
			if(FixY) {
				touchDeltaPosition.y = 0;
			}
			Vector3 move = transform.position + new Vector3(-touchDeltaPosition.x * Speed.x, -touchDeltaPosition.y * Speed.y, 0);
			SetPosition(move);
		} else if(touches.Length > 0 && touches[0].phase == TouchPhase.Ended) {
			//StartCoroutine(SetPositionInternal());
		}
	}

	public void SetPosition(Vector3 pos) {
		float height = GetCamera().orthographicSize;
		float width = height * GetCamera().aspect;


		if(!FixX && !(2*width > (MaxX - MinX))) {
			if(pos.x - width < MinX) {
				pos.x = MinX + width;
			}
			if(pos.x + width > MaxX) {
				pos.x = MaxX - width;
			}
		}
		if(!FixY) {
			if(pos.y - height < MinY) {
				pos.y = MinY + height;
			}
			if(pos.y + height > MaxY) {
				pos.y = MaxY - height;
			}
		}

		transform.position = pos;
	//	StartCoroutine(SetPositionInternal(pos));
	}

	 IEnumerator SetPositionInternal() {
		float curTime = 0;
		Vector3 startPos = transform.position;
		Vector3 pos = startPos + new Vector3(touchDeltaPosition.x, touchDeltaPosition.y, 0);
		animate = true;

		while(curTime < AnimateTime) {
			curTime += Time.deltaTime;
			float r = Mathf.Min(curTime/AnimateTime, 1);
			transform.position = Vector3.Lerp(startPos, pos, r);
			yield return 0;
		}
		animate = false;
	}
}
