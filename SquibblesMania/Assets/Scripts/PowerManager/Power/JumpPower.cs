using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")]
	[Range(0.0f, 4.0f)] public int radiusMin;
	[Range(0.0f, 5.0f)] public int radiusMax;
	[Range(0.0f, 3.0f)] public float speedBloc;
	[Range(0.0f, 10.0f)] public float ySpawn;
	
	[Space] public LayerMask layer;
	[Space] public LayerMask layerBlocTouched;
	[Space]
	[HideInInspector] public Collider[] collidersMin;
	[HideInInspector] public Collider[] collidersMax;
	public List<Collider> collidersFinished = new List<Collider>();
	[Header("MATERIAL SETTINGS")]
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

		DisplayPower();
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
				NFCManager.Instance.powerActivated = true;
				
				var tCurrentPlayerTurn = GameManager.Instance.currentPlayerTurn.transform;
				var posHitInfo = hitInfo.transform.position;

				var playerPos = tCurrentPlayerTurn.position;

				var xSpawn = (posHitInfo.x + playerPos.x) /2;
				var zSpawn = (posHitInfo.z + playerPos.z) /2;

				var listPoint = new List<Vector3>();

				listPoint.Add(playerPos);
				listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
				listPoint.Add(posHitInfo);

				BezierAlgorithm.Instance.ObjectToMoveWithBezierCurve(tCurrentPlayerTurn.gameObject, listPoint, 0.02f);
				
				var hitInfoTransform = hitInfo.transform.GetComponentInParent<GroupBlockDetection>().transform;
				
				if (hitInfo.collider.CompareTag("Platform"))
				{
					StartCoroutine(WaitPlayerOnBlocBeforeSitDownHim(hitInfoTransform));
				}

				ClearPower();
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	IEnumerator WaitPlayerOnBlocBeforeSitDownHim(Transform hitInfoTransform)
	{
		yield return new WaitForSeconds(1.5f);
		
		var hitPosition = hitInfoTransform.position;
		hitInfoTransform.DOMove(new Vector3(hitPosition.x,
			hitPosition.y - 1, hitPosition.z), speedBloc);
	}

	public void DisplayPower()
	{
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
			var color = colFinished.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
			color = secondMat.color;
			colFinished.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor",color);
			
			colFinished.gameObject.layer = 10;
		}
	}

	public void CancelPower()
	{
		
	}

	public void DoPower()
	{
		
	}

	public void ClearPower()
	{
		foreach (var colFinished in collidersFinished)
		{
			colFinished.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", firstMat.color);
			
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
		
		PowerManager.Instance.ActivateDeactivatePower(2, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
}