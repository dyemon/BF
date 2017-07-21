using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Map1Scene : BaseScene {
	public GameObject[] cities;
	private MapData mapData;

	private GameObject currentCity;

	void Start () {
		mapData = GameResources.Instance.GetMapData();
	//	currentCity = Instantiate(cities[App.GetCurrentCity()], Vector3.zero, Quaternion.identity);
	}
	
	void Update() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			if(EventSystem.current.IsPointerOverGameObject(InputController.GetFingerId())) {
				return;
			}

			Ray ray = InputController.TouchToRay(touches[0]);

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
			if(hit.collider != null) {		
				OnSelectLocation(hit.collider.gameObject);
			}
		}
	}

	void OnSelectLocation(GameObject location) {
		
	}
}
