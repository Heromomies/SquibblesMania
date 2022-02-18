using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Antenna;

public class PlayerCardState : PlayerBaseState
{
	//The state when player put card on Square one
	public override void EnterState(PlayerStateManager player)
	{
		//Turn of player x
		//Message player turn x "Put a card on the corresponding surface"
	
		if (player.isPlayerStun)
		{
			player.stunCount--;
			PlayerIsStun(player);
			
		}

		if (player.isPlayerShielded)
		{
			player.shieldCount--;
		}
		
		//If the current player is this player
		if (GameManager.Instance.currentPlayerTurn == player)
		{
			player.indicatorPlayer.SetActive(true);
			NFCController.OnNewTag = OnNewTagDetected;
			NFCController.OnTagRemoved = OnTagRemoveDetected;
			NFCController.StartPollingAsync(NFCManager.Instance.antennaPlayerOne);
		}

	}
	private void OnNewTagDetected(NFC_DEVICE_ID device, NFCTag nfcTag)  // When the player put a card on the tablet
	{
		
		if (GameManager.Instance.currentPlayerTurn.CurrentState == GameManager.Instance.currentPlayerTurn.PlayerCardState && !NFCManager.Instance.clicked)
		{
			NFCManager.Instance.SetActivePlayerActionButton(true);
			NFCManager.Instance.charCards = nfcTag.Data.ToCharArray();
			UiManager.Instance.buttonNextTurn.SetActive(false);
			NFCManager.Instance.hasRemovedCard = false;
		}
		else
		{
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
	}
	
	private void OnTagRemoveDetected(NFC_DEVICE_ID device, NFCTag nfcTag) // When a card is removed
	{
		
		if(GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && NFCManager.Instance.clicked)
		{
			NFCManager.Instance.SetActivePlayerActionButton(false);
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false); break;
				case 2: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false); break;
				case 3: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false); break;
				case 4: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false); break;
			}
			UiManager.Instance.buttonNextTurn.SetActive(true);
			NFCController.StopPolling();
		} 
		
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && !NFCManager.Instance.clicked)
		{
			NFCManager.Instance.SetActivePlayerActionButton(false);
		}

		if (GameManager.Instance.currentPlayerTurn.isPlayerShielded && GameManager.Instance.currentPlayerTurn.shieldCount == 0)
		{
			GameManager.Instance.currentPlayerTurn.gameObject.layer = 6;
			GameManager.Instance.currentPlayerTurn.isPlayerShielded = false;
		}
		
		NFCManager.Instance.hasRemovedCard = true;
	}
	
	void PlayerIsStun(PlayerStateManager player)
	{
		player.indicatorPlayer.SetActive(false);
		//If the stunCount is less than zero player is now not stun
		if (player.stunCount <= 0)
		{
			player.isPlayerStun = false;
		}

		switch (player.playerNumber)
		{
			case 0:
				GameManager.Instance.ChangePlayerTurn(1);

				break;
			case 1:
				GameManager.Instance.ChangePlayerTurn(2);

				break;
			case 2:
				GameManager.Instance.ChangePlayerTurn(3);

				break;
			case 3:
				GameManager.Instance.ChangePlayerTurn(0);
				break;
		}
	}

	public override void UpdateState(PlayerStateManager player)
	{
		
		if (NFCManager.Instance.hasRemovedCard && GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && NFCManager.Instance.clicked)
		{
			
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false); break;
				case 2: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false); break;
				case 3: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false); break;
				case 4: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false); break;
			}
			NFCManager.Instance.clicked = false;
			NFCManager.Instance.hasRemovedCard = false;
		} 
		
		
	}

	public override void ExitState(PlayerStateManager player)
	{
	
	}
}