using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Character
{
    IState<BossController> _cState;


    [Header("Boss")]
    [SerializeField] private float _attackCDTime;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _backwardRange;

    [Header("Animation")]
    public float _startAttackTime;
    public float _endAttackTime;

    [Header("Weapon")]
    public Weapon _weapon;

    private float _anger;

    public bool _canAttack { get; private set; }
    public bool _canSpecialAttack { get; private set; }

    Coroutine _crtAttackCD;

    public BossIdle _IdleState;
    public BossChase _ChaseState;
    public BossReaction _ReactionState;
    public BossAttack _AttackState;
    public BossBackward _BackwardState;
    public BossSpecial _SpecialState;
    public BossDie _DieState;
    private void Awake()
    {
        _IdleState = new BossIdle(_animator, _characterController, transform);
        _ChaseState = new BossChase(_animator, _characterController, transform, _speed);
        _ReactionState = new BossReaction(_animator, _characterController, transform, _knockSpeed);
        _AttackState = new BossAttack(_animator, _characterController, _weapon, _startAttackTime, _endAttackTime);
        _BackwardState = new BossBackward(_animator, _characterController, transform, _speed);
        _SpecialState = new BossSpecial(_animator, _characterController, _weapon, _approachSpeed);
        _DieState = new BossDie(_animator);

        _characterATK = 30 * Mathf.Pow((1f + 0.2f), (_characterLevel - 1));
        _characterMaxHP = 500 * Mathf.Pow((1f + 0.15f), (_characterLevel - 1));

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
    public void ChangeState(IState<BossController> nState)
    {
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
    public void SpecialAttackCompleted()
    {
        _canSpecialAttack = false;
        _anger = 0;
    }
    public override void Reaction(Transform attacker, int side, float force)
    {
        base.Reaction(attacker, side, force);
        _anger += 1;
        _canSpecialAttack = _anger >= 5;
        if (_canReaction)
        {
            ChangeState(_ReactionState);
        }
    }
    public override void Die()
    {
        base.Die();
        Time.timeScale = 0.3f;
        ChangeState(_DieState);
    }
    public bool ApplyChaseState(float distance) => _canAttack && distance > _attackRange;
    public bool ApplyBackwardState(float distance) => !_canAttack && distance < _backwardRange;
    public bool ApplyAttackState(float distance) => _canAttack && distance <= _attackRange;
    public bool ApplySpecialState(float distance) => _canSpecialAttack && distance <= _attackRange;
    IEnumerator AttackCD(float time)
    {
        yield return Cache.GetWFSByTime(time);
        _canAttack = true;
    }
}
