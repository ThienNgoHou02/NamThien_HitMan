using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    private Transform tf;
    public GameEnities _gameEntity;
    public Transform TF
    {
        get
        {
            if (!tf)
                tf = transform;
            return tf;
        }
    }
}
public enum GameEnities
{
    Player,
    BatEnemy,
    AxeEnemy,
    MacheteEnemy,
    HammerEnemy,
    MeleeVFX,
    WeaponVFX,
    PopupText,
    KnifeProjectile,
    CleaverProjectile,
    KnifeEnemy,
    CleaverEnemy,
    AxeBoss,
    BatBoss,
    HammerBoss,
    MacheteBoss,
    Money,
    FrenchFries,
    SodaDrink
}
