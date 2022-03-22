using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Wizama.Hardware.Antenna;

public class PlayerCardState : PlayerBaseState
{
	
	private List<int> _number = new List<int> {1, 2, 6, 7, 8, 9, 10};
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
			
			NFCManager.Instance.charCards = nfcTag.Data.ToCharArray();

			if (nfcTag.Data.Contains("=") || nfcTag.Data.Contains("<") || nfcTag.Data.Contains(";"))
			{
				GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerPowerCardState);
				switch (NFCManager.Instance.charCards[1]) // Check the letter of the card for the color and launch the appropriate power
				{
					case 'B': PowerManager.Instance.ActivateDeactivatePower(0, true); break;
					case 'R': PowerManager.Instance.ActivateDeactivatePower(1, true); break;
					case 'G': PowerManager.Instance.ActivateDeactivatePower(2, true); break;
					case 'Y': PowerManager.Instance.ActivateDeactivatePower(3, true); break;
				}
			}
			if (nfcTag.Data.Contains("3") || nfcTag.Data.Contains("4") || nfcTag.Data.Contains("5"))
			{
				NFCManager.Instance.clicked = true;
				NFCManager.Instance.numberOfTheCard = NFCManager.Instance.charCards[0] - '0';
				GameManager.Instance.currentPlayerTurn.playerActionPoint = NFCManager.Instance.numberOfTheCard;
				
				UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
				GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn
					.PlayerActionPointCardState);
			}

			foreach (var n in _number)
			{
				if (nfcTag.Data.Contains(n.ToString()))
				{
					Camera.main.DOShakePosition(1, 0.3f);
				}
			}
			
			
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
			NFCManager.Instance.SetActivePlayerActionButton(0,false);
			NFCManager.Instance.SetActivePlayerActionButton(1,false);
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
			NFCManager.Instance.SetActivePlayerActionButton(0,false);
			NFCManager.Instance.SetActivePlayerActionButton(1,false);
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
			player.psStun.SetActive(false);
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