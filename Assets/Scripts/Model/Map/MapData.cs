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

	public Vector3 GetLocation(int level) {
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

		return new Vector3(curCity, curLocation, level - levelSum);
	}

	public int GetLevel(int city, int location, int locationLevel) {
		int levelSum = locationLevel;
		bool find = false;

		for(int i = 0; i <= CityData.Length; i++) {
			if(i == city - 1) {
				for(int k = 0; k <= CityData[i].LocationData.Length; k++) {
					if(k == location - 1) {
						find = true;
						goto find;
					}

					levelSum += CityData[i].LocationData[k].LevelsCount;
				}
			}

			levelSum += CityData[i].LevelsCount;
		}

		find:
		return find ? levelSum : -1; 
	}

	public int GetMaxLevel() {
		int sum = 0;

		foreach(CityData city in CityData) {
			sum += city.LevelsCount;
		}

		return sum;
	}


}
