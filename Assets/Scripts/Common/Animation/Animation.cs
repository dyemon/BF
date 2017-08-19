using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Animation {

public class Animation : System.ICloneable {
	private IDictionary<AnimationType, List<ABase>> animations = new Dictionary<AnimationType, List<ABase>>();
	public int? LayerSortingOrder { get; set;}

	public void AddAnimation(AnimationType type, ABase a) {
		if(!animations.ContainsKey(type)) {
			animations.Add(type, new List<ABase>());
		}
		animations[type].Add(a);
	}

	public ABase GetAnimation(AnimationType type) {
		List<ABase> val;
		animations.TryGetValue(type, out val);
		if(val == null || val.Count == 0) {
			return null;
		}
		ABase a = val[0];
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
		foreach(List<ABase> list in animations.Values) {
			if(list.Count > 0) {
				list[0].Run();
			}
		}
	}

	public void Reset() {
		foreach(List<ABase> list in animations.Values) {
			foreach(ABase a in list) {
				a.Reset();
			}
		}
	}

	public object Clone() {
		IDictionary<AnimationType, List<ABase>> newA = new Dictionary<AnimationType, List<ABase>>();
		foreach(KeyValuePair<AnimationType, List<ABase>> pair in animations) {
			List<ABase> newList = new List<ABase>();
			foreach(ABase a in pair.Value) {
				newList.Add((ABase)a.Clone());
			}
			newA[pair.Key] = newList;
		}

		return new Animation() { animations = newA, LayerSortingOrder = this.LayerSortingOrder };
	}
}
}
