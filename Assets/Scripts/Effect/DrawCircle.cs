using UnityEngine;
using System.Collections;

public class DrawCircle : MonoBehaviour
{
	public GameObject arcObject;
	public Vector2 pCenter = Vector2.zero;
	public float pRadius = 4f;
	[Header("Angles")]
	public float pStartAngle = 0f;
	public float pStopAngle = 180f;
	[Space(10)]
	public float pSegments = 8f;

	// Test stuff
	Vector2 tCenter = Vector2.zero;
	float tRadius = 0f;
	float tStartAngle = 0f;
	float tStopAngle = 0f;
	float tSegments = 0f;


	private void Update()
	{

		// Testing
		if (Input.GetButton("Fire1"))
		{
			pStopAngle += 1;
		}

		if (
			pCenter != tCenter ||
			pRadius != tRadius ||
			pStartAngle != tStartAngle ||
			pStopAngle != tStopAngle ||
			pSegments != tSegments
		)
		{
			EraseCircle();

			DrawArc(pCenter, pRadius, pStartAngle, pStopAngle, pSegments);

			tCenter = pCenter;
			tRadius = pRadius;
			tStartAngle = pStartAngle;
			tStopAngle = pStopAngle;
			tSegments = pSegments;
		}
	}

	void DrawArc(Vector2 midPoint, float radius = 1f, float startAngle = 0f, float stopAngle = 360f, float segments = 8f)
	{
		if (segments <= 0)
		{
			return;
		}

		if (stopAngle < startAngle)
		{
			float t = stopAngle;

			stopAngle = startAngle;
			startAngle = t;
		}

		if (Mathf.Abs(stopAngle - startAngle) > 360)
		{
			stopAngle = startAngle + 360;
		}

		float step = (stopAngle - startAngle) / segments;

		Vector2 extents = arcObject.GetComponent<Renderer>().bounds.extents;

		float scale = 1 / (2 * extents.y);

		float cover = Mathf.Sin(step / 2f * Mathf.Deg2Rad) * extents.x;

		Vector2 currentPoint = midPoint + radius * new Vector2(Mathf.Cos(startAngle * Mathf.Deg2Rad), Mathf.Sin(startAngle * Mathf.Deg2Rad));
		Vector2 nextPoint = midPoint + radius * new Vector2(Mathf.Cos((startAngle + step) * Mathf.Deg2Rad), Mathf.Sin((startAngle + step) * Mathf.Deg2Rad)); ;

		float length = (currentPoint - nextPoint).magnitude;
		Vector3 lScale = arcObject.transform.localScale;

		lScale.y *= (length + cover * 2) * scale;

		float angle = startAngle;
		for (int i = 0; i < segments; i++)
		{
			nextPoint =
				midPoint + radius * new Vector2(Mathf.Cos((angle + step) * Mathf.Deg2Rad), Mathf.Sin((angle + step) * Mathf.Deg2Rad));

			Vector2 dir = (nextPoint - currentPoint).normalized;

			currentPoint = nextPoint;

			nextPoint += dir * cover;

			GameObject go =
				Instantiate(
					arcObject,
					nextPoint,
					Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90)
				) as GameObject;

			go.transform.localScale = lScale;

			go.transform.SetParent(transform);

			go.name = "Arc " + i;

			angle += step;
		}
	}

	void EraseCircle()
	{
		foreach (Transform t in transform.GetComponentsInChildren<Transform>())
		{
			if (t != transform)
			{
				Destroy(t.gameObject);
			}
		}
	}
}
