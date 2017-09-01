using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanCamera : MonoBehaviour {
	public float Speed = 1;
	public float ShowdownSpeed = 0;
	public float AnimateTime = 1f;

	public bool FixX = false;
	public bool FixY = false;
	public float MinX = 0;
	public float MinY = 0;
	public float MaxX = 0;
	public float MaxY = 0;

	private bool animate = false;
	private Vector3 moveDelta;

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
			Vector2 touchDeltaPosition = touches[0].deltaPosition;

			float height = GetCamera().orthographicSize;
			float width = height * GetCamera().aspect;

			if(FixX || 2 * width > (MaxX - MinX)) {
				touchDeltaPosition.x = 0;
			}
			if(FixY) {
				touchDeltaPosition.y = 0;
			}
			Vector3 move = GetCamera().WorldToScreenPoint(transform.position) + new Vector3(-touchDeltaPosition.x, -touchDeltaPosition.y, 0);
			moveDelta = GetCamera().ScreenToWorldPoint(move) - transform.position;
			SetPosition(transform.position + moveDelta*Speed);
		} else if(touches.Length > 0 && touches[0].phase == TouchPhase.Ended) {
			StartCoroutine(SetPositionInternal());
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
		Vector3 pos = transform.position;
		Vector3 delta = moveDelta;
		float speed = ShowdownSpeed;

		while(delta.magnitude > 0.001f) {
			SetPosition(transform.position + delta);
			delta *= speed;
			yield return 0;
		}
	}
}
