using UnityEngine;
using System.Collections;

public class FillScreen : MonoBehaviour {

	private int screenWidth;
	private int screenHeight;
 
	void Update() {
 
//		if(Screen.width != screenWidth || Screen.height != screenHeight) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			ResizeSpriteToScreen();
	//	}
	}

	void ResizeSpriteToScreen() {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		if(sr == null)
			return;

		transform.localScale = new Vector3(1, 1, 1);

		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
	//	transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
	}
}
