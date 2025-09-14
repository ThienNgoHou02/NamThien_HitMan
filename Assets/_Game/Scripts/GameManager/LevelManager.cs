
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
    CanvasGameplay _gamePlayCanvas;
    Action<float> _playerHealthRecovery;
    Action<float> _playerStaminaRecovery;

    [Header("Map")]
    [SerializeField] private MapManager _mapGame;

    [Header("Player")]
    [SerializeField] private PlayerController _playerPrefab;

    [Header("Enemy")]
    [SerializeField] private int _maxEnemy;
    [SerializeField] private int _maxOnMapEnemy;
    [SerializeField] private EnemyOnLevel[] _enemyOnLevel;
    [SerializeField] private EnemyOnLevel _bossOnLevel;

    [Header("Level")]
    [SerializeField] private Transform _characterParentTF;

    [Header("Data")]
    [SerializeField] private GameData _data;

    private Transform[] _revivePoints;
    private int _pointIndex = 0;
    private int _currentAlives = 0;
    private int _rewardMoney = 0;
    private int _enemyLeftAmount;

    Coroutine _crtResetTimeScale;
    private void Awake()
    {

    }
    private void Start()
    {
        _enemyLeftAmount = _maxEnemy;

        MapManager _map = Instantiate(_mapGame);
        _map.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        _revivePoints = new Transform[]
        {
            _map._leftUpPointTF, _map._rightUpPointTF, _map._leftDownPointTF, _map._rightDownPointTF
        };
        _gamePlayCanvas = UIManager.Instance.OpenUI<CanvasGameplay>();
        _gamePlayCanvas.SetEnemyAlive(_enemyLeftAmount);

        PlayerController _player = Instantiate(_playerPrefab, _characterParentTF);
        _player.transform.SetPositionAndRotation(_map._playerPointTF.position, Quaternion.identity);
        _player.Joystick = _gamePlayCanvas.Joystick;
        _gamePlayCanvas._attackButton.onClick.AddListener(_player.AttackInput);
        _gamePlayCanvas._rollButton.onClick.AddListener(_player.RollInput);
        _player._OnHealthChanged += _gamePlayCanvas.SetPlayerHealth;
        _player._OnStaminaChanged += _gamePlayCanvas.SetPlayerStamina;
        _player._OnDeadAction += EndGame;

        _playerHealthRecovery += _player.HealthRecovery;
        _playerStaminaRecovery += _player.StaminaRecovery;

        CameraController.Instance.Follow = _player.transform;


        for (int i = 0; i < _maxOnMapEnemy; i++)
        {
            if (_currentAlives > _maxEnemy)
            {
                break;
            }
            EnemyController enemy = MasterPool.Pop<EnemyController>(_enemyOnLevel[i]._enemyTag, _revivePoints[i].position, Quaternion.identity);
            enemy._characterLevel = _enemyOnLevel[i]._enemyLevel;
            enemy._OnHealthChanged += _gamePlayCanvas.SetEnemyHealth;
            enemy._OnRivie += Revive;
            _currentAlives++;
        }
    }
    private void Revive(EnemyController enemy)
    {
        _enemyLeftAmount -= 1;
        _currentAlives -= 1;
        _gamePlayCanvas.SetEnemyAlive(_enemyLeftAmount);

        if (_currentAlives + _enemyLeftAmount <= 0)
        {
            _gamePlayCanvas.DisableEnemyHB();
            Boss(); 
            return;
        }
        enemy._OnHealthChanged -= _gamePlayCanvas.SetEnemyHealth;
        enemy._OnRivie -= Revive;
        
        Vector3 pos = enemy.TF.position;
        pos.y = 1f;
        SpawnItem(pos);


        if (_enemyLeftAmount >= 1 && _currentAlives < _maxOnMapEnemy)
        {
            _pointIndex += 1;
            if (_pointIndex >= 4)
            {
                _pointIndex = 0;
            }
            EnemyController e = MasterPool.Pop<EnemyController>(enemy._gameEntity, _revivePoints[_pointIndex].position, Quaternion.identity);
            e._OnHealthChanged += _gamePlayCanvas.SetEnemyHealth;
            e._OnRivie += Revive;
            _currentAlives += 1;
        }
    }
    private void Boss()
    {
        Time.timeScale = 0;
        _gamePlayCanvas.EnableBossHB();
        UIManager.Instance.OpenUI<CanvasBossAppear>();
        BossController _boss = MasterPool.Pop<BossController>(_bossOnLevel._enemyTag, _revivePoints[0].position, Quaternion.identity);
        _boss._characterLevel = _bossOnLevel._enemyLevel;
        _boss._OnHealthChanged += _gamePlayCanvas.SetBossHealth;
        _boss._OnDeadAction += EndGame;
    }
    private void EndGame(Character character)
    {
        _data._money += _rewardMoney;
        if (_crtResetTimeScale != null)
        {
            StopCoroutine(_crtResetTimeScale);
        }
        _gamePlayCanvas.Close(0);
        if (character is PlayerController)
        {
            _crtResetTimeScale = StartCoroutine(ResetTimeScale(2f, Defeat));
        }
        if (character is BossController)
        {
            _crtResetTimeScale = StartCoroutine(ResetTimeScale(2f, Victory));
        }
    }
    private void Victory()
    {
        UIManager.Instance.OpenUI<CanvasVictory>().Reward(_rewardMoney);
    }
    private void Defeat()
    {
        UIManager.Instance.OpenUI<CanvasDefeat>().Reward(_rewardMoney, _maxEnemy - _enemyLeftAmount);
    }
    private void MoneyLoot(int money)
    {
        _rewardMoney += money;
        _gamePlayCanvas.MoneyLoot($"+{money}$");
    }
    private void FoodLoot(int nutrition)
    {
        _playerHealthRecovery?.Invoke((float)nutrition);
        _gamePlayCanvas.FoodLoot($"+{nutrition}");
    }
    private void DrinkLoot(int nutrition)
    {
        _playerStaminaRecovery?.Invoke((float)nutrition);
        _gamePlayCanvas.DrinkLoot($"+{nutrition}");
    }
    private void SpawnItem(Vector3 pos)
    {
        int _tmp = Random.Range(0, 10);
        if (_tmp > 0 && _tmp <= 2)
        {
            Item french = MasterPool.Pop<Item>(GameEnities.FrenchFries, pos, Quaternion.identity);
            french._lootAction += FoodLoot;
        }
        else
        if (_tmp > 2 && _tmp <= 4)
        {
            Item soda = MasterPool.Pop<Item>(GameEnities.SodaDrink, pos, Quaternion.identity);
            soda._lootAction += DrinkLoot;
        }
        else
        {

            Item money = MasterPool.Pop<Item>(GameEnities.Money, pos, Quaternion.identity);
            money._lootAction += MoneyLoot;
        }
    }
    IEnumerator ResetTimeScale(float time, Action action)
    {
        yield return Cache.GetWFSRealTimeByTime(time);
        Time.timeScale = 1;
        action?.Invoke();
    }
}
[System.Serializable]
public class EnemyOnLevel
{
    public GameEnities _enemyTag;
    public int _enemyLevel;
}
