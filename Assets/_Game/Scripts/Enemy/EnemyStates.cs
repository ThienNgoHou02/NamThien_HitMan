using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : IState<EnemyController>
{
    private readonly int IDLE = Animator.StringToHash("Idle");
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _ownerTF;
    private float _idleTime;
    private float _idleDuration;
    public EnemyIdle(Animator animator, CharacterController characterController, Transform ownerTF)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
    }
    public void OnEnter(EnemyController owner)
    {
        _animator.SetBool(IDLE, true);
        _characterController?.Move(Vector3.zero);
        _idleDuration = Random.Range(1, 3);
        _idleTime = 0;
    }

    public void OnExecute(EnemyController owner)
    {
        _idleTime += Time.deltaTime;
        if (owner._targetTF)
        {
            owner.FaceToTarget();
            if (_idleTime >= _idleDuration)
            {
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
                if (owner.ApplyApproachState(distance))
                {
                    owner.ChangeState(owner._ApproachState);
                }
                else
                if (owner.ApplyAttackState(distance))
                {
                    owner.ChangeState(owner._AttackState);
                }
            }
        }
        else
        {
            owner.ChangeState(owner._TurnState);
        }
    }

    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(IDLE, false);
    }
}
public class EnemyChase : IState<EnemyController>
{
    private readonly int CHASE = Animator.StringToHash("WalkForward");
    private readonly float _chaseSpeed = 6.5f;
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _ownerTF;
    public EnemyChase(Animator animator, CharacterController characterController, Transform ownerTF)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
    }
    public void OnEnter(EnemyController owner)
    {
        _animator.SetBool(CHASE, true);
    }

    public void OnExecute(EnemyController owner)
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
            if (owner.ApplyApproachState(distance))
            {
                owner.ChangeState(owner._ApproachState);
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

    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(CHASE, false);
    }
}
public class EnemyApproach : IState<EnemyController>
{
    private readonly int APPROACH = Animator.StringToHash("WalkForward");
    private readonly float _approachSpeed = 6f;
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _ownerTF;
    public EnemyApproach(Animator animator, CharacterController characterController, Transform ownerTF)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
    }
    public void OnEnter(EnemyController owner)
    {
        _animator.SetBool(APPROACH, true);
    }

    public void OnExecute(EnemyController owner)
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
                owner.ChangeState(owner._BackwardState);
            }
            else
            if (owner.ApplyApproachState(distance))
            {
                Vector3 direction = (owner._targetTF.position - _ownerTF.position).normalized;
                direction.y = 0;
                owner.FaceToTarget();
                _characterController?.Move(direction * _approachSpeed * Time.deltaTime);
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

    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(APPROACH, false);
    }
}

public class EnemyTurn : IState<EnemyController>
{
    private readonly int TURN = Animator.StringToHash("Turn");
    private readonly int ANGLE = Animator.StringToHash("Angle");
    private Animator _animator;
    private Transform _ownerTF;
    private Quaternion _targetRotation;
    private string _stateNm;
    private int _turnAngle;
    public EnemyTurn(Animator animator, Transform ownerTF)
    {
        _animator = animator;
        _ownerTF = ownerTF;
    }
    public void OnEnter(EnemyController owner)
    {
        _turnAngle = Random.Range(-5, 5) > 0 ? 45 : -45;
        _animator.SetBool(TURN, true);
        _animator.SetInteger(ANGLE, _turnAngle);
        _targetRotation = _ownerTF.rotation;
        _targetRotation *= Quaternion.Euler(0, _turnAngle, 0);
        _stateNm = _turnAngle > 0 ? "TurnRight" : "TurnLeft";
    }
    public void OnExecute(EnemyController owner)
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_stateNm) && state.normalizedTime >= 1f)
        {
            owner.ChangeState(owner._IdleState);
        }
        _ownerTF.rotation = Quaternion.RotateTowards(_ownerTF.rotation, _targetRotation, 90f * Time.deltaTime);
        if (Quaternion.Angle(_ownerTF.rotation, _targetRotation) < 0.1f)
        {
            _ownerTF.rotation = _targetRotation;
        }
    }
    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(TURN, false);
    }
}
public class EnemyReaction : IState<EnemyController>
{
    private readonly int REACTION = Animator.StringToHash("Reaction");
    private readonly int SIDE = Animator.StringToHash("Side");
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _ownerTF;
    private string _stateNm;
    private float _knockSpeed;
    public EnemyReaction(Animator animator, CharacterController characterController, Transform ownerTF, float knockSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
        _knockSpeed = knockSpeed;
    }
    public void OnEnter(EnemyController owner)
    {
        _animator.Play(_stateNm, 0, 0f);
        _animator.SetBool(REACTION, true);
        _animator.SetInteger(SIDE, owner.REACTION_SIDE);
        _ownerTF.LookAt(owner.ATTACKER);
        _stateNm = owner.REACTION_SIDE > 0 ? "RightReaction" : "LeftReaction";
    }
    public void OnExecute(EnemyController owner)
    {
        if (owner._targetTF)
        {
            float distance = Vector3.Distance(_ownerTF.position, owner._targetTF.position);
            if (owner.ApplyAttackState(distance))
            {
                owner.ChangeState(owner._AttackState);
            }
        }
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
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
    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(REACTION, false);
    }
}
public class EnemyAttack : IState<EnemyController>
{
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly string _stateNm = "Attack";
    private Animator _animator;
    private CharacterController _characterController;
    private Weapon _weapon;
    private float _start;
    private float _end;
    private bool _trailOn;

    public EnemyAttack(Animator animator, CharacterController characterController, Weapon weapon, float start, float end)
    {
        _animator = animator;
        _characterController = characterController;
        _weapon = weapon;
        _start = start;
        _end = end;
    }
    public void OnEnter(EnemyController owner)
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
    public void OnExecute(EnemyController owner)
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
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
    public void OnExit(EnemyController owner)
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
public class EnemyBackward : IState<EnemyController>
{
    private readonly int RETURN = Animator.StringToHash("WalkBackward");
    private readonly float _backwardSpeed = 4.5f;
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _ownerTF;

    public EnemyBackward(Animator animator, CharacterController characterController, Transform ownerTF)
    {
        _animator = animator;
        _characterController = characterController;
        _ownerTF = ownerTF;
    }

    public void OnEnter(EnemyController owner)
    {
        _animator.SetBool(RETURN, true);
    }
    public void OnExecute(EnemyController owner)
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
                _characterController?.Move(-_ownerTF.forward * _backwardSpeed * Time.deltaTime);
            }
            else
            if (owner.ApplyApproachState(distance))
            {
                owner.ChangeState(owner._ApproachState);
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
    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(RETURN, false);
    }
}
public class EnemyDie : IState<EnemyController>
{
    private readonly int DIE = Animator.StringToHash("Die");
    private readonly string _stateNm = "Dying";
    private Animator _animator;
    private CharacterController _characterController;
    public EnemyDie(Animator animator, CharacterController characterController)
    {
        _animator = animator;
        _characterController = characterController;
    }
    public void OnEnter(EnemyController owner)
    {
        _animator.SetBool(DIE, true);
    }
    public void OnExecute(EnemyController owner)
    {
        AnimatorStateInfo state = owner._animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_stateNm) && state.normalizedTime >= 1f)
        {
            owner._OnRivie?.Invoke(owner);
            MasterPool.Push(owner);
        }
    }
    public void OnExit(EnemyController owner)
    {
        _animator.SetBool(DIE, false);
        _characterController.enabled = true;
    }
}
