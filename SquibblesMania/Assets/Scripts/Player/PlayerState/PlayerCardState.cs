using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class PlayerCardState : PlayerBaseState
{
	private List<int> _number = new List<int> {1, 2, 6, 7, 8, 9, 10};
	private int _maxNumberOfTheCard;
	private char _charCardsStockage;
	private char _cardNumber;
	
	private CardEffect _cardEffect = new CardEffect();

	private PlayerStateManager _currentPlayer;

	//The state when player put card on Square one
	public override void EnterState(PlayerStateManager player)
	{
		//Turn of player x
		//Message player turn x "Put a card on the corresponding surface"
		_currentPlayer = player;
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

	private void OnNewTagDetected(NFC_DEVICE_ID device, NFCTag nfcTag) // When the player put a card on the tablet
	{
		if (!NFCManager.Instance.newCardDetected)
		{
			NFCManager.Instance.charCards = nfcTag.Data.ToCharArray();

			if (nfcTag.Data.Contains("=") || nfcTag.Data.Contains("<") || nfcTag.Data.Contains(";"))
			{
				AudioManager.Instance.Play("CardTrue");
				NFCManager.Instance.newCardDetected = true;

				GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerPowerCardState);
				switch (NFCManager.Instance.charCards[1]) // Check the letter of the card for the color and launch the appropriate power
				{
					case 'B': PowerManager.Instance.ActivateDeactivatePower(0, true);
						ChangeColorLight(LIGHT_COLOR.COLOR_BLUE, _currentPlayer);
						break;
					case 'R':
						PowerManager.Instance.ActivateDeactivatePower(1, true);
						ChangeColorLight(LIGHT_COLOR.COLOR_RED, _currentPlayer);
						break;
					case 'Y':
						PowerManager.Instance.ActivateDeactivatePower(2, true);
						ChangeColorLight(LIGHT_COLOR.COLOR_YELLOW, _currentPlayer);
						break;
					case 'G':
						PowerManager.Instance.ActivateDeactivatePower(3, true);
						ChangeColorLight(LIGHT_COLOR.COLOR_GREEN, _currentPlayer);
						break;
				}
			}
			else if (nfcTag.Data.Contains("3") || nfcTag.Data.Contains("4") || nfcTag.Data.Contains("5"))
			{
				AudioManager.Instance.Play("CardTrue");
				NFCManager.Instance.newCardDetected = true;

				_maxNumberOfTheCard = NFCManager.Instance.charCards[0] - '0';

				NFCManager.Instance.numberOfTheCard = NFCManager.Instance.charCards[0] - '0';
				GameManager.Instance.currentPlayerTurn.playerActionPoint = NFCManager.Instance.numberOfTheCard;

				switch (NFCManager.Instance.charCards[1]) // Check the letter of the card for the color and launch the appropriate power
				{
					case 'B':
						ChangeColorLight(LIGHT_COLOR.COLOR_BLUE, _currentPlayer);
						break;
					case 'R':
						ChangeColorLight(LIGHT_COLOR.COLOR_RED, _currentPlayer);
						break;
					case 'G':
						ChangeColorLight(LIGHT_COLOR.COLOR_GREEN, _currentPlayer);
						break;
					case 'Y':
						ChangeColorLight(LIGHT_COLOR.COLOR_YELLOW, _currentPlayer);
						break;
				}
				
				GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState);
			}

			foreach (var n in _number)
			{
				if (nfcTag.Data.Contains(n.ToString()))
				{
					AudioManager.Instance.Play("CardFalse");
					Camera.main.DOShakePosition(1, 0.3f);
					ChangeColorLight(LIGHT_COLOR.COLOR_BLACK, _currentPlayer);
				}
			}
			
			_cardNumber = nfcTag.Data[0];
			_charCardsStockage = nfcTag.Data[1];
		}
	}

	void ChangeColorLight(LIGHT_COLOR lightColor, PlayerStateManager currentPlayer)
	{
		switch (NFCManager.Instance.indexPlayer)
		{
			case 0:
				LightController.Colorize(NFCManager.Instance.lightIndexesPlayerOne, lightColor, false);
				if (!currentPlayer.currentCardEffect)
					currentPlayer.currentCardEffect = _cardEffect.SetActiveCardEffect(UiManager.Instance.parentSpawnCardUiVFX[0], lightColor);
				break;
			case 1:
				LightController.Colorize(NFCManager.Instance.lightIndexesPlayerTwo, lightColor, false);
				if (!currentPlayer.currentCardEffect)
					currentPlayer.currentCardEffect = _cardEffect.SetActiveCardEffect(UiManager.Instance.parentSpawnCardUiVFX[1], lightColor);
				break;
			case 2:
				LightController.Colorize(NFCManager.Instance.lightIndexesPlayerThree, lightColor, false);
				if (!currentPlayer.currentCardEffect)
					currentPlayer.currentCardEffect = _cardEffect.SetActiveCardEffect(UiManager.Instance.parentSpawnCardUiVFX[2], lightColor);
				break;
			case 3:
				LightController.Colorize(NFCManager.Instance.lightIndexesPlayerFour, lightColor, false);
				if (!currentPlayer.currentCardEffect)
					currentPlayer.currentCardEffect = _cardEffect.SetActiveCardEffect(UiManager.Instance.parentSpawnCardUiVFX[3], lightColor);
				break;
		}
	}

	private void OnTagRemoveDetected(NFC_DEVICE_ID device, NFCTag nfcTag) // When a card is removed
	{
		if (nfcTag.Data[0] == _cardNumber && nfcTag.Data[1] == _charCardsStockage)
		{
			if (nfcTag.Data.Contains("3") || nfcTag.Data.Contains("4") || nfcTag.Data.Contains("5"))
			{
				if (GameManager.Instance.currentPlayerTurn.playerActionPoint == _maxNumberOfTheCard && NFCManager.Instance.newCardDetected &&
				    !NFCManager.Instance.displacementActivated)
				{
					PlayerMovementManager.Instance.ResetDisplacement();
					NFCManager.Instance.newCardDetected = false;
				}
				else
				{
					NFCController.StopPolling();
				}
			}

			if (nfcTag.Data.Contains("=") || nfcTag.Data.Contains("<") || nfcTag.Data.Contains(";"))
			{
				if (NFCManager.Instance.newCardDetected && !NFCManager.Instance.powerActivated)
				{
					foreach (var power in PowerManager.Instance.powers)
					{
						if (power.activeSelf)
							power.GetComponent<IManagePower>().ClearPower();
					}

					GameManager.Instance.DecreaseVariable();
					NFCManager.Instance.newCardDetected = false;
				}
			}
			ChangeColorLight(LIGHT_COLOR.COLOR_WHITE, _currentPlayer);
			if (_currentPlayer.currentCardEffect != null)
			{
				_currentPlayer.currentCardEffect.SetActive(false);
				_currentPlayer.currentCardEffect = null;
			}
			_charCardsStockage = '0';
		}
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
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint == 0)
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1:
					NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false);
					break;
				case 2:
					NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false);
					break;
				case 3:
					NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false);
					break;
				case 4:
					NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false);
					break;
			}
		}
	}

	public override void ExitState(PlayerStateManager player)
	{
	}
}