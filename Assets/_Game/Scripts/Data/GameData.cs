using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
public class GameData : ScriptableObject
{
    public int _money;
    public int _nextLevel;

    public int _attackUpLevel;
    public int _healthUpLevel;
    public int _staminaRecUpLevel;
}
