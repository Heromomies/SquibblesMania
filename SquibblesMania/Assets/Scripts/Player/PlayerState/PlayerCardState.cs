using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class PlayerCardState : PlayerBaseState
{
    private List<int> _number = new List<int> { 2, 6, 7, 8, 9, 10 };
    private int _maxNumberOfTheCard;
    private char _charCardsStockage;
    private char _cardNumber;



    private CardEffect _cardEffect = new CardEffect();

    private PlayerStateManager _currentPlayer;

    //The state when player put card on Square one
    public override void EnterState(PlayerStateManager player)
    {
        //Turn of player x
        
        _currentPlayer = player;
        if (player.isPlayerStun && player.stunCount > 0)
        {
            PlayerIsStun(player);
        }

        //If the current player is this player
        if (GameManager.Instance.currentPlayerTurn == player)
        {
            player.indicatorPlayerRenderer.gameObject.SetActive(true);
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
            NFCManager.Instance.newCardDetected = true;
            UiManager.Instance.ResetValueNextTurn();
            
            if (nfcTag.Data.Contains("1"))
            {
                TestClickButtonLaunchEvent.Instance.LaunchMeteoriteOnPlayer();
            }

            if (nfcTag.Data.Contains(":"))
            {
                TeamInventoryManager.Instance.AddResourcesToInventory(1, GameManager.Instance.currentPlayerTurn.playerTeam);
            }

            if (nfcTag.Data.Contains("=") || nfcTag.Data.Contains("<") || nfcTag.Data.Contains(";"))
            {
                AudioManager.Instance.Play("CardTrue");
                GameManager.Instance.turnCount++;
                GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerPowerCardState);
                switch (NFCManager.Instance.charCards[1]) // Check the letter of the card for the color and launch the appropriate power
                {
                    case 'B':
                        PowerManager.Instance.ActivateDeactivatePower(0, true);
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
                GameManager.Instance.turnCount++;
                _maxNumberOfTheCard = NFCManager.Instance.charCards[0] - '0';

                NFCManager.Instance.numberOfTheCard = NFCManager.Instance.charCards[0] - '0';
                GameManager.Instance.currentPlayerTurn.playerActionPoint = NFCManager.Instance.numberOfTheCard;

                switch (NFCManager.Instance.charCards[1]) // Check the letter of the card for the color and launch the appropriate power
                {
                    case 'B': ChangeColorLight(LIGHT_COLOR.COLOR_BLUE, _currentPlayer); break;
                    case 'R': ChangeColorLight(LIGHT_COLOR.COLOR_RED, _currentPlayer); break;
                    case 'Y': ChangeColorLight(LIGHT_COLOR.COLOR_YELLOW, _currentPlayer); break;
                    case 'G': ChangeColorLight(LIGHT_COLOR.COLOR_GREEN, _currentPlayer); break;
                }

                GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState);
            }

            foreach (var n in _number)
            {
                if (nfcTag.Data.Contains(n.ToString()))
                {
                    AudioManager.Instance.Play("CardFalse");
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

                    PowerManager.Instance.isPlayerInJumpOrSwap = false;
                    GameManager.Instance.DecreaseVariable();
                }
                else
                {
                    NFCController.StopPolling();
                }
            }

            ChangeColorLight(LIGHT_COLOR.COLOR_WHITE, _currentPlayer);
            NFCManager.Instance.newCardDetected = false;

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
        player.indicatorPlayerRenderer.gameObject.SetActive(false);
        //If the stunCount is less than zero player is now not stun
        if (player.stunCount <= 0)
        {
            player.isPlayerStun = false;
            if (player.vfxStun != null) player.vfxStun.SetActive(false);
            
            if (GameManager.Instance.conditionVictory.mapTheme == ConditionVictory.Theme.Mountain)
            {
                AudioManager.Instance.Play("FreezeOff");
            }

            player.indicatorPlayerRenderer.gameObject.SetActive(true);
            NFCManager.Instance.PlayerChangeTurn();
        }
        else
        {
            PlayerStateEventManager.Instance.PlayerStunTextTriggerEnter(GameManager.Instance.actualCamPreset.presetNumber, true);
            NFCController.StopPolling();
            LightController.ShutdownAllLights();
        }

    }

    public override void UpdateState(PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (player.isPlayerStun)
        {
            if (player.stunCount <= 0)
            {
                player.isPlayerStun = false;
                player.vfxStun.SetActive(false);
            }

            player.indicatorPlayerRenderer.gameObject.SetActive(false);
            //Switch to next player of another team to play
            switch (player.playerNumber)
            {
                case 0: GameManager.Instance.ChangePlayerTurn(1); break;
                case 1: GameManager.Instance.ChangePlayerTurn(2); break;
                case 2: GameManager.Instance.ChangePlayerTurn(3); break;
                case 3: GameManager.Instance.ChangePlayerTurn(0); break;
            }
        }
        else
        {
            player.indicatorPlayerRenderer.gameObject.SetActive(false);
            //Switch to next player of another team to play
            switch (player.playerNumber)
            {
                case 0: GameManager.Instance.ChangePlayerTurn(1); break;
                case 1: GameManager.Instance.ChangePlayerTurn(2); break;
                case 2: GameManager.Instance.ChangePlayerTurn(3); break;
                case 3: GameManager.Instance.ChangePlayerTurn(0); break;
            }
        }

    }
}