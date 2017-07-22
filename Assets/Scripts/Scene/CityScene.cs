using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CityScene : BaseScene {
	public const string SceneName = "City";

	public GameObject[] cities;
	public Sprite UnavaliableLocationIcon;
	public Sprite CurrentLocationIcon;

	public Text DescriptionText;
	public Text NameText;

	private MapData mapData;
	private GameObject currentCity;

	private int maxLevel;

	void Start () {
		mapData = GameResources.Instance.GetMapData();
		maxLevel = mapData.GetMaxLevel();

		currentCity = Instantiate(cities[App.CurrentCity - 1], Vector3.zero, Quaternion.identity);
		UpdateLocationAvailability();
	
		DescriptionText.text = mapData.CityData[App.CurrentCity - 1].Description;
		NameText.text = mapData.CityData[App.CurrentCity - 1].Name;
	}
	
	void Update() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0 && touches[0].phase == TouchPhase.Began) {
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
		Debug.Log(location.name);
		LocationData lData = mapData.CityData[App.CurrentCity - 1].GetLocationById(location.name);
		Preconditions.NotNull(lData, "Can not get location data for id " + location.name);
		int userLevel = 89;//GameResources.Instance.GetUserData().Level;
		Vector2 locationParams = mapData.CalcLocationParams(userLevel);

		bool avaliable = userLevel > maxLevel;
		if(!avaliable) {
			avaliable = 0 <= DetectLocationAvailability((int)locationParams.x, (int)locationParams.y, App.CurrentCity, lData.AccessOrder);
		}

		if(!avaliable) {
			return;
		}

		App.CurrentLocation = lData.AccessOrder;
	/*	location.transform.Find("Point").GetComponent<SpriteRenderer>().sprite = null;
		AnimatedObject ao = location.AddComponent<AnimatedObject>();
		ao.AddMove(null, new Vector3(0, 0, location.transform.position.z), 0.5f)
			.AddResize(null, new Vector3(1.5f, 1.5f, 1), 0.5f)
			.Build().Run();
				*/
	}

	void UpdateLocationAvailability() {
		UserData uData = GameResources.Instance.GetUserData();
	//	App.CurrentCity = 2;
		int usaerLevel = uData.Level;
		if(usaerLevel > maxLevel) {
			return;
		}

		Vector2 locationParams = mapData.CalcLocationParams(usaerLevel);

		foreach(LocationData lData in mapData.CityData[App.CurrentCity - 1].LocationData) {
			Transform currentLocation = currentCity.transform.Find(lData.Id);
			int compare = DetectLocationAvailability((int)locationParams.x, (int)locationParams.y, App.CurrentCity, lData.AccessOrder);
			Sprite sp = (compare == -1) ? UnavaliableLocationIcon : (compare == 0) ? CurrentLocationIcon : null;
		
			SetLocationAvailabilityIcon(sp, currentLocation);
		}
	}

	void SetLocationAvailabilityIcon(Sprite icon, Transform location) {
		Transform point = location.Find("Point");
		point.localScale = new Vector3(0.7f, 0.7f, 1);
		SpriteRenderer renderer	= point.GetComponent<SpriteRenderer>();
		renderer.sprite = icon;
	}

	int DetectLocationAvailability(int userCity, int userLocation, int currentCity, int currentLocation) {
		if(userCity == currentCity && userLocation == currentLocation) {
			return 0;
		}
		if(currentCity > userCity) {
			return -1;
		}
		if(currentCity < userCity) {
			return 1;
		}

		return (userLocation >= currentLocation) ? 1 : -1;
	}
}
