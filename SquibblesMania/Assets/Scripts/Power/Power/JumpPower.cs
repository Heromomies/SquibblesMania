using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpPower : MonoBehaviour
{
	[Header("POWER SETTINGS")]
	[Range(0.0f, 4.0f)] public int radiusMin;
	[Range(0.0f, 5.0f)] public int radiusMax;
	[Range(0.0f, 3.0f)] public float speed;
	
	[Space] public LayerMask layer;
	[Space] public LayerMask layerBlocTouched;
	[Space]
	[HideInInspector] public Collider[] collidersMin;
	[HideInInspector] public Collider[] collidersMax;
	public List<Collider> collidersFinished = new List<Collider>();
	[Space]
	public Material firstMat;
	public Material secondMat;
	
	private PanGestureRecognizer SwapTouchGesture { get; set; }
	private Camera _cam;
	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
	
	void OnEnable()
	{
		_cam = Camera.main;
		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);

		var tPosPower = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = tPosPower;

		// ReSharper disable once Unity.PreferNonAllocApi
		collidersMin = Physics.OverlapSphere(tPosPower, radiusMin, layer); // Detect bloc around the object
		// ReSharper disable once Unity.PreferNonAllocApi
		collidersMax = Physics.OverlapSphere(tPosPower, radiusMax, layer); // Detect bloc around the object

		collidersFinished.AddRange(collidersMin);

		foreach (var coll in collidersMax)
		{
			if (collidersFinished.Contains(coll))
			{
				collidersFinished.Remove(coll);
			}
			else
			{
				collidersFinished.Add(coll);
			}
		}

		foreach (var colFinished in collidersFinished)
		{
			PulsingBloc.PulsingEmissiveColorSquareBloc(firstMat.color, secondMat.color, colFinished.transform, 0.2f);
			colFinished.gameObject.layer = 10;
		}
	}

	private void PlayerTouchGestureUpdated(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);
			
			EventSystem.current.RaycastAll(p, _raycast);
			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerBlocTouched))
			{
				var tCurrentPlayerTurn = GameManager.Instance.currentPlayerTurn.transform;
				var posHitInfo = hitInfo.transform.position;
				
				tCurrentPlayerTurn.position = new Vector3(posHitInfo.x, posHitInfo.y +0.5f, posHitInfo.z);

				hitInfo.transform.GetComponentInParent<GroupBlockDetection>().transform.position += Vector3.down;
				/*var rbPlayer = GameManager.Instance.currentPlayerTurn.GetComponent<Rigidbody>();
				GameManager.Instance.currentPlayerTurn.isInJump = true;
				
				rbPlayer.constraints = RigidbodyConstraints.None;
				rbPlayer.constraints = RigidbodyConstraints.FreezeRotation;
				
				Vector3 vo = CalculateVelocity(hitInfo.transform.position, transform.position, speed);
				GameManager.Instance.currentPlayerTurn.transform.rotation = Quaternion.LookRotation(vo);
				rbPlayer.velocity = vo;*/
				Clear();
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	private void Clear()
	{
		foreach (var colFinished in collidersFinished)
		{
			colFinished.GetComponent<Renderer>().materials[2] = firstMat;
			colFinished.gameObject.layer = 3;
		}
		
		for ( int i = 0; i < collidersMax.Length; i++)
		{
			collidersMax[i] = null;
		}
		for ( int i = 0; i < collidersMin.Length; i++)
		{
			collidersMin[i] = null;
		}
		collidersFinished.Clear();
	}
	Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float velocity) // Function to make a parabola
	{
		//define the distance x and y first
		Vector3 distance = target - origin;
		Vector3 distanceXZ = distance;
		distanceXZ.Normalize();
		distanceXZ.y = 0;

		//creating a float that represents our distance 
		float sy = distance.y;
		float sxz = distance.magnitude;

		//calculating initial x velocity
		//Vx = x / t

		float vxz = sxz / velocity;

		////calculating initial y velocity
		//Vy0 = y/t + 1/2 * g * t

		float vy = sy / velocity + 0.8f * Mathf.Abs(Physics.gravity.y) * velocity;
		Vector3 result = distanceXZ * vxz;
		result.y = vy;

		return result;
	}
}