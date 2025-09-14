using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenu : UICanvas
{
    [Header("Button")]
    [SerializeField] private Button _attackUpButton;
    [SerializeField] private Button _healthUpButton;
    [SerializeField] private Button _staminaRecUpButton;
    [SerializeField] private Button _playButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _moneyAmount;
    [SerializeField] private TextMeshProUGUI _attackUpPrice;
    [SerializeField] private TextMeshProUGUI _healthUpPrice;
    [SerializeField] private TextMeshProUGUI _staminaRecUpPrice;

    [Header("Data")]
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private GameData _gameData;

    private void Awake()
    {
        _playButton.onClick.AddListener(Play);

        _attackUpButton.onClick.AddListener(() => Upgrade(ref _gameData._attackUpLevel, ref _playerData._playerATK, 10, ref _attackUpPrice));
        _healthUpButton.onClick.AddListener(() => Upgrade(ref _gameData._healthUpLevel, ref _playerData._playerHP, 10, ref _healthUpPrice));
        _staminaRecUpButton.onClick.AddListener(() => Upgrade(ref _gameData._staminaRecUpLevel, ref _playerData._playerStaRecovery, 10, ref _staminaRecUpPrice));
    }
    private void OnEnable()
    {
        _moneyAmount.text = $"${_gameData._money}";

        _attackUpPrice.text = $"${(int)(50 * Mathf.Pow(1.15f, _gameData._attackUpLevel))}";
        _healthUpPrice.text = $"${(int)(50 * Mathf.Pow(1.15f, _gameData._healthUpLevel))}";
        _staminaRecUpPrice.text = $"${(int)(50 * Mathf.Pow(1.15f, _gameData._staminaRecUpLevel))}";
    }
    private void Upgrade(ref int level, ref float upgradeField, int upgradeValue, ref TextMeshProUGUI _tmp)
    {
        int price = (int)(50 * Mathf.Pow(1.15f, level));
        if (_gameData._money >= price)
        {
            _gameData._money -= price;
            level += 1;
            upgradeField += upgradeValue;
            _tmp.text = $"${(int)(50 * Mathf.Pow(1.15f, level))}";
        }
    }
    private void Play()
    {
        Close(0);
        UIManager.Instance.OpenUI<CanvasLevelSelect>();
    }
}
