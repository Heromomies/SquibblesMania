using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierAlgorithm : MonoBehaviour
{
	private GameObject _go;
	private List<Vector3> _points;
	private float _f;
	private bool _canMove;

	private float _timeLeft;
	private static BezierAlgorithm _instance = null;
     
	// Game Instance Singleton
	public static BezierAlgorithm Instance
	{
		get
		{ 
			return _instance; 
		}
	}
     
	private void Awake()
	{
		// if the singleton hasn't been initialized yet
		if (_instance != null && _instance != this) 
		{
			Destroy(this.gameObject);
		}
 
		_instance = this;
	}

	
	// Casteljau's algorithm based on the implementation in: "3D Math Primer for Graphics and Game Development"
	public static Vector3 Curve(List<Vector3> points, float time)
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
				allPoints[i] = allPoints[i] * (1.0f - time) + allPoints[i + 1] * time;
			}
		}

		return allPoints[0];
	}

	public void ObjectToMoveWithBezierCurve(GameObject objectToMove, List<Vector3> pointsList, float f)
	{
		_go = objectToMove;
		_points = pointsList;
		_f = f;

		_canMove = true;
	}

	private void Update()
	{
		if (_canMove)
		{
			_timeLeft += _f;
			if (_timeLeft < 1)
			{
				_go.transform.position = Curve(_points, _timeLeft);
			}
			else
			{
				_canMove = false;
				_go = null;
				_points.Clear();
				_f = 0f;
				_timeLeft = 0f;
			}
		}
	}

	// Example code to visualize the trajectory
	// The "m_curvePoints" variable is a List<Vector3> that holds the curve's control points.
	
	private void OnDrawGizmos()
	{
		if (_points == null)
		{
			return;
		}
		else if (_points.Count == 0)
		{
			return;
		}

		const float delta = 0.01f;
		var previous = _points[0];

		for (float t = delta; t <= 1.0f; t += delta)
		{
			var point = Curve(_points, t);
			
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(previous, point);

			previous = point;
		}
	}
	
}