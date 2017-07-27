using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanCamera : MonoBehaviour {
	public Vector2 Speed = Vector2.zero;
	public float AnimateTime = 0.1f;

	public bool FixX = false;
	public bool FixY = false;
	public float MinX = 0;
	public float MinY = 0;
	public float MaxX = 0;
	public float MaxY = 0;

	private bool animate = false;

	Camera curCamera;

	void Start() {
		curCamera = GetComponent<Camera>();
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
			Vector2 touchDeltaPosition = touches[0].deltaPosition;
			float height = curCamera.orthographicSize;
			float width = height * curCamera.aspect;

			if(FixX || 2*width > (MaxX - MinX)) {
				touchDeltaPosition.x = 0;
			}
			if(FixY) {
				touchDeltaPosition.y = 0;
			}
			Vector3 move = transform.position + new Vector3(-touchDeltaPosition.x * Speed.x, -touchDeltaPosition.y * Speed.y, 0);
			SetPosition(move);
		}
	}

	void SetPosition(Vector3 pos) {
		float height = curCamera.orthographicSize;
		float width = height * curCamera.aspect;


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

	 IEnumerator SetPositionInternal(Vector3 pos) {
		float curTime = 0;
		Vector3 startPos = transform.position;
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
