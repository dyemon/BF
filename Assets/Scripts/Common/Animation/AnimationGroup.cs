using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationGroup : MonoBehaviour {
	public delegate void CompleteAnimation<T>(T param);

	private IList<AnimatedObject> objects = new List<AnimatedObject>();

	public void Add(AnimatedObject o) {
		objects.Add(o);
	}

	public void Clear() {
		objects.Clear();
	}

	public void Run<T>(CompleteAnimation<T> complete, T param) {
		StartCoroutine(RunAnimation(complete, param));
	}

	public void Run() {
		StartCoroutine(RunAnimation(DoNothing, 5));
	}

	private IEnumerator RunAnimation<T>(CompleteAnimation<T> complete, T param) {
		yield return null;
		foreach(AnimatedObject o in objects) {
			o.Run();
		}

		bool done = false;
		while(!done) {
			done = true;
			foreach(AnimatedObject o in objects) {
				if(!o.IsDone) {
					done = false;
					break;
				}
			}
			if(!done) {
				yield return null;
			}
		}

		if(complete != null) {
			complete(param);
		}
	}

	public static void DoNothing(int param) {

	}

	public bool AnimationExist() {
		return objects.Count > 0;
	}
}
