using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityData {
	public string Name;
	public string Description;
	public LocationData[] LocationData;

	public int LevelsCountActual {
		get {
			int sum = 0;
			foreach(LocationData item in LocationData) {
				sum += item.LevelsCountActual;
			}

			return sum;
		}
	}

	public void Init() {
		System.Array.Sort<LocationData>(LocationData, (x, y) => {
			return x.AccessOrder.CompareTo(y.AccessOrder);
		});
	
		foreach(LocationData item in LocationData) {
			item.Init();
		}
	}

	public LocationData GetLocationById(string id) {
		foreach(LocationData item in LocationData) {
			if(item.Id == id) {
				return item;
			}
		}

		return null;
	}
}
