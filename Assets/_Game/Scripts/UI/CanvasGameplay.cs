using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameplay : UICanvas
{
    [Header("Joystick")]
    [SerializeField] private Joystick _joystick;

    [Header("Button")]
    [SerializeField] private Button _settingButton;
    public Button _attackButton;
    public Button _rollButton;

    [Header("Image")]
    [SerializeField] private Image _playerHealthBarImage;
    [SerializeField] private Image _playerStaminaBarImage;
    [SerializeField] private Image _enemyHealthBarImage;
    [SerializeField] private Image _bossHealthBarImage;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _enemyAmountText;
    [SerializeField] private TextMeshProUGUI _enemNameText;

    [Header("Component")]
    [SerializeField] private Transform _enemyHBContainer;
    [SerializeField] private Transform _bossHBContainer;
    [SerializeField] private PopupItemUI _moneyItemPopup;
    [SerializeField] private PopupItemUI _hpItemPopup;
    [SerializeField] private PopupItemUI _staItemPopup;

    Queue<ItemUI> _itemsPopup = new Queue<ItemUI>();

    private bool _waiting;
    private float _timer;
    private void Awake()
    {
        _settingButton.onClick.AddListener(Setting);
    }
    private void OnEnable()
    {
        _waiting = false;
    }
    private void Update()
    {
        if (!_waiting && _itemsPopup.Count > 0)
        {
            _waiting = true;
            _timer = 0f;
            ItemUI item = _itemsPopup.Dequeue();
            item._item.gameObject.SetActive(true);
        }
        if (_waiting)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                _waiting = false;
            }
        }
    }
    public void SetPlayerHealth(Character character)
    {
        UpdateCharacterBar(_playerHealthBarImage, character._characterMaxHP, character._characterHP);
    }
    public void SetEnemyHealth(Character character)
    {
        _enemNameText.text = $"Lv{character._characterLevel}. {character._characterName}";
        UpdateCharacterBar(_enemyHealthBarImage, character._characterMaxHP, character._characterHP);
    }
    public void SetBossHealth(Character character)
    {
        UpdateCharacterBar(_bossHealthBarImage, character._characterMaxHP, character._characterHP);
    }
    public void SetPlayerStamina(PlayerController player)
    {
        UpdateCharacterBar(_playerStaminaBarImage, player._maxStamina, player._currentStamina);
    }
    public void EnableEnemyHB(string eName)
    {
        _enemNameText.text = eName;
        _enemyHBContainer.gameObject.SetActive(true);
    }
    public void DisableEnemyHB()
    {
        _enemyHBContainer.gameObject.SetActive(false);
    }
    public void EnableBossHB()
    {
        _bossHBContainer.gameObject.SetActive(true);
    }
    public void UpdateCharacterBar(Image hpBar, float maxHP, float currentHP)
    {
        hpBar.fillAmount = currentHP / maxHP;
    }
    public void MoneyLoot(string value)
    {
        ItemUI item = new ItemUI(_moneyItemPopup);
        item._item.SetText(value);
        item._item.gameObject.SetActive(false);
        _itemsPopup.Enqueue(item);
    }    
    public void FoodLoot(string value)
    {
        ItemUI item = new ItemUI(_hpItemPopup);
        item._item.SetText(value);
        item._item.gameObject.SetActive(false);
        _itemsPopup.Enqueue(item);
    }
    public void DrinkLoot(string value)
    {
        ItemUI item = new ItemUI(_staItemPopup);
        item._item.SetText(value);
        item._item.gameObject.SetActive(false);
        _itemsPopup.Enqueue(item);
    }
    public void Setting()
    {
        Time.timeScale = 0f;
        Close(0);
        UIManager.Instance.OpenUI<CanvasSetting>();
    }
    public void SetEnemyAlive(int alive)
    {
        _enemyAmountText.text = $"Enemy: {alive}";
    }
    public Joystick Joystick
    {
        get { return _joystick; }
        set { _joystick = value; }
    }
}
public class ItemUI
{
    public PopupItemUI _item;
    public ItemUI(PopupItemUI item)
    {
        _item = item;
    }
}
