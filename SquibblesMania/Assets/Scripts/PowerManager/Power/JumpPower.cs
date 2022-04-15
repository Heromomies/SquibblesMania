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
	[Range(0.0f, 0.1f)] public float speedAnimationCurve;
	
	public AnimationCurve curve;
	
	[Space] public LayerMask layerInteractable;
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

		_particleImpact = BezierAlgorithm.Instance.particleImpact;

		if (_particleImpact != null)
		{
			_particleImpact.SetActive(false);
			_particleImpact = null;
		}

		if (_particleImpulse != null)
		{
			_particleImpulse.SetActive(false);
			_particleImpulse = null;
		}
		
		
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
				var player = GameManager.Instance.currentPlayerTurn;
				var tCurrentPlayerTurn = player.transform;
				
				player.RemoveParentBelowPlayer(tCurrentPlayerTurn);
				
				var posHitInfo = hitInfo.transform.position;

				var playerPos = tCurrentPlayerTurn.position;
				
				var xSpawn = (posHitInfo.x + playerPos.x) /2;
				var zSpawn = (posHitInfo.z + playerPos.z) /2;
				
				var listPoint = new List<Vector3>();

				listPoint.Add(playerPos);
				listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
				listPoint.Add(posHitInfo);

				_particleImpulse = PoolManager.Instance.SpawnObjectFromPool("ParticleJumpImpulse", GameManager.Instance.currentPlayerTurn.transform.position, Quaternion.identity, null);
				
				BezierAlgorithm.Instance.ObjectToMoveWithBezierCurve(tCurrentPlayerTurn.gameObject, listPoint, speedAnimationCurve, curve);
				
				var hitInfoTransform = hitInfo.transform.GetComponentInParent<GroupBlockDetection>().transform;

				if (Physics.Raycast(hitInfo.transform.position, Vector3.down, out var hitInfoTwo, Mathf.Infinity))
				{
					player.currentBlockPlayerOn = hitInfoTwo.transform;
					if (hitInfoTwo.collider.CompareTag("Platform"))
					{
						StartCoroutine(WaitPlayerOnBlocBeforeSitDownHim(hitInfoTransform));
					}
					else
					{
						ClearPower();
					}
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	IEnumerator WaitPlayerOnBlocBeforeSitDownHim(Transform hitInfoTransform)
	{
		yield return new WaitForSeconds(2f);

		var hitPosition = hitInfoTransform.position;
		
		if (hitPosition.y - 1 >= GameManager.Instance.minHeightBlocMovement)
		{
			hitInfoTransform.DOMove(new Vector3(hitPosition.x,
				hitPosition.y -1, hitPosition.z), speedBloc);
		}
		
		ClearPower();
	}


	public void DisplayPower()
	{
		var tPosPower = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = tPosPower;

		// ReSharper disable once Unity.PreferNonAllocApi
		collidersMin = Physics.OverlapSphere(tPosPower, radiusMin, layerInteractable); // Detect bloc around the object
		// ReSharper disable once Unity.PreferNonAllocApi
		collidersMax = Physics.OverlapSphere(tPosPower, radiusMax, layerInteractable); // Detect bloc around the object

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
			if (colFinished != null && colFinished.gameObject.GetComponent<Node>() && colFinished.gameObject.GetComponent<Node>().isActive)
			{
				var objPos = colFinished.transform.position;
		
				GameObject obj = PoolManager.Instance.SpawnObjectFromPool("PlanePowerPath",
					new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.identity, colFinished.transform);

				listObjectToSetActiveFalse.Add(obj);
			}
		}
	}
	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(SwapTouchGesture);
		}
	}
	
	public void ClearPower()
	{
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
		
		var player = GameManager.Instance.currentPlayerTurn;
		player.DetectParentBelowPlayer(player.transform);
		
		PowerManager.Instance.ActivateDeactivatePower(2, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
}