using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class NFCManager : MonoBehaviour
{
	#region ANTENNA SETTINGS

	[Header("ANTENNA SETTINGS")] public NFC_DEVICE_ID[] antennaPlayerOne;
	[Space] public NFC_DEVICE_ID[] antennaPlayerTwo;
	[Space] public NFC_DEVICE_ID[] antennaPlayerThree;
	[Space] public NFC_DEVICE_ID[] antennaPlayerFour;

	#endregion

	#region UI SETTINGS

	[Space] [Header("UI SETTINGS")] public ActionPlayerPreset[] actionPlayerPreset;

	[Serializable]
	public struct ActionPlayerPreset
	{
		public TextMeshProUGUI textTakeOffCard;
	}

	#endregion

	#region LIGHT SETTINGS

	[Space] [Header("LIGHT SETTINGS")] public LIGHT_INDEX[] lightIndexesPlayerOne;
	public LIGHT_INDEX[] lightIndexesPlayerTwo;
	public LIGHT_INDEX[] lightIndexesPlayerThree;
	public LIGHT_INDEX[] lightIndexesPlayerFour;
	public List<LIGHT_COLOR> lightColor;

	public List<LIGHT_INDEX> fullIndex;

	#endregion

	#region PRIVATE VAR

	[HideInInspector] public int numberOfTheCard;
	public char[] charCards;
	public bool newCardDetected;
	public bool powerActivated;
	public bool displacementActivated;
	[HideInInspector] public int changeColor;

	#endregion

	#region Singleton

	private static NFCManager nfcManager;

	public static NFCManager Instance => nfcManager;

	private WaitForSeconds _timeBetweenTwoLight = new WaitForSeconds(0.5f);

	private WaitForSeconds _timeAntennaOneByOne = new WaitForSeconds(0.2f);
	// Start is called before the first frame update

	private void Awake()
	{
		nfcManager = this;
	}

	#endregion

	public void PlayerChangeTurn() // When we change the turn of the player, the color and the antenna who can detect change too
	{
		StopAllCoroutines();
		NFCController.StopPolling();

		switch (GameManager.Instance.currentPlayerTurn.playerNumber)
		{
			case 0:
				NFCController.StartPollingAsync(antennaPlayerOne);
				LightController.Colorize(lightIndexesPlayerOne, lightColor[0], false);
				break;
			case 1:
				NFCController.StartPollingAsync(antennaPlayerTwo);
				LightController.Colorize(lightIndexesPlayerTwo, lightColor[1], false);
				break;
			case 2:
				NFCController.StartPollingAsync(antennaPlayerThree);
				LightController.Colorize(lightIndexesPlayerThree, lightColor[0], false);
				break;
			case 3:
				NFCController.StartPollingAsync(antennaPlayerFour);
				LightController.Colorize(lightIndexesPlayerFour, lightColor[1], false);
				break;
		}
	}

	private IEnumerator ColorOneRange(LIGHT_INDEX[] lightIndex) // Color One range with different colors
	{
		for (int i = 0; i < lightColor.Capacity; i++)
		{
			if (i == lightColor.Capacity - 1)
			{
				i = 0;
			}

			LightController.Colorize(lightIndex, lightColor[i], false);
			yield return _timeBetweenTwoLight;
		}
	}

	public IEnumerator ColorOneByOneAllTheAntennas() // Color One by One all the antennas 
	{
		for (int i = 0; i < fullIndex.Count; i++)
		{
			changeColor++;
			if (i == fullIndex.Count - 1)
			{
				i = 0;
			}

			if (changeColor == lightColor.Count)
			{
				changeColor = 0;
			}

			LightController.ColorizeOne(fullIndex[i], lightColor[changeColor], false);
			yield return _timeAntennaOneByOne;
		}
	}

	private void OnDisable() // Stop polling on disable, can't detect card
	{
		NFCController.StopPolling();
	}
}