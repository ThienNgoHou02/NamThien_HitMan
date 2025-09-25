using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerController : Character
{
    IState<PlayerController> _cState;
    GameObject _gameObject;
    Joystick _joystick;
    public Action<PlayerController> _OnStaminaChanged;

    public PlayerIdle _IdleState;
    public PlayerRun _RunState;
    public PlayerAttack _AttackState;
    public PlayerReaction _ReactionState;
    public PlayerRoll _RollState;
    public PlayerDie _DieState;

    [Header("Player")]
    public float _rotateSpeed;
    public float _attackSpeed;
    public float _rollSpeed;

    [Header("Data")]
    public PlayerData _data;

    public bool _isAttacking;
    public bool _isRolling;
    public int _combo { get; private set; }
    public float _maxStamina { get; private set; }
    public float _currentStamina { get; private set; }
    public float _staminaRecovery {  get; private set; }


    [Header("Weapon")]
    public PlayerWeapons[] _playerWeapons;

    private void Awake()
    {
        _characterATK = _data._playerATK;
        _characterMaxHP = _data._playerHP;
        _maxStamina = _data._playerStamina;
        _staminaRecovery = _data._playerStaRecovery;

        _IdleState = new PlayerIdle(_animator, _characterController);
        _RunState = new PlayerRun(_animator, _characterController, _staminaRecovery);
        _AttackState = new PlayerAttack(_animator, _characterController, _playerWeapons, _approachSpeed);
        _ReactionState = new PlayerReaction(_animator, _characterController, _knockSpeed);
        _RollState = new PlayerRoll(_animator, _characterController, _rollSpeed);
        _DieState = new PlayerDie(_animator);
    }

    private void Start()
    {
        _gameObject = gameObject;
        _combo = 0;
        _characterHP = _characterMaxHP;
        _OnHealthChanged?.Invoke(this);
        _currentStamina = _maxStamina;
        _OnStaminaChanged?.Invoke(this);
        WeaponSetDamage();
        ChangeState(_IdleState);
    }
    private void Update()
    {
        _cState?.OnExecute(this);
    }
    public void ChangeState(IState<PlayerController> nState)
    {
        _cState?.OnExit(this);
        _cState = nState;
        _cState?.OnEnter(this);
    }
    public void Translate(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.deltaTime);
    }
    public void Rotate(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion playerRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }
    private void WeaponSetDamage()
    {
        for (int i = 0; i < _playerWeapons.Length; i++)
        {
            _playerWeapons[i]._weapon.DAMAGE = _characterATK;
        }
    }
    public override void Reaction(Transform attacker, int side, float force)
    {
        base.Reaction(attacker, side, force);
        ChangeState(_ReactionState);
    }
    public override void Die()
    {
        base.Die();
        Time.timeScale = 0.3f;
        ChangeState(_DieState);
    }
    public void ComboIncrease()
    {
        _combo += 1;
    }
    public void ComboReset()
    {
        _combo = 0;
    }
    public void AttackPerformed()
    {
        _currentStamina -= 10f;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        _OnStaminaChanged?.Invoke(this);
    }
    public void RollPerformed()
    {
        _currentStamina -= 15f;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        _OnStaminaChanged?.Invoke(this);
    }
    public void HealthRecovery(float percent)
    {
        if (_characterHP < _characterMaxHP)
        {
            float _rec = _characterMaxHP * (percent / 100f);
            _characterHP += _rec;
            _characterHP = Mathf.Clamp(_characterHP, 0, _characterMaxHP);
            _OnHealthChanged?.Invoke(this);
        }
    }
    public void StaminaRecovery(float percent)
    {
        if (_currentStamina < _maxStamina)
        {
            float _rec = _maxStamina * (percent / 100f);
            _currentStamina += _rec;
            _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
            _OnStaminaChanged?.Invoke(this);
        }
    }
    public void EnVisible()
    {
        string _visible = "Visible";
        _gameObject.layer = LayerMask.NameToLayer(_visible);
    }
    public void DisVisible()
    {
        string _player = "Player";
        _gameObject.layer = LayerMask.NameToLayer(_player);
    }
    public void AttackInput()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
        }
    }
    public void RollInput()
    {
        if (!_isRolling)
        {
            _isRolling = true;
        }
    }
    public Joystick Joystick 
    { 
        get { return _joystick; } 
        set { _joystick = value; }
    }
    public bool ApplyRun() => _joystick.Vertical != 0 || _joystick.Horizontal != 0;
    public bool ApplyAttack() => _isAttacking && _currentStamina >= 5f;
    public bool ApplyRoll() => _isRolling && _currentStamina >= 10f;
}
[System.Serializable]
public class PlayerWeapons
{
    public MeleeCombo _melee;
    public Weapon _weapon;
}
public enum MeleeCombo
{
    Punch,
    Hook,
    Kick
}
