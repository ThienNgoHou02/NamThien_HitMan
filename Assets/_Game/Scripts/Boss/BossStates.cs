using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdle : IState<BossController>
{
    private readonly int IDLE = Animator.StringToHash("Idle");
    private Transform _ownerTF;
    private Animator _animator;
    private CharacterController _characterController;
    public BossIdle(Animator animator, CharacterController characterController, Transform ownerTF)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(IDLE, true);
        _characterController?.Move(Vector3.zero);
    }

    public void OnExecute(BossController owner)
    {
        if (owner._targetTF)
        {
            owner.FaceToTarget();
            float distance = Vector3.Distance(_ownerTF.position, owner._targetTF.position);
            if (owner.ApplyChaseState(distance))
            {
                owner.ChangeState(owner._ChaseState);
            }
            else
            if (owner.ApplyBackwardState(distance))
            {
                owner.ChangeState(owner._BackwardState);
            }
            else
            if (owner.ApplySpecialState(distance))
            {
                owner.ChangeState(owner._SpecialState);
            }
            else
            if (owner.ApplyAttackState(distance))
            {
                owner.ChangeState(owner._AttackState);
            }
        }
    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(IDLE, false);
    }
}
public class BossChase : IState<BossController>
{
    private readonly int CHASE = Animator.StringToHash("Run");
    private Transform _ownerTF;
    private Animator _animator;
    private CharacterController _characterController;
    private float _chaseSpeed;
    public BossChase(Animator animator, CharacterController characterController, Transform ownerTF, float chaseSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _chaseSpeed = chaseSpeed;
        _ownerTF = ownerTF;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(CHASE, true);
    }

    public void OnExecute(BossController owner)
    {
        if (owner._targetTF)
        {
            float distance = Vector3.Distance(_ownerTF.position, owner._targetTF.position);
            if (owner.ApplyChaseState(distance))
            {
                Vector3 direction = (owner._targetTF.position - _ownerTF.position).normalized;
                direction.y = 0;
                owner.FaceToTarget();
                _characterController?.Move(direction * _chaseSpeed * Time.deltaTime);
            }
            else
            if (owner.ApplyBackwardState(distance))
            {
                owner.ChangeState(owner._BackwardState);
            }
            else
            if (owner.ApplySpecialState(distance))
            {
                owner.ChangeState(owner._SpecialState);
            }
            else
            if (owner.ApplyAttackState(distance))
            {
                owner.ChangeState(owner._AttackState);
            }
            else
            {
                owner.ChangeState(owner._IdleState);
            }
        }
        else
        {
            owner.ChangeState(owner._IdleState);
        }
    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(CHASE, false);
    }
}
public class BossReaction : IState<BossController>
{
    private readonly int REACTION = Animator.StringToHash("Reaction");
    private readonly int SIDE = Animator.StringToHash("Side");
    private Transform _ownerTF;
    private Animator _animator;
    private CharacterController _characterController;
    private float _knockSpeed;
    private string _stateNm;
    public BossReaction(Animator animator, CharacterController characterController, Transform ownerTF, float knockSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
        _knockSpeed = knockSpeed;
    }
    public void OnEnter(BossController owner)
    {
        _stateNm = owner.REACTION_SIDE > 0 ? "RightReaction" : "LeftReaction";
        _animator.Play(_stateNm, 0, 0f);
        _animator.SetBool(REACTION, true);
        _animator.SetInteger(SIDE, owner.REACTION_SIDE);
        _ownerTF.LookAt(owner.ATTACKER);
    }

    public void OnExecute(BossController owner)
    {
        if (owner._targetTF)
        {
            float distance = Vector3.Distance(_ownerTF.position, owner._targetTF.position);
            if (owner.ApplySpecialState(distance))
            {
                owner.ChangeState(owner._SpecialState);
            }
            else
            if (owner.ApplyAttackState(distance))
            {
                owner.ChangeState(owner._AttackState);
            }
        }
        AnimatorStateInfo state = owner._animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_stateNm) && state.normalizedTime >= 1f)
        {
            owner.ChangeState(owner._IdleState);
        }
        if (owner.KNOCK.magnitude > 0.1f)
        {
            _characterController?.Move(owner.KNOCK * _knockSpeed * Time.deltaTime);
            owner.KNOCK = Vector3.MoveTowards(owner.KNOCK, Vector3.zero, _knockSpeed * Time.deltaTime);
        }
    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(REACTION, false);
    }
}
public class BossAttack : IState<BossController>
{
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly string _stateNm = "Attack";
    private Animator _animator;
    private CharacterController _characterController;
    private Weapon _weapon;
    private bool _trailOn;
    private float _start;
    private float _end;
    public BossAttack(Animator animator, CharacterController characterController, Weapon weapon, float start, float end)
    {
        _animator = animator;
        _characterController = characterController;
        _weapon = weapon;
        _start = start;
        _end = end;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(ATTACK, true);
        _characterController?.Move(Vector3.zero);
        if (_weapon._isEnabled)
        {
            _weapon.DisableWeapon();
        }
        if (owner._targetTF)
        {
            owner.FaceToTarget();
        }
        _trailOn = false;
        owner._canReaction = false;
    }

    public void OnExecute(BossController owner)
    {
        AnimatorStateInfo state = owner._animator.GetCurrentAnimatorStateInfo(0);
        float _normalizedTime = state.normalizedTime;
        if (!_trailOn && _normalizedTime >= _start && _normalizedTime < _end)
        {
            _trailOn = true;
            _weapon.EnableWeapon();
        }
        if (_trailOn && _normalizedTime >= _end)
        {
            _trailOn = false;
            _weapon.DisableWeapon();
        }
        if (state.IsName(_stateNm))
        {
            if (_normalizedTime >= 1f)
            {
                owner.ChangeState(owner._IdleState);
            }
        }
    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(ATTACK, false);
        owner.AttackCompleted();
        if (_weapon._isEnabled)
        {
            _weapon.DisableWeapon();
        }
        owner._canReaction = true;
    }
}

public class BossBackward : IState<BossController>
{
    private readonly int BACK = Animator.StringToHash("Backward");
    private Transform _ownerTF;
    private Animator _animator;
    private CharacterController _characterController;
    private float _backwardSpeed;
    public BossBackward(Animator animator, CharacterController characterController, Transform ownerTF, float backwardSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
        _backwardSpeed = backwardSpeed;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(BACK, true);
    }

    public void OnExecute(BossController owner)
    {
        if (owner._targetTF)
        {
            float distance = Vector3.Distance(_ownerTF.position, owner._targetTF.position);
            if (owner.ApplyChaseState(distance))
            {
                owner.ChangeState(owner._ChaseState);
            }
            else
            if (owner.ApplyBackwardState(distance))
            {
                owner.FaceToTarget();
                _characterController?.Move(-_ownerTF.forward * _backwardSpeed * Time.deltaTime);
            }
            else
            if (owner.ApplySpecialState(distance))
            {
                owner.ChangeState(owner._SpecialState);
            }
            else
            if (owner.ApplyAttackState(distance))
            {
                owner.ChangeState(owner._AttackState);
            }
            else
            {
                owner.ChangeState(owner._IdleState);
            }
        }
        else
        {
            owner.ChangeState(owner._IdleState);
        }
    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(BACK, false);
    }
}
public class BossSpecial : IState<BossController>
{
    private readonly int SPECIAL = Animator.StringToHash("Special");
    private readonly string _stateNm = "Special";
    private Animator _animator;
    private CharacterController _characterController;
    private Weapon _weapon;
    private Vector3 _approachDirection;
    private float _approachSpeed;
    public BossSpecial(Animator animator, CharacterController characterController, Weapon weapon, float approachSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _weapon = weapon;
        _approachSpeed = approachSpeed;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(SPECIAL, true);
        _weapon.EnableWeapon();
        owner._canReaction = false;
    }

    public void OnExecute(BossController owner)
    {
        AnimatorStateInfo state = owner._animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_stateNm))
        {
            if (state.normalizedTime >= 1f)
            {
                owner.ChangeState(owner._IdleState);
            }
        }
        if (owner._targetTF)
        {
            owner.FaceToTarget();
            _approachDirection = owner.GetDirectionToTarget();
            if (_approachDirection.magnitude > 0.5f)
            {
                _characterController?.Move(_approachDirection * _approachSpeed * Time.deltaTime);
            }
        }
    }

    public void OnExit(BossController owner)
    {
        _weapon.DisableWeapon();
        _animator.SetBool(SPECIAL, false);
        owner.SpecialAttackCompleted();
        owner._canReaction = true;
    }
}
public class BossDie : IState<BossController>
{
    private readonly int DIE = Animator.StringToHash("Die");
    private Animator _animator;
    public BossDie(Animator animator)
    {
        _animator = animator;
    }
    public void OnEnter(BossController owner)
    {
        _animator.SetBool(DIE, true);
    }

    public void OnExecute(BossController owner)
    {

    }

    public void OnExit(BossController owner)
    {
        _animator.SetBool(DIE, true);
    }
}
