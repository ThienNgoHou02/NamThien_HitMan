using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    [SerializeField] private Character _owner;

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetCharacterByCollider(other);
        _owner.AddTarget(character);
    }
    private void OnTriggerExit(Collider other)
    {
        Character character = Cache.GetCharacterByCollider(other);
        _owner.RemoveTarget(character);
    }
}
