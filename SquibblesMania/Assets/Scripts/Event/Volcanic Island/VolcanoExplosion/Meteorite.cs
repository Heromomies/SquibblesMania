using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class Meteorite : MonoBehaviour
{
	public float speedTurnAround;
	public int lifeParticle;

	private Rigidbody _rb;
	private int _turn;
	
	public bool stopRotating;
	private GameObject _particleFireToDelete;
	private const string UntaggedString = "Untagged";
	private const string PlatformString = "Platform";
	private const float DelayDeactivate = 0.5f;
	private static Vector3 _meteorite = new Vector3(1500, -1500, 1500);
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	public void ChangeTurn()
	{
		if (_turn != 0 && GameManager.Instance.turnCount >= _turn + lifeParticle)
		{
			var node = _particleFireToDelete.GetComponentInParent<Node>();
			node.isActive = true;
			
			Debug.Log(node.isActive, node);
			
			_particleFireToDelete.gameObject.SetActive(false);
			node.gameObject.layer = 3;
			if (node.colorBloc == Node.ColorBloc.None)
			{
				node.gameObject.tag = UntaggedString;
			}
			else
			{
				node.gameObject.tag = PlatformString;
			}

			StartCoroutine(DeactivateParticle(DelayDeactivate, gameObject));
		}
	}

	private void Update()
	{
		if (!stopRotating)
		{
			var rotate = Random.Range(DelayDeactivate, 3f);
			transform.Rotate(new Vector3(rotate,rotate,rotate) * (speedTurnAround * Time.deltaTime), Space.World);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			var player = other.gameObject.GetComponent<PlayerStateManager>();
			if (!player.isPlayerStun)
			{
				AudioManager.Instance.Play("Stun");
				
				player.vfxStun = PoolManager.Instance.SpawnObjectFromPool("StunVFX", other.transform.position + new Vector3(0, 1, 0), Quaternion.identity, other.transform);
			
				PlayerStateEventManager.Instance.PlayerStunTriggerEnter(player, 1);
			
				if (GameManager.Instance.currentPlayerTurn.isPlayerStun) 
				{ 
					PlayerStateEventManager.Instance.PlayerStunTextTriggerEnter(GameManager.Instance.actualCamPreset.presetNumber, true);
				}
			}
			
			StartCoroutine(SetActiveFalseBullet(0.01f));
		}
		else
		{
			StartCoroutine(SetActiveFalseBullet(2f));

			var otherNode = other.gameObject.GetComponent<Node>();
			
			if (otherNode != null)
			{
				VolcanoManager.Instance.meteorites.Add(this);
				
				otherNode.isActive = false;
			
				transform.parent = other.transform;
				_turn = GameManager.Instance.turnCount;

				AudioManager.Instance.Play("FireballEnd");
			
				transform.rotation = new Quaternion(0,0,0,0);
				stopRotating = true;

				var posPlayer = transform.position;
			
				var explosionPS = PoolManager.Instance.SpawnObjectFromPool("ExplosionVFXMeteorite", posPlayer, Quaternion.identity, null);
				StartCoroutine(DeactivateParticle(2f, explosionPS));
				
				var otherPosition = other.transform.position;
				var firePS = PoolManager.Instance.SpawnObjectFromPool("FireVFXMeteorite",
					new Vector3(otherPosition.x, otherPosition.y + 1.25f, otherPosition.z), Quaternion.identity, other.transform);
				_particleFireToDelete = firePS;
			
				_rb.constraints = RigidbodyConstraints.FreezeAll;
			
				transform.position = new Vector3(otherPosition.x, posPlayer.y -0.1f, otherPosition.z);

				transform.rotation = other.transform.rotation;
			}
		}
	}

	IEnumerator DeactivateParticle(float delay, GameObject goToDeactivate)
	{
		yield return new WaitForSeconds(delay);
		goToDeactivate.SetActive(false);
	}
	
	IEnumerator SetActiveFalseBullet(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		gameObject.transform.position = _meteorite;
		_rb.constraints = RigidbodyConstraints.FreezeAll;
	}
}