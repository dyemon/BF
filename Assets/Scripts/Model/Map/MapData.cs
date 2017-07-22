using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData {
	public CityData[] CityData;

	public void Init() {
		foreach(CityData item in CityData) {
			item.Init();
		}
	}

	public  Vector2 CalcLocationParams(int level) {
		int curCity = 1;
		int curLocation = 1;
		int levelSum = 0;

		foreach(CityData city in CityData) {
			if(levelSum + city.LevelsCount >= level) {
				foreach(LocationData location in city.LocationData) {
					if(levelSum + location.LevelsCount >= level) {
						goto find;
					}

					levelSum += location.LevelsCount;
					curLocation++;
				}
			}

			levelSum += city.LevelsCount;
			curCity++;
		}

		find:

		if(curCity > CityData.Length) {
			curCity = CityData.Length;
		}

		return new Vector2(curCity, curLocation);
	}

	public int GetMaxLevel() {
		int sum = 0;

		foreach(CityData city in CityData) {
			sum += city.LevelsCount;
		}

		return sum;
	}
}
