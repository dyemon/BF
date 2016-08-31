using UnityEngine;
using System.Collections;

public class AnimationParam<T> {
	public T Value { get; set;}

	public AnimationParam(T value) {
		Value = value;
	}
}
