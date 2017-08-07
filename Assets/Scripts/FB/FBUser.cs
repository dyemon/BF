using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBUser {
	public string Name;
	public string Id;
	public string PictureUrl;

	public FBUser(System.Object obj) {
		Dictionary<string,System.Object> data = (Dictionary<string,System.Object>)obj; 

		Name = (string)data["name"];
		Id = (string)data["id"];

		object pictureObj;
		if (data.TryGetValue("picture", out pictureObj))
		{
			var pictureData = (Dictionary<string, object>)(((Dictionary<string, object>)pictureObj)["data"]);
			PictureUrl = (string)pictureData["url"];
		}

		Debug.Log(PictureUrl);
	}
}
