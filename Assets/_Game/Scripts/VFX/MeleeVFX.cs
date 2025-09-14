using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeVFX : GameEntity
{
    private void OnParticleSystemStopped()
    {
        MasterPool.Push(this);
    }
}
