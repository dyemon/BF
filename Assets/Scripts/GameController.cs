using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public int numColumns = 7;
	public int numRows = 7;
	public GameObject tileItem;
	public Vector3 prevPos;

	void Start() {
		//       SpawnTileItems();
		prevPos = tileItem.transform.position;
	}
	// Update is called once per frame
	void Update() {
		ProcessInput();
	}

	void SpawnTileItems() {
		for(float i = -numColumns/2f + 0.5f; i < numColumns/2f; i++) {
			Instantiate(tileItem, new Vector3(i, 0.5f, 0), Quaternion.identity);
		}
	}

	private void ProcessInput() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			InputController.Touch touch = touches[0];
			if(touch.phase == TouchPhase.Began) {
				Ray ray = InputController.TouchToRay(touches[0]);
				Debug.Log(touches[0]);
			
				AnimatedObject aObj = Predicates.NotNull( tileItem.GetComponent<AnimatedObject>() );
				aObj.AddMove(prevPos, ray.origin, App.moveSpeed)
					.Build().Run();

				prevPos = ray.origin;
			}
		}
	}
}
