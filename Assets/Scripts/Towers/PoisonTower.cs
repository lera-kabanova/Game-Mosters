using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTower : Tower
{
    [SerializeField]
    private float tickTime;

    [SerializeField]
    private PoisonSplash splashPrefab;

    [SerializeField]
    private int splashDamage;

    private int SplashDamage
    {
        get
        {
            return splashDamage;
        }
    }

    private float TickTime
    {
        get
        {
           return tickTime;     
        }
    }
    private void Start()
    {
        ElementType = Element.POISON;
    }

    public override Debuff GetDebuff()
    {
        return new PoisonDebuff(splashDamage, tickTime, splashPrefab, DebuffDuration, Target);
    }
}
