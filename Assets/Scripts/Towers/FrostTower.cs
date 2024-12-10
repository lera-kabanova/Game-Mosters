using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostTower : Tower
{
    [SerializeField]
    private float slowingFactor;

    public float SlowingFactor
    {
        get { return slowingFactor; } 
    }
    private void Start()
    {
        ElementType = Element.FROST;
    }

    public override Debuff GetDebuff()
    {
        return new FrostDebuff(slowingFactor, DebuffDuration, Target);
    }
}
