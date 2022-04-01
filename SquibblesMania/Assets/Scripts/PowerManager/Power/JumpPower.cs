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
	[Space] public LayerMask layerPowerPath;
	[Space]
	[HideInInspector] public Collider[] collidersMin;
	[HideInInspector] public Collider[] collidersMax;
	public List<Collider> collidersFinished = new List<Collider>();

	[HideInInspector] public List<GameObject> listObjectToSetActiveFalse;
	private GameObject _particleImpact, _particleImpulse;
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

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerPowerPath))
			{
				NFCManager.Instance.powerActivated = true;
				
				var tCurrentPlayerTurn = GameManager.Instance.currentPlayerTurn.transform;

				var posHitInfo = hitInfo.transform.position;
				posHitInfo.y += 1;
				
				var playerPos = tCurrentPlayerTurn.position;
				
				var xSpawn = (posHitInfo.x + playerPos.x) /2;
				var zSpawn = (posHitInfo.z + playerPos.z) /2;
				
				var listPoint = new List<Vector3>();

				listPoint.Add(playerPos);
				listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
				listPoint.Add(posHitInfo);

				_particleImpulse = PoolManager.Instance.SpawnObjectFromPool("ParticleJumpImpulse", GameManager.Instance.currentPlayerTurn.transform.position, Quaternion.identity, null);
				
				BezierAlgorithm.Instance.ObjectToMoveWithBezierCurve(tCurrentPlayerTurn.gameObject, listPoint, 0.02f);
				
				var hitInfoTransform = hitInfo.transform.GetComponentInParent<GroupBlockDetection>().transform;

				StartCoroutine(WaitPlayerOnBlocBeforeSitDownHim(hitInfoTransform));

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
			hitPosition.y -1, hitPosition.z), speedBloc);
	}

	IEnumerator CoroutineClearParticles()
	{
		yield return new WaitForSeconds(3f);
		
		_particleImpact = BezierAlgorithm.Instance.particleImpact;
		
		if(_particleImpact != null)
			_particleImpact.SetActive(false);
		
		if(_particleImpulse != null)
			_particleImpulse.SetActive(false);
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
			if (colFinished != null && colFinished.gameObject.GetComponent<Node>() && colFinished.gameObject.GetComponent<Node>().colorBloc != Node.ColorBloc.None)
			{
				var objPos = colFinished.transform.position;
		
				GameObject obj = PoolManager.Instance.SpawnObjectFromPool("PlanePowerPath",
					new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.identity, colFinished.transform);

				listObjectToSetActiveFalse.Add(obj);
			}
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
		StartCoroutine(CoroutineClearParticles());
		
		foreach (var colFinished in listObjectToSetActiveFalse)
		{
			colFinished.SetActive(false);
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
		listObjectToSetActiveFalse.Clear();
		
		PowerManager.Instance.ActivateDeactivatePower(3, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
}