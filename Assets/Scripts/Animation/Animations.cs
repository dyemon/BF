using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animations {
	
	public static float CreateAwardAnimation(GameObject target, Vector3 start, Vector3 end, Sprite icon, int? val, Vector3? endSize = null) {
		Vector3 direction = (end - start).normalized;
		float dist = Vector3.Distance(start, end);
		Vector3 end1 = start + direction * dist * 0.1f;

		if(endSize == null) {
			endSize = new Vector3(0.82f, 0.82f, 1f);
		}
		target.transform.localScale = Vector3.one;

		if(icon != null) {
			Image img = target.transform.Find("Image").gameObject.GetComponent<Image>();
			img.sprite = icon;
		}
		if(val != null) {
			Text text = target.transform.Find("Text").gameObject.GetComponent<Text>();
			text.text = val.ToString();
		}
		//	text.color = award.Type.ToColor();

		AnimatedObject ao = target.GetComponent<AnimatedObject>();
		float time1 = 0.3f;
		float time2 = 1f;
		float idle = 0f;

		ao.AddMoveByTime(start, end1, time1)
			.AddResize(null, new Vector3(1.4f, 1.4f, 1f), time1)
			.Build()
		//	.AddIdle(idle).Build()
			.AddMoveByTime(null, end, time2, ABase.BezierPoints.EaseInOut)
			.AddResize(null, endSize.Value, time2)
			.Build();
		
		//ao.AddMoveByTime(start, end, 3f).Build();

		return time1 + time2 + idle;
	}
}
