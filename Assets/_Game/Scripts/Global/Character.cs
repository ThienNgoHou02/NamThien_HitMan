using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : GameEntity
{
    private readonly string PLAYER = "Player";

    private List<Character> _Targets = new List<Character>();

    [Header("Character")]
    public int _characterLevel;
    public string _characterName;
    public Animator _animator;
    public CharacterController _characterController;
    public Transform _targetCircleTF;
    public AudioSource _audioSource;
    public float _speed;
    [Range(0f, 15f)]
    public float _approachSpeed;
    [Range(0f, 15f)]
    public float _knockSpeed;

    public Transform _targetTF { get; private set; }
    public Transform _attackerTF { get; private set; }
    public Vector3 _knockToward {  get; private set; }
    public int _reactionSide {  get; private set; }

    public Action<Character> _OnDeadAction;
    public Action<Character> _OnHealthChanged;

    public float _characterATK { get; protected set; }
    public float _characterMaxHP { get; protected set; }
    public float _characterHP { get; protected set; }
    public bool _canReaction;

    public void AddTarget(Character character)
    {
        //Character character = Cache.GetCharacterByCollider(collider);
        if (character && !_Targets.Contains(character))
        {
            _Targets.Add(character);
            character._OnDeadAction += RemoveTarget;
            if (!_targetTF)
            {
                _targetTF = Cache.GetTransformByCharacter(character);
                if (transform.CompareTag(PLAYER))
                {
                    character.EnableTargetCircle();
                }
            }
        }
    }
    public void RemoveTarget(Character character) 
    {
        //Character character = Cache.GetCharacterByCollider(collider);
        if (character && _Targets.Contains(character))
        {
            _Targets.Remove(character);
            character._OnDeadAction -= RemoveTarget;
            if (_targetTF == Cache.GetTransformByCharacter(character))
            {
                if (transform.CompareTag(PLAYER))
                {
                    if (_targetTF)
                    {
                        character.DisableTargetCircle();
                        /*LevelManager.Instance._DisnableHB?.Invoke();*/
                    }
                }
                _targetTF = GetTarget();
                if (_targetTF)
                {
                    Cache.GetCharacterByTransform(_targetTF).EnableTargetCircle();
/*                    LevelManager.Instance._EnableHB?.Invoke("");
                    LevelManager.Instance._UpdateHB?.Invoke(_characterMaxHP, _characterHP);*/
                }
            }
        }
    }
    public void EnableTargetCircle()
    {
        _targetCircleTF.gameObject.SetActive(true);
    }
    public void DisableTargetCircle()
    {
        _targetCircleTF.gameObject.SetActive(false);
    }
    public Transform GetTarget()
    {
        if (_Targets.Count > 0)
        {
            Transform _target = null;
            float min = Mathf.Infinity;
            foreach (Character character in _Targets)
            {
                Transform target = Cache.GetTransformByCharacter(character);
                float tmp = Vector3.Distance(transform.position, target.position);
                if (tmp < min)
                {
                    tmp = min;
                    _target = target;
                }
            }
            return _target;
        }
        return null;
    }
    public Vector3 GetDirectionToTarget()
    {
        if (_targetTF)
        {          
            Vector3 direction = (_targetTF.position - transform.position).normalized;
            direction.y = 0;
            return direction;
        }
        return Vector3.zero;
    }
    public void FaceToTarget()
    {
        Vector3 direction = (_targetTF.position - transform.position).normalized;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion playerRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        transform.rotation = playerRotation;
    }
    public void Damaged(Transform attacker, int side, float force, float damage) 
    {
        _characterHP -= damage;
        _OnHealthChanged?.Invoke(this);
        if (_characterHP > 0)
        {
            Reaction(attacker, side, force);
        }
        if (_characterHP <= 0) 
        {
            Die();
        }
    }
    public virtual void Reaction(Transform attacker, int side, float force) 
    { 
        _attackerTF = attacker;
        _reactionSide = side;
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0;
        _knockToward = dir * force;
        _audioSource.Play();
    }
    public virtual void Die() 
    {
        _characterController.enabled = false;
        _OnDeadAction?.Invoke(this);
    }
    public Transform ATTACKER
    {
        get { return _attackerTF; }
    }
    public Vector3 KNOCK
    {
        get { return _knockToward; }
        set { _knockToward = value; }
    }
    public int REACTION_SIDE
    {
        get { return _reactionSide; }
        set { _reactionSide = value; }
    }
}
