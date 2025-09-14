using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float _playerATK;
    public float _playerHP;
    public float _playerStamina;
    public float _playerStaRecovery;
}
