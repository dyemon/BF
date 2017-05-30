 using UnityEngine;

[System.Serializable]
public class GameObject2DArray {
     
	[System.Serializable]
	public struct RowData {
		public GameObject[] row;
	}

	public RowData[] data;

	public GameObject[] GetRow(int index) {
		return data[index].row;
	}
 }