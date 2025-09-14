using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Character
{
    [Header("Enemy")]
    [SerializeField] private float _attackCDTime;
    [SerializeField] private float _chaseRange;
    [SerializeField] private float _approachRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _backwardRange;

    [Header("Animation")]
    public float _startAttackTime;
    public float _endAttackTime;

    [Header("Weapon")]
    public Weapon _weapon;

    IState<EnemyController> _cState;

    public EnemyIdle _IdleState;
    public EnemyChase _ChaseState;
    public EnemyApproach _ApproachState;
    public EnemyTurn _TurnState;
    public EnemyReaction _ReactionState;
    public EnemyAttack _AttackState;
    public EnemyBackward _BackwardState;
    public EnemyDie _DieState;

    public bool _canAttack {  get; private set; }

    Coroutine _crtAttackCD;

    public Action<EnemyController> _OnRivie;

    private void Awake()
    {
        _IdleState = new EnemyIdle(_animator, _characterController, transform);
        _ChaseState = new EnemyChase(_animator, _characterController, transform);
        _ApproachState = new EnemyApproach(_animator, _characterController, transform);
        _TurnState = new EnemyTurn(_animator, transform);
        _ReactionState = new EnemyReaction(_animator, _characterController, transform, _knockSpeed);
        _AttackState = new EnemyAttack(_animator, _characterController, _weapon, _startAttackTime, _endAttackTime);
        _BackwardState = new EnemyBackward(_animator, _characterController, transform);
        _DieState = new EnemyDie(_animator, _characterController);

        _characterATK = 20 * Mathf.Pow((1f + 0.2f), (_characterLevel - 1));
        _characterMaxHP = 100 * Mathf.Pow((1f + 0.15f), (_characterLevel - 1));

        _weapon.DAMAGE = _characterATK;
    }
    private void OnEnable()
    {
        DisableTargetCircle();
        ChangeState(_IdleState);
        _canAttack = true;
        _characterHP = _characterMaxHP;
    }
    private void Update()
    {
        _cState?.OnExecute(this);
    }
    public void ChangeState(IState<EnemyController> nState)
    {
        //Debug.Log($"{_cState}->{nState}");
        _cState?.OnExit(this);
        _cState = nState;
        _cState?.OnEnter(this);
    }
    public void AttackCompleted()
    {
        _canAttack = false;
        if (_crtAttackCD != null)
        {
            StopCoroutine(_crtAttackCD);
        }
        _crtAttackCD = StartCoroutine(AttackCD(_attackCDTime));
    }
    public override void Reaction(Transform attacker, int side, float force)
    {
        base.Reaction(attacker, side, force);
        if (_canReaction)
        {
            ChangeState(_ReactionState);    
        }
    }
    public override void Die()
    {
        base.Die();
        ChangeState(_DieState);
    }
    public bool ApplyChaseState(float distance) => distance > _chaseRange;
    public bool ApplyApproachState(float distance) => _canAttack && distance > _approachRange && distance <= _chaseRange;
    public bool ApplyAttackState(float distance) => _canAttack && distance <= _attackRange;
    public bool ApplyBackwardState(float distance) => !_canAttack && distance < _backwardRange;
    IEnumerator AttackCD(float time)
    {
        yield return Cache.GetWFSByTime(time);
        _canAttack = true;
    }
}