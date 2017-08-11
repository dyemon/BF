using UnityEngine;
using System.Collections;

namespace Common.Animation {
public abstract class ABase {
	public abstract bool Animate(GameObject gameObject);
	public abstract void Run();
	public abstract bool IsCompleteAnimation();

	public class BezierPoints {
		public Vector2 p1;
		public Vector2 p2;

		public static readonly BezierPoints EaseInOut = new BezierPoints(0.42f, 0f, 0.58f, 1f);

		public BezierPoints(float x1, float y1, float x2, float y2) {
			this.p1 = new Vector2(x1, y1);
			this.p2 = new Vector2(x2, y2);
		}
	}

	private BezierPoints bezierPoints;

	public void SetBezierPoints(BezierPoints bezierPoints) {
		this.bezierPoints = bezierPoints;
	}

	public Vector3 LerpBezier(Vector3 start, Vector3 end, float t) {
		Vector3 p1 = start + new Vector3(100, 0, 0);
		Vector3 p2 = start + new Vector3(0, 100, 0);
		return CalculateCubicBezierPoint(t, start, p1, p2, end);
	}

	protected float SmothTime(float t) {
		if(bezierPoints == null) {
			return t;
		}

		Vector2 res = CalculateCubicBezierPoint(t, Vector2.zero, bezierPoints.p1, bezierPoints.p2, Vector2.one);
		return res.y;
	}

	Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector2 p = uuu * p0; 
		p += 3 * uu * t * p1; 
		p += 3 * u * tt * p2; 
		p += ttt * p3; 

		return p;
	}
	/*
	Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0; 
		p += 3 * uu * t * p1; 
		p += 3 * u * tt * p2; 
		p += ttt * p3; 

		return p;
	}
	*/
}
}