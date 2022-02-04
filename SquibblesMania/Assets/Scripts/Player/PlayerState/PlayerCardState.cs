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
		
		
		
		NFCController.OnNewTag = OnNewTagDetected;
		NFCController.OnTagRemoved = OnTagRemoveDetected;
		NFCController.StartPollingAsync(NFCManager.Instance.antennaPlayerOne);
	}
	private void OnNewTagDetected(NFC_DEVICE_ID device, NFCTag nfcTag)  // When the player put a card on the tablet
	{
		if (!GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
		{
			NFCManager.Instance.SetActiveButton(true);
		
			NFCManager.Instance.charCards = nfcTag.Data.ToCharArray();
			UiManager.Instance.buttonNextTurn.SetActive(false);
			NFCManager.Instance.hasRemovedCard = false;
		}
		else
		{
			Debug.Log("Don't detect another card");
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
	}
	
	private void OnTagRemoveDetected(NFC_DEVICE_ID device, NFCTag nfcTag) // When a card is removed
	{
		if(GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && NFCManager.Instance.clicked)
		{
			NFCManager.Instance.SetActiveButton(false);
			NFCManager.Instance.textTakeOffCard.text = "";
			UiManager.Instance.buttonNextTurn.SetActive(true);
			NFCController.StopPolling();
		} 
		if(GameManager.Instance.currentPlayerTurn.playerActionPoint > 0 && !NFCManager.Instance.clicked)
		{
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && !NFCManager.Instance.clicked)
		{
			NFCManager.Instance.SetActiveButton(false);
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
		
		NFCManager.Instance.hasRemovedCard = true;
	}
	
	void PlayerIsStun(PlayerStateManager player)
	{
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
		// if player touch the power button
		if (Input.GetKeyDown(KeyCode.A))
		{
			player.SwitchState(player.PlayerPowerCardState);
		}
		
		if (NFCManager.Instance.hasRemovedCard && GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && NFCManager.Instance.clicked)
		{
			UiManager.Instance.buttonNextTurn.SetActive(true);
			NFCManager.Instance.textTakeOffCard.text = "";
			NFCManager.Instance.clicked = false;
			NFCManager.Instance.hasRemovedCard = false;
		} 
		
		
	}

	public override void ExitState(PlayerStateManager player)
	{
	
	}
}