using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : IState<PlayerController>
{
    private readonly int IDLE = Animator.StringToHash("Idle");
    private Animator _animator;
    private CharacterController _characterController;
    public PlayerIdle(Animator animator, CharacterController characterController)
    {
        _animator = animator;
        _characterController = characterController;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.SetBool(IDLE, true);
        _characterController.Move(Vector3.zero);
    }
    public void OnExecute(PlayerController owner)
    {
        if (owner.ApplyAttack())
        {
            owner._isAttacking = false;
            owner.ChangeState(owner._AttackState);
        }
        else
        if (owner.ApplyRoll())
        {
            owner._isRolling = false;
            owner.ChangeState(owner._RollState);
        }
        else
        if (owner.ApplyRun())
        {
            owner.ChangeState(owner._RunState);
        }
    }

    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(IDLE, false);
    }
}
public class PlayerRun : IState<PlayerController>
{
    private readonly int RUN = Animator.StringToHash("Run");
    private Animator _animator;
    private CharacterController _characterController;
    private float _staminaRecovery;
    public PlayerRun(Animator animator, CharacterController characterController, float staminaRecovery)
    {
        _animator = animator;
        _characterController = characterController;
        _staminaRecovery = staminaRecovery;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.SetBool(RUN, true);
    }
    public void OnExecute(PlayerController owner)
    {
        if (owner.ApplyAttack())
        {
            owner._isAttacking = false;
            owner.ChangeState(owner._AttackState);
        }
        else
        if (owner.ApplyRoll())
        {
            owner._isRolling = false;
            owner.ChangeState(owner._RollState);
        }
        else
        if (owner.ApplyRun())
        {
            Vector3 _direction = (owner.Joystick.Horizontal * Vector3.right + owner.Joystick.Vertical * Vector3.forward).normalized;
            owner.Translate(_direction);
            owner.Rotate(_direction);
            owner.StaminaRecovery(_staminaRecovery);
        }
        else
        {
            owner.ChangeState(owner._IdleState);
        }
    }
    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(RUN, false);
    }
}
public class PlayerReaction : IState<PlayerController>
{
    private readonly int REACTION = Animator.StringToHash("Reaction");
    private readonly string _stateNm = "Reaction";
    private Animator _animator;
    private CharacterController _characterController;
    private float _knockSpeed;
    public PlayerReaction(Animator animator, CharacterController characterController, float knockSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _knockSpeed = knockSpeed;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.Play(_stateNm, 0, 0f);
        _animator.SetBool(REACTION, true);
        owner.transform.LookAt(owner.ATTACKER);
        owner.StaminaRecovery(5f);
    }
    public void OnExecute(PlayerController owner)
    {
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
    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(REACTION, false);
    }
}

public class PlayerAttack : IState<PlayerController>
{
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int COMBO = Animator.StringToHash("Combo");
    private Animator _animator;
    private CharacterController _characterController;
    private PlayerWeapons[] _playerWeapons;
    private PlayerWeapons _playerWeapon;
    private string _attackState;
    private bool _nextCombo;
    private float _approachSpeed;
    private Vector3 _approachDirection;
    public PlayerAttack(Animator animator, CharacterController characterController, PlayerWeapons[] playerWeapons, float approachSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _playerWeapons = playerWeapons;
        _approachSpeed = approachSpeed;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.SetBool(ATTACK, true);
        _animator.SetInteger(COMBO, owner._combo);
        _playerWeapon = _playerWeapons[owner._combo];
        _attackState = _playerWeapon._melee.ToString();
        _playerWeapon._weapon.EnableWeapon();
        _approachDirection = owner.GetDirectionToTarget();
        _nextCombo = false;
    }

    public void OnExecute(PlayerController owner)
    {
        if (owner.ApplyRoll())
        {
            owner._isRolling = false;
            owner.ChangeState(owner._RollState);
        }
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_attackState))
        {
            if (!_nextCombo && owner.ApplyAttack() && state.normalizedTime > 0.3f && state.normalizedTime < 0.8f)
            {
                owner._isAttacking = false;
                _nextCombo = true;
            }
            if (state.normalizedTime >= 1f)
            {
                if (_nextCombo && owner._combo < 2)
                {
                    owner.ComboIncrease();
                    owner.ChangeState(owner._AttackState);
                }
                else
                {
                    owner.ComboReset();
                    owner.ChangeState(owner._IdleState);
                }
            }
        }
        if (owner._targetTF)
        {
            owner.FaceToTarget();
        }
        if (_approachDirection.magnitude > 0.5f)
        {
            _characterController?.Move(_approachDirection * _approachSpeed * Time.deltaTime);
        }
    }
    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(ATTACK, false);
        _playerWeapon._weapon.DisableWeapon();
        owner.AttackPerformed();
    }
}
public class PlayerRoll : IState<PlayerController>
{
    private readonly int ROLL = Animator.StringToHash("Roll");
    private readonly string _stateNm = "Roll";
    private Animator _animator;
    private CharacterController _characterController;
    private float _rollSpeed;
    public PlayerRoll(Animator animator, CharacterController characterController, float rollSpeed)
    {
        _animator = animator;
        _characterController = characterController;
        _rollSpeed = rollSpeed;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.SetBool(ROLL, true);
        owner.EnVisible();
    }
    public void OnExecute(PlayerController owner)
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(_stateNm) && state.normalizedTime >= 1f)
        {
            _characterController?.Move(Vector3.zero);
            owner.ChangeState(owner._IdleState);
        }
        _characterController?.Move(owner.TF.forward * _rollSpeed * Time.deltaTime);
    }
    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(ROLL, false);
        owner.DisVisible();
        owner.RollPerformed();
    }
}

public class PlayerDie : IState<PlayerController>
{
    private readonly int DIE = Animator.StringToHash("Die");
    private Animator _animator;
    public PlayerDie(Animator animator)
    {
        _animator = animator;
    }
    public void OnEnter(PlayerController owner)
    {
        _animator.SetBool(DIE, true);
    }
    public void OnExecute(PlayerController owner)
    {

    }

    public void OnExit(PlayerController owner)
    {
        _animator.SetBool(DIE, false);
    }
}
