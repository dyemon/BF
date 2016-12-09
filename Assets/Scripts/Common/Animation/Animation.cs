using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation {
	private IDictionary<AnimationType, List<IABase>> animations = new Dictionary<AnimationType, List<IABase>>();
	public int? LayerSortingOrder { get; set;}

	public void AddAnimation(AnimationType type, IABase a) {
		if(!animations.ContainsKey(type)) {
			animations.Add(type, new List<IABase>());
		}
		animations[type].Add(a);
	}

	public IABase GetAnimation(AnimationType type) {
		List<IABase> val;
		animations.TryGetValue(type, out val);
		if(val == null || val.Count == 0) {
			return null;
		}
		IABase a = val[0];
		if(!a.IsCompleteAnimation()) {
			return a;
		}
		val.Remove(a);
		if( (val.Count == 0)) {
			return null;
		}
		val[0].Run();

		return val[0];
	}
		
	public void Run() {
		foreach(List<IABase> list in animations.Values) {
			if(list.Count > 0) {
				list[0].Run();
			}
		}
	}
}

