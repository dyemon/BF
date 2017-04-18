using UnityEngine;
using System.Collections;

public class BackgroundMS : MonoBehaviour {

	private int screenWidth;
	private int screenHeight;

	public float centerYOffset;
	public GameObject[] resizeListeners;
	
	void Start() {
	}

	void Update() {
		if(Screen.width != screenWidth || Screen.height != screenHeight) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		//	ResizeToScreen();
		}
	}

	void ResizeToScreen() {
		transform.localScale = new Vector3(1, 1, 1);

		Vector2 bounds = UnityUtill.GetBounds(gameObject);
		float width = bounds.x;
		float height = bounds.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		float resizeRation = worldScreenHeight / height;

		transform.position = new Vector3(0.0f, Camera.main.orthographicSize + centerYOffset*resizeRation, transform.position.z);
//		transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
		transform.localScale = new Vector3(resizeRation, resizeRation, 1);
		
		Vector2 size = new Vector2(Mathf.Min(width * resizeRation, worldScreenWidth), worldScreenHeight);
		foreach(GameObject go in resizeListeners) {
			IResizeListener rListener = go.GetComponent<IResizeListener>();
			if(rListener != null) {
				rListener.OnResize(resizeRation, size);
			}
		}
	}
}
			