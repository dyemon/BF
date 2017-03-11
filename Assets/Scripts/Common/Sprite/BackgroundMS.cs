using UnityEngine;
using System.Collections;

public class BackgroundMS : MonoBehaviour {

	private int screenWidth;
	private int screenHeight;

	public float centerYOffset;

	void Start() {
	}

	void Update() {
		if(Screen.width != screenWidth || Screen.height != screenHeight) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			ResizeToScreen();
		}
	}

	void ResizeToScreen() {
		transform.localScale = new Vector3(1, 1, 1);

		float minX = float.PositiveInfinity;
		float maxX = float.NegativeInfinity; 
		float minY = float.PositiveInfinity;
		float maxY = float.NegativeInfinity;

		foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>()) {
			if(r == null) {
				continue;
			}

			Bounds bounds = r.bounds;
			if(bounds.max.x > maxX) {
				maxX = bounds.max.x;
			}
			if(bounds.max.y > maxY) {
				maxY = bounds.max.y;
			}
			if(bounds.min.x < minX) {
				minX = bounds.min.x;
			}
			if(bounds.min.y < minY) {
				minY = bounds.min.y;
			}
		}

		Renderer renderer = gameObject.GetComponent<Renderer>();
		float width = maxX - minX;
		float height = maxY - minY;

		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		float resizeRation = worldScreenHeight / height;

		transform.position = new Vector3(0.0f, Camera.main.orthographicSize + centerYOffset*resizeRation, transform.position.z);
//		transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
		transform.localScale = new Vector3(resizeRation, resizeRation, 1);
	}
}
			