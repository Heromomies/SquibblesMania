using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class BezierAlgorithm : MonoBehaviour
{
	public List<Transform> points;
	
	public List<Vector3> m_curvePoints;


	public GameObject goToMove;
	private void Start()
	{
		for (int i = 0; i < points.Count; i++)
		{
			m_curvePoints.Add(points[i].position);
		}

		Curve(m_curvePoints, 0.5f);
		
		goToMove.transform.position = Vector3.Lerp(m_curvePoints[0], m_curvePoints[m_curvePoints.Count-1], 5f);
	}

	// Casteljau's algorithm based on the implementation in: "3D Math Primer for Graphics and Game Development"
	public static Vector3 Curve(List<Vector3> points, float t)
	{
		Vector3[] allPoints = points.ToArray();
		int n = allPoints.Length;

		if (n == 0)
		{
			Debug.LogError("No positions specified!");
			return Vector3.zero;
		}
		else if (n < 3)
		{
			Debug.LogError("Not enough positions for a Bezier curve!");
			return Vector3.zero;
		}

		while (n > 1)
		{
			--n;

			// Perform the next round of interpolation, reducing the degree of the curve by one
			for (int i = 0; i < n; ++i)
			{
				allPoints[i] = allPoints[i] * (1.0f - t) + allPoints[i + 1] * t;
			}
		}

		return allPoints[0];
	}

	// Example code to visualize the trajectory
	// The "m_curvePoints" variable is a List<Vector3> that holds the curve's control points.
	//
	private void OnDrawGizmos()
	{
		if (m_curvePoints == null)
		{
			return;
		}
		else if (m_curvePoints.Count == 0)
		{
			return;
		}

		const float delta = 0.01f;
		Vector3 previous = m_curvePoints[0];

		for (float t = delta; t <= 1.0f; t += delta)
		{
			Vector3 point = Curve(m_curvePoints, t);
			
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(previous, point);

			previous = point;
		}
	}
}